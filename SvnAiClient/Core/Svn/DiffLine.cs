namespace SvnAiClient.Core.Svn
{
    /// <summary>Diff의 개별 라인 정보.</summary>
    public class DiffLine
    {
        #region Properties

        /// <summary>라인 유형</summary>
        public DiffLineType LineType { get; set; }

        /// <summary>라인 내용 (접두사 기호 포함)</summary>
        public string Content { get; set; }

        /// <summary>원본 파일의 라인 번호</summary>
        public int? OriginalLineNumber { get; set; }

        /// <summary>수정된 파일의 라인 번호</summary>
        public int? ModifiedLineNumber { get; set; }

        #endregion
    }
}
