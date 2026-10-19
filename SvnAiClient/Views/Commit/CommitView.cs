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

namespace SvnAiClient.Views.Commit
{
    /// <summary>커밋 대상, AI 검수 결과와 메시지를 표시하고 사용자 요청을 전달한다.</summary>
    public partial class CommitView : ThemedView, CommitInterface
    {
        #region Fields

        /// <summary>AI 검수와 커밋 승인 처리를 위임하는 Presenter.</summary>
        private readonly CommitPresenter _presenter;

        /// <summary>초기 목록 바인딩이 검수 대상 변경 이벤트로 처리되지 않도록 한다.</summary>
        private bool _updatingControls;

        /// <summary>검수 또는 커밋 승인 중에는 새 작업과 이슈 Diff 열기를 막는다.</summary>
        private bool _actionsEnabled = true;

        /// <summary>변경 배경 입력란을 펼쳐 표시하는지 여부.</summary>
        private bool _contextExpanded;

        /// <summary>AI 제안 메시지를 최종 메시지 옆에 표시하는지 여부.</summary>
        private bool _suggestionExpanded;

        #endregion

        #region Properties

        /// <summary>화면이 폐기되지 않아 결과를 표시할 수 있는지 여부.</summary>
        public bool IsAvailable { get { return !IsDisposed && !Disposing; } }

        /// <summary>최종 승인된 커밋 메시지를 호출 화면에 전달한다.</summary>
        public string CommitMessage { get { return _presenter.CommitMessage; } }

        /// <summary>현재 선택한 AI 프로바이더 이름.</summary>
        public string SelectedProvider { get { return cboAiProvider.SelectedItem?.ToString(); } }

        /// <summary>개발자가 입력한 변경 배경과 검수 요청 내용.</summary>
        public string ChangeContext { get { return txtChangeContext.Text; } }

        /// <summary>사용자가 편집 중인 커밋 메시지.</summary>
        public string CommitMessageText
        {
            get { return txtCommitMessage.Text; }
            set { txtCommitMessage.Text = value; }
        }

        /// <summary>검수 결과에서 제안한 커밋 메시지.</summary>
        public string SuggestedMessage
        {
            get { return txtSuggestedMsg.Text; }
            set
            {
                txtSuggestedMsg.Text = value ?? string.Empty;
                bool available = !string.IsNullOrWhiteSpace(value);
                btnCopyCommitMsg.Enabled = _actionsEnabled && available;
                btnToggleSuggestion.Enabled = _actionsEnabled && available;
                if (!available) SetSuggestionExpanded(false);
                else UpdateSuggestionButton();
            }
        }

        /// <summary>검수 진행 단계와 결과를 표시하는 상태 문구.</summary>
        public string ReviewStatus
        {
            get { return lblAiStatus.Text; }
            set
            {
                lblAiStatus.Text = value ?? string.Empty;
                lblAiStatus.ToolTip = value ?? string.Empty;
            }
        }

        #endregion

        #region Initialization

        public CommitView(List<SvnFileStatus> files, AppSettings settings,
            SvnService svnService, string workingCopyPath, ApplicationFonts fonts = null)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (svnService == null) throw new ArgumentNullException(nameof(svnService));
            if (string.IsNullOrWhiteSpace(workingCopyPath))
                throw new ArgumentException("작업 폴더 경로가 필요합니다.", nameof(workingCopyPath));

            InitializeComponent();
            if (fonts != null) InitializeFonts(fonts);
            _presenter = new CommitPresenter(this, files, settings, svnService, workingCopyPath);
            ConfigureGrid();
            ConfigureIssueGrid();
            AddEventHandlers();
            _presenter.Initialize();
            ClearReview();
            SetContextExpanded(false);
        }

        private void AddEventHandlers()
        {
            btnAiReview.Click += Review_Click;
            btnCopyCommitMsg.Click += CopySuggestedMessage_Click;
            btnCommit.Click += Commit_Click;
            btnCancel.Click += Cancel_Click;
            btnToggleContext.Click += ToggleContext_Click;
            filesSplitter.Paint += FilesSplitter_Paint;
            btnToggleSuggestion.Click += ToggleSuggestion_Click;
            cboAiProvider.SelectedIndexChanged += Provider_SelectedIndexChanged;
            gridViewCommitFiles.CellValueChanged += CommitFiles_CellValueChanged;
            txtChangeContext.EditValueChanged += ChangeContext_EditValueChanged;
            gridViewIssues.FocusedRowChanged += Issue_FocusedRowChanged;
            gridViewIssues.DoubleClick += Issue_DoubleClick;
            gridViewIssues.CustomColumnDisplayText += Issue_CustomColumnDisplayText;
            gridViewIssues.CustomDrawCell += Issue_CustomDrawCell;
            btnIssueDiff.Click += ReviewIssueDiff_Click;
            btnFileDiff.Click += FileDiff_Click;
            gridViewCommitFiles.DoubleClick += CommitFile_DoubleClick;
            gridViewCommitFiles.FocusedRowChanged += CommitFile_FocusedRowChanged;
            Activated += View_Activated;
            Disposed += View_Disposed;
        }

        private void InitializeFonts(ApplicationFonts fonts)
        {
            ApplyControlFonts(this, fonts);
            foreach (var view in new[] { gridViewCommitFiles, gridViewIssues })
            {
                view.Appearance.Row.Font = fonts.GetFont(10F);
                view.Appearance.Row.Options.UseFont = true;
                view.Appearance.HeaderPanel.Font = fonts.GetFont(10F);
                view.Appearance.HeaderPanel.Options.UseFont = true;
                view.Appearance.Empty.Font = fonts.GetFont(10F);
                view.Appearance.Empty.Options.UseFont = true;
            }
            tabsReview.AppearancePage.Header.Font = fonts.GetFont(10F);
            tabsReview.AppearancePage.Header.Options.UseFont = true;
            lblReviewOutcome.Appearance.Font = fonts.GetFont(10F, FontStyle.Bold);
            txtIssueTitle.Properties.Appearance.Font = fonts.GetFont(10F, FontStyle.Bold);
            btnCommit.Appearance.Font = fonts.GetFont(9F, FontStyle.Bold);
            lblAiProvider.Appearance.Font = fonts.GetFont(9F);
            lblAiStatus.Appearance.Font = fonts.GetFont(9F);
            lblFooterHint.Appearance.Font = fonts.GetFont(9F);
            lblContextHint.Appearance.Font = fonts.GetFont(9F);
            lblMessageHint.Appearance.Font = fonts.GetFont(9F);
            cboAiProvider.Font = fonts.GetFont(9F);
            cboAiProvider.Properties.Appearance.Font = fonts.GetFont(9F);
            cboAiProvider.Properties.AppearanceDropDown.Font = fonts.GetFont(9F);
        }

        private void ConfigureIssueGrid()
        {
            gridViewIssues.OptionsBehavior.Editable = false;
            gridViewIssues.OptionsView.ShowGroupPanel = false;
            gridViewIssues.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridViewIssues.Columns.AddVisible("Severity", "심각도").Width = 75;
            gridViewIssues.Columns.AddVisible("Category", "분류").Width = 100;
            gridViewIssues.Columns.AddVisible("Title", "제목").Width = 260;
            gridViewIssues.Columns.AddVisible("FilePath", "파일").Width = 180;
        }

        private void ConfigureGrid()
        {
            // 그리드 컬럼 구성
            var view = gridViewCommitFiles;
            view.Columns.Clear();

            var colCheck = view.Columns.Add();
            colCheck.FieldName = "IsChecked";
            colCheck.Caption = "커밋";
            colCheck.ToolTip = "열 제목을 클릭하면 전체 파일 선택을 전환합니다.";
            colCheck.Width = 52;
            colCheck.OptionsColumn.FixedWidth = true;
            colCheck.VisibleIndex = 0;
            colCheck.OptionsColumn.AllowEdit = true;
            var checkEdit =
                new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();

            checkEdit.EditValueChanged += CommitCheck_EditValueChanged;
            gridCommitFiles.RepositoryItems.Add(checkEdit);

            colCheck.ColumnEdit = checkEdit;

            var colStatus = view.Columns.Add();
            colStatus.FieldName = "StatusText";
            colStatus.Caption = "상태";
            colStatus.Width = 130;
            colStatus.MinWidth = 90;
            colStatus.VisibleIndex = 1;
            colStatus.OptionsColumn.AllowEdit = false;

            var colName = view.Columns.Add();
            colName.FieldName = "FileName";
            colName.Caption = "파일명";
            colName.Width = 250;
            colName.VisibleIndex = 2;
            colName.OptionsColumn.AllowEdit = false;

            var colPath = view.Columns.Add();
            colPath.FieldName = "RelativePath";
            colPath.Caption = "경로";
            colPath.Width = 350;
            colPath.VisibleIndex = 3;
            colPath.OptionsColumn.AllowEdit = false;

            view.OptionsBehavior.Editable = true;
            view.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsSelection.EnableAppearanceFocusedCell = false;
            view.CustomDrawCell += GridViewCommitFiles_CustomDrawCell;
            view.MouseDown += GridViewCommitFiles_MouseDown;
            view.ShowingEditor += CommitFiles_ShowingEditor;
        }

        #endregion

        #region Event Handlers

        private void FilesSplitter_Paint(object sender, PaintEventArgs e)
        {
            int centerX = filesSplitter.Width / 2;
            int centerY = filesSplitter.Height / 2;
            using (var pen = new Pen(SystemColors.ControlDark))
                e.Graphics.DrawLine(pen, centerX - 18, centerY, centerX + 18, centerY);
        }

        private void ToggleContext_Click(object sender, EventArgs e)
        {
            SetContextExpanded(!_contextExpanded);
            if (_contextExpanded) txtChangeContext.Focus();
        }

        private void ToggleSuggestion_Click(object sender, EventArgs e)
        {
            SetSuggestionExpanded(!_suggestionExpanded);
        }

        private async void FileDiff_Click(object sender, EventArgs e)
        {
            await _presenter.OpenFileDiffAsync(gridViewCommitFiles.GetFocusedRow() as SvnFileStatus);
        }

        private async void CommitFile_DoubleClick(object sender, EventArgs e)
        {
            var hit = gridViewCommitFiles.CalcHitInfo(gridCommitFiles.PointToClient(Control.MousePosition));
            if (hit.InRowCell && hit.Column != null && hit.Column.FieldName != "IsChecked")
                await _presenter.OpenFileDiffAsync(gridViewCommitFiles.GetRow(hit.RowHandle) as SvnFileStatus);
        }

        private void CommitFile_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateFileDiffButton();
        }

        private async void Review_Click(object sender, EventArgs e)
        {
            await _presenter.ReviewAsync();
        }

        private async void Commit_Click(object sender, EventArgs e)
        {
            await _presenter.TryCommitAsync();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _presenter.Cancel();
        }

        private async void CopySuggestedMessage_Click(object sender, EventArgs e)
        {
            await _presenter.CopySuggestedMessageAsync();
        }

        private void Provider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updatingControls && IsAvailable)
                _presenter.InvalidateReview("AI 제공자가 변경되었습니다.");
        }

        private void CommitFiles_CellValueChanged(
            object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (!_updatingControls && IsAvailable && e.Column?.FieldName == "IsChecked")
                _presenter.InvalidateReview("검수 대상이 변경되었습니다.");
            UpdateFileCount();
            UpdateFileDiffButton();
        }

        private void ChangeContext_EditValueChanged(object sender, EventArgs e)
        {
            UpdateContextSummary();
            if (IsAvailable)
                _presenter.InvalidateReview("변경 배경이 수정되었습니다.");
        }

        private void CommitCheck_EditValueChanged(object sender, EventArgs e)
        {
            PostFileEdits();
        }

        private void GridViewCommitFiles_MouseDown(object sender, MouseEventArgs e)
        {
            var hit = gridViewCommitFiles.CalcHitInfo(new Point(e.X, e.Y));
            if (hit.InColumn && hit.Column?.FieldName == "IsChecked")
                _presenter.ToggleAllFiles();
        }

        private void ReviewIssueDiff_Click(object sender, EventArgs e)
        {
            _presenter.OpenReviewIssue(gridViewIssues.GetFocusedRow() as AiReviewIssue);
        }

        private async void View_Activated(object sender, EventArgs e)
        {
            await _presenter.ActivatedAsync();
        }

        private void Issue_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateSelectedIssue();
        }

        private void Issue_DoubleClick(object sender, EventArgs e)
        {
            var hit = gridViewIssues.CalcHitInfo(gridIssues.PointToClient(Cursor.Position));
            if (hit.InRowCell && hit.RowHandle >= 0 && btnIssueDiff.Enabled)
                _presenter.OpenReviewIssue(gridViewIssues.GetRow(hit.RowHandle) as AiReviewIssue);
        }

        private void Issue_CustomColumnDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Severity" && e.Value is IssueSeverity)
                e.DisplayText = FormatSeverity((IssueSeverity)e.Value);
        }

        private void Issue_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName != "Severity" || gridViewIssues.FocusedRowHandle == e.RowHandle)
                return;
            var issue = gridViewIssues.GetRow(e.RowHandle) as AiReviewIssue;
            if (issue == null) return;
            e.Appearance.ForeColor = issue.Severity == IssueSeverity.Error ? Color.DarkRed
                : issue.Severity == IssueSeverity.Warning ? Color.SaddleBrown : Color.SteelBlue;
        }

        private void CommitFiles_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var item = gridViewCommitFiles.GetFocusedRow() as SvnFileStatus;
            if (!_actionsEnabled || item == null || item.IsDirectory)
                e.Cancel = true;
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

        private void GridViewCommitFiles_CustomDrawCell(
            object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            var item = gridViewCommitFiles.GetRow(e.RowHandle) as SvnFileStatus;
            if (item == null) return;
            if (e.Column.FieldName == "IsChecked" && item.IsDirectory)
            {
                e.Appearance.DrawBackground(e.Cache, e.Bounds);
                e.Handled = true;
                return;
            }
            if ((e.Column.FieldName != "FileName" && e.Column.FieldName != "StatusText")
                || gridViewCommitFiles.FocusedRowHandle == e.RowHandle) return;
            switch (item.Status)
            {
                case SvnStatusKind.Modified: e.Appearance.ForeColor = Color.DarkGreen; break;
                case SvnStatusKind.Added: e.Appearance.ForeColor = Color.DarkBlue; break;
                case SvnStatusKind.Deleted:
                case SvnStatusKind.Missing: e.Appearance.ForeColor = Color.DarkRed; break;
                case SvnStatusKind.Conflicted: e.Appearance.ForeColor = Color.OrangeRed; break;
            }
        }

        #endregion

        #region Control Methods

        public void ShowFiles(List<SvnFileStatus> files, string provider)
        {
            _updatingControls = true;
            try
            {
                cboAiProvider.Properties.Items.Clear();
                cboAiProvider.Properties.Items.AddRange(new[] { "Claude", "OpenAI", "Gemini", "Local" });
                cboAiProvider.SelectedItem = provider;
                gridCommitFiles.DataSource = files;
            }
            finally
            {
                _updatingControls = false;
            }
            UpdateFileCount();
            UpdateFileDiffButton();
        }

        public void RefreshFiles(List<SvnFileStatus> files)
        {
            _updatingControls = true;
            try
            {
                gridCommitFiles.DataSource = null;
                gridCommitFiles.DataSource = files;
            }
            finally
            {
                _updatingControls = false;
            }
            UpdateFileCount();
            UpdateFileDiffButton();
        }

        public void PostFileEdits()
        {
            gridViewCommitFiles.PostEditor();
            gridViewCommitFiles.UpdateCurrentRow();
        }

        public void SetActionsEnabled(bool enabled)
        {
            _actionsEnabled = enabled;
            btnAiReview.Enabled = enabled;
            btnCommit.Enabled = enabled;
            cboAiProvider.Enabled = enabled;
            gridCommitFiles.Enabled = enabled;
            UpdateFileDiffButton();
            txtChangeContext.Enabled = enabled;
            btnToggleContext.Enabled = enabled;
            btnToggleSuggestion.Enabled = enabled && !string.IsNullOrWhiteSpace(txtSuggestedMsg.Text);
            btnCopyCommitMsg.Enabled = enabled && !string.IsNullOrEmpty(txtSuggestedMsg.Text);
            UpdateSelectedIssue();
        }

        public void ClearReview()
        {
            gridIssues.DataSource = null;
            lblReviewOutcome.Text = "검수 결과 없음";
            lblTokenUsage.Text = "검수 실행 후 토큰 사용량이 표시됩니다.";
            txtReviewSummary.Text = "변경 배경과 검수 요청을 입력한 뒤 'AI 검수 실행'을 누르세요.";
            txtReviewErrors.Text = string.Empty;
            pageErrors.PageVisible = false;
            pageIssues.Text = "이슈 (0)";
            tabsReview.SelectedTabPage = pageSummary;
            SuggestedMessage = string.Empty;
            UpdateSelectedIssue();
        }

        public void ShowMessage(string message, string title, CommitNotice kind)
        {
            var icon = kind == CommitNotice.Error ? MessageBoxIcon.Error
                : kind == CommitNotice.Warning ? MessageBoxIcon.Warning : MessageBoxIcon.Information;
            XtraMessageBox.Show(this, message, title, MessageBoxButtons.OK, icon);
        }

        public bool Confirm(string message, string title)
        {
            return XtraMessageBox.Show(this, message, title, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        public void ShowIssueDiff(string path, string diff, int? lineNumber)
        {
            using (var viewer = new DiffViewerView())
            {
                viewer.SetDiff(path, diff);
                viewer.Shown += (sender, e) => viewer.ShowIssueLocation(lineNumber);
                viewer.ShowDialog(this);
            }
        }

        public void ShowHeadWorkingDiff(string path, string diff)
        {
            using (var viewer = new DiffViewerView())
            {
                viewer.SetDiff(path, diff);
                viewer.TitleText = System.IO.Path.GetFileName(path) + " - 저장소 최신(HEAD) ↔ 작업 파일";
                viewer.SetSideLabels("저장소 최신 (HEAD)", "작업 파일 (Working)");
                viewer.StatusText = "저장소 최신 버전과 현재 작업 파일의 비교입니다. " + viewer.StatusText;
                viewer.ShowDialog(this);
            }
        }

        public void CompleteCommit()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public void CancelCommit()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public void ShowReviewResult(AiReviewResult result)
        {
            if (result == null) { ClearReview(); return; }
            SuggestedMessage = string.Empty;
            var issues = (result.Issues ?? new List<AiReviewIssue>()).Where(issue => issue != null).ToList();
            gridIssues.DataSource = issues;
            gridViewIssues.FocusedRowHandle = issues.Count > 0 ? 0 : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
            pageIssues.Text = "이슈 (" + issues.Count + ")";
            lblReviewOutcome.Text = FormatReviewState(result.Status)
                + " / 오류 " + issues.Count(issue => issue.Severity == IssueSeverity.Error)
                + " / 경고 " + issues.Count(issue => issue.Severity == IssueSeverity.Warning)
                + " / 참고 " + issues.Count(issue => issue.Severity == IssueSeverity.Info);
            lblTokenUsage.Text = "추정 입력 " + result.EstimatedInputTokens.ToString("N0")
                + " / 실제 입력 " + (result.ActualInputTokens.HasValue ? result.ActualInputTokens.Value.ToString("N0") : "미제공")
                + " / 실제 출력 " + (result.ActualOutputTokens.HasValue ? result.ActualOutputTokens.Value.ToString("N0") : "미제공")
                + " 토큰 / 소요 " + (result.ElapsedMs / 1000.0).ToString("N1") + "초";
            lblTokenUsage.ToolTip = lblTokenUsage.Text;
            txtReviewSummary.Text = string.IsNullOrWhiteSpace(result.Summary)
                ? "검수 요약이 제공되지 않았습니다." : result.Summary;
            txtReviewErrors.Text = result.ErrorMessage ?? string.Empty;
            pageErrors.PageVisible = !string.IsNullOrWhiteSpace(result.ErrorMessage);
            tabsReview.SelectedTabPage = result.Status == AiReviewStatus.Failed && pageErrors.PageVisible
                ? pageErrors : pageSummary;
            UpdateSelectedIssue();
        }

        #endregion

        #region Operations

        /// <summary>앱에서 공유하는 글꼴을 본문과 버튼의 역할에 맞춰 적용한다.</summary>
        private static void ApplyControlFonts(Control control, ApplicationFonts fonts)
        {
            control.Font = fonts.GetFont(control is SimpleButton ? 9F : 10F);
            var edit = control as BaseEdit;
            if (edit != null)
            {
                edit.Properties.Appearance.Font = control.Font;
                edit.Properties.Appearance.Options.UseFont = true;
            }
            var button = control as SimpleButton;
            if (button != null)
            {
                button.Appearance.Font = control.Font;
                button.Appearance.Options.UseFont = true;
            }
            var label = control as LabelControl;
            if (label != null)
            {
                label.Appearance.Font = control.Font;
                label.Appearance.Options.UseFont = true;
            }
            var group = control as GroupControl;
            if (group != null)
            {
                group.AppearanceCaption.Font = fonts.GetFont(10F, FontStyle.Bold);
                group.AppearanceCaption.Options.UseFont = true;
            }
            foreach (Control child in control.Controls) ApplyControlFonts(child, fonts);
        }

        /// <summary>변경 배경 입력란의 표시 상태와 해당 행 높이를 함께 변경한다.</summary>
        private void SetContextExpanded(bool expanded)
        {
            _contextExpanded = expanded;
            layoutRoot.SuspendLayout();
            try
            {
                grpChangeContext.Visible = expanded;
                layoutRoot.RowStyles[0].Height = contextTools.Height * (expanded ? 3 : 1);
                btnToggleContext.Text = expanded ? "변경 배경 접기" : "변경 배경 펼치기";
                UpdateContextSummary();
            }
            finally { layoutRoot.ResumeLayout(true); }
        }

        /// <summary>입력값을 변경하지 않고 접힌 변경 배경의 첫 줄과 전체 내용 툴팁을 표시한다.</summary>
        private void UpdateContextSummary()
        {
            string context = txtChangeContext.Text.Trim();
            string firstLine = context.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? string.Empty;
            lblContextHint.Text = context.Length == 0
                ? "선택 사항 · 변경 배경과 검수 요청을 입력할 수 있습니다."
                : "입력됨 · " + firstLine;
            lblContextHint.ToolTip = context;
        }

        /// <summary>최종 메시지의 폭을 우선하고 필요할 때만 AI 제안 영역을 나란히 표시한다.</summary>
        private void SetSuggestionExpanded(bool expanded)
        {
            _suggestionExpanded = expanded && !string.IsNullOrWhiteSpace(txtSuggestedMsg.Text);
            layoutMessages.SuspendLayout();
            try
            {
                grpSuggestedMessage.Visible = _suggestionExpanded;
                layoutMessages.ColumnStyles[0].Width = _suggestionExpanded ? 65F : 100F;
                layoutMessages.ColumnStyles[1].SizeType = _suggestionExpanded ? SizeType.Percent : SizeType.Absolute;
                layoutMessages.ColumnStyles[1].Width = _suggestionExpanded ? 35F : 0F;
                grpCommitMessage.Margin = new Padding(0, 0, _suggestionExpanded ? filesSplitter.Height : 0, 0);
                UpdateSuggestionButton();
            }
            finally { layoutMessages.ResumeLayout(true); }
        }

        /// <summary>AI 제안 유무와 펼침 상태를 표시 전환 버튼에 반영한다.</summary>
        private void UpdateSuggestionButton()
        {
            btnToggleSuggestion.Text = string.IsNullOrWhiteSpace(txtSuggestedMsg.Text)
                ? "AI 제안 없음" : _suggestionExpanded ? "AI 제안 접기" : "AI 제안 보기";
        }

        /// <summary>진행 중인 작업과 포커스된 파일에 따라 최신본 비교 버튼을 갱신한다.</summary>
        private void UpdateFileDiffButton()
        {
            var file = gridViewCommitFiles.GetFocusedRow() as SvnFileStatus;
            btnFileDiff.Enabled = _actionsEnabled && file != null && !file.IsDirectory;
        }

        /// <summary>전체 파일 수와 현재 체크한 커밋 대상 수를 표시한다.</summary>
        private void UpdateFileCount()
        {
            var files = gridCommitFiles.DataSource as List<SvnFileStatus>;
            int total = files == null ? 0 : files.Count(file => file != null && !file.IsDirectory);
            int selected = files == null ? 0 : files.Count(file => file != null && !file.IsDirectory && file.IsChecked);
            grpCommitFiles.Text = "커밋 대상 · 선택 " + selected + " / 전체 " + total;
            lblFileDiffHint.Text = "커밋 열 제목: 전체 선택 전환 · 파일 더블클릭: 최신 버전과 비교";
        }

        /// <summary>선택한 이슈의 제목과 위치 및 설명을 표시하고 Diff 열기 가능 여부를 반영한다.</summary>
        private void UpdateSelectedIssue()
        {
            var issue = gridViewIssues.GetFocusedRow() as AiReviewIssue;
            if (issue == null)
            {
                txtIssueTitle.Text = "선택한 이슈 없음";
                txtIssueTitle.ToolTip = string.Empty;
                txtIssueLocation.Text = string.Empty;
                txtIssueLocation.ToolTip = string.Empty;
                txtIssueDescription.Text = "이슈를 선택하면 설명과 파일 위치가 표시됩니다.";
                btnIssueDiff.Enabled = false;
                return;
            }
            txtIssueTitle.Text = "[" + FormatSeverity(issue.Severity) + "] "
                + (issue.Category ?? string.Empty) + " / " + (issue.Title ?? "제목 없음");
            txtIssueTitle.ToolTip = txtIssueTitle.Text;
            txtIssueLocation.Text = (string.IsNullOrWhiteSpace(issue.FilePath) ? "파일 위치 미지정" : issue.FilePath)
                + (issue.LineNumber.HasValue && issue.LineNumber.Value > 0 ? " : " + issue.LineNumber.Value + "행" : string.Empty);
            txtIssueLocation.ToolTip = txtIssueLocation.Text;
            txtIssueDescription.Text = string.IsNullOrWhiteSpace(issue.Description)
                ? "설명이 제공되지 않았습니다." : issue.Description;
            btnIssueDiff.Enabled = _actionsEnabled && !string.IsNullOrWhiteSpace(issue.FilePath);
        }

        /// <summary>심각도 값을 화면에서 읽기 쉬운 문구로 변환한다.</summary>
        private static string FormatSeverity(IssueSeverity severity)
        {
            return severity == IssueSeverity.Error ? "오류" : severity == IssueSeverity.Warning ? "경고" : "참고";
        }

        /// <summary>코드 이슈의 유무와 구분하여 검수 요청의 처리 상태를 표시한다.</summary>
        private static string FormatReviewState(AiReviewStatus state)
        {
            switch (state)
            {
                case AiReviewStatus.Succeeded: return "검수 완료";
                case AiReviewStatus.Partial: return "부분 성공";
                case AiReviewStatus.Failed: return "검수 실패";
                case AiReviewStatus.Cancelled: return "검수 취소";
                default: return "미검수";
            }
        }



        /// <summary>최종 승인한 파일 경로를 MainView에 반환한다.</summary>
        public List<string> GetCheckedFilePaths()
        {
            return _presenter.GetCheckedFilePaths();
        }

        /// <summary>최종 승인 시점의 파일 해시와 SVN 상태 스냅샷을 MainView에 반환한다.</summary>
        public Dictionary<string, SvnFileSnapshot> GetApprovedCommitFiles()
        {
            return _presenter.GetApprovedCommitFiles();
        }

        /// <summary>성공 리비전을 연결할 현재 검수 이력 ID를 반환한다.</summary>
        public Guid GetReviewHistoryId()
        {
            return _presenter.GetReviewHistoryId();
        }

        #endregion
    }
}
