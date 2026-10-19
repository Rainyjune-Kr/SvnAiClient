using System;
using System.Collections.Generic;
using System.Linq;

namespace SvnAiClient.Models
{
    /// <summary>현재 변경 내용에 대한 AI 검수 결과와 토큰 사용량 및 처리 상태를 보관한다.</summary>
    public class AiReviewResult
    {
        #region Properties

        /// <summary>검수를 실행한 AI 프로바이더 표시 이름.</summary>
        public string ProviderName { get; set; }

        /// <summary>AI가 작성한 변경 내용 요약.</summary>
        public string Summary { get; set; }

        /// <summary>AI 검수 결과에 포함된 커밋 메시지 제안.</summary>
        public string SuggestedCommitMessage { get; set; }

        /// <summary>검수에서 발견된 이슈 목록.</summary>
        public List<AiReviewIssue> Issues { get; set; }
            = new List<AiReviewIssue>();

        /// <summary>전송 전에 추정한 입력 토큰 수.</summary>
        public int EstimatedInputTokens { get; set; }

        /// <summary>프로바이더가 제공한 실제 입력 토큰 수이며 미제공이면 null.</summary>
        public int? ActualInputTokens { get; set; }

        /// <summary>프로바이더가 제공한 실제 출력 토큰 수이며 미제공이면 null.</summary>
        public int? ActualOutputTokens { get; set; }

        /// <summary>검수 결과가 기록된 시각.</summary>
        public DateTime ReviewedAt { get; set; } = DateTime.Now;

        /// <summary>검수 요청 처리에 걸린 시간(밀리초).</summary>
        public long ElapsedMs { get; set; }

        /// <summary>검수 처리 상태.</summary>
        public AiReviewStatus Status { get; set; }
            = AiReviewStatus.NotReviewed;

        /// <summary>검수 실패 또는 취소 원인.</summary>
        public AiReviewFailureKind FailureKind { get; set; }

        /// <summary>검수 처리 실패에 대한 오류 설명.</summary>
        public string ErrorMessage { get; set; }

        /// <summary>검수 요청 단위의 처리 결과 목록.</summary>
        public List<AiReviewBatch> Batches { get; set; }
            = new List<AiReviewBatch>();

        /// <summary>전체 성공 여부이며 값을 지정하면 성공 또는 실패 상태로 반영한다.</summary>
        public bool IsSuccess
        {
            get { return Status == AiReviewStatus.Succeeded; }
            set
            {
                Status = value
                    ? AiReviewStatus.Succeeded
                    : AiReviewStatus.Failed;
            }
        }

        /// <summary>성공 상태인 검수 요청 단위의 개수.</summary>
        public int SuccessfulBatchCount
        {
            get
            {
                return Batches.Count(
                    batch => batch.Status == AiReviewStatus.Succeeded);
            }
        }

        #endregion
    }
}
