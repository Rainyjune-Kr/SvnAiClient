using Newtonsoft.Json;

namespace SvnAiClient.Models
{
    /// <summary>검수 이력에 저장할 개별 이슈의 스냅샷.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AiReviewHistoryIssue
    {
        #region Properties

        /// <summary>이슈의 심각도.</summary>
        [JsonProperty]
        public IssueSeverity Severity { get; set; }

        /// <summary>이슈 분류.</summary>
        [JsonProperty]
        public string Category { get; set; }

        /// <summary>이슈 제목.</summary>
        [JsonProperty]
        public string Title { get; set; }

        /// <summary>발생 조건과 영향 및 수정 방법을 포함한 이슈 설명.</summary>
        [JsonProperty]
        public string Description { get; set; }

        /// <summary>이슈와 관련된 파일 경로이며 위치를 알 수 없으면 null.</summary>
        [JsonProperty]
        public string FilePath { get; set; }

        /// <summary>수정본 기준 1부터 시작하는 라인 번호이며 위치를 알 수 없으면 null.</summary>
        [JsonProperty]
        public int? LineNumber { get; set; }

        #endregion
    }
}
