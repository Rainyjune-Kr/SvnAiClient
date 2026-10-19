namespace SvnAiClient.Models
{
    /// <summary>AI 검수에서 발견한 이슈와 관련 파일 및 라인 위치를 보관한다.</summary>
    public class AiReviewIssue
    {
        #region Properties

        /// <summary>이슈의 심각도.</summary>
        public IssueSeverity Severity { get; set; }

        /// <summary>이슈 분류.</summary>
        public string Category { get; set; }

        /// <summary>이슈 제목.</summary>
        public string Title { get; set; }

        /// <summary>발생 조건과 영향 및 수정 방법을 포함한 이슈 설명.</summary>
        public string Description { get; set; }

        /// <summary>이슈와 관련된 파일 경로이며 위치를 알 수 없으면 null.</summary>
        public string FilePath { get; set; }

        /// <summary>수정본 기준 1부터 시작하는 라인 번호이며 위치를 알 수 없으면 null.</summary>
        public int? LineNumber { get; set; }

        /// <summary>화면에 표시할 대문자 심각도 문자열.</summary>
        public string SeverityText => Severity.ToString().ToUpper();

        #endregion
    }
}
