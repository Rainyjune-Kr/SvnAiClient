using System;
using NLog;
using SvnAiClient.Core.AI.Providers;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>프로바이더 이름과 설정에 맞는 AI 클라이언트를 생성한다.</summary>
    public class AiProviderFactory
    {
        #region Fields

        /// <summary>프로바이더 생성 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>프로바이더 생성에 사용할 애플리케이션 설정.</summary>
        private readonly AppSettings _settings;

        #endregion

        #region Constructors

        public AiProviderFactory(AppSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
        }

        #endregion

        #region Public Methods

        /// <summary>지정한 프로바이더 이름에 해당하는 설정으로 클라이언트를 생성한다.</summary>
        public IAiProvider CreateProvider(string providerName)
        {
            Logger.Debug($"AI 프로바이더 생성: {providerName}");

            switch (providerName)
            {
                case "Claude":
                    return new ClaudeAiProvider(_settings.AiProviders.Claude);
                case "OpenAI":
                    return new OpenAiProvider(_settings.AiProviders.OpenAi);
                case "Gemini":
                    return new GeminiAiProvider(_settings.AiProviders.Gemini);
                case "Local":
                    return new LocalLlmProvider(_settings.AiProviders.LocalLlm);
                default:
                    throw new NotSupportedException($"지원하지 않는 AI 프로바이더: {providerName}");
            }
        }

        #endregion
    }
}
