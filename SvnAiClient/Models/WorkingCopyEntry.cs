using SvnAiClient.Helpers;

namespace SvnAiClient.Models
{
    /// <summary>등록된 작업 복사본의 로컬 경로와 표시 이름 및 암호화된 자격 증명을 보관한다.</summary>
    public class WorkingCopyEntry
    {
        #region Properties

        /// <summary>등록한 작업 복사본의 로컬 경로.</summary>
        public string LocalPath { get; set; } = string.Empty;

        /// <summary>작업 복사본 목록과 트리에 표시할 사용자 지정 이름.</summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>SVN 인증 사용자 이름.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>현재 사용자 DPAPI로 암호화한 SVN 비밀번호의 Base64 문자열.</summary>
        public string EncryptedPassword { get; set; } = string.Empty;

        #endregion

        #region Public Methods

        /// <summary>사용자 지정 이름이 없으면 로컬 폴더 이름을 반환한다.</summary>
        public string GetDisplayName()
        {
            if (!string.IsNullOrEmpty(DisplayName))
                return DisplayName;

            if (!string.IsNullOrEmpty(LocalPath))
                return System.IO.Path.GetFileName(LocalPath.TrimEnd('\\', '/'));

            return "(이름 없음)";
        }

        /// <summary>입력한 SVN 비밀번호를 DPAPI로 암호화하여 보관한다.</summary>
        public void SetPassword(string plainPassword)
        {
            EncryptedPassword = DpapiHelper.Encrypt(plainPassword);
        }

        #endregion
    }
}
