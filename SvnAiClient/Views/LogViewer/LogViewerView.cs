using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SvnAiClient.Core.Svn;
using SvnAiClient.Helpers;
using SvnAiClient.Views.DiffViewer;
using SvnAiClient.Models;
using SvnAiClient.Views.Common;

namespace SvnAiClient.Views.LogViewer
{
    /// <summary>SVN 로그와 리비전 비교 결과를 표시하고 사용자 요청을 전달한다.</summary>
    public partial class LogViewerView : ThemedView, LogViewerInterface
    {
        #region Fields

        /// <summary>로그 조회, 비교 범위 및 취소 상태의 처리를 위임하는 Presenter.</summary>
        private readonly LogViewerPresenter _presenter;

        /// <summary>목록 바인딩 중 발생하는 선택 이벤트가 상세 표시를 덮어쓰지 않도록 한다.</summary>
        private bool _updatingControls;

        /// <summary>조회 및 Diff 처리 중 파일 열기 버튼의 재활성화를 방지한다.</summary>
        private bool _isBusy;

        #endregion

        #region Properties

        /// <summary>화면이 폐기되지 않아 결과를 표시할 수 있는지 여부.</summary>
        public bool IsAvailable
        {
            get { return !IsDisposed && !Disposing; }
        }

        /// <summary>사용자가 지정한 최대 로그 조회 건수.</summary>
        public int RequestedCount
        {
            get { return (int)numCount.Value; }
        }

        /// <summary>로그 목록에서 포커스된 항목. 항목이 없으면 null이다.</summary>
        public SvnLogEntry FocusedLog
        {
            get { return viewLogs.GetFocusedRow() as SvnLogEntry; }
        }

        /// <summary>로그 목록에서 선택한 데이터 행의 항목들.</summary>
        public List<SvnLogEntry> SelectedLogs
        {
            get
            {
                return viewLogs.GetSelectedRows()
                    .Where(handle => handle >= 0)
                    .Select(handle => viewLogs.GetRow(handle) as SvnLogEntry)
                    .Where(entry => entry != null)
                    .ToList();
            }
        }

        /// <summary>변경 경로 목록에서 선택한 항목. 선택하지 않았으면 null이다.</summary>
        public SvnChangedPath SelectedChange
        {
            get { return listPaths.SelectedItem as SvnChangedPath; }
        }

        #endregion

        #region Initialization

        public LogViewerView(string workingCopyPath, SvnService service, ApplicationFonts fonts = null)
        {
            if (string.IsNullOrWhiteSpace(workingCopyPath))
                throw new ArgumentException("작업 폴더 경로가 필요합니다.", nameof(workingCopyPath));
            if (service == null) throw new ArgumentNullException(nameof(service));

            InitializeComponent();
            Text = "SVN 로그";
            txtTargetPath.Text = workingCopyPath;
            txtTargetPath.ToolTip = workingCopyPath;
            if (fonts != null) InitializeFonts(fonts);
            ConfigureGrid();
            _presenter = new LogViewerPresenter(this, workingCopyPath, service);
            AddEventHandlers();
        }

        private void ConfigureGrid()
        {
            viewLogs.OptionsBehavior.Editable = false;
            viewLogs.OptionsSelection.MultiSelect = true;
            viewLogs.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            viewLogs.OptionsSelection.EnableAppearanceFocusedCell = false;
            viewLogs.OptionsView.ShowGroupPanel = false;
            viewLogs.OptionsView.ShowAutoFilterRow = true;
            viewLogs.OptionsView.ShowIndicator = false;
            viewLogs.OptionsBehavior.AutoPopulateColumns = false;
            viewLogs.OptionsDetail.EnableMasterViewMode = false;
            var revision = viewLogs.Columns.AddVisible("Revision", "리비전");
            revision.Width = 90;
            revision.MinWidth = 80;
            revision.OptionsColumn.FixedWidth = true;
            revision.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            revision.DisplayFormat.FormatString = "'r'0";
            var author = viewLogs.Columns.AddVisible("Author", "작성자");
            author.Width = 130;
            author.MinWidth = 90;
            var date = viewLogs.Columns.AddVisible("Date", "커밋 시각");
            date.Width = TextRenderer.MeasureText("2000-00-00 00:00:00", Font).Width + 28;
            date.MinWidth = date.Width;
            date.OptionsColumn.FixedWidth = true;
            date.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            date.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            viewLogs.Columns.AddVisible("Message", "커밋 메시지").Width = 650;
        }

        private void InitializeFonts(ApplicationFonts fonts)
        {
            ApplyControlFonts(this, fonts);
            viewLogs.Appearance.Row.Font = fonts.GetFont(10F);
            viewLogs.Appearance.Row.Options.UseFont = true;
            viewLogs.Appearance.HeaderPanel.Font = fonts.GetFont(10F);
            viewLogs.Appearance.HeaderPanel.Options.UseFont = true;
            viewLogs.Appearance.Empty.Font = fonts.GetFont(10F);
            viewLogs.Appearance.Empty.Options.UseFont = true;
            foreach (var label in new[] { lblGridHint, lblPathHint, lblStatus })
                label.Appearance.Font = fonts.GetFont(9F);
            lblSelection.Appearance.Font = fonts.GetFont(10F, FontStyle.Bold);
            lblComparisonScope.Appearance.Font = fonts.GetFont(10F, FontStyle.Bold);
        }

        private void AddEventHandlers()
        {
            Shown += View_Shown;
            Disposed += View_Disposed;
            btnRefresh.Click += Refresh_Click;
            btnStop.Click += Stop_Click;
            btnCompare.Click += Compare_Click;
            viewLogs.DoubleClick += LogGrid_DoubleClick;
            viewLogs.SelectionChanged += LogGrid_SelectionChanged;
            viewLogs.FocusedRowChanged += LogGrid_FocusedRowChanged;
            listPaths.DoubleClick += ChangedPath_DoubleClick;
            listPaths.SelectedIndexChanged += ChangedPath_SelectedIndexChanged;
            btnOpenDiff.Click += OpenDiff_Click;
            btnClose.Click += Close_Click;
            viewLogs.CustomColumnDisplayText += LogGrid_CustomColumnDisplayText;
        }

        #endregion

        #region Event Handlers

        private async void View_Shown(object sender, EventArgs e)
        {
            await _presenter.LoadLogsAsync();
        }

        private async void Refresh_Click(object sender, EventArgs e)
        {
            await _presenter.LoadLogsAsync();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            _presenter.Cancel();
        }

        private async void Compare_Click(object sender, EventArgs e)
        {
            await _presenter.CompareSelectedRevisionsAsync();
        }

        private async void LogGrid_DoubleClick(object sender, EventArgs e)
        {
            var hit = viewLogs.CalcHitInfo(gridLogs.PointToClient(Cursor.Position));
            if (!hit.InRowCell || hit.RowHandle < 0) return;

            await _presenter.OpenRevisionAsync(viewLogs.GetRow(hit.RowHandle) as SvnLogEntry);
        }

        private void LogGrid_SelectionChanged(
            object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (!_updatingControls)
                _presenter.SelectionChanged();
        }

        private void LogGrid_FocusedRowChanged(
            object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (!_updatingControls)
                _presenter.SelectionChanged();
        }

        private async void ChangedPath_DoubleClick(object sender, EventArgs e)
        {
            await _presenter.OpenSelectedRevisionDiffAsync();
        }

        private async void OpenDiff_Click(object sender, EventArgs e)
        {
            await _presenter.OpenSelectedRevisionDiffAsync();
        }

        private void ChangedPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDiffButton();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LogGrid_CustomColumnDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Message" && e.Value != null)
                e.DisplayText = e.Value.ToString().Replace("\r", " ").Replace("\n", " ").Trim();
        }

        private void View_Disposed(object sender, EventArgs e)
        {
            _presenter.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!e.Cancel)
                _presenter.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Control Methods

        public void ShowLogs(List<SvnLogEntry> logs)
        {
            _updatingControls = true;
            try
            {
                gridLogs.DataSource = logs;
                viewLogs.FocusedRowHandle = logs.Count > 0 ? 0 : -1;
                grpLogs.Text = "리비전 목록 / 조회 " + logs.Count + "건";
            }
            finally
            {
                _updatingControls = false;
            }
        }

        public void ShowLogDetails(SvnLogEntry entry)
        {
            txtMessage.Text = entry?.Message ?? string.Empty;
            lblComparisonScope.Text = entry == null ? "표시 범위 · 리비전을 선택하세요."
                : entry.Revision < 1 ? "표시 범위 · r0 (이전 시점 없음)"
                : "표시 범위 · r" + (entry.Revision - 1) + " → r" + entry.Revision + " · 선택 리비전의 변경";
            lblComparisonScope.ToolTip = lblComparisonScope.Text;
            grpMessage.Text = entry == null ? "커밋 메시지" : "커밋 메시지 / r" + entry.Revision;
            lblLogInfo.Text = entry == null ? "리비전을 선택하세요."
                : entry.Author + " / " + entry.DateText;
            lblLogInfo.ToolTip = lblLogInfo.Text;
            listPaths.Items.Clear();
            grpPaths.Text = "변경 경로 (저장소 기준)";
            if (entry == null)
            {
                UpdateDiffButton();
                return;
            }

            foreach (var path in entry.Changes)
                listPaths.Items.Add(path);
            grpPaths.Text = "변경 경로 - r" + entry.Revision
                + " (" + entry.Changes.Count + "건)";
            UpdateDiffButton();
        }

        public void ShowComparison(long fromRevision, long toRevision,
            string message, List<SvnChangedPath> changes)
        {
            txtMessage.Text = message ?? string.Empty;
            lblComparisonScope.Text = "표시 범위 · r" + fromRevision + " → r" + toRevision
                + " · 변경 " + changes.Count + "건";
            lblComparisonScope.ToolTip = lblComparisonScope.Text;
            grpMessage.Text = "비교 대상 커밋 메시지";
            lblLogInfo.Text = "r" + fromRevision + " → r" + toRevision + " / 두 시점 사이의 변경";
            lblLogInfo.ToolTip = lblLogInfo.Text;
            listPaths.Items.Clear();
            foreach (var change in changes)
                listPaths.Items.Add(change);
            grpPaths.Text = "변경 경로 - r" + fromRevision + " → r" + toRevision
                + " (" + changes.Count + "건)";
            UpdateDiffButton();
        }

        public void ClearChanges()
        {
            lblComparisonScope.Text = "비교 범위 조회 중…";
            lblComparisonScope.ToolTip = lblComparisonScope.Text;
            listPaths.Items.Clear();
            UpdateDiffButton();
        }

        public void SelectChange(SvnChangedPath change)
        {
            listPaths.SelectedItem = change;
            UpdateDiffButton();
        }

        public void SetBusy(bool busy)
        {
            _isBusy = busy;
            _updatingControls = true;
            try
            {
                btnRefresh.Enabled = !busy;
                numCount.Enabled = !busy;
                btnStop.Enabled = busy;
                btnCompare.Enabled = false;
                gridLogs.Enabled = !busy;
                listPaths.Enabled = !busy;
            }
            finally
            {
                _updatingControls = false;
            }
            UpdateDiffButton();
            UpdateSelectionText();
        }

        public void SetCompareEnabled(bool enabled)
        {
            btnCompare.Enabled = enabled;
            UpdateSelectionText();
        }

        public void SetStatus(string message)
        {
            lblStatus.Text = message;
            lblStatus.ToolTip = message;
        }

        public void ShowInformation(string message)
        {
            XtraMessageBox.Show(this, message, "알림");
        }

        public void ShowError(string message)
        {
            XtraMessageBox.Show(this, message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowRevisionDiff(string path, string diff, long fromRevision, long toRevision)
        {
            using (var viewer = new DiffViewerView())
            {
                viewer.SetRevisionDiff(path, diff, fromRevision, toRevision);
                viewer.ShowDialog(this);
            }
        }

        #endregion

        #region Operations

        /// <summary>선택 개수와 비교할 리비전 범위를 표시하고 선택 방법을 안내한다.</summary>
        private void UpdateSelectionText()
        {
            var selected = SelectedLogs.Select(entry => entry.Revision).Distinct().OrderBy(value => value).ToList();
            lblSelection.Text = selected.Count == 2
                ? "비교할 범위 · r" + selected[0] + " → r" + selected[1] + " (오래된 시점 → 최근 시점)"
                : selected.Count == 1
                    ? "선택: r" + selected[0] + " / Ctrl+클릭으로 비교할 리비전을 추가하세요."
                    : selected.Count > 2
                        ? "선택 " + selected.Count + "개 / 비교할 리비전 2개만 선택하세요."
                        : "Ctrl+클릭: 두 시점 선택 · 더블클릭: 이전 시점과 변경 내용 보기";
            lblSelection.ToolTip = lblSelection.Text;
        }

        /// <summary>처리 중이 아니며 파일을 선택했을 때 Diff 열기를 활성화한다.</summary>
        private void UpdateDiffButton()
        {
            btnOpenDiff.Enabled = !_isBusy && SelectedChange != null && !SelectedChange.IsDirectory;
        }

        /// <summary>공유 글꼴을 본문과 버튼 및 DevExpress 표시 속성에 적용한다.</summary>
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
            var group = control as GroupControl;
            if (group != null)
            {
                group.AppearanceCaption.Font = fonts.GetFont(10F, FontStyle.Bold);
                group.AppearanceCaption.Options.UseFont = true;
            }
            foreach (Control child in control.Controls) ApplyControlFonts(child, fonts);
        }

        #endregion
    }
}
