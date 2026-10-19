using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NLog;
using ScintillaNET;
using SvnAiClient.Core.Svn;

namespace SvnAiClient.Controls
{
    /// <summary>좌우 Diff 렌더링과 줄번호 및 마커 표시, 스크롤 동기화와 변경 위치 이동을 담당한다.</summary>
    public partial class DiffViewerControl : UserControl
    {
        #region Fields

        /// <summary>Diff 표시 중 발생한 오류를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>수정본 실제 줄번호와 화면 표시 행의 대응 관계.</summary>
        private readonly Dictionary<int, int> _modifiedLineRows = new Dictionary<int, int>();

        /// <summary>이전 및 다음 이동에 사용할 변경 구간의 시작 행 목록.</summary>
        private readonly List<int> _changeRows = new List<int>();

        /// <summary>현재 선택한 변경 구간의 인덱스이며 미선택이면 -1.</summary>
        private int _currentChangeIndex = -1;

        /// <summary>좌우 스크롤 동기화 중 발생하는 재진입을 막는 상태.</summary>
        private bool _isSyncing = false;

        /// <summary>추가 행의 배경색.</summary>
        private static readonly Color ColorAdded = Color.FromArgb(198, 239, 206);

        /// <summary>삭제 행의 배경색.</summary>
        private static readonly Color ColorRemoved = Color.FromArgb(255, 199, 206);

        /// <summary>수정 행의 배경색.</summary>
        private static readonly Color ColorModified = Color.FromArgb(255, 235, 156);

        /// <summary>정렬용 빈 행의 배경색.</summary>
        private static readonly Color ColorEmpty = Color.FromArgb(220, 220, 220);

        /// <summary>변경 구간 헤더의 배경색.</summary>
        private static readonly Color ColorHeader = Color.FromArgb(189, 215, 238);

        #endregion

        #region Properties

        /// <summary>표시 중인 변경 구간의 개수.</summary>
        public int ChangeCount
        {
            get { return _changeRows.Count; }
        }

        /// <summary>현재 선택한 변경 구간의 1부터의 번호이며 미선택이면 0.</summary>
        public int CurrentChangeNumber
        {
            get { return _currentChangeIndex + 1; }
        }

        #endregion

        #region Events

        /// <summary>변경 구간의 선택 상태 또는 개수가 달라졌음을 알린다.</summary>
        public event EventHandler ChangeNavigationChanged;

        #endregion

        #region Initialization

        public DiffViewerControl()
        {
            InitializeComponent();
            ConfigureScintilla(scintLeft);
            ConfigureScintilla(scintRight);
            AddEventHandlers();
        }

        private void ConfigureScintilla(Scintilla scint)
        {
            scint.Margins[0].Type = MarginType.RightText;
            scint.Margins[0].Mask = 0;
            scint.Margins[0].Width = 42;
            scint.Margins[1].Width = 4;

            scint.Styles[Style.Default].Font = "Consolas";
            scint.Styles[Style.Default].Size = 10;
            scint.StyleClearAll();

            scint.Styles[Style.LineNumber].ForeColor = Color.Gray;
            scint.Styles[Style.LineNumber].BackColor = Color.FromArgb(245, 245, 245);

            SetupMarker(scint, DiffDisplayMarker.Added, ColorAdded);
            SetupMarker(scint, DiffDisplayMarker.Removed, ColorRemoved);
            SetupMarker(scint, DiffDisplayMarker.Modified, ColorModified);
            SetupMarker(scint, DiffDisplayMarker.Empty, ColorEmpty);
            SetupMarker(scint, DiffDisplayMarker.Header, ColorHeader);
        }

        private void AddEventHandlers()
        {
            scintLeft.UpdateUI += ScintLeft_UpdateUI;
            scintRight.UpdateUI += ScintRight_UpdateUI;
            scintLeft.ZoomChanged += Scint_ZoomChanged;
            scintRight.ZoomChanged += Scint_ZoomChanged;
            scintLeft.HorizontalWheelScrolled += Scint_HorizontalWheelScrolled;
            scintRight.HorizontalWheelScrolled += Scint_HorizontalWheelScrolled;
        }

        #endregion

        #region Event Handlers

        private void ScintLeft_UpdateUI(
            object sender,
            UpdateUIEventArgs e)
        {
            SynchronizeScroll(scintLeft, scintRight, e.Change);
        }

        private void ScintRight_UpdateUI(
            object sender,
            UpdateUIEventArgs e)
        {
            SynchronizeScroll(scintRight, scintLeft, e.Change);
        }

        private void Scint_ZoomChanged(object sender, EventArgs e)
        {
            UpdateHorizontalScrollWidth();
        }

        private void Scint_HorizontalWheelScrolled(object sender, EventArgs e)
        {
            if (scintLeft == null || scintRight == null)
                return;

            var source = (Scintilla)sender;
            var target = ReferenceEquals(source, scintLeft) ? scintRight : scintLeft;
            SynchronizeScroll(source, target, UpdateChange.HScroll);
        }

        #endregion

        #region Control Methods

        /// <summary>코드 본문과 실제 줄번호에 동일한 고정폭 글꼴을 적용한다.</summary>
        public void SetCodeFont(Font font)
        {
            if (font == null) throw new ArgumentNullException(nameof(font));
            foreach (var scint in new[] { scintLeft, scintRight })
            {
                foreach (int index in new[] { 0, Style.Default, Style.LineNumber })
                {
                    scint.Styles[index].Font = font.Name;
                    scint.Styles[index].Size = (int)Math.Round(font.SizeInPoints);
                }
            }
            UpdateHorizontalScrollWidth();
        }

        /// <summary>Diff를 좌우 표시 데이터로 변환하여 읽기 전용 편집기에 표시한다.</summary>
        public void SetDiff(string diffText)
        {
            try
            {
                scintLeft.ReadOnly = false;
                scintRight.ReadOnly = false;
                scintLeft.ClearAll();
                scintRight.ClearAll();

                ResetNavigation();

                scintLeft.XOffset = 0;
                scintRight.XOffset = 0;

                if (string.IsNullOrEmpty(diffText))
                {
                    scintLeft.Text = "(변경 없음)";
                    scintRight.Text = "(변경 없음)";
                    scintLeft.ReadOnly = true;
                    scintRight.ReadOnly = true;

                    UpdateHorizontalScrollWidth();

                    NotifyNavigationChanged();
                    return;
                }

                var parser = new DiffParser();
                var lines = parser.Parse(diffText);
                ApplyDisplayData(DiffDisplayBuilder.Build(lines));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SetDiff 오류");
                throw;
            }
            finally
            {
                scintLeft.ReadOnly = true;
                scintRight.ReadOnly = true;
            }
        }

        /// <summary>좌우 편집기와 줄번호 및 변경 위치 상태를 초기화한다.</summary>
        public void Clear()
        {
            scintLeft.ReadOnly = false;
            scintRight.ReadOnly = false;

            scintLeft.ClearAll();
            scintRight.ClearAll();

            scintLeft.Margins.ClearAllText();
            scintRight.Margins.ClearAllText();

            scintLeft.XOffset = 0;
            scintRight.XOffset = 0;

            ResetNavigation();

            UpdateHorizontalScrollWidth();

            scintLeft.ReadOnly = true;
            scintRight.ReadOnly = true;

            NotifyNavigationChanged();
        }

        /// <summary>좌우 편집기의 원본과 수정본 안내 문구를 변경한다.</summary>
        public void SetSideLabels(string left, string right)
        {
            lblLeft.Text = left;
            lblRight.Text = right;
        }

        /// <summary>표시 데이터의 이동 위치를 보관하고 좌우 편집기에 행을 적용한다.</summary>
        private void ApplyDisplayData(DiffDisplayData display)
        {
            _changeRows.AddRange(display.ChangeRows);

            foreach (var line in display.ModifiedLineRows)
                _modifiedLineRows[line.Key] = line.Value;

            ApplyToScintilla(scintLeft, display.Rows, true);
            ApplyToScintilla(scintRight, display.Rows, false);

            UpdateHorizontalScrollWidth();
            NotifyNavigationChanged();
        }

        /// <summary>편집기에 텍스트와 실제 줄번호 및 행 마커를 적용한다.</summary>
        private void ApplyToScintilla(
            Scintilla scint,
            List<DiffDisplayRow> rows,
            bool left)
        {
            var text = new List<string>();

            foreach (var row in rows)
                text.Add(left ? row.LeftText : row.RightText);

            scint.Text = string.Join("\n", text);
            scint.Margins.ClearAllText();

            int digits = 1;

            for (int i = 0; i < rows.Count && i < scint.Lines.Count; i++)
            {
                var row = rows[i];

                int? number = left
                    ? row.LeftNumber
                    : row.RightNumber;

                // 빈 정렬 행과 구간 헤더에는 줄번호를 표시하지 않는다.
                string numberText = number.HasValue && number.Value > 0
                    ? number.Value.ToString()
                    : string.Empty;

                scint.Lines[i].MarginText = numberText;
                scint.Lines[i].MarginStyle = Style.LineNumber;

                digits = Math.Max(digits, numberText.Length);

                int marker = (int)(left
                    ? row.LeftMarker
                    : row.RightMarker);

                if (marker >= 0)
                    scint.Lines[i].MarkerAdd(marker);
            }

            // 큰 줄번호도 잘리지 않도록 마진 폭을 계산한다.
            scint.Margins[0].Width = Math.Max(
                42,
                scint.TextWidth(
                    Style.LineNumber,
                    new string('9', digits)) + 12);
        }

        /// <summary>지정한 마커를 배경색 표시용으로 구성한다.</summary>
        private void SetupMarker(Scintilla scint, DiffDisplayMarker marker, Color color)
        {
            scint.Markers[(int)marker].Symbol = MarkerSymbol.Background;
            scint.Markers[(int)marker].SetBackColor(color);
            scint.Markers[(int)marker].SetForeColor(color);
        }

        /// <summary>재진입을 막으면서 변경된 세로 또는 가로 스크롤 위치를 반대편에 반영한다.</summary>
        private void SynchronizeScroll(
            Scintilla source,
            Scintilla target,
            UpdateChange change)
        {
            if (_isSyncing)
                return;

            _isSyncing = true;

            try
            {
                if ((change & UpdateChange.VScroll) != 0 &&
                    target.FirstVisibleLine != source.FirstVisibleLine)
                {
                    target.FirstVisibleLine = source.FirstVisibleLine;
                }

                if ((change & UpdateChange.HScroll) != 0)
                {
                    if (target.XOffset != source.XOffset)
                        target.XOffset = source.XOffset;

                    source.Invalidate();
                    target.Invalidate();
                }
            }
            finally
            {
                _isSyncing = false;
            }
        }

        /// <summary>좌우 내용의 최대 표시 폭에 맞춰 동일한 가로 스크롤 범위를 적용한다.</summary>
        private void UpdateHorizontalScrollWidth()
        {
            if (scintLeft == null || scintRight == null)
                return;

            // 좌우에 같은 범위를 적용하여 횡스크롤 위치를 맞춘다.
            int width = Math.Max(
                MeasureContentWidth(scintLeft),
                MeasureContentWidth(scintRight)) + 32;

            scintLeft.ScrollWidth = width;
            scintRight.ScrollWidth = width;
        }

        /// <summary>현재 위치의 다음 변경 구간으로 이동한다.</summary>
        public bool MoveToNextChange()
        {
            return MoveToChange(_currentChangeIndex + 1);
        }

        /// <summary>현재 위치의 이전 변경 구간으로 이동한다.</summary>
        public bool MoveToPreviousChange()
        {
            return MoveToChange(_currentChangeIndex - 1);
        }

        /// <summary>변경 구간 인덱스에 맞춰 좌우 편집기의 선택과 표시 위치를 이동한다.</summary>
        private bool MoveToChange(int index)
        {
            if (index < 0 || index >= _changeRows.Count)
                return false;

            _currentChangeIndex = index;

            int row = _changeRows[index];

            bool wasSyncing = _isSyncing;
            _isSyncing = true;

            try
            {
                scintLeft.SetEmptySelection(
                    scintLeft.Lines[row].Position);

                scintRight.SetEmptySelection(
                    scintRight.Lines[row].Position);

                // 변경 앞쪽 문맥도 함께 보이도록 한다.
                int firstVisible = Math.Max(0, row - 3);

                scintLeft.FirstVisibleLine = firstVisible;
                scintRight.FirstVisibleLine = firstVisible;
            }
            finally
            {
                _isSyncing = wasSyncing;
            }

            NotifyNavigationChanged();
            return true;
        }

        /// <summary>수정본 실제 줄번호로 이동하고 해당 줄을 선택한 뒤 변경 구간 위치를 갱신한다.</summary>
        public bool MoveToModifiedLine(int lineNumber)
        {
            int row;

            if (lineNumber <= 0 ||
                !_modifiedLineRows.TryGetValue(lineNumber, out row))
            {
                return false;
            }

            bool wasSyncing = _isSyncing;
            _isSyncing = true;

            try
            {
                var line = scintRight.Lines[row];

                scintLeft.SetEmptySelection(
                    scintLeft.Lines[row].Position);

                scintRight.SetSelection(
                    line.Position,
                    line.EndPosition);

                scintRight.XOffset = 0;
                scintLeft.XOffset = 0;

                scintRight.Focus();
                scintRight.ScrollCaret();

                int firstVisible = Math.Max(0, row - 3);

                scintLeft.FirstVisibleLine = firstVisible;
                scintRight.FirstVisibleLine = firstVisible;

                // 이전·다음 변경 이동도 현재 위치를 기준으로 동작한다.
                _currentChangeIndex = _changeRows.FindLastIndex(
                    changeRow => changeRow <= row);
            }
            finally
            {
                _isSyncing = wasSyncing;
            }

            NotifyNavigationChanged();
            return true;
        }

        #endregion

        #region Operations

        /// <summary>탭을 고려하여 현재 글꼴과 배율로 표시할 소스의 최대 폭을 계산한다.</summary>
        private static int MeasureContentWidth(Scintilla scint)
        {
            int width = 1;

            var tabSpaces = new string(
                ' ',
                Math.Max(1, scint.TabWidth));

            foreach (var line in scint.Lines)
            {
                // 탭이 있는 줄도 충분한 스크롤 범위를 확보한다.
                var text = line.Text
                    .TrimEnd('\r', '\n')
                    .Replace("\t", tabSpaces);

                width = Math.Max(
                    width,
                    scint.TextWidth(Style.Default, text));
            }

            return width;
        }

        /// <summary>변경 구간과 수정본 줄번호의 대응 관계 및 현재 선택을 초기화한다.</summary>
        private void ResetNavigation()
        {
            _changeRows.Clear();
            _modifiedLineRows.Clear();
            _currentChangeIndex = -1;
        }

        /// <summary>변경 구간 선택 상태의 갱신을 구독자에게 알린다.</summary>
        private void NotifyNavigationChanged()
        {
            var handler = ChangeNavigationChanged;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
