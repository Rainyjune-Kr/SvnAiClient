namespace SvnAiClient.Core.Svn
{
    /// <summary>Diff 라인 유형</summary>
    public enum DiffLineType
    {
        /// <summary>추가된 라인 ('+', 초록색)</summary>
        Added,
        /// <summary>삭제된 라인 ('-', 빨간색)</summary>
        Removed,
        /// <summary>변경 없는 컨텍스트 라인</summary>
        Context,
        /// <summary>헝크 헤더 ('@@', 회색)</summary>
        HunkHeader,
        /// <summary>파일 헤더 ('Index:', '---', '+++')</summary>
        FileHeader
    }
}
