using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SvnAiClient.Views.DiffViewer
{
    /// <summary>Diff 종류 판별, 리비전 안내 및 변경 위치 이동 요청을 담당한다.</summary>
    public sealed class DiffViewerPresenter
    {
        #region Fields

        /// <summary>속성 변경 블록의 시작 위치를 찾는 패턴.</summary>
        private static readonly Regex PropertyMarker = new Regex(
            @"^Property changes on:", RegexOptions.Multiline);

        /// <summary>좌우 텍스트 비교가 가능한 Unified Diff 변경 구간 헤더 패턴.</summary>
        private static readonly Regex HunkMarker = new Regex(
            @"^@@ -\d+(?:,\d+)? \+\d+(?:,\d+)? @@", RegexOptions.Multiline);

        /// <summary>Diff 표시와 위치 이동을 요청하는 화면 계약.</summary>
        private readonly DiffViewerInterface _view;

        #endregion

        #region Initialization

        public DiffViewerPresenter(DiffViewerInterface view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            _view = view;
        }

        #endregion

        #region View Requests

        public void MoveToNextChange()
        {
            _view.MoveToNextChange();
        }

        public void MoveToPreviousChange()
        {
            _view.MoveToPreviousChange();
        }

        #endregion

        #region View Updates

        /// <summary>선택 탭과 변경 구간 번호에 맞춰 이동 버튼과 위치 표시를 갱신한다.</summary>
        public void UpdateNavigation()
        {
            bool comparing = _view.IsComparisonSelected && _view.ChangeCount > 0;
            _view.SetNavigation(
                comparing && _view.CurrentChangeNumber > 1,
                comparing && _view.CurrentChangeNumber < _view.ChangeCount,
                "변경 " + _view.CurrentChangeNumber + " / " + _view.ChangeCount);
        }

        #endregion

        #region Operations

        /// <summary>텍스트 Diff와 속성 변경을 분리하고 표시할 탭과 안내 문구를 결정한다.</summary>
        public void SetDiff(string filePath, string diffText)
        {
            string rawDiff = diffText ?? string.Empty;
            var properties = PropertyMarker.Match(rawDiff);
            string textDiff = properties.Success ? rawDiff.Substring(0, properties.Index) : rawDiff;
            bool hasTextDiff = HunkMarker.IsMatch(textDiff);

            _view.TitleText = "변경 비교 - " + Path.GetFileName(filePath);
            _view.ShowContent(filePath, rawDiff, textDiff, hasTextDiff);
            if (hasTextDiff)
                _view.MoveToNextChange();

            _view.StatusText = GetContentStatus(rawDiff, hasTextDiff, properties.Success);
            UpdateNavigation();
        }

        /// <summary>두 리비전의 Diff를 표시하고 비교 제목과 좌우 리비전 표기를 적용한다.</summary>
        public void SetRevisionDiff(string path, string diffText, long fromRevision, long toRevision)
        {
            SetDiff(path, diffText);
            _view.TitleText = "이력 비교 - " + path + " (r" + fromRevision + " → r" + toRevision + ")";
            _view.SetSideLabels("원본 r" + fromRevision, "수정본 r" + toRevision);
            _view.StatusText = "저장소 이력 비교입니다. " + _view.StatusText;
        }

        /// <summary>검수 당시 수정본의 행으로 이동하고 위치가 없으면 기존 안내와 함께 설명한다.</summary>
        public void ShowIssueLocation(int? lineNumber)
        {
            _view.TitleText += " [검수 당시]";
            string guidance = _view.StatusText;
            if (!lineNumber.HasValue || lineNumber.Value <= 0)
            {
                _view.StatusText = "검수 당시 Diff입니다. 이슈의 줄번호가 없습니다. " + guidance;
                return;
            }

            if (_view.IsComparisonSelected && _view.MoveToModifiedLine(lineNumber.Value))
            {
                _view.StatusText = "검수 당시 Diff · 수정본 " + lineNumber.Value + "행을 선택했습니다.";
                UpdateNavigation();
            }
            else
            {
                _view.StatusText = "검수 당시 Diff에 수정본 " + lineNumber.Value
                    + "행이 없어 위치로 이동할 수 없습니다. " + guidance;
            }
        }

        /// <summary>텍스트, 바이너리, 속성 변경 및 빈 Diff에 맞는 안내 문구를 만든다.</summary>
        private static string GetContentStatus(string rawDiff, bool hasTextDiff, bool hasProperties)
        {
            if (hasTextDiff)
                return "왼쪽은 원본, 오른쪽은 수정본의 실제 줄번호입니다."
                    + (hasProperties ? " 속성 변경은 원문 탭에서 확인하세요." : string.Empty);
            if (string.IsNullOrWhiteSpace(rawDiff))
                return "변경 내용이 없습니다.";
            if (rawDiff.IndexOf("file marked as a binary type", StringComparison.OrdinalIgnoreCase) >= 0)
                return "바이너리 파일입니다. 원문 정보를 확인하세요.";
            if (hasProperties)
                return "속성 변경입니다. 원문에서 변경값을 확인하세요.";
            return "텍스트 비교 형식이 없습니다. 원문을 확인하세요.";
        }

        #endregion
    }
}
