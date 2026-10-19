namespace SvnAiClient.Controls
{
    /// <summary>Diff 표시 행에 적용할 마커 종류와 기존 마커 번호.</summary>
    internal enum DiffDisplayMarker
    {
        #region Values

        /// <summary>마커를 표시하지 않는 일반 문맥 행.</summary>
        None = -1,

        /// <summary>추가된 행.</summary>
        Added = 0,

        /// <summary>삭제된 행.</summary>
        Removed = 1,

        /// <summary>좌우의 삭제와 추가를 짝지은 수정 행.</summary>
        Modified = 2,

        /// <summary>좌우 정렬을 위해 채운 빈 행.</summary>
        Empty = 3,

        /// <summary>변경 구간 헤더 행.</summary>
        Header = 4

        #endregion
    }
}
