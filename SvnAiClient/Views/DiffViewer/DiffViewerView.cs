using System;
using System.Windows.Forms;
using System.Drawing;
using SvnAiClient.Helpers;
using DevExpress.XtraEditors;
using SvnAiClient.Views.Common;

namespace SvnAiClient.Views.DiffViewer
{
    /// <summary>좌우 비교, Diff 원문 및 변경 위치 이동 컨트롤을 제공한다.</summary>
    public partial class DiffViewerView : ThemedView, DiffViewerInterface
    {
        #region Fields

        /// <summary>Diff 분류와 표시 정책을 위임하는 Presenter.</summary>
        private readonly DiffViewerPresenter _presenter;

        /// <summary>화면과 코드 표시에서 사용하는 프로세스 전용 글꼴.</summary>
        private readonly ApplicationFonts _applicationFonts;

        /// <summary>호출자가 글꼴을 전달하지 않은 경우 화면 종료 시 직접 해제한다.</summary>
        private readonly bool _ownsFonts;
        /// <summary>범례를 접은 상태의 상단 영역 높이.</summary>
        private readonly int _collapsedHeaderHeight;
        /// <summary>범례를 펼쳐 표시하는지 여부.</summary>
        private bool _legendExpanded;

        #endregion

        #region Properties

        /// <summary>화면 제목과 리비전·검수 당시 표기.</summary>
        public string TitleText
        {
            get { return Text; }
            set { Text = value; }
        }

        /// <summary>Diff 종류와 위치 이동 결과를 표시하는 안내 문구.</summary>
        public string StatusText
        {
            get { return lblStatus.Text; }
            set { lblStatus.Text = value; lblStatus.ToolTip = value; }
        }

        /// <summary>좌우 비교 탭이 선택되어 있는지 여부.</summary>
        public bool IsComparisonSelected { get { return tabs.SelectedTabPage == tabCompare; } }

        /// <summary>좌우 비교 컨트롤이 계산한 변경 구간 수.</summary>
        public int ChangeCount { get { return diffViewer.ChangeCount; } }

        /// <summary>현재 선택된 변경 구간의 번호.</summary>
        public int CurrentChangeNumber { get { return diffViewer.CurrentChangeNumber; } }

        #endregion

        #region Initialization

        public DiffViewerView() : this(null)
        {
        }

        public DiffViewerView(ApplicationFonts fonts)
        {
            InitializeComponent();
            _collapsedHeaderHeight = header.Height;
            _applicationFonts = fonts ?? new ApplicationFonts();
            _ownsFonts = fonts == null;
            InitializeFonts(_applicationFonts);
            _presenter = new DiffViewerPresenter(this);
            diffViewer.ChangeNavigationChanged += DiffNavigation_Changed;
            btnPreviousChange.Click += btnPreviousChange_Click;
            btnNextChange.Click += btnNextChange_Click;
            tabs.SelectedPageChanged += tabs_SelectedPageChanged;
            btnClose.Click += Close_Click;
            btnToggleLegend.Click += ToggleLegend_Click;
            _presenter.UpdateNavigation();
        }

        private void InitializeFonts(ApplicationFonts fonts)
        {
            ApplyControlFonts(this, fonts);
            tabs.AppearancePage.Header.Font = fonts.GetFont(10F);
            tabs.AppearancePage.Header.Options.UseFont = true;
            lblChangePosition.Appearance.Font = fonts.GetFont(10F, FontStyle.Bold);
            lblComparisonScope.Appearance.Font = fonts.GetFont(9F, FontStyle.Bold);
            var codeFont = fonts.GetCodeFont(10F);
            txtRawDiff.Font = codeFont;
            txtRawDiff.Properties.Appearance.Font = codeFont;
            txtRawDiff.Properties.Appearance.Options.UseFont = true;
            diffViewer.SetCodeFont(codeFont);
            lblStatus.Appearance.Font = fonts.GetFont(9F);
        }

        #endregion

        #region Event Handlers

        private void DiffNavigation_Changed(object sender, EventArgs e)
        {
            _presenter.UpdateNavigation();
        }

        private void btnPreviousChange_Click(object sender, EventArgs e)
        {
            _presenter.MoveToPreviousChange();
        }

        private void btnNextChange_Click(object sender, EventArgs e)
        {
            _presenter.MoveToNextChange();
        }

        private void tabs_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            _presenter?.UpdateNavigation();
        }

        private void ToggleLegend_Click(object sender, EventArgs e)
        {
            SetLegendExpanded(!_legendExpanded);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsComparisonSelected && keyData == Keys.F7)
            {
                _presenter.MoveToNextChange();
                return true;
            }
            if (IsComparisonSelected && keyData == (Keys.Shift | Keys.F7))
            {
                _presenter.MoveToPreviousChange();
                return true;
            }
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Control Methods

        public void ShowContent(string path, string rawDiff, string textDiff, bool compare)
        {
            txtFilePath.Text = path;
            txtFilePath.ToolTip = path;
            txtRawDiff.Text = rawDiff;
            SetSideLabels("원본 (BASE)", "작업 파일");
            if (compare)
            {
                diffViewer.SetDiff(textDiff);
                tabs.SelectedTabPage = tabCompare;
            }
            else
            {
                diffViewer.Clear();
                tabs.SelectedTabPage = tabRaw;
            }
        }

        public void SetSideLabels(string original, string modified)
        {
            diffViewer.SetSideLabels(original, modified);
            lblComparisonScope.Text = original + " → " + modified;
            lblComparisonScope.ToolTip = lblComparisonScope.Text;
        }

        public void SetNavigation(bool previousEnabled, bool nextEnabled, string position)
        {
            btnPreviousChange.Enabled = previousEnabled;
            btnNextChange.Enabled = nextEnabled;
            lblChangePosition.Text = IsComparisonSelected ? position : "원문 보기";
            btnToggleLegend.Enabled = IsComparisonSelected;
            if (!IsComparisonSelected) SetLegendExpanded(false);
        }

        public void MoveToNextChange()
        {
            diffViewer.MoveToNextChange();
        }

        public void MoveToPreviousChange()
        {
            diffViewer.MoveToPreviousChange();
        }

        public bool MoveToModifiedLine(int lineNumber)
        {
            return diffViewer.MoveToModifiedLine(lineNumber);
        }

        #endregion

        #region Operations

        /// <summary>범례가 필요할 때만 상단 높이를 늘리고 기본 상태에서는 코드 영역을 유지한다.</summary>
        private void SetLegendExpanded(bool expanded)
        {
            _legendExpanded = expanded && IsComparisonSelected;
            header.SuspendLayout();
            try
            {
                legendBar.Visible = _legendExpanded;
                header.Height = _collapsedHeaderHeight + (_legendExpanded ? legendBar.Height : 0);
                btnToggleLegend.Text = _legendExpanded ? "범례 접기" : "범례 보기";
            }
            finally { header.ResumeLayout(true); }
        }


        /// <summary>코드 글꼴 적용에 앞서 일반 UI에 본문과 버튼용 공유 글꼴을 적용한다.</summary>
        private static void ApplyControlFonts(Control control, ApplicationFonts fonts)
        {
            control.Font = fonts.GetFont(control is SimpleButton ? 9F : 10F);
            var edit = control as BaseEdit;
            if (edit != null)
            {
                edit.Properties.Appearance.Font = control.Font;
                edit.Properties.Appearance.Options.UseFont = true;
            }
            var label = control as LabelControl;
            if (label != null)
            {
                label.Appearance.Font = control.Font;
                label.Appearance.Options.UseFont = true;
            }
            var button = control as SimpleButton;
            if (button != null)
            {
                button.Appearance.Font = control.Font;
                button.Appearance.Options.UseFont = true;
            }
            foreach (Control child in control.Controls) ApplyControlFonts(child, fonts);
        }

        /// <summary>작업 복사본의 Diff를 분류하여 좌우 비교 또는 원문 탭에 표시한다.</summary>
        public void SetDiff(string filePath, string diffText)
        {
            _presenter.SetDiff(filePath, diffText);
        }

        /// <summary>단일 리비전과 그 직전 리비전의 차이를 표시한다.</summary>
        public void SetRevisionDiff(string path, string diffText, long revision)
        {
            _presenter.SetRevisionDiff(path, diffText, revision - 1, revision);
        }

        /// <summary>지정한 두 리비전 사이의 차이를 표시한다.</summary>
        public void SetRevisionDiff(string path, string diffText, long fromRevision, long toRevision)
        {
            _presenter.SetRevisionDiff(path, diffText, fromRevision, toRevision);
        }

        /// <summary>검수 당시의 수정본 행으로 이동하고 결과를 안내한다.</summary>
        public void ShowIssueLocation(int? lineNumber)
        {
            _presenter.ShowIssueLocation(lineNumber);
        }

        #endregion
    }
}
