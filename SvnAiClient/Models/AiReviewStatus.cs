namespace SvnAiClient.Models
{
    /// <summary>미검수, 성공, 부분 성공, 실패 및 취소를 구분하는 검수 상태.</summary>
    public enum AiReviewStatus
    {
        #region Values

        /// <summary>아직 검수를 실행하지 않은 상태.</summary>
        NotReviewed,

        /// <summary>전체 검수가 성공한 상태.</summary>
        Succeeded,

        /// <summary>검수의 일부만 성공한 상태.</summary>
        Partial,

        /// <summary>검수에 실패한 상태.</summary>
        Failed,

        /// <summary>검수가 취소된 상태.</summary>
        Cancelled

        #endregion
    }
}
