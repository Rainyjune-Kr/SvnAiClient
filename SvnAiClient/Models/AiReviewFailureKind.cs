namespace SvnAiClient.Models
{
    /// <summary>검수 실패 또는 취소의 원인을 구분한다.</summary>
    public enum AiReviewFailureKind
    {
        #region Values

        /// <summary>실패 원인 없음.</summary>
        None,

        /// <summary>AI 응답이 요구한 JSON 형식을 충족하지 못함.</summary>
        InvalidResponse,

        /// <summary>프로바이더 요청 또는 응답 처리 중 오류 발생.</summary>
        ProviderError,

        /// <summary>요청 제한 시간 초과.</summary>
        Timeout,

        /// <summary>사용자가 검수 취소를 요청함.</summary>
        Cancelled,

        /// <summary>입력 또는 응답 예산이 유효하지 않거나 토큰 한도 초과.</summary>
        InputTooLarge,

        /// <summary>검수할 Diff가 유효하지 않음.</summary>
        InvalidDiff

        #endregion
    }
}
