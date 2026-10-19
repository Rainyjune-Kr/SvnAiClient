using System.Collections.Generic;

namespace SvnAiClient.Controls
{
    /// <summary>Diff 표시 행과 변경 구간 및 수정본 줄번호의 이동 위치를 보관한다.</summary>
    internal sealed class DiffDisplayData
    {
        #region Properties

        /// <summary>좌우를 정렬한 표시 행 목록.</summary>
        public List<DiffDisplayRow> Rows { get; private set; }
            = new List<DiffDisplayRow>();

        /// <summary>변경 구간이 시작하는 0부터의 표시 행 번호 목록.</summary>
        public List<int> ChangeRows { get; private set; }
            = new List<int>();

        /// <summary>수정본의 실제 줄번호에 대응하는 0부터의 표시 행 번호.</summary>
        public Dictionary<int, int> ModifiedLineRows { get; private set; }
            = new Dictionary<int, int>();

        #endregion
    }
}
