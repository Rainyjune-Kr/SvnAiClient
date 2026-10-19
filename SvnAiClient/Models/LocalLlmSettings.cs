namespace SvnAiClient.Models
{
    /// <summary>OpenAI 호환 로컬 서버의 주소, 모델 및 검수 요청 한도를 보관한다.</summary>
    public class LocalLlmSettings
    {
        #region Properties

        /// <summary>OpenAI 호환 로컬 AI 서버의 기본 URL.</summary>
        public string EndpointUrl { get; set; } = "http://localhost:11434";

        /// <summary>검수 요청에 사용할 모델 ID.</summary>
        public string Model { get; set; } = "llama3";

        /// <summary>검수 요청에 허용할 추정 입력 토큰 상한.</summary>
        public int InputTokenLimit { get; set; } = 2048;

        /// <summary>검수 응답에 허용할 최대 생성 토큰 수.</summary>
        public int MaxTokens { get; set; } = 4096;

        /// <summary>요청 제한 시간(초).</summary>
        public int TimeoutSeconds { get; set; } = 120;

        #endregion
    }
}
