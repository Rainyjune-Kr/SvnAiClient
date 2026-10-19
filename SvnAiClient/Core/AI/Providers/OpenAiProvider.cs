using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SvnAiClient.Helpers;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI.Providers
{
    /// <summary>OpenAI 규격으로 AI 검수를 수행한다.</summary>
    public class OpenAiProvider : IAiProvider
    {
        #region Fields

        /// <summary>검수 요청 실패 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>요청별 시간 제한을 적용하며 재사용하는 HTTP 클라이언트.</summary>
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };

        /// <summary>프로바이더 API 요청 주소.</summary>
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

        /// <summary>모델과 연결 및 요청 한도에 사용할 프로바이더 설정.</summary>
        private readonly OpenAiSettings _settings;

        #endregion

        #region Properties

        /// <summary>검수 결과에 기록할 프로바이더 표시 이름.</summary>
        public string ProviderName => "OpenAI";

        /// <summary>검수 요청에 필요한 필수 설정의 구성 여부.</summary>
        public bool IsConfigured => !string.IsNullOrEmpty(_settings.EncryptedApiKey);

        #endregion

        #region Constructors

        public OpenAiProvider(OpenAiSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
        }

        #endregion

        #region Public Methods

        /// <summary>AI 검수 결과와 토큰 사용량, 취소 및 실패 상태와 소요 시간을 기록한다.</summary>
        public async Task<AiReviewResult> ReviewCodeAsync(
            string prompt, string diffContent,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new AiReviewResult { ProviderName = ProviderName };
            var sw = Stopwatch.StartNew();

            try
            {
                var responseText = await CallApiAsync(prompt, diffContent, _settings.MaxTokens, cancellationToken,
                    (input, output) =>
                    {
                        result.ActualInputTokens = input;
                        result.ActualOutputTokens = output;
                    });
                if (responseText == null)
                {
                    result.IsSuccess = false;
                    result.FailureKind = AiReviewFailureKind.ProviderError;
                    result.ErrorMessage = "API 호출 실패.";
                    return result;
                }
                ReviewResponseParser.Parse(responseText, result);
            }
            catch (OperationCanceledException)
            {
                bool cancelled = cancellationToken.IsCancellationRequested;

                result.Status = cancelled
                    ? AiReviewStatus.Cancelled
                    : AiReviewStatus.Failed;

                result.FailureKind = cancelled
                    ? AiReviewFailureKind.Cancelled
                    : AiReviewFailureKind.Timeout;

                result.ErrorMessage = cancelled
                    ? "검수 취소됨"
                    : "요청 시간이 초과되었습니다.";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.FailureKind = AiReviewFailureKind.ProviderError;

                result.ErrorMessage = AiHttpTransport.FormatException(
                    ex,
                    DpapiHelper.DecryptSafe(_settings.EncryptedApiKey));

                Logger.Error(
                    "OpenAI ReviewCodeAsync 오류: {0}",
                    result.ErrorMessage);
            }
            finally
            {
                sw.Stop();
                result.ElapsedMs = sw.ElapsedMilliseconds;
                result.ReviewedAt = DateTime.Now;
            }

            return result;
        }

        #endregion

        #region Internal Methods

        /// <summary>프로바이더 규격으로 요청을 전송하고 사용량 및 응답 텍스트를 읽는다.</summary>
        internal async Task<string> CallApiAsync(
            string systemPrompt, string userContent, int maxTokens,
            CancellationToken cancellationToken,
            Action<int?, int?> usage = null)
        {
            var apiKey = DpapiHelper.DecryptSafe(_settings.EncryptedApiKey);
            if (string.IsNullOrEmpty(apiKey)) return null;

            var messages = new JArray();
            if (!string.IsNullOrEmpty(systemPrompt))
                messages.Add(new JObject { ["role"] = "system", ["content"] = systemPrompt });
            messages.Add(new JObject { ["role"] = "user", ["content"] = userContent });
            var requestBody = new JObject
            {
                ["model"] = _settings.Model,
                ["max_completion_tokens"] = maxTokens,
                ["messages"] = messages
            };
            var preset = AiModelCatalog.Find("OpenAI", _settings.Model);
            if (preset != null && preset.Efforts.Length > 0)
            {
                string effort = _settings.ReasoningEffort;
                if (Array.IndexOf(preset.Efforts, effort) < 0)
                    throw new InvalidOperationException("이 모델에서 지원하지 않는 추론 강도입니다.");
                requestBody["reasoning_effort"] = effort;
            }

            var json = JsonConvert.SerializeObject(requestBody);
            var responseJson = await AiHttpTransport.PostJsonAsync(
                HttpClient,
                ProviderName,
                ApiUrl,
                json,
                _settings.TimeoutSeconds,
                cancellationToken,
                request => request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey),
                apiKey);

            var jObj = JObject.Parse(responseJson);
            usage?.Invoke((int?)jObj["usage"]?["prompt_tokens"],
                (int?)jObj["usage"]?["completion_tokens"]);
            if ((string)jObj["choices"]?[0]?["finish_reason"] == "length")
                throw new InvalidOperationException(
                    "AI 응답이 최대 응답 토큰에 도달하여 중단되었습니다. 응답 한도를 늘려 다시 검수하세요.");
            return jObj["choices"]?[0]?["message"]?["content"]?.ToString()?.Trim();
        }

        #endregion
    }
}
