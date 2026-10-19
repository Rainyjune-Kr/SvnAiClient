using System.Globalization;
using Newtonsoft.Json.Linq;

namespace SvnAiClient.Core.AI
{
    /// <summary>프로바이더 응답에서 유효한 토큰 사용량을 읽는다.</summary>
    internal static class TokenUsageParser
    {
        #region Public Methods

        /// <summary>음수가 아닌 정수 사용량을 읽으며, 누락되거나 잘못된 값은 미제공으로 처리한다.</summary>
        public static int? ReadTokenCount(JToken token)
        {
            if (token == null || token.Type != JTokenType.Integer)
                return null;

            long value;

            if (!long.TryParse(
                    token.ToString(),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out value) ||
                value < 0 ||
                value > int.MaxValue)
            {
                return null;
            }

            return (int)value;
        }

        /// <summary>필수 사용량과 제공된 선택 사용량을 합산하며, 유효하지 않은 합계는 미제공으로 처리한다.</summary>
        public static int? SumTokenCounts(
            JObject usage,
            string requiredName,
            params string[] optionalNames)
        {
            if (usage == null)
                return null;

            int? required = ReadTokenCount(usage[requiredName]);

            if (!required.HasValue)
                return null;

            long total = required.Value;

            foreach (string name in optionalNames)
            {
                var token = usage[name];

                if (token == null || token.Type == JTokenType.Null)
                    continue;

                int? count = ReadTokenCount(token);

                if (!count.HasValue)
                    return null;

                total += count.Value;
            }

            return total <= int.MaxValue ? (int?)total : null;
        }

        #endregion
    }
}
