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
    /// <summary>Claude 규격으로 AI 검수를 수행한다.</summary>
    public class ClaudeAiProvider : IAiProvider
    {
        #region Fields

        /// <summary>검수 요청 실패 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>요청별 시간 제한을 적용하며 재사용하는 HTTP 클라이언트.</summary>
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };

        /// <summary>프로바이더 API 요청 주소.</summary>
        private const string ApiUrl = "https://api.anthropic.com/v1/messages";

        /// <summary>Claude 요청에 적용할 API 버전.</summary>
        private const string AnthropicVersion = "2023-06-01";

        /// <summary>모델과 연결 및 요청 한도에 사용할 프로바이더 설정.</summary>
        private readonly ClaudeSettings _settings;

        #endregion

        #region Properties

        /// <summary>검수 결과에 기록할 프로바이더 표시 이름.</summary>
        public string ProviderName => "Claude";

        /// <summary>검수 요청에 필요한 필수 설정의 구성 여부.</summary>
        public bool IsConfigured => !string.IsNullOrEmpty(_settings.EncryptedApiKey);

        #endregion

        #region Constructors

        public ClaudeAiProvider(ClaudeSettings settings)
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
                var apiKey = DpapiHelper.DecryptSafe(_settings.EncryptedApiKey);
                if (string.IsNullOrEmpty(apiKey))
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "API Key가 설정되지 않았습니다.";
                    return result;
                }

                var requestBody = new
                {
                    model = _settings.Model,
                    max_tokens = _settings.MaxTokens,
                    system = prompt,
                    messages = new[]
                    {
                        new { role = "user", content = diffContent }
                    }
                };

                var body = JObject.FromObject(requestBody);
                var preset = AiModelCatalog.Find("Claude", _settings.Model);
                if (preset != null)
                {
                    body["thinking"] = new JObject { ["type"] = "adaptive" };
                    body["output_config"] = new JObject { ["effort"] = preset.Effort };
                }
                var json = body.ToString(Formatting.None);
                var responseJson = await AiHttpTransport.PostJsonAsync(
                    HttpClient,
                    ProviderName,
                    ApiUrl,
                    json,
                    _settings.TimeoutSeconds,
                    cancellationToken,
                    request =>
                    {
                        request.Headers.Add("x-api-key", apiKey);
                        request.Headers.Add("anthropic-version", AnthropicVersion);
                    },
                    apiKey);

                var jObj = JObject.Parse(responseJson);
                var usage = jObj["usage"] as JObject;

                result.ActualInputTokens = TokenUsageParser.SumTokenCounts(
                    usage,
                    "input_tokens",
                    "cache_creation_input_tokens",
                    "cache_read_input_tokens");

                result.ActualOutputTokens = TokenUsageParser.ReadTokenCount(
                    usage?["output_tokens"]);

                var text = ReadClaudeText(jObj);

                ReviewResponseParser.Parse(text, result);
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
                    "Claude ReviewCodeAsync 오류: {0}",
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

        /// <summary>생성 한도 중단 여부를 확인하고 Claude 응답의 텍스트 블록을 조합한다.</summary>
        private static string ReadClaudeText(JObject response)
        {
            if ((string)response["stop_reason"] == "max_tokens")
                throw new InvalidOperationException("Claude 응답이 생성 토큰 한도에서 중단되었습니다.");
            var text = new StringBuilder();
            var blocks = response["content"] as JArray;
            if (blocks != null)
                foreach (var block in blocks)
                    if ((string)block["type"] == "text")
                        text.Append((string)block["text"]);
            return text.ToString().Trim();
        }

        #endregion
    }
}
