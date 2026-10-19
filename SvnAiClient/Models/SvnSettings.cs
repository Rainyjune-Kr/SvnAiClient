namespace SvnAiClient.Models
{
    /// <summary>SVN 저장소 연결과 암호화된 자격 증명 및 작업 설정을 보관한다.</summary>
    public class SvnSettings
    {
        #region Properties

        /// <summary>SVN 저장소 URL.</summary>
        public string RepositoryUrl { get; set; } = string.Empty;

        /// <summary>SVN 인증 사용자 이름.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>현재 사용자 DPAPI로 암호화한 SVN 비밀번호의 Base64 문자열.</summary>
        public string EncryptedPassword { get; set; } = string.Empty;

        /// <summary>SVN 서버의 SSL 인증서 오류를 무시할지 여부.</summary>
        public bool IgnoreSslErrors { get; set; } = false;

        /// <summary>SVN 작업 제한 시간 설정값(초).</summary>
        public int TimeoutSeconds { get; set; } = 30;

        #endregion
    }
}
