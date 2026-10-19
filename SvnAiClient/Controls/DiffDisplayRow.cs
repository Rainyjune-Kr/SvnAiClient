namespace SvnAiClient.Controls
{
    /// <summary>좌우 Diff 화면에서 같은 높이에 표시할 한 행의 정보.</summary>
    internal sealed class DiffDisplayRow
    {
        #region Properties

        /// <summary>원본 영역에 표시할 텍스트.</summary>
        public string LeftText { get; set; } = string.Empty;

        /// <summary>수정본 영역에 표시할 텍스트.</summary>
        public string RightText { get; set; } = string.Empty;

        /// <summary>원본의 실제 줄번호이며 빈 행이나 헤더이면 null.</summary>
        public int? LeftNumber { get; set; }

        /// <summary>수정본의 실제 줄번호이며 빈 행이나 헤더이면 null.</summary>
        public int? RightNumber { get; set; }

        /// <summary>원본 영역에 적용할 행 마커.</summary>
        public DiffDisplayMarker LeftMarker { get; set; } = DiffDisplayMarker.None;

        /// <summary>수정본 영역에 적용할 행 마커.</summary>
        public DiffDisplayMarker RightMarker { get; set; } = DiffDisplayMarker.None;

        #endregion
    }
}
