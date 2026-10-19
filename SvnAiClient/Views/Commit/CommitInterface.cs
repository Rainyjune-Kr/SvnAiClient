using System.Collections.Generic;
using SvnAiClient.Models;

namespace SvnAiClient.Views.Commit
{
    /// <summary>커밋 검수 화면의 입력, 결과 표시 및 승인 요청에 대한 UI 독립 계약.</summary>
    public interface CommitInterface
    {
        #region Properties

        /// <summary>화면이 폐기되지 않아 결과를 표시할 수 있는지 여부.</summary>
        bool IsAvailable { get; }

        /// <summary>화면에서 선택한 AI 프로바이더 이름.</summary>
        string SelectedProvider { get; }

        /// <summary>개발자가 입력한 변경 배경과 검수 요청 내용.</summary>
        string ChangeContext { get; }

        /// <summary>사용자가 편집 중인 최종 커밋 메시지.</summary>
        string CommitMessageText { get; set; }

        /// <summary>현재 검수 결과가 제안한 커밋 메시지.</summary>
        string SuggestedMessage { get; set; }

        /// <summary>진행 단계와 검수 결과를 표시하는 상태 문구.</summary>
        string ReviewStatus { get; set; }

        #endregion

        #region View Contract

        void ShowFiles(List<SvnFileStatus> files, string provider);
        void RefreshFiles(List<SvnFileStatus> files);
        void PostFileEdits();
        void SetActionsEnabled(bool enabled);
        void ClearReview();
        void ShowReviewResult(AiReviewResult result);
        void ShowMessage(string message, string title, CommitNotice kind);
        bool Confirm(string message, string title);
        void ShowIssueDiff(string path, string diff, int? lineNumber);
        void ShowHeadWorkingDiff(string path, string diff);
        void CompleteCommit();
        void CancelCommit();

        #endregion
    }

    /// <summary>UI 라이브러리와 독립적인 검수 화면 알림의 종류.</summary>
    public enum CommitNotice
    {
        Information,
        Warning,
        Error
    }
}
