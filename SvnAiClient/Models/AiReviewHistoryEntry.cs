using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SvnAiClient.Models
{
    /// <summary>조회 전용 검수 이력 스냅샷이며 현재 변경 내용의 검수 완료 상태로 재사용하지 않는다.</summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AiReviewHistoryEntry
    {
        #region Properties

        /// <summary>로컬 검수 이력 JSON의 스키마 버전.</summary>
        [JsonProperty]
        public int SchemaVersion { get; set; } = 1;

        /// <summary>이력 JSON 파일 이름에 사용하는 검수 이력의 고유 ID.</summary>
        [JsonProperty]
        public Guid Id { get; set; }

        /// <summary>검수 결과가 기록된 시각.</summary>
        [JsonProperty]
        public DateTimeOffset ReviewedAt { get; set; }

        /// <summary>검수 대상이 속한 작업 복사본 경로.</summary>
        [JsonProperty]
        public string WorkingCopyPath { get; set; }

        /// <summary>검수를 실행한 AI 프로바이더 표시 이름.</summary>
        [JsonProperty]
        public string ProviderName { get; set; }

        /// <summary>검수 시작 시 선택한 모델 이름.</summary>
        [JsonProperty]
        public string ModelName { get; set; }

        /// <summary>검수 대상 파일의 경로 목록.</summary>
        [JsonProperty]
        public List<string> FilePaths { get; set; } = new List<string>();

        /// <summary>개발자가 입력한 변경 배경과 검수 요청 내용.</summary>
        [JsonProperty]
        public string ChangeContext { get; set; }

        /// <summary>검수 처리 상태.</summary>
        [JsonProperty]
        public AiReviewStatus Status { get; set; }

        /// <summary>검수 실패 또는 취소 원인.</summary>
        [JsonProperty]
        public AiReviewFailureKind FailureKind { get; set; }

        /// <summary>AI가 작성한 변경 내용 요약.</summary>
        [JsonProperty]
        public string Summary { get; set; }

        /// <summary>검수에서 발견된 이슈 목록.</summary>
        [JsonProperty]
        public List<AiReviewHistoryIssue> Issues { get; set; }
            = new List<AiReviewHistoryIssue>();

        /// <summary>검수 요청 단위의 처리 결과 목록.</summary>
        [JsonProperty]
        public List<AiReviewHistoryBatch> Batches { get; set; }
            = new List<AiReviewHistoryBatch>();

        /// <summary>AI 검수 결과에 포함된 커밋 메시지 제안.</summary>
        [JsonProperty]
        public string SuggestedCommitMessage { get; set; }

        /// <summary>전송 전에 추정한 입력 토큰 수.</summary>
        [JsonProperty]
        public int EstimatedInputTokens { get; set; }

        /// <summary>프로바이더가 제공한 실제 입력 토큰 수이며 미제공이면 null.</summary>
        [JsonProperty]
        public int? ActualInputTokens { get; set; }

        /// <summary>프로바이더가 제공한 실제 출력 토큰 수이며 미제공이면 null.</summary>
        [JsonProperty]
        public int? ActualOutputTokens { get; set; }

        /// <summary>검수 요청 처리에 걸린 시간(밀리초).</summary>
        [JsonProperty]
        public long ElapsedMs { get; set; }

        /// <summary>커밋 성공 후 연결한 SVN 리비전이며 미연결이면 null.</summary>
        [JsonProperty]
        public long? SvnRevision { get; set; }

        /// <summary>SVN 리비전을 이력에 연결한 시각이며 미연결이면 null.</summary>
        [JsonProperty]
        public DateTimeOffset? CommittedAt { get; set; }

        #endregion
    }
}
