namespace SvnAiClient.Views.DiffViewer
{
    /// <summary>Diff 표시와 변경 위치 이동에 대한 UI 독립 계약.</summary>
    public interface DiffViewerInterface
    {
        #region Properties

        /// <summary>현재 창의 제목.</summary>
        string TitleText { get; set; }

        /// <summary>현재 Diff의 종류와 이동 결과를 설명하는 문구.</summary>
        string StatusText { get; set; }

        /// <summary>좌우 비교 탭이 선택되어 있는지 여부.</summary>
        bool IsComparisonSelected { get; }

        /// <summary>현재 좌우 비교에 표시된 변경 구간 수.</summary>
        int ChangeCount { get; }

        /// <summary>현재 선택한 변경 구간의 1부터 시작하는 번호.</summary>
        int CurrentChangeNumber { get; }

        #endregion

        #region View Contract

        void ShowContent(string path, string rawDiff, string textDiff, bool compare);
        void SetSideLabels(string original, string modified);
        void SetNavigation(bool previousEnabled, bool nextEnabled, string position);
        void MoveToNextChange();
        void MoveToPreviousChange();
        bool MoveToModifiedLine(int lineNumber);

        #endregion
    }
}
