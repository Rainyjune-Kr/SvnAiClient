using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SvnAiClient.Helpers;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI.Providers
{
    /// <summary>Gemini 규격으로 AI 검수를 수행한다.</summary>
    public class GeminiAiProvider : IAiProvider
    {
        #region Fields

        /// <summary>검수 요청 실패 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>요청별 시간 제한을 적용하며 재사용하는 HTTP 클라이언트.</summary>
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };

        /// <summary>프로바이더 API 요청 주소.</summary>
        private const string ApiUrlFormat =
            "https://generativelanguage.googleapis.com/v1beta/models/{0}:generateContent?key={1}";

        /// <summary>모델과 연결 및 요청 한도에 사용할 프로바이더 설정.</summary>
        private readonly GeminiSettings _settings;

        #endregion

        #region Properties

        /// <summary>검수 결과에 기록할 프로바이더 표시 이름.</summary>
        public string ProviderName => "Gemini";

        /// <summary>검수 요청에 필요한 필수 설정의 구성 여부.</summary>
        public bool IsConfigured => !string.IsNullOrEmpty(_settings.EncryptedApiKey);

        #endregion

        #region Constructors

        public GeminiAiProvider(GeminiSettings settings)
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
                var fullContent = string.IsNullOrEmpty(prompt)
                    ? diffContent
                    : $"{prompt}\n\n{diffContent}";

                var responseText = await CallApiAsync(
                    fullContent,
                    _settings.MaxTokens,
                    cancellationToken,
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
                    "Gemini ReviewCodeAsync 오류: {0}",
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

        #region Private Methods

        /// <summary>프로바이더 규격으로 요청을 전송하고 사용량 및 응답 텍스트를 읽는다.</summary>
        private async Task<string> CallApiAsync(
            string content,
            int maxTokens,
            CancellationToken cancellationToken,
            Action<int?, int?> usage = null)
        {
            var apiKey = DpapiHelper.DecryptSafe(_settings.EncryptedApiKey);
            if (string.IsNullOrEmpty(apiKey)) return null;

            var url = string.Format(ApiUrlFormat, _settings.Model, apiKey);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = content }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = maxTokens
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var responseJson = await AiHttpTransport.PostJsonAsync(
                HttpClient,
                ProviderName,
                url,
                json,
                _settings.TimeoutSeconds,
                cancellationToken,
                null,
                apiKey);
            var jObj = JObject.Parse(responseJson);
            var metadata = jObj["usageMetadata"] as JObject;

            usage?.Invoke(
                TokenUsageParser.SumTokenCounts(
                    metadata,
                    "promptTokenCount",
                    "toolUsePromptTokenCount"),
                TokenUsageParser.SumTokenCounts(
                    metadata,
                    "candidatesTokenCount",
                    "thoughtsTokenCount"));
            var candidate = jObj["candidates"]?[0];
            if ((string)candidate?["finishReason"] == "MAX_TOKENS")
                throw new InvalidOperationException("Gemini 응답이 생성 토큰 한도에서 중단되었습니다.");
            var text = new StringBuilder();
            var parts = candidate?["content"]?["parts"] as JArray;
            if (parts != null)
                foreach (var part in parts)
                    if ((bool?)part["thought"] != true && part["text"] != null)
                        text.Append((string)part["text"]);
            return text.ToString().Trim();
        }

        #endregion
    }
}
