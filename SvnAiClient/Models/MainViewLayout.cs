using System.Collections.Generic;

namespace SvnAiClient.Models
{
    /// <summary>메인 화면에서 사용자가 조정한 분할 위치와 열 너비를 보관한다.</summary>
    public class MainViewLayout
    {
        #region Properties

        /// <summary>폴더 탐색 패널의 너비.</summary>
        public int FolderPanelWidth { get; set; }

        /// <summary>커밋 대상 패널의 높이.</summary>
        public int CommitPanelHeight { get; set; }

        /// <summary>파일 목록의 필드 이름별 열 너비.</summary>
        public Dictionary<string, int> FileColumnWidths { get; set; } = new Dictionary<string, int>();

        /// <summary>커밋 대상 목록의 필드 이름별 열 너비.</summary>
        public Dictionary<string, int> CommitColumnWidths { get; set; } = new Dictionary<string, int>();

        #endregion
    }
}
