using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SvnAiClient.Core.Svn;
using SvnAiClient.Models;

namespace SvnAiClient.Views.LogViewer
{
    /// <summary>SVN 로그 조회, 비교 범위 및 비동기 작업의 취소 상태를 관리한다.</summary>
    public sealed class LogViewerPresenter
    {
        #region Fields

        /// <summary>로그 조회와 리비전 비교 중 발생한 오류를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>선택 항목을 읽고 조회 결과의 표시를 요청하는 화면 계약.</summary>
        private readonly LogViewerInterface _view;

        /// <summary>호출 화면에서 전달한 SVN 서비스. 수명은 호출 화면이 관리한다.</summary>
        private readonly SvnService _service;

        /// <summary>로그와 리비전 비교의 범위가 되는 작업 복사본의 폴더 또는 파일 경로.</summary>
        private readonly string _targetPath;

        /// <summary>현재 조회 작업의 취소 요청을 관리하며 작업이 끝나면 해제한다.</summary>
        private CancellationTokenSource _cts;

        /// <summary>화면 종료 후 새 작업과 결과 표시를 방지한다.</summary>
        private bool _closing;

        /// <summary>현재 변경 경로 목록에 적용된 비교 시작 리비전.</summary>
        private long? _comparisonFrom;

        /// <summary>현재 변경 경로 목록에 적용된 비교 종료 리비전.</summary>
        private long? _comparisonTo;

        #endregion

        #region Properties

        /// <summary>로그, 변경 목록 또는 Diff를 조회 중인지 여부.</summary>
        private bool IsBusy
        {
            get { return _cts != null; }
        }

        /// <summary>종료 처리가 시작되지 않았고 화면에 결과를 표시할 수 있는지 여부.</summary>
        private bool CanUpdateView
        {
            get { return !_closing && _view.IsAvailable; }
        }

        #endregion

        #region Initialization

        public LogViewerPresenter(LogViewerInterface view, string targetPath, SvnService service)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (string.IsNullOrWhiteSpace(targetPath))
                throw new ArgumentException("작업 폴더 경로가 필요합니다.", nameof(targetPath));
            if (service == null) throw new ArgumentNullException(nameof(service));

            _view = view;
            _targetPath = targetPath;
            _service = service;
        }

        #endregion

        #region View Requests

        public async Task LoadLogsAsync()
        {
            if (IsBusy || !CanUpdateView) return;

            int count = _view.RequestedCount;
            var cancellation = BeginOperation("저장소 이력 조회 중...");
            try
            {
                var logs = await _service.GetLogAsync(_targetPath, count, cancellation.Token);
                if (!CanUpdateView) return;

                logs.Sort((left, right) => right.Revision.CompareTo(left.Revision));
                _view.ShowLogs(logs);
                ShowFocusedLog();
                _view.SetStatus("조회 완료: " + logs.Count + "건");
            }
            catch (OperationCanceledException)
            {
                if (CanUpdateView)
                    _view.SetStatus("조회가 중단되었습니다.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SVN Log 조회 실패");
                if (CanUpdateView)
                {
                    _view.SetStatus("조회 실패");
                    _view.ShowError("이력 조회 실패:\n" + ex.Message);
                }
            }
            finally
            {
                EndOperation(cancellation);
            }
        }

        public void SelectionChanged()
        {
            if (IsBusy || !CanUpdateView) return;

            ShowFocusedLog();
            UpdateCompareButton();
        }

        public async Task OpenRevisionAsync(SvnLogEntry entry)
        {
            if (IsBusy || !CanUpdateView || entry == null) return;
            if (entry.Revision < 1)
            {
                _view.ShowInformation("이 리비전의 이전 시점은 비교할 수 없습니다.");
                return;
            }

            await LoadRevisionRangeAndOpenAsync(entry.Revision - 1, entry.Revision, entry.Message);
        }

        public async Task CompareSelectedRevisionsAsync()
        {
            if (IsBusy || !CanUpdateView) return;

            var selected = GetComparisonSelection();
            if (selected.Count != 2) return;

            var oldLog = selected[0];
            var newLog = selected[1];
            string message = "[r" + oldLog.Revision + "]\r\n" + oldLog.Message
                + "\r\n\r\n[r" + newLog.Revision + "]\r\n" + newLog.Message;

            await LoadRevisionRangeAndOpenAsync(oldLog.Revision, newLog.Revision, message);
        }

        public async Task OpenSelectedRevisionDiffAsync()
        {
            if (IsBusy || !CanUpdateView) return;

            var entry = _view.FocusedLog;
            var change = _view.SelectedChange;
            if (entry == null || change == null) return;

            if (!_comparisonFrom.HasValue && GetComparisonSelection().Count > 1)
            {
                _view.ShowInformation("두 로그를 선택한 뒤 '선택 리비전 비교'를 눌러주세요.");
                return;
            }

            if (change.IsDirectory)
            {
                _view.ShowInformation("파일을 선택하세요. 폴더 비교는 아직 지원하지 않습니다.");
                return;
            }

            long fromRevision = _comparisonFrom ?? (entry.Revision - 1);
            long toRevision = _comparisonTo ?? entry.Revision;
            string range = GetRangeText(fromRevision, toRevision);
            var cancellation = BeginOperation(range + " 변경 내용 조회 중...");

            try
            {
                string diff = await _service.GetRevisionDiffAsync(
                    _targetPath, change, fromRevision, toRevision, cancellation.Token);
                if (!CanUpdateView) return;

                _view.ShowRevisionDiff(change.Path, diff, fromRevision, toRevision);
                if (CanUpdateView)
                    _view.SetStatus(range + " 비교 완료");
            }
            catch (OperationCanceledException)
            {
                if (CanUpdateView)
                    _view.SetStatus("Diff 조회가 중단되었습니다.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "리비전 Diff 조회 실패");
                if (CanUpdateView)
                {
                    _view.SetStatus("Diff 조회 실패");
                    _view.ShowError(ex.Message);
                }
            }
            finally
            {
                EndOperation(cancellation);
            }
        }

        public void Cancel()
        {
            _cts?.Cancel();
        }

        public void Close()
        {
            _closing = true;
            Cancel();
        }

        #endregion

        #region View Updates

        private void ShowFocusedLog()
        {
            _comparisonFrom = null;
            _comparisonTo = null;
            _view.ShowLogDetails(_view.FocusedLog);
        }

        private void UpdateCompareButton()
        {
            _view.SetCompareEnabled(!IsBusy && GetComparisonSelection().Count == 2);
        }

        #endregion

        #region Operations

        /// <summary>비교 범위의 변경 목록을 조회하고 첫 번째 파일의 Diff를 표시한다.</summary>
        private async Task LoadRevisionRangeAndOpenAsync(
            long fromRevision, long toRevision, string message)
        {
            if (IsBusy || !CanUpdateView) return;

            _comparisonFrom = null;
            _comparisonTo = null;
            _view.ClearChanges();
            SvnChangedPath firstFile = null;
            var cancellation = BeginOperation(GetRangeText(fromRevision, toRevision) + " 차이 조회 중...");

            try
            {
                var changes = await _service.GetRevisionChangesAsync(
                    _targetPath, fromRevision, toRevision, cancellation.Token);
                if (!CanUpdateView) return;

                _comparisonFrom = fromRevision;
                _comparisonTo = toRevision;
                _view.ShowComparison(fromRevision, toRevision, message, changes);
                firstFile = changes.FirstOrDefault(change => !change.IsDirectory);
                _view.SetStatus(changes.Count == 0
                    ? "두 시점 사이에 변경 내용이 없습니다."
                    : firstFile == null
                        ? "폴더 변경만 있어 파일 Diff를 열 수 없습니다."
                        : "첫 번째 변경 파일의 Diff를 여는 중...");
            }
            catch (OperationCanceledException)
            {
                if (CanUpdateView)
                {
                    ShowFocusedLog();
                    _view.SetStatus("리비전 비교 조회가 중단되었습니다.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "리비전 간 변경 목록 조회 실패");
                if (CanUpdateView)
                {
                    ShowFocusedLog();
                    _view.SetStatus("리비전 비교 조회 실패");
                    _view.ShowError(ex.Message);
                }
            }
            finally
            {
                EndOperation(cancellation);
            }

            // 변경 목록 조회를 끝내고 취소 토큰을 해제한 후 다음 조회를 시작한다.
            if (firstFile != null && CanUpdateView)
            {
                _view.SelectChange(firstFile);
                await OpenSelectedRevisionDiffAsync();
            }
        }

        /// <summary>선택한 로그에서 중복 리비전을 제거하고 오래된 리비전부터 정렬한다.</summary>
        private List<SvnLogEntry> GetComparisonSelection()
        {
            return _view.SelectedLogs
                .Where(entry => entry != null)
                .GroupBy(entry => entry.Revision)
                .Select(group => group.First())
                .OrderBy(entry => entry.Revision)
                .ToList();
        }

        /// <summary>새 조회의 취소 토큰을 만들고 화면을 작업 중 상태로 전환한다.</summary>
        private CancellationTokenSource BeginOperation(string message)
        {
            var cancellation = new CancellationTokenSource();
            _cts = cancellation;
            _view.SetBusy(true);
            _view.SetStatus(message);
            return cancellation;
        }

        /// <summary>완료한 조회의 취소 토큰을 해제하고 사용 가능한 화면의 조작 상태를 복원한다.</summary>
        private void EndOperation(CancellationTokenSource cancellation)
        {
            if (ReferenceEquals(_cts, cancellation))
                _cts = null;
            cancellation.Dispose();

            if (CanUpdateView)
            {
                _view.SetBusy(false);
                UpdateCompareButton();
            }
        }

        /// <summary>비교 시작과 종료 리비전을 상태 표시용 문자열로 변환한다.</summary>
        private static string GetRangeText(long fromRevision, long toRevision)
        {
            return "r" + fromRevision + " → r" + toRevision;
        }

        #endregion
    }
}
