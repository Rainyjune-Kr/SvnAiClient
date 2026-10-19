namespace SvnAiClient.Models
{
    /// <summary>AI 검수 이슈의 심각도 수준.</summary>
    public enum IssueSeverity
    {
        #region Values

        /// <summary>정보성 개선 사항.</summary>
        Info = 0,

        /// <summary>구체적인 위험 또는 비교적 경미한 결함.</summary>
        Warning = 1,

        /// <summary>심각한 결함.</summary>
        Error = 2

        #endregion
    }
}
