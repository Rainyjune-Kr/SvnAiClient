using System;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>검수 요청의 결과와 저장된 로컬 이력 ID를 함께 전달한다.</summary>
    internal sealed class AiReviewExecutionResult
    {
        #region Properties

        /// <summary>AI 검수 요청에서 반환된 결과.</summary>
        public AiReviewResult Result { get; set; }

        /// <summary>저장 성공한 검수 이력 ID이며 저장 실패 시 Guid.Empty.</summary>
        public Guid HistoryId { get; set; }

        #endregion
    }
}
