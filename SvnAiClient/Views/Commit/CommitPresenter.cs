using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SvnAiClient.Core.AI;
using SvnAiClient.Core.Svn;
using SvnAiClient.Models;

namespace SvnAiClient.Views.Commit
{
    /// <summary>AI 검수, 유효성 확인, 이력 저장 및 최종 커밋 승인 상태를 관리한다.</summary>
    public sealed class CommitPresenter
    {
        #region Fields

        /// <summary>검수 요청, 승인 확인 및 이력 저장 중 발생한 오류를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>화면 입력을 읽고 결과 표시를 요청하는 UI 독립 계약.</summary>
        private readonly CommitInterface _view;

        /// <summary>커밋 화면에 전달된 파일 목록과 현재 체크 상태.</summary>
        private readonly List<SvnFileStatus> _files;

        /// <summary>검수 프로바이더와 커밋 정책에 사용할 앱 설정.</summary>
        private readonly AppSettings _settings;

        /// <summary>호출한 메인 Presenter가 수명을 관리하는 SVN 서비스.</summary>
        private readonly SvnService _svnService;

        /// <summary>검수 이력에 기록할 작업 복사본 루트 경로.</summary>
        private readonly string _workingCopyPath;

        /// <summary>검수, 유효성 확인 또는 커밋 준비 작업의 취소 토큰.</summary>
        private CancellationTokenSource _cts;

        /// <summary>현재 검수 결과에 대응하는 대상, 내용, 설정 및 이력의 스냅샷.</summary>
        private CommitReviewSnapshot _reviewSnapshot;

        /// <summary>최종 커밋 승인 흐름의 중복 실행을 막는다.</summary>
        private bool _commitFlowBusy;

        /// <summary>실제 SVN 커밋 전에 재확인할 승인 시점의 파일 스냅샷.</summary>
        private Dictionary<string, SvnFileSnapshot> _approvedCommitFiles;

        /// <summary>검수 결과 유효성 확인의 중복 실행을 막는다.</summary>
        private bool _reviewCheckBusy;

        /// <summary>종료 후 새 작업과 컨트롤 갱신을 방지한다.</summary>
        private bool _closing;

        #endregion

        #region Properties

        /// <summary>최종 승인된 커밋 메시지. 화면 입력과 구분해 보관한다.</summary>
        public string CommitMessage { get; private set; }

        /// <summary>현재 검수 결과. 검수 전, 무효화 후 또는 텍스트 검수 대상이 없으면 null.</summary>
        private AiReviewResult LastReviewResult
        {
            get { return _reviewSnapshot == null ? null : _reviewSnapshot.Result; }
        }

        /// <summary>종료 처리가 시작되지 않았고 화면에 결과를 표시할 수 있는지 여부.</summary>
        private bool CanUpdateView { get { return !_closing && _view.IsAvailable; } }

        #endregion

        #region Initialization

        public CommitPresenter(CommitInterface view, List<SvnFileStatus> files,
            AppSettings settings, SvnService svnService, string workingCopyPath)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (svnService == null) throw new ArgumentNullException(nameof(svnService));
            if (string.IsNullOrWhiteSpace(workingCopyPath))
                throw new ArgumentException("작업 폴더 경로가 필요합니다.", nameof(workingCopyPath));

            _view = view;
            _files = files;
            _settings = settings;
            _svnService = svnService;
            _workingCopyPath = workingCopyPath;
        }

        public void Initialize()
        {
            foreach (var file in _files)
            {
                file.IsChecked = !file.IsDirectory &&
                    (file.Status == SvnStatusKind.Modified ||
                     file.Status == SvnStatusKind.Added ||
                     file.Status == SvnStatusKind.Deleted ||
                     (file.Status == SvnStatusKind.Normal && file.HasPropertyChanges));
            }
            _view.ShowFiles(_files, _settings.ActiveAiProvider);
        }

        #endregion

        #region View Requests

        public async Task ReviewAsync()
        {
            if (_commitFlowBusy || !CanUpdateView) return;
            await RunAiReviewAsync();
        }

        public async Task ActivatedAsync()
        {
            if (_commitFlowBusy || !CanUpdateView) return;
            await EnsureReviewCurrentAsync();
        }

        public async Task CopySuggestedMessageAsync()
        {
            if (!CanUpdateView || !await EnsureReviewCurrentAsync() || !CanUpdateView)
                return;
            if (!string.IsNullOrEmpty(_view.SuggestedMessage))
                _view.CommitMessageText = _view.SuggestedMessage;
        }

        public void ToggleAllFiles()
        {
            if (_cts != null || _commitFlowBusy || _reviewCheckBusy || !CanUpdateView) return;
            _view.PostFileEdits();
            bool anyChecked = _files.Exists(file => !file.IsDirectory && file.IsChecked);
            foreach (var file in _files)
            {
                if (!file.IsDirectory)
                    file.IsChecked = !anyChecked;
            }
            InvalidateReview("검수 대상이 변경되었습니다.");
            _view.RefreshFiles(_files);
        }

        public void Cancel()
        {
            Close();
            _view.CancelCommit();
        }

        public void Close()
        {
            _closing = true;
            _cts?.Cancel();
        }

        public async Task TryCommitAsync()
        {
            if (!CanUpdateView || _commitFlowBusy ||
                _cts != null ||
                _reviewCheckBusy)
            {
                return;
            }

            _commitFlowBusy = true;
            _approvedCommitFiles = null;
            SetUiBusy(true);

            try
            {
                if (GetCheckedFiles().Count == 0)
                {
                    _view.ShowMessage("Commit할 파일을 선택하세요.", "알림", CommitNotice.Information);

                    return;
                }

                bool current = await EnsureReviewCurrentAsync();

                if (!CanUpdateView)
                    return;

                if (_settings.AutoReviewBeforeCommit)
                {
                    // 검수 결과가 없거나 무효화되었으면 자동 실행한다.
                    if (_reviewSnapshot == null)
                    {
                        await RunAiReviewAsync();

                        if (!CanUpdateView)
                            return;

                        if (LastReviewResult != null &&
                            LastReviewResult.IsSuccess)
                        {
                            _view.ShowMessage("자동 검수가 완료되었습니다.\n"
                                + "결과와 Commit 메시지를 확인한 뒤 "
                                + "Commit을 다시 눌러주세요.", "검수 완료", CommitNotice.Information);
                        }

                        // 자동 검수 직후에는 결과 확인 기회를 제공한다.
                        return;
                    }

                    if (LastReviewResult != null && !LastReviewResult.IsSuccess)
                    {
                        _view.ShowMessage("자동 검수가 켜져 있어 전체 검수 성공이 필요합니다.\n"
                            + "실패 또는 부분 완료 원인을 해결하고 "
                            + "AI 검수를 다시 실행하세요.", "검수 필요", CommitNotice.Warning);

                        return;
                    }
                }
                else if (!current)
                {
                    // 기존 결과가 무효화된 경우 이번 클릭은 중단한다.
                    return;
                }

                if (string.IsNullOrWhiteSpace(_view.CommitMessageText))
                {
                    _view.ShowMessage("Commit 메시지를 입력하세요.", "알림", CommitNotice.Information);

                    return;
                }

                if (LastReviewResult != null &&
                    LastReviewResult.Issues.Exists(
                        issue => issue.Severity == IssueSeverity.Error))
                {
                    var answer = _view.Confirm("검수 결과에 ERROR 이슈가 있습니다.\n"
                        + "결과를 확인했으며 현재 변경 내용을 "
                        + "Commit하시겠습니까?", "오류 이슈 확인");

                    if (!answer)
                        return;
                }

                var cancellation = new CancellationTokenSource();
                _cts = cancellation;

                try
                {
                    var paths = GetCheckedFilePaths();

                    var files = await _svnService.GetFileSnapshotsAsync(paths, cancellation.Token);

                    if (!CanUpdateView)
                        return;

                    // 결과 확인 도중 외부 편집 등으로 바뀌었는지 확인한다.
                    if (_reviewSnapshot != null &&
                        (!CommitReviewSnapshot.HaveSameTargets(
                            GetCheckedFilePaths(), _reviewSnapshot.Paths) ||
                         !SvnFileSnapshot.AreEqual(_reviewSnapshot.Files, files)))
                    {
                        InvalidateReview(
                            "Commit 확인 중 변경 내용이 달라졌습니다.");

                        return;
                    }

                    _approvedCommitFiles =
                        new Dictionary<string, SvnFileSnapshot>(
                            files,
                            StringComparer.OrdinalIgnoreCase);

                    CommitMessage = _view.CommitMessageText.Trim();

                    _view.CompleteCommit();
                }
                finally
                {
                    if (ReferenceEquals(_cts, cancellation))
                        _cts = null;

                    cancellation.Dispose();
                }
            }
            catch (OperationCanceledException)
            {
                if (CanUpdateView)
                    _view.ReviewStatus = "Commit 준비가 취소되었습니다.";
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Commit 준비 실패");

                if (CanUpdateView)
                {
                    _view.ShowMessage("Commit 준비 실패:\n" + ex.Message, "오류", CommitNotice.Error);
                }
            }
            finally
            {
                _commitFlowBusy = false;

                if (CanUpdateView)
                    SetUiBusy(false);
            }
        }

        public void InvalidateReview(string reason)
        {
            if (_reviewSnapshot == null)
                return;

            _reviewSnapshot = null;

            if (CanUpdateView)
            {
                _view.ClearReview();
                _view.ReviewStatus = reason + " 다시 검수하세요.";
            }
        }

        /// <summary>선택한 파일의 저장소 최신본과 작업본을 조회하고 기존 Diff 화면으로 표시한다.</summary>
        public async Task OpenFileDiffAsync(SvnFileStatus file)
        {
            if (!CanUpdateView || file == null || file.IsDirectory || _cts != null ||
                _commitFlowBusy || _reviewCheckBusy) return;
            var cancellation = new CancellationTokenSource();
            _cts = cancellation;
            SetUiBusy(true);
            try
            {
                var diff = await _svnService.GetHeadWorkingDiffAsync(file.FullPath, cancellation.Token);
                if (CanUpdateView) _view.ShowHeadWorkingDiff(file.FullPath, diff);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Logger.Error(ex, "커밋 대상 최신 저장소 비교 실패");
                if (CanUpdateView) _view.ShowMessage("변경 내용 조회 실패:\n" + ex.Message, "오류", CommitNotice.Error);
            }
            finally
            {
                if (ReferenceEquals(_cts, cancellation)) _cts = null;
                cancellation.Dispose();
                if (CanUpdateView) SetUiBusy(false);
            }
            if (CanUpdateView) await EnsureReviewCurrentAsync();
        }

        public void OpenReviewIssue(AiReviewIssue issue)
        {
            if (!CanUpdateView || issue == null || _cts != null ||
                _commitFlowBusy ||
                _reviewCheckBusy)
            {
                return;
            }

            if (_reviewSnapshot == null || _reviewSnapshot.ViewerDiffs == null)
            {
                _view.ShowMessage("유효한 검수 결과가 없습니다. 다시 검수하세요.", "알림", CommitNotice.Information);

                return;
            }

            string filePath = issue.FilePath;
            string diffText;

            if (string.IsNullOrWhiteSpace(filePath) ||
                !_reviewSnapshot.ViewerDiffs.TryGetValue(filePath, out diffText))
            {
                _view.ShowMessage("이슈의 파일 경로가 검수 대상과 일치하지 않습니다.\n"
                    + "AI가 반환한 경로: "
                    + (filePath ?? "(없음)"), "위치 확인", CommitNotice.Information);

                return;
            }

            _view.ShowIssueDiff(filePath, diffText, issue.LineNumber);
        }

        #endregion

        #region Approved Results

        /// <summary>현재 체크된 파일의 경로를 최종 커밋용으로 반환한다.</summary>
        public List<string> GetCheckedFilePaths()
        {
            var result = new List<string>();
            foreach (var f in _files)
            {
                if (!f.IsDirectory && f.IsChecked)
                    result.Add(f.FullPath);
            }

            return result;
        }

        /// <summary>승인 시점의 파일 정보를 독립된 스냅샷으로 호출 화면에 반환한다.</summary>
        public Dictionary<string, SvnFileSnapshot> GetApprovedCommitFiles()
        {
            return _approvedCommitFiles == null
                ? null
                : new Dictionary<string, SvnFileSnapshot>(
                    _approvedCommitFiles,
                    StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>현재 검수 결과가 유효한 경우에만 이력 ID를 반환한다.</summary>
        public Guid GetReviewHistoryId()
        {
            return _reviewSnapshot != null && LastReviewResult != null
                ? _reviewSnapshot.HistoryId : Guid.Empty;
        }

        #endregion

        #region View Updates

        private void SetUiBusy(bool busy)
        {
            if (CanUpdateView)
                _view.SetActionsEnabled(!busy && !_commitFlowBusy);
        }

        private void ShowReviewResult(AiReviewResult result)
        {
            _view.ReviewStatus = BuildReviewStatus(result);
            _view.ShowReviewResult(result);
        }

        #endregion

        #region Operations

        /// <summary>선택 파일의 변경 전후 소스를 수집하고 이력 저장과 최신 Diff 확인을 포함한 검수를 실행한다.</summary>
        private async Task RunAiReviewAsync()
        {
            if (!CanUpdateView || _cts != null || _reviewCheckBusy)
                return;

            _reviewSnapshot = null;
            var recorder = new AiReviewHistoryRecorder(_workingCopyPath);

            var checkedFiles = GetCheckedFiles();
            if (checkedFiles.Count == 0)
            {
                _view.ShowMessage("검수할 파일을 선택하세요.", "알림", CommitNotice.Information);
                return;
            }

            // 이전 검수 결과와 메시지가 새 검수 결과처럼 남지 않도록 한다.
            _view.ClearReview();
            string changeContext = _view.ChangeContext.Trim();

            var reviewPaths = checkedFiles.ConvertAll(f => f.FullPath);
            string providerName =
                _view.SelectedProvider
                ?? _settings.ActiveAiProvider;

            SetUiBusy(true);

            var cancellation = new CancellationTokenSource();
            _cts = cancellation;

            try
            {
                // Diff 수집
                string modelName = GetReviewModelName(providerName);
                _view.ReviewStatus = "Diff 수집 중...";
                var fileSnapshots = await _svnService.GetFileSnapshotsAsync(reviewPaths, cancellation.Token);
                var fileDiffs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var pair in fileSnapshots)
                    if (!string.IsNullOrEmpty(pair.Value.Diff)) fileDiffs.Add(pair.Key, pair.Value.Diff);

                if (fileDiffs.Count == 0)
                {
                    _view.ShowMessage("변경 내용이 없습니다.", "알림", CommitNotice.Information);
                    return;
                }

                var reviewFiles = await CollectReviewSourcesAsync(
                    checkedFiles, fileDiffs, cancellation.Token);
                if (reviewFiles == null || !CanUpdateView) return;

                if (reviewFiles.Count == 0)
                {
                    var currentFiles = await _svnService.GetFileSnapshotsAsync(reviewPaths, cancellation.Token);
                    if (!CanUpdateView) return;
                    if (!SvnFileSnapshot.AreEqual(fileSnapshots, currentFiles) ||
                        !CommitReviewSnapshot.HaveSameTargets(GetCheckedFilePaths(), reviewPaths) ||
                        changeContext != _view.ChangeContext.Trim())
                    {
                        _view.ReviewStatus = "확인 중 변경 내용이 달라졌습니다. 다시 확인하세요.";
                        return;
                    }
                    ApplyReviewSnapshot(new CommitReviewSnapshot(fileDiffs,
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), reviewPaths,
                        providerName, changeContext, null, Guid.Empty, fileSnapshots));
                    return;
                }

                var textPaths = reviewFiles.ConvertAll(file => file.FilePath);
                var viewerDiffs = await _svnService.GetDiffsAsync(
                    textPaths, cancellation.Token, fullContext: true);
                var factory = new AiProviderFactory(_settings);
                var provider = factory.CreateProvider(providerName);
                if (!provider.IsConfigured)
                {
                    _view.ShowMessage($"{providerName} API Key가 설정되지 않았습니다.\n설정 창에서 API Key를 입력해주세요.", "알림", CommitNotice.Warning);
                    return;
                }

                // AI 검수 실행
                _view.ReviewStatus = $"AI 검수 중... ({providerName}, 텍스트 {textPaths.Count}개 / 제외 {reviewPaths.Count - textPaths.Count}개)";
                var reviewService = new AiReviewService(provider, _settings);
                var outcome = await recorder.ReviewAsync(
                    reviewService,
                    reviewFiles,
                    textPaths,
                    provider.ProviderName,
                    modelName,
                    changeContext,
                    cancellation.Token,
                    reviewPaths.Count - textPaths.Count);

                var result = outcome.Result;

                if (!CanUpdateView)
                    return;

                _view.ReviewStatus = "검수 결과와 최신 Diff 비교 중...";

                var currentFilesAfterReview = await _svnService.GetFileSnapshotsAsync(
                    reviewPaths, cancellation.Token);

                if (!CanUpdateView)
                    return;

                if (!CommitReviewSnapshot.HaveSameTargets(GetCheckedFilePaths(),
                        checkedFiles.ConvertAll(f => f.FullPath)) ||
                    !SvnFileSnapshot.AreEqual(fileSnapshots, currentFilesAfterReview) ||
                    !string.Equals(
                        changeContext,
                        _view.ChangeContext.Trim(),
                        StringComparison.Ordinal))
                {
                    _view.ReviewStatus =
                        "검수 중 변경 내용 또는 변경 배경이 달라졌습니다. 다시 검수하세요.";

                    return;
                }

                ApplyReviewSnapshot(new CommitReviewSnapshot(
                    fileDiffs, viewerDiffs, checkedFiles.ConvertAll(f => f.FullPath),
                    providerName, changeContext, result, outcome.HistoryId, fileSnapshots));
            }
            catch (OperationCanceledException)
            {
                if (CanUpdateView)
                    _view.ReviewStatus = "검수 취소됨";
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "AI 검수 오류");

                if (!CanUpdateView)
                    return;

                _view.ShowMessage($"AI 검수 실패:\n{ex.Message}", "오류", CommitNotice.Error);
            }
            finally
            {
                if (ReferenceEquals(_cts, cancellation))
                    _cts = null;

                cancellation.Dispose();

                if (CanUpdateView)
                {
                    SetUiBusy(false);

                    if (recorder.HasSaveFailure)
                        _view.ReviewStatus += " / 이력 저장 실패 (로그 확인)";
                }
            }
        }

        /// <summary>대상, 프로바이더, 변경 배경과 최신 Diff를 확인하고 오래된 검수 결과를 무효화한다.</summary>
        private async Task<bool> EnsureReviewCurrentAsync()
        {
            if (!CanUpdateView || _reviewCheckBusy || _cts != null)
                return false;

            if (_reviewSnapshot == null)
                return true;

            var snapshot = _reviewSnapshot;

            string provider =
                _view.SelectedProvider
                ?? _settings.ActiveAiProvider;

            if (!CommitReviewSnapshot.HaveSameTargets(
                    GetCheckedFilePaths(), _reviewSnapshot.Paths) ||
                provider != _reviewSnapshot.ProviderName ||
                !string.Equals(
                    _reviewSnapshot.ChangeContext,
                    _view.ChangeContext.Trim(),
                    StringComparison.Ordinal))
            {
                InvalidateReview(
                    "검수 대상, AI 제공자 또는 변경 배경이 변경되었습니다.");

                return false;
            }

            _reviewCheckBusy = true;
            SetUiBusy(true);

            var cancellation = new CancellationTokenSource();
            _cts = cancellation;

            try
            {
                var current = await _svnService.GetFileSnapshotsAsync(
                    GetCheckedFilePaths(), cancellation.Token);

                if (!CanUpdateView)
                    return false;

                if (!ReferenceEquals(snapshot, _reviewSnapshot))
                    return false;

                if (!CommitReviewSnapshot.HaveSameTargets(
                    GetCheckedFilePaths(), _reviewSnapshot.Paths) ||
                    !SvnFileSnapshot.AreEqual(snapshot.Files, current))
                {
                    InvalidateReview(
                        "검수 이후 변경 내용이 달라졌습니다.");

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "검수 결과 유효성 확인 실패");

                if (CanUpdateView)
                {
                    InvalidateReview(
                        "최신 변경 내용을 확인하지 못했습니다.");
                }

                return false;
            }
            finally
            {
                if (ReferenceEquals(_cts, cancellation))
                    _cts = null;

                cancellation.Dispose();
                _reviewCheckBusy = false;

                if (CanUpdateView)
                    SetUiBusy(false);
            }
        }

        /// <summary>전체 소스를 수집하고 수집 전후 Diff가 같을 때만 검수 입력을 반환한다.</summary>
        private async Task<List<AiReviewFile>> CollectReviewSourcesAsync(
            List<SvnFileStatus> checkedFiles, Dictionary<string, string> fileDiffs,
            CancellationToken token)
        {
            _view.ReviewStatus = "변경 전후 전체 소스 수집 중...";
            var reviewFiles = await _svnService.GetReviewFilesAsync(
                checkedFiles.ConvertAll(f => f.FullPath), token);
            var sourceDiffs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in reviewFiles)
                if (!string.IsNullOrEmpty(file.Diff)) sourceDiffs[file.FilePath] = file.Diff;
            var expectedDiffs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in reviewFiles)
            {
                string diff;
                if (fileDiffs.TryGetValue(file.FilePath, out diff)) expectedDiffs[file.FilePath] = diff;
            }
            if (!CommitReviewSnapshot.SameDiffs(expectedDiffs, sourceDiffs))
            {
                _view.ReviewStatus = "소스 수집 중 변경 내용이 달라졌습니다. 다시 검수하세요.";
                return null;
            }

            return reviewFiles;
        }

        /// <summary>현재 내용과 일치하는 검수 스냅샷을 반영하고 결과와 커밋 메시지 제안을 표시한다.</summary>
        private void ApplyReviewSnapshot(CommitReviewSnapshot snapshot)
        {
            _reviewSnapshot = snapshot;
            var result = snapshot.Result;

            if (result == null)
            {
                _view.ClearReview();
                _view.ReviewStatus = "AI 검수 대상 텍스트 없음 / " + snapshot.Paths.Count
                    + "개 파일의 내용·SVN 상태 확인 완료. 커밋 메시지를 확인하세요.";
                return;
            }
            ShowReviewResult(result);

            if (result.IsSuccess && !string.IsNullOrEmpty(result.SuggestedCommitMessage))
                _view.SuggestedMessage = result.SuggestedCommitMessage;
        }

        /// <summary>선택한 프로바이더에 설정된 모델명을 구한다.</summary>
        private string GetReviewModelName(string providerName)
        {
            switch (providerName)
            {
                case "OpenAI": return _settings.AiProviders.OpenAi.Model;
                case "Claude": return _settings.AiProviders.Claude.Model;
                case "Gemini": return _settings.AiProviders.Gemini.Model;
                case "Local": return _settings.AiProviders.LocalLlm.Model;
                default:
                    throw new NotSupportedException(
                        "지원하지 않는 AI 프로바이더입니다.");
            }
        }

        /// <summary>편집 중인 체크값을 확정하고 선택된 파일을 수집한다.</summary>
        private List<SvnFileStatus> GetCheckedFiles()
        {
            _view.PostFileEdits();

            var result = new List<SvnFileStatus>();
            foreach (var f in _files)
            {
                if (!f.IsDirectory && f.IsChecked)
                    result.Add(f);
            }

            return result;
        }

        /// <summary>검수 상태, 성공 배치 수 및 이슈 수를 상태 표시용 문자열로 변환한다.</summary>
        private static string BuildReviewStatus(AiReviewResult result)
        {
            string state;
            switch (result.Status)
            {
                case AiReviewStatus.Succeeded: state = "검수 완료"; break;
                case AiReviewStatus.Partial: state = "부분 완료"; break;
                case AiReviewStatus.Cancelled: state = "검수 취소"; break;
                case AiReviewStatus.Failed:
                    state = result.Batches.Count > 0 && result.Batches.TrueForAll(
                        batch => batch.FailureKind == AiReviewFailureKind.Timeout)
                        ? "시간 초과" : "검수 실패";
                    break;
                default: state = "미검수"; break;
            }
            return state + " - 성공 배치 " + result.SuccessfulBatchCount + "/" + result.Batches.Count
                + ", 이슈 " + result.Issues.Count + "개";
        }

        #endregion

    }
}
