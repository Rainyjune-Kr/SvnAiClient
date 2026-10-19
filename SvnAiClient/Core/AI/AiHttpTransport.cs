using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SvnAiClient.Core.AI
{
    /// <summary>AI JSON 요청의 시간 제한, 취소, 리소스 해제 및 오류 메시지 정리를 담당한다.</summary>
    internal static class AiHttpTransport
    {
        #region Public Methods

        /// <summary>취소와 시간 제한을 적용해 JSON을 전송하고 응답 본문을 반환한다.</summary>
        public static async Task<string> PostJsonAsync(
            HttpClient client,
            string providerName,
            string url,
            string json,
            int timeoutSeconds,
            CancellationToken cancellationToken,
            Action<HttpRequestMessage> configureHeaders = null,
            params string[] secrets)
        {
            if (timeoutSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timeoutSeconds),
                    "요청 제한 시간은 1초 이상이어야 합니다.");
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var linked =
                    CancellationTokenSource.CreateLinkedTokenSource(
                        cancellationToken))
                using (var request =
                    new HttpRequestMessage(HttpMethod.Post, url))
                {
                    linked.CancelAfter(
                        TimeSpan.FromSeconds(timeoutSeconds));

                    request.Content = new StringContent(
                        json, Encoding.UTF8, "application/json");

                    configureHeaders?.Invoke(request);

                    // 본문 수신까지 동일한 취소/시간 제한을 적용한다.
                    using (var response = await client.SendAsync(
                        request,
                        HttpCompletionOption.ResponseContentRead,
                        linked.Token).ConfigureAwait(false))
                    {
                        string responseJson =
                            await response.Content.ReadAsStringAsync()
                                .ConfigureAwait(false);

                        linked.Token.ThrowIfCancellationRequested();

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new InvalidOperationException(
                                BuildHttpError(
                                    providerName,
                                    response,
                                    responseJson));
                        }

                        return responseJson;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 호출자가 원래 사용자 토큰으로 취소/시간 초과를 구분한다.
                throw;
            }
            catch (Exception ex)
            {
                // 원본 예외를 붙이면 로그에 민감한 내용이 다시 포함될 수 있다.
                throw new InvalidOperationException(
                    FormatException(ex, secrets));
            }
        }

        /// <summary>예외 메시지에서 비밀 값과 인증 정보를 가리고 표시 길이를 제한한다.</summary>
        public static string FormatException(
            Exception exception,
            params string[] secrets)
        {
            var text = new StringBuilder();

            for (var current = exception;
                 current != null;
                 current = current.InnerException)
            {
                if (text.Length > 0)
                    text.AppendLine();

                text.Append(current.GetType().Name)
                    .Append(": ")
                    .Append(current.Message);
            }

            string safe = text.ToString();

            foreach (string secret in secrets ?? new string[0])
            {
                if (string.IsNullOrEmpty(secret))
                    continue;

                safe = safe.Replace(secret, "[비공개]");
                safe = safe.Replace(
                    Uri.EscapeDataString(secret), "[비공개]");
            }

            safe = Regex.Replace(
                safe,
                @"(?i)([?&](?:key|api[_-]?key|access[_-]?token)=)[^&\s""'<>]+",
                "$1[비공개]");

            safe = Regex.Replace(
                safe,
                @"(?i)(\b(?:authorization|x-api-key|x-goog-api-key)\s*[:=]\s*)(?:bearer\s+)?[^\s,""'<>]+",
                "$1[비공개]");

            return safe.Length <= 2000
                ? safe
                : safe.Substring(0, 2000);
        }

        #endregion

        #region Private Methods

        /// <summary>응답 원문 대신 허용된 오류 속성으로 HTTP 오류 설명을 구성한다.</summary>
        private static string BuildHttpError(
            string providerName,
            HttpResponseMessage response,
            string responseJson)
        {
            var details = new StringBuilder();

            try
            {
                var root = JObject.Parse(responseJson);
                var error = root["error"] as JObject;

                if (error != null)
                {
                    foreach (string name in
                        new[] { "code", "type", "status", "message" })
                    {
                        var value = error[name];

                        if (value == null ||
                            (value.Type != JTokenType.String &&
                             value.Type != JTokenType.Integer))
                        {
                            continue;
                        }

                        if (details.Length > 0)
                            details.AppendLine();

                        details.Append(name)
                            .Append(": ")
                            .Append(value.ToString());
                    }
                }
                else if (root["error"]?.Type == JTokenType.String)
                {
                    details.Append((string)root["error"]);
                }
                else if (root["message"]?.Type == JTokenType.String)
                {
                    details.Append((string)root["message"]);
                }
            }
            catch (JsonException)
            {
                // HTML 등 응답 원문은 메시지나 로그에 포함하지 않는다.
            }

            return providerName
                + " API 오류: HTTP " + (int)response.StatusCode
                + " " + response.ReasonPhrase
                + "\n"
                + (details.Length == 0
                    ? "서버가 오류 설명을 제공하지 않았습니다."
                    : details.ToString());
        }

        #endregion
    }
}
