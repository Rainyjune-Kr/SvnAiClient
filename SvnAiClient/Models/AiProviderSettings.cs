namespace SvnAiClient.Models
{
    /// <summary>AI 프로바이더별 연결 및 검수 요청 설정을 보관한다.</summary>
    public class AiProviderSettings
    {
        #region Properties

        /// <summary>Claude 프로바이더 설정.</summary>
        public ClaudeSettings Claude { get; set; } = new ClaudeSettings();

        /// <summary>OpenAI 프로바이더 설정.</summary>
        public OpenAiSettings OpenAi { get; set; } = new OpenAiSettings();

        /// <summary>Gemini 프로바이더 설정.</summary>
        public GeminiSettings Gemini { get; set; } = new GeminiSettings();

        /// <summary>로컬 AI 서버 설정.</summary>
        public LocalLlmSettings LocalLlm { get; set; } = new LocalLlmSettings();

        #endregion
    }
}
