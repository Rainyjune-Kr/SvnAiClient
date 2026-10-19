using SvnAiClient.Models;

namespace SvnAiClient.Views.AddWorkingCopy
{
    /// <summary>작업 복사본 등록·편집 화면의 입력과 승인 결과에 대한 계약.</summary>
    public interface AddWorkingCopyInterface
    {
        #region Properties

        /// <summary>입력하거나 선택한 작업 복사본의 로컬 경로.</summary>
        string LocalPath { get; set; }

        /// <summary>작업 복사본 목록에 표시할 이름.</summary>
        string DisplayName { get; set; }

        /// <summary>SVN 인증에 사용할 사용자명.</summary>
        string Username { get; set; }

        /// <summary>새로 입력한 비밀번호. 편집 시 비어 있으면 기존 값을 유지한다.</summary>
        string Password { get; }

        #endregion

        #region View Contract

        string SelectFolder(string initialPath);
        void ShowExisting(WorkingCopyEntry entry);
        void ShowValidation(string message);
        void FocusLocalPath();
        void Complete(WorkingCopyEntry entry);
        void Cancel();

        #endregion
    }
}
