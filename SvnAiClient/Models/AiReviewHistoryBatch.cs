using System.Collections.Generic;
using Newtonsoft.Json;

namespace SvnAiClient.Models
{
    /// <summary>검수 이력에 저장할 요청 단위의 대상과 상태 및 메시지 제안.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AiReviewHistoryBatch
    {
        #region Properties

        /// <summary>검수 대상 파일의 경로 목록.</summary>
        [JsonProperty]
        public List<string> FilePaths { get; set; } = new List<string>();

        /// <summary>검수 처리 상태.</summary>
        [JsonProperty]
        public AiReviewStatus Status { get; set; }

        /// <summary>검수 실패 또는 취소 원인.</summary>
        [JsonProperty]
        public AiReviewFailureKind FailureKind { get; set; }

        /// <summary>AI 검수 결과에 포함된 커밋 메시지 제안.</summary>
        [JsonProperty]
        public string SuggestedCommitMessage { get; set; }

        #endregion
    }
}
