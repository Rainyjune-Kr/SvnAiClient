using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>선택 파일 전체의 검수 입력을 구성하고 한도 확인 후 AI 검수를 요청한다.</summary>
    public class AiReviewService
    {
        #region Fields

        /// <summary>검수 요청을 실행할 AI 프로바이더.</summary>
        private readonly IAiProvider _provider;

        /// <summary>입력 및 응답 한도와 모델 검증에 사용할 설정.</summary>
        private readonly AppSettings _settings;

        /// <summary>검수 기준과 개발자 문맥을 조합하는 프롬프트 생성기.</summary>
        private readonly PromptBuilder _prompts;

        #endregion

        #region Properties

        /// <summary>선택된 프로바이더의 입력 토큰 한도.</summary>
        private int InputLimit
        {
            get
            {
                switch (_provider.ProviderName)
                {
                    case "OpenAI": return _settings.AiProviders.OpenAi.InputTokenLimit;
                    case "Claude": return _settings.AiProviders.Claude.InputTokenLimit;
                    case "Gemini": return _settings.AiProviders.Gemini.InputTokenLimit;
                    default: return _settings.AiProviders.LocalLlm.InputTokenLimit;
                }
            }
        }

        /// <summary>선택된 프로바이더의 최대 응답 토큰 수.</summary>
        private int OutputLimit
        {
            get
            {
                switch (_provider.ProviderName)
                {
                    case "OpenAI": return _settings.AiProviders.OpenAi.MaxTokens;
                    case "Claude": return _settings.AiProviders.Claude.MaxTokens;
                    case "Gemini": return _settings.AiProviders.Gemini.MaxTokens;
                    default: return _settings.AiProviders.LocalLlm.MaxTokens;
                }
            }
        }

        #endregion

        #region Constructors

        public AiReviewService(IAiProvider provider, AppSettings settings)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _provider = provider;
            _settings = settings;
            _prompts = new PromptBuilder(settings);
        }

        #endregion

        #region Public Methods

        /// <summary>전체 소스와 Diff를 통합 요청으로 검수하고 추정 사용량 및 처리 상태를 기록한다.</summary>
        public async Task<AiReviewResult> ReviewAsync(
            List<AiReviewFile> files,
            CancellationToken cancellationToken = default(CancellationToken),
            string changeContext = null)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("검수할 파일이 없습니다.", nameof(files));

            string prompt = _prompts.BuildReviewSystemPrompt(changeContext);

            var prepared = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                string content = JsonConvert.SerializeObject(new { files });
                int estimate = checked(EstimateInput(prompt) + EstimateInput(content) + 2048);
                return new { Content = content, Estimate = estimate };
            }, cancellationToken);

            var result = new AiReviewResult { ProviderName = _provider.ProviderName };
            string error = null;

            if (InputLimit <= 0 || OutputLimit <= 0)
                error = "입력 및 응답 토큰 설정이 올바르지 않습니다.";
            else
            {
                string model = _provider.ProviderName == "OpenAI"
                    ? _settings.AiProviders.OpenAi.Model
                    : _provider.ProviderName == "Claude" ? _settings.AiProviders.Claude.Model
                    : _provider.ProviderName == "Gemini" ? _settings.AiProviders.Gemini.Model
                    : _settings.AiProviders.LocalLlm.Model;

                error = AiModelCatalog.Validate(_provider.ProviderName, model, InputLimit, OutputLimit);
            }

            if (error == null && prepared.Estimate > InputLimit)
                error = "전체 파일 통합 검수 입력이 한도를 초과합니다. 추정 입력 "
                    + prepared.Estimate.ToString("N0") + " / 설정 한도 "
                    + InputLimit.ToString("N0") + " 토큰. 파일을 자르거나 일부만 전송하지 않았습니다.";

            if (error != null)
            {
                result.Status = AiReviewStatus.Failed;
                result.FailureKind = AiReviewFailureKind.InputTooLarge;
                result.ErrorMessage = error;
            }
            else
            {
                result = await _provider.ReviewCodeAsync(prompt, prepared.Content, cancellationToken);

                if (result == null)
                    throw new InvalidOperationException("AI가 검수 결과를 반환하지 않았습니다.");
            }

            result.EstimatedInputTokens = prepared.Estimate;
            result.Batches.Clear();
            result.Batches.Add(new AiReviewBatch
            {
                FilePaths = files.ConvertAll(file => file.FilePath),
                ScopeDescription = "선택 파일 전체 소스 통합 검수",
                Status = result.Status,
                FailureKind = result.FailureKind,
                ErrorMessage = result.ErrorMessage,
                SuggestedCommitMessage = result.SuggestedCommitMessage
            });

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>UTF-8 바이트 수로 입력 토큰을 추정하며 모델의 실제 토큰 수와는 다를 수 있다.</summary>
        private static int EstimateInput(string text)
        {
            return (int)Math.Ceiling(Encoding.UTF8.GetByteCount(text) / 3.0);
        }

        #endregion
    }
}
