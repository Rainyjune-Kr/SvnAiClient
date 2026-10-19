using System.Collections.Generic;

namespace SvnAiClient.Models
{
    /// <summary>검수 요청 단위의 대상 파일, 처리 상태 및 커밋 메시지 제안을 보관한다.</summary>
    public class AiReviewBatch
    {
        #region Properties

        /// <summary>검수 대상 파일의 경로 목록.</summary>
        public List<string> FilePaths { get; set; }
            = new List<string>();

        /// <summary>검수 처리 상태.</summary>
        public AiReviewStatus Status { get; set; }

        /// <summary>검수 실패 또는 취소 원인.</summary>
        public AiReviewFailureKind FailureKind { get; set; }

        /// <summary>검수 처리 실패에 대한 오류 설명.</summary>
        public string ErrorMessage { get; set; }

        /// <summary>AI 검수 결과에 포함된 커밋 메시지 제안.</summary>
        public string SuggestedCommitMessage { get; set; }

        /// <summary>검수 요청이 포함한 대상 범위에 대한 설명.</summary>
        public string ScopeDescription { get; set; }

        #endregion
    }
}
