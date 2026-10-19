using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SvnAiClient.Core.AI;
using SvnAiClient.Core.Settings;
using SvnAiClient.Core.Svn;
using SvnAiClient.Models;
using static SvnAiClient.Helpers.WorkingCopyPath;

namespace SvnAiClient.Views.Main
{
    /// <summary>작업 복사본, 탐색 상태, 커밋 선택 및 SVN 작업 흐름을 관리한다.</summary>
    public sealed class MainPresenter
    {
        #region Fields

        /// <summary>메인 화면의 설정, 탐색 및 SVN 처리 오류를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>화면 입력과 컨트롤 표시를 요청하는 UI 독립 계약.</summary>
        private readonly MainInterface _view;

        /// <summary>설정 파일의 읽기와 저장을 담당한다.</summary>
        private readonly SettingsService _settingsService;

        /// <summary>현재 적용된 앱 설정과 등록된 작업 복사본 목록.</summary>
        private AppSettings _settings;

        /// <summary>현재 선택한 작업 복사본. 선택하지 않았으면 null이다.</summary>
        private WorkingCopyEntry _currentEntry;

        /// <summary>현재 작업 복사본의 자격 증명을 사용하는 SVN 서비스.</summary>
        private SvnService _svnService;

        /// <summary>작업 복사본 상태 캐시와 표시 목록 및 누적 커밋 선택을 관리한다.</summary>
        private readonly MainWorkspaceState _workspace = new MainWorkspaceState();

        /// <summary>탐색 중인 폴더의 작업 복사본 루트 기준 상대 경로.</summary>
        private string _currentGridPath = string.Empty;

        /// <summary>실행 중인 SVN 작업의 취소 요청을 관리한다.</summary>
        private CancellationTokenSource _cts;

        /// <summary>SVN 작업과 후속 갱신이 끝날 때까지 중복 실행을 차단한다.</summary>
        private bool _isBusy;

        /// <summary>종료 후 새 작업과 화면 갱신을 방지한다.</summary>
        private bool _isClosing;

        /// <summary>종료 시 설정 저장과 서비스 해제가 한 번만 실행되도록 한다.</summary>
        private bool _closed;

        #endregion

        #region Properties

        /// <summary>SVN 작업 또는 상태 갱신이 진행 중인지 여부.</summary>
        public bool IsBusy { get { return _isBusy; } }

        /// <summary>화면 종료 처리가 시작되었는지 여부.</summary>
        public bool IsClosing { get { return _isClosing; } }

        /// <summary>현재 작업 복사본이 선택되어 있는지 여부.</summary>
        public bool HasWorkingCopy { get { return _currentEntry != null; } }

        /// <summary>탐색 중인 폴더의 작업 복사본 루트 기준 상대 경로.</summary>
        public string CurrentRelativePath { get { return _currentGridPath; } }

        #endregion

        #region Initialization

        public MainPresenter(MainInterface view, SettingsService settingsService)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            _view = view;
            _settingsService = settingsService;
            _settings = settingsService.Load();
        }

        public async Task InitializeAsync()
        {
            try
            {
                int index = FindWorkingCopyIndex(_settings.LastSelectedWorkingCopy);
                PublishWorkingCopies(index);
                RefreshCommitTargetGrid();
                UpdateStatusBar();
                if (index >= 0)
                    await SelectWorkingCopyAsync(index);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "MainView 초기화 오류");
            }
        }

        #endregion

        #region View Requests

        public async Task SelectWorkingCopyAsync(int index)
        {
            if (_isBusy || _isClosing || index < 0 || index >= _settings.WorkingCopies.Count)
                return;

            ClearWorkingCopySelection();
            _currentEntry = _settings.WorkingCopies[index];
            _settings.LastSelectedWorkingCopy = _currentEntry.LocalPath;
            _svnService = CreateSvnService(_currentEntry);
            UpdateStatusBar();
            await RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            await RunSvnOperationAsync("파일 상태 조회 중...", null);
        }

        public async Task UpdateAsync()
        {
            if (!CheckWorkingCopySelected()) return;
            var succeeded = await RunSvnOperationAsync("업데이트 중...",
                (service, entry, token) => service.UpdateAsync(
                    entry.LocalPath, SetStatusText, token));
            if (succeeded && !_isClosing)
                _view.ShowMessage("업데이트 완료.", "알림", MainNotice.Information);
        }

        public async Task RevertSelectedAsync()
        {
            if (!CheckWorkingCopySelected())
                return;
            var selected = _view.SelectedFiles;
            if (selected.Count == 0)
            {
                _view.ShowMessage("되돌리기할 파일을 선택하세요.", "알림", MainNotice.Information);
                return;
            }

            if (!_view.Confirm($"{selected.Count}개 파일을 되돌리겠습니까?\n변경 내용이 모두 사라집니다.",
                "되돌리기 확인", true))
                return;
            var paths = selected.ConvertAll(file => file.FullPath);
            await RunSvnOperationAsync(
                "되돌리기 중...",
                (service, entry, token) => service.RevertAsync(paths, token),
                paths);
        }

        public void NavigateToFolder(string relativePath)
        {
            if (_isBusy || _isClosing) return;
            NavigateGrid(relativePath);
        }

        public void NavigateToFullPath(string path)
        {
            if (_currentEntry == null || _isBusy || _isClosing) return;
            NavigateGrid(GetRelativePath(_currentEntry.LocalPath, path));
        }

        public async Task OpenItemAsync(SvnFileStatus file)
        {
            if (file == null || _currentEntry == null || _isBusy || _isClosing) return;
            if (file.IsDirectory)
                NavigateToFullPath(file.FullPath);
            else
                await OpenDiffViewerAsync(file);
        }

        public async Task AddWorkingCopyAsync()
        {
            if (_isBusy || _isClosing) return;
            var entry = _view.CreateWorkingCopy();
            if (entry == null) return;

            _settings.WorkingCopies.Add(entry);
            _settingsService.Save(_settings);
            int index = _settings.WorkingCopies.Count - 1;
            PublishWorkingCopies(index);
            await SelectWorkingCopyAsync(index);
        }

        public void RemoveCurrentWorkingCopy()
        {
            if (_isBusy || _isClosing || _currentEntry == null) return;
            if (!_view.Confirm($"'{_currentEntry.GetDisplayName()}'을(를) 목록에서 제거하겠습니까?",
                "제거 확인", false)) return;

            _settings.WorkingCopies.Remove(_currentEntry);
            _settingsService.Save(_settings);
            ClearWorkingCopySelection();
            PublishWorkingCopies(-1);
            UpdateStatusBar();
        }

        public async Task OpenSettingsAsync()
        {
            if (_isBusy || _isClosing)
            {
                _view.ShowMessage("현재 작업이 완료된 후 설정을 변경해주세요.", "알림", MainNotice.Information);
                return;
            }
            var saved = _view.EditSettings(_settingsService, _settings);
            if (saved != null)
                await ApplySavedSettingsAsync(saved);
        }

        public async Task RequestCommitAsync()
        {
            if (!CheckWorkingCopySelected()) return;
            _view.PostFileEdits();
            if (_workspace.CommitTargets.Count == 0)
            {
                _view.ShowMessage("커밋할 파일을 선택하세요.\n파일 목록에서 체크박스를 선택하면 하단 커밋 대상에 추가됩니다.",
                    "알림", MainNotice.Information);
                return;
            }
            var request = _view.EditCommit(_workspace.CommitTargets, _settings, _svnService, _currentEntry.LocalPath);
            if (request != null)
                await CommitAsync(request.FilePaths, request.Message, request.ApprovedFiles, request.ReviewHistoryId);
        }

        public void OpenFolderLog()
        {
            if (!CheckWorkingCopySelected()) return;
            OpenLogViewerForPath(Path.Combine(_currentEntry.LocalPath, _currentGridPath));
        }

        public void OpenReviewHistory()
        {
            if (_isBusy || _isClosing) return;
            try
            {
                _view.ShowReviewHistory(_currentEntry?.LocalPath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "검수 이력 화면 열기 실패");
                _view.ShowMessage("검수 이력 화면 열기 실패:\n" + ex.Message, "오류", MainNotice.Error);
            }
        }

        public bool SetCommitChecked(SvnFileStatus file, bool requestedCheck)
        {
            if (file == null) return false;
            bool selected = _workspace.SetCommitChecked(file, requestedCheck);
            RefreshCommitTargetGrid();
            return selected;
        }

        public void RemoveCommitTarget(SvnFileStatus selected)
        {
            if (selected == null)
                return;

            _workspace.RemoveCommitTarget(selected);

            var displayedFiles = _view.DisplayedFiles;

            if (displayedFiles != null)
                RestoreCommitChecks(displayedFiles);

            RefreshCommitTargetGrid();
            _view.RefreshFiles();
        }

        public async Task ScheduleSelectedFilesAsync(bool isAdd)
        {
            if (!CheckWorkingCopySelected())
                return;

            _view.PostFileEdits();

            var files = _view.SelectedFiles;

            if (files.Count == 0)
                return;

            bool valid = files.All(file =>
                isAdd
                    ? file.Status == SvnStatusKind.Unversioned
                    : file.Status == SvnStatusKind.Normal ||
                      file.Status == SvnStatusKind.Modified ||
                      file.Status == SvnStatusKind.Missing);

            if (!valid)
            {
                _view.ShowMessage("선택한 파일 중 처리할 수 없는 상태가 있습니다.", "알림", MainNotice.Information);

                return;
            }

            // 현재 Working Copy 내부 파일만 처리한다.
            var rootPath = _currentEntry.LocalPath;

            if (files.Any(file =>
                    !IsDescendantPath(rootPath, file.FullPath)))
            {
                _view.ShowMessage("현재 작업 폴더 내부 파일만 처리할 수 있습니다.", "알림", MainNotice.Information);

                return;
            }

            if (!isAdd)
            {
                var targets = string.Join(
                    Environment.NewLine,
                    files.Select(file => file.RelativePath));

                var answer = _view.Confirm("다음 파일의 SVN 삭제를 예약하시겠습니까?\n\n"
                    + targets
                    + "\n\n로컬 파일은 유지됩니다."
                    + "\n저장소에서는 다음 커밋 시 삭제됩니다.", "SVN 삭제 예약", true);

                if (!answer)
                    return;
            }

            var paths = files.ConvertAll(file => file.FullPath);
            var succeeded = await RunSvnOperationAsync(
                isAdd ? "SVN 추가 중..." : "SVN 삭제 예약 중...",
                (service, entry, token) => isAdd
                    ? service.AddAsync(paths, token)
                    : service.DeleteAsync(paths, token),
                paths);
            if (succeeded && !_isClosing)
                _view.ShowMessage(isAdd
                    ? "SVN 추가가 완료되었습니다."
                    : "SVN 삭제 예약이 완료되었습니다.", "알림", MainNotice.Information);
        }

        public void OpenLogViewerForPath(string targetPath)
        {
            if (!CheckWorkingCopySelected() || _isBusy || _isClosing ||
                _svnService == null || string.IsNullOrWhiteSpace(targetPath))
            {
                return;
            }

            try
            {
                string logPath = NormalizeFilePath(targetPath);
                string rootPath = _currentEntry.LocalPath;

                if (!IsSameFilePath(rootPath, logPath) &&
                    !IsDescendantPath(rootPath, logPath))
                {
                    throw new InvalidOperationException(
                        "현재 작업 폴더 내부 경로만 조회할 수 있습니다.");
                }

                _view.ShowLogViewer(logPath, _svnService);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SVN 로그 화면 열기 실패");

                _view.ShowMessage("로그 화면을 열지 못했습니다:\n" + ex.Message, "오류", MainNotice.Error);
            }
        }

        public async Task OpenDiffViewerAsync(SvnFileStatus file)
        {
            if (_isBusy ||
                _isClosing ||
                _currentEntry == null ||
                file == null)
                return;

            if (!CanViewDiff(file))
            {
                _view.ShowMessage("비교할 수 없는 상태입니다.\n"
                    + "미등록 파일은 SVN 추가 후, "
                    + "누락 파일은 삭제 예약 또는 되돌리기 후 비교해주세요.", "알림", MainNotice.Information);

                return;
            }

            var entry = _currentEntry;
            var service = _svnService;

            if (service == null ||
                !IsDescendantPath(entry.LocalPath, file.FullPath))
                return;

            var cancellation = new CancellationTokenSource();
            _cts = cancellation;

            string diffText = null;
            SetUiBusy(true, "변경 내용 조회 중...");

            try
            {
                diffText = await service.GetDiffAsync(file.FullPath, cancellation.Token, fullContext: true);
            }
            catch (OperationCanceledException)
            {
                SetStatusText("변경 내용 조회 취소됨");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "변경 내용 조회 실패");

                if (!_isClosing && _view.IsAvailable)
                {
                    _view.ShowMessage("변경 내용 조회 실패:\n" + ex.Message, "오류", MainNotice.Information);
                }
            }
            finally
            {
                if (ReferenceEquals(_cts, cancellation))
                    _cts = null;

                cancellation.Dispose();

                if (!_isClosing && _view.IsAvailable)
                {
                    SetUiBusy(false);
                    UpdateStatusBar();
                }
            }

            if (diffText == null ||
                _isClosing ||
                !_view.IsAvailable ||
                !ReferenceEquals(entry, _currentEntry))
                return;

            try
            {
                _view.ShowDiff(file.FullPath, diffText);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "변경 내용 화면 표시 실패");

                _view.ShowMessage("변경 내용 화면 표시 실패:\n" + ex.Message, "오류", MainNotice.Information);
            }
        }

        public void BeginClose()
        {
            _isClosing = true;
            _cts?.Cancel();
        }

        public void Close()
        {
            if (_closed) return;
            _closed = true;
            BeginClose();
            _svnService?.Dispose();
            try
            {
                _settingsService.Save(_settings);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "종료 시 설정 저장 실패");
            }
        }

        #endregion

        #region View Updates

        private void PublishWorkingCopies(int selectedIndex)
        {
            _view.SetWorkingCopies(_settings.WorkingCopies
                .Select(entry => entry.GetDisplayName() + " - " + entry.LocalPath).ToList(), selectedIndex);
        }

        private void RefreshCommitTargetGrid()
        {
            _view.ShowCommitTargets(_workspace.CommitTargets);
        }

        private void SetUiBusy(bool busy, string message = "")
        {
            _isBusy = busy;
            _view.SetBusy(busy);
            if (!string.IsNullOrEmpty(message))
                SetStatusText(message);
        }

        private void SetStatusText(string message)
        {
            _view.SetStatusText(message);
        }

        private void UpdateStatusBar()
        {
            string repository = "저장소: -";
            if (_svnService != null && _currentEntry != null)
            {
                try
                {
                    string url = _svnService.GetRepositoryUrl(_currentEntry.LocalPath);
                    if (!string.IsNullOrEmpty(url))
                        repository = "저장소: " + url;
                }
                catch
                {
                    repository = "저장소: (연결 불가)";
                }
            }
            _view.SetStatusBar(_settings.ActiveAiProvider, repository);
        }

        #endregion

        #region Operations

        /// <summary>지정 폴더의 SVN 상태와 디스크 항목을 병합하고 필터와 커밋 선택을 적용한다.</summary>
        private void NavigateGrid(string relativePath)
        {
            if (_currentEntry == null)
                return;

            // 목록을 교체하기 전에 마지막 체크값을 반영한다.
            _view.PostFileEdits();

            var rootPath = NormalizeFilePath(_currentEntry.LocalPath);

            var fullPath = string.IsNullOrEmpty(relativePath)
                ? rootPath
                : NormalizeFilePath(Path.Combine(rootPath, relativePath));

            // Working Copy 밖으로 이동하지 않도록 한다.
            if (!IsSameFilePath(rootPath, fullPath) &&
                !IsDescendantPath(rootPath, fullPath))
            {
                _view.ShowMessage("작업 폴더 내부 경로만 탐색할 수 있습니다.", "알림", MainNotice.Information);

                return;
            }

            _currentGridPath = IsSameFilePath(rootPath, fullPath)
                ? string.Empty
                : GetRelativePath(rootPath, fullPath);

            bool includeDescendants =
                _view.OnlyChanged &&
                _view.IncludeSubfolders;

            var items = _workspace.GetFiles(rootPath, fullPath,
                _view.OnlyChanged, includeDescendants, ReportDiskReadError);

            _view.ShowFiles(items);

            _view.SyncTreeToPath(fullPath);
        }

        /// <summary>디스크 목록 조회 오류를 기존 안내 문구와 로그로 전달한다.</summary>
        private void ReportDiskReadError(Exception error)
        {
            if (error is UnauthorizedAccessException)
            {
                Logger.Warn(error, "디스크 목록 접근 권한 부족");
                _view.ShowMessage("폴더 접근 권한이 없습니다.\n"
                    + "조회된 SVN 상태를 기준으로 표시합니다.", "알림", MainNotice.Information);
            }
            else
            {
                Logger.Warn(error, "디스크 목록 조회 실패");
                _view.ShowMessage("디스크 목록을 완전히 읽지 못했습니다.\n"
                    + "조회된 SVN 상태를 기준으로 표시합니다.", "알림", MainNotice.Information);
            }
        }

        /// <summary>커밋 직전 Diff를 재검증하고 성공 리비전의 이력 연결과 대상 경로 갱신을 수행한다.</summary>
        private async Task CommitAsync(
            List<string> filePaths,
            string message,
            Dictionary<string, SvnFileSnapshot> approvedFiles,
            Guid reviewHistoryId)
        {
            long revision = -1;
            bool committed = false;
            string historyWarning = null;

            var succeeded = await RunSvnOperationAsync(
                "커밋 중...",
                async (service, entry, token) =>
                {
                    if (filePaths.Count == 0 ||
                        filePaths.Any(path =>
                            !IsDescendantPath(entry.LocalPath, path)))
                    {
                        throw new InvalidOperationException(
                            "현재 작업 폴더의 파일만 커밋할 수 있습니다.");
                    }

                    if (approvedFiles == null)
                    {
                        throw new InvalidOperationException(
                            "커밋 승인 정보를 확인할 수 없습니다.");
                    }

                    // 실제 SVN Commit 호출 직전에 다시 확인한다.
                    var currentFiles = await service.GetFileSnapshotsAsync(filePaths, token);
                    bool changed = !SvnFileSnapshot.AreEqual(approvedFiles, currentFiles);

                    if (changed)
                    {
                        throw new InvalidOperationException(
                            "커밋 확인 이후 변경 내용이 달라졌습니다. "
                            + "커밋 창에서 다시 확인하세요.");
                    }

                    revision = await service.CommitAsync(
                        filePaths,
                        message,
                        SetStatusText,
                        token);

                    if (revision < 0)
                    {
                        throw new InvalidOperationException(
                            "커밋 완료 리비전을 확인할 수 없습니다.");
                    }

                    committed = true;

                    if (reviewHistoryId != Guid.Empty)
                    {
                        historyWarning = await Task.Run(() =>
                        {
                            try
                            {
                                string error;
                                bool linked = new AiReviewHistoryService().TryLinkRevision(
                                    reviewHistoryId,
                                    revision,
                                    out error);

                                return linked ? null : error;
                            }
                            catch (Exception ex)
                            {
                                return "검수 이력 리비전 연결 오류: " + ex.Message;
                            }
                        }); // SVN 성공 후에는 취소 토큰을 적용하지 않는다.

                        if (historyWarning != null)
                            Logger.Warn(historyWarning);
                    }

                    _workspace.RemoveCommittedTargets(filePaths);
                }, filePaths, isCommitOperation: true);

            if (committed && !_isClosing && _view.IsAvailable)
            {
                string completionMessage = succeeded
                    ? $"커밋 완료. (r{revision})"
                    : $"커밋은 완료되었습니다. (r{revision})\n"
                      + "상태 갱신 결과를 확인해주세요.";

                if (historyWarning != null)
                {
                    completionMessage +=
                        "\n검수 이력에 리비전을 연결하지 못했습니다. "
                        + "로그를 확인해주세요.";
                }

                _view.ShowMessage(completionMessage, "알림", MainNotice.Information);
            }
        }

        /// <summary>SVN 작업과 후속 상태 갱신이 끝날 때까지 하나의 실행 잠금을 유지한다.</summary>
        private async Task<bool> RunSvnOperationAsync(
            string message,
            Func<SvnService, WorkingCopyEntry, CancellationToken, Task> operation,
            IEnumerable<string> refreshPaths = null,
            bool isCommitOperation = false)
        {
            if (_isBusy || _isClosing || _svnService == null || _currentEntry == null)
                return false;
            _view.PostFileEdits();
            var service = _svnService;
            var entry = _currentEntry;
            var cancellation = new CancellationTokenSource();
            _cts = cancellation;
            SetUiBusy(true, message);
            Exception operationError = null;
            bool cancelled = false;
            try
            {
                if (operation != null)
                {
                    try
                    {
                        await operation(service, entry, cancellation.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        cancelled = true;
                    }
                    catch (Exception ex)
                    {
                        operationError = ex;
                        Logger.Error(ex, "SVN 작업 실패");
                    }
                }

                if (_isClosing || !_view.IsAvailable)
                    return false;

                // 오류가 발생한 작업도 일부 변경이 남을 수 있으므로 다시 조회한다.
                SetStatusText(refreshPaths == null
                    ? "전체 파일 상태 갱신 중..."
                    : "작업한 경로 상태 갱신 중...");

                var refreshToken = cancelled || isCommitOperation
                    ? CancellationToken.None
                    : cancellation.Token;

                if (refreshPaths == null)
                    await ReloadWorkingCopyAsync(service, entry, refreshToken);
                else
                    await ReloadPathsAsync(service, entry, refreshPaths, refreshToken);

                if (_isClosing || !_view.IsAvailable)
                    return false;
                if (operationError != null)
                {
                    SetStatusText("작업 실패");
                    _view.ShowMessage("작업 중 오류가 발생했습니다.\n\n"
                                              + operationError.Message
                                              + "\n\n일부 변경은 반영됐을 수 있습니다. 갱신된 상태를 확인해주세요.", "오류", MainNotice.Error);
                    return false;
                }

                if (cancelled)
                {
                    SetStatusText("작업 취소됨");

                    if (isCommitOperation)
                    {
                        _view.ShowMessage("커밋 취소가 요청되었습니다.\n\n"
                            + "완료 리비전을 확인하지 못했습니다. "
                            + "서버에서 이미 커밋되었을 수 있으므로 "
                            + "SVN 로그에서 완료 여부를 확인한 후 다시 시도하세요.", "커밋 확인", MainNotice.Information);
                    }

                    return false;
                }

                SetStatusText("준비 완료");
                return true;
            }
            catch (OperationCanceledException)
            {
                SetStatusText("작업 취소됨");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SVN 상태 갱신 실패");
                if (!_isClosing && _view.IsAvailable)
                {
                    if (refreshPaths == null)
                    {
                        _workspace.Clear();
                        _view.ClearWorkspace();
                    }
                    else
                    {
                        var displayed = _view.DisplayedFiles;

                        if (displayed != null)
                            RestoreCommitChecks(displayed);

                        _view.RefreshFiles();
                    }

                    RefreshCommitTargetGrid();

                    SetStatusText("상태 갱신 실패");
                    _view.ShowMessage("상태를 갱신하지 못했습니다.\n\n"
                                              + ex.Message + "\n\n새로고침 후 실제 상태를 확인해주세요.", "오류", MainNotice.Error);
                }

                return false;
            }
            finally
            {
                if (ReferenceEquals(_cts, cancellation))
                    _cts = null;
                cancellation.Dispose();
                if (!_isClosing && _view.IsAvailable)
                {
                    SetUiBusy(false);
                    UpdateStatusBar();
                }
            }
        }

        /// <summary>전체 상태를 조회하고 유효한 커밋 선택과 탐색 위치를 복원한다.</summary>
        private async Task ReloadWorkingCopyAsync(SvnService service,
            WorkingCopyEntry entry, CancellationToken token)
        {
            var selectedPaths = _workspace.GetCheckedPaths();
            var relativePath = _currentGridPath;
            var statuses = await service.GetStatusAsync(entry.LocalPath, token);
            token.ThrowIfCancellationRequested();
            if (_isClosing || !_view.IsAvailable ||
                !ReferenceEquals(entry, _currentEntry) || !ReferenceEquals(service, _svnService))
                return;

            _workspace.ReplaceStatuses(entry.LocalPath, statuses, selectedPaths);
            _currentGridPath = _workspace.GetExistingRelativePath(entry.LocalPath, relativePath);
            RefreshCommitTargetGrid();
            _view.RebuildFolderTree(_currentEntry, _currentGridPath);
            NavigateGrid(_currentGridPath);
        }

        /// <summary>지정 파일과 부모 폴더 자체 상태만 조회하여 캐시와 커밋 선택을 갱신한다.</summary>
        private async Task ReloadPathsAsync(
            SvnService service,
            WorkingCopyEntry entry,
            IEnumerable<string> paths,
            CancellationToken token)
        {
            var queryPaths = GetRefreshPaths(entry.LocalPath, paths);

            if (queryPaths.Count == 0)
                return;

            var checkedPaths = _workspace.GetCheckedPaths();

            var statuses = await service.GetStatusForPathsAsync(
                queryPaths, token);

            token.ThrowIfCancellationRequested();

            if (_isClosing || !_view.IsAvailable ||
                !ReferenceEquals(service, _svnService) ||
                !ReferenceEquals(entry, _currentEntry))
            {
                return;
            }

            _workspace.MergeStatuses(entry.LocalPath, queryPaths, statuses, checkedPaths);

            RefreshCommitTargetGrid();
            NavigateGrid(_currentGridPath);

            // 트리를 재구성하지 않고 변경 표시만 다시 그린다.
            _view.InvalidateTree();
        }

        /// <summary>현재 SVN 서비스와 탐색 캐시, 커밋 선택 및 화면 내용을 초기화한다.</summary>
        private void ClearWorkingCopySelection()
        {
            _svnService?.Dispose();
            _svnService = null;
            _currentEntry = null;

            _currentGridPath = string.Empty;

            _workspace.Clear();

            _view.ClearWorkspace();

            RefreshCommitTargetGrid();
        }

        /// <summary>작업 복사본 경로를 대소문자 구분 없이 비교하여 목록 인덱스를 찾는다.</summary>
        private int FindWorkingCopyIndex(string localPath)
        {
            if (string.IsNullOrWhiteSpace(localPath))
                return -1;

            return _settings.WorkingCopies.FindIndex(entry =>
                string.Equals(
                    entry.LocalPath,
                    localPath,
                    StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>등록되지 않은 항목과 폴더를 제외하여 파일 로그 조회 가능 여부를 판단한다.</summary>
        public static bool CanViewFileLog(SvnFileStatus file)
        {
            return file != null &&
                   !file.IsDirectory &&
                   !string.IsNullOrWhiteSpace(file.FullPath) &&
                   file.Status != SvnStatusKind.None &&
                   file.Status != SvnStatusKind.Unversioned &&
                   file.Status != SvnStatusKind.Ignored;
        }

        /// <summary>파일의 SVN 상태를 기준으로 Diff 조회 가능 여부를 판단한다.</summary>
        public static bool CanViewDiff(SvnFileStatus file)
        {
            return file != null &&
                   !file.IsDirectory &&
                   (file.Status == SvnStatusKind.Normal ||
                    file.Status == SvnStatusKind.Modified ||
                    file.Status == SvnStatusKind.Added ||
                    file.Status == SvnStatusKind.Deleted ||
                    file.Status == SvnStatusKind.Conflicted);
        }

        /// <summary>누적 커밋 대상 목록을 기준으로 표시 항목의 체크 상태를 복원한다.</summary>
        private void RestoreCommitChecks(IEnumerable<SvnFileStatus> files)
        {
            _workspace.RestoreCommitChecks(files);
        }

        /// <summary>작업 가능 상태와 작업 복사본 선택 여부를 검사하고 필요한 안내를 표시한다.</summary>
        private bool CheckWorkingCopySelected()
        {
            if (_isBusy || _isClosing)
                return false;
            if (_currentEntry != null) return true;
            _view.ShowMessage("작업 폴더를 선택하세요.", "알림", MainNotice.Information);
            return false;
        }

        /// <summary>폴더 자체 또는 하위 항목에 SVN 변경이 있는지 판단한다.</summary>
        public bool HasFolderChanges(string folderPath)
        {
            return _workspace.HasFolderChanges(folderPath);
        }

        /// <summary>캐시된 SVN 관리 항목을 보존하면서 트리에 표시할 폴더인지 판단한다.</summary>
        public bool ShouldShowFolder(string folderPath)
        {
            return _workspace.ShouldShowPath(folderPath, true);
        }

        /// <summary>선택한 작업 복사본의 자격 증명을 적용한 SVN 서비스를 생성한다.</summary>
        private static SvnService CreateSvnService(WorkingCopyEntry entry)
        {
            return new SvnService(new SvnSettings
            {
                Username = entry.Username,
                EncryptedPassword = entry.EncryptedPassword
            });
        }

        /// <summary>같은 작업 복사본의 설정 변경은 캐시를 보존하고 선택이 바뀌는 경우에만 전체 로드한다.</summary>
        private async Task ApplySavedSettingsAsync(AppSettings saved)
        {
            var previous = _currentEntry;
            _settings = saved;
            int index = FindWorkingCopyIndex(previous?.LocalPath);

            if (previous != null && index >= 0)
            {
                var current = _settings.WorkingCopies[index];
                bool sameCredentials = string.Equals(previous.Username, current.Username, StringComparison.Ordinal)
                    && string.Equals(previous.EncryptedPassword, current.EncryptedPassword, StringComparison.Ordinal);
                if (!sameCredentials || _svnService == null)
                {
                    _svnService?.Dispose();
                    _svnService = CreateSvnService(current);
                }
                _currentEntry = current;
                _settings.LastSelectedWorkingCopy = current.LocalPath;
                PublishWorkingCopies(index);
                _view.UpdateWorkingCopyRoot(current);
                UpdateStatusBar();
                return;
            }

            ClearWorkingCopySelection();
            index = FindWorkingCopyIndex(_settings.LastSelectedWorkingCopy);
            PublishWorkingCopies(index);
            if (index >= 0)
                await SelectWorkingCopyAsync(index);
            else
            {
                _settings.LastSelectedWorkingCopy = string.Empty;
                UpdateStatusBar();
            }
        }

        #endregion
    }
}
