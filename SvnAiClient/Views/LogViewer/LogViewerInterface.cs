using System.Collections.Generic;
using SvnAiClient.Models;

namespace SvnAiClient.Views.LogViewer
{
    /// <summary>SVN 로그 화면의 입력과 표시 기능. UI 라이브러리 형식은 노출하지 않는다.</summary>
    public interface LogViewerInterface
    {
        #region Properties

        /// <summary>화면이 폐기되지 않아 결과를 표시할 수 있는지 여부.</summary>
        bool IsAvailable { get; }

        /// <summary>사용자가 지정한 최대 로그 조회 건수.</summary>
        int RequestedCount { get; }

        /// <summary>로그 목록에서 포커스된 항목. 항목이 없으면 null이다.</summary>
        SvnLogEntry FocusedLog { get; }

        /// <summary>로그 목록에서 선택한 항목들.</summary>
        List<SvnLogEntry> SelectedLogs { get; }

        /// <summary>변경 경로 목록에서 선택한 항목. 선택하지 않았으면 null이다.</summary>
        SvnChangedPath SelectedChange { get; }

        #endregion

        #region View Contract

        void ShowLogs(List<SvnLogEntry> logs);
        void ShowLogDetails(SvnLogEntry entry);
        void ShowComparison(long fromRevision, long toRevision,
            string message, List<SvnChangedPath> changes);
        void ClearChanges();
        void SelectChange(SvnChangedPath change);
        void SetBusy(bool busy);
        void SetCompareEnabled(bool enabled);
        void SetStatus(string message);
        void ShowInformation(string message);
        void ShowError(string message);
        void ShowRevisionDiff(string path, string diff, long fromRevision, long toRevision);

        #endregion
    }
}
