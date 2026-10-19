namespace SvnAiClient.Models
{
    /// <summary>Gemini 검수 요청에 사용할 모델, 암호화된 API 키 및 요청 한도를 보관한다.</summary>
    public class GeminiSettings
    {
        #region Properties

        /// <summary>현재 사용자 DPAPI로 암호화한 API 키의 Base64 문자열.</summary>
        public string EncryptedApiKey { get; set; } = string.Empty;

        /// <summary>검수 요청에 사용할 모델 ID.</summary>
        public string Model { get; set; } = "gemini-3.8-flash";

        /// <summary>검수 요청에 허용할 추정 입력 토큰 상한.</summary>
        public int InputTokenLimit { get; set; } = 240000;

        /// <summary>검수 응답에 허용할 최대 생성 토큰 수.</summary>
        public int MaxTokens { get; set; } = 32768;

        /// <summary>요청 제한 시간(초).</summary>
        public int TimeoutSeconds { get; set; } = 300;

        #endregion
    }
}
