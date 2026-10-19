using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using SvnAiClient.Core.AI;
using SvnAiClient.Helpers;
using SvnAiClient.Models;
using SvnAiClient.Views.Common;

namespace SvnAiClient.Views.AiReviewHistory
{
    /// <summary>로컬에 저장한 AI 검수 이력의 목록과 상세를 읽기 전용으로 표시한다.</summary>
    public sealed partial class AiReviewHistoryView : ThemedView, AiReviewHistoryInterface
    {
        #region Fields

        /// <summary>이력 조회와 필터, 상세 문구 생성을 위임하는 Presenter.</summary>
        private readonly AiReviewHistoryPresenter _presenter;

        /// <summary>목록 바인딩 중 발생하는 선택 이벤트의 중복 상세 표시를 막는다.</summary>
        private bool _updatingControls;
        /// <summary>조회용 부가 정보 탭을 표시하는지 여부.</summary>
        private bool _showMoreDetails;

        #endregion

        #region Properties

        /// <summary>화면이 폐기되지 않아 조회 결과를 표시할 수 있는지 여부.</summary>
        public bool IsAvailable { get { return !IsDisposed && !Disposing; } }

        /// <summary>현재 작업 복사본만 표시하는 필터의 선택 여부.</summary>
        public bool CurrentOnly { get { return _currentOnly.Checked; } }

        /// <summary>이력 그리드의 포커스 항목. 항목이 없으면 null이다.</summary>
        public AiReviewHistoryEntry SelectedEntry
        {
            get { return _view.GetFocusedRow() as AiReviewHistoryEntry; }
        }

        #endregion

        #region Initialization

        public AiReviewHistoryView() : this(null)
        {
        }

        public AiReviewHistoryView(string workingCopyPath, ApplicationFonts fonts = null)
        {
            InitializeComponent();
            txtWorkingCopy.Text = string.IsNullOrWhiteSpace(workingCopyPath)
                ? "현재 작업 폴더 없음 / 전체 이력 조회" : workingCopyPath;
            txtWorkingCopy.ToolTip = txtWorkingCopy.Text;
            if (fonts != null) InitializeFonts(fonts);
            ConfigureGrids();
            _presenter = new AiReviewHistoryPresenter(this, workingCopyPath, new AiReviewHistoryService());
            _presenter.Initialize();
            AddEventHandlers();
            ShowDetail(null, "이력을 선택하세요.");
        }

        private void ConfigureGrids()
        {
            foreach (var grid in new[] { _view, viewIssues })
            {
                grid.OptionsBehavior.Editable = false;
                grid.OptionsBehavior.AutoPopulateColumns = false;
                grid.OptionsDetail.EnableMasterViewMode = false;
                grid.OptionsSelection.EnableAppearanceFocusedCell = false;
                grid.OptionsView.ShowGroupPanel = false;
                grid.OptionsView.ShowIndicator = false;
            }
            _view.OptionsView.ShowAutoFilterRow = true;
            var date = _view.Columns.AddVisible("ReviewedAt", "검수 시각");
            date.Width = TextRenderer.MeasureText("2000-00-00 00:00:00", Font).Width + 28;
            date.MinWidth = date.Width;
            date.OptionsColumn.FixedWidth = true;
            _view.Columns.AddVisible("Status", "검수 상태").Width = 100;
            var revision = _view.Columns.AddVisible("SvnRevision", "연결 리비전");
            revision.Width = 125;
            revision.ToolTip = "커밋 성공 후 연결한 SVN 리비전입니다. 미연결은 커밋 여부를 뜻하지 않습니다.";
            _view.Columns.AddVisible("ProviderName", "프로바이더").Width = 130;
            _view.Columns.AddVisible("ModelName", "모델").Width = 220;
            _view.Columns.AddVisible("WorkingCopyPath", "작업 폴더").Width = 380;
            viewIssues.Columns.AddVisible("Severity", "심각도").Width = 80;
            viewIssues.Columns.AddVisible("Category", "분류").Width = 100;
            viewIssues.Columns.AddVisible("Title", "이슈 제목").Width = 420;
            viewIssues.Columns.AddVisible("FilePath", "파일").Width = 300;
            viewIssues.Columns.AddVisible("LineNumber", "행").Width = 65;
        }

        private void InitializeFonts(ApplicationFonts fonts)
        {
            ApplyControlFonts(this, fonts);
            foreach (var grid in new[] { _view, viewIssues })
            {
                grid.Appearance.Row.Font = fonts.GetFont(10F);
                grid.Appearance.Row.Options.UseFont = true;
                grid.Appearance.HeaderPanel.Font = fonts.GetFont(10F);
                grid.Appearance.HeaderPanel.Options.UseFont = true;
                grid.Appearance.Empty.Font = fonts.GetFont(10F);
                grid.Appearance.Empty.Options.UseFont = true;
            }
            tabs.AppearancePage.Header.Font = fonts.GetFont(10F);
            tabs.AppearancePage.Header.Options.UseFont = true;
            lblOutcome.Appearance.Font = fonts.GetFont(10F, FontStyle.Bold);
            foreach (var label in new[] { lblReadOnly, lblTokenInfo, _status })
                label.Appearance.Font = fonts.GetFont(9F);
        }

        private void AddEventHandlers()
        {
            btnHistoryDetails.Click += HistoryDetails_Click;
            _view.CustomColumnDisplayText += History_CustomColumnDisplayText;
            _view.FocusedRowChanged += History_FocusedRowChanged;
            _currentOnly.CheckedChanged += CurrentOnly_CheckedChanged;
            _refresh.Click += Refresh_Click;
            btnClose.Click += Close_Click;
            viewIssues.FocusedRowChanged += Issue_FocusedRowChanged;
            viewIssues.CustomColumnDisplayText += Issue_CustomColumnDisplayText;
            _view.RowCellStyle += History_RowCellStyle;
            Shown += View_Shown;
            Disposed += View_Disposed;
        }

        #endregion

        #region Event Handlers

        private void HistoryDetails_Click(object sender, EventArgs e)
        {
            _showMoreDetails = !_showMoreDetails;
            if (!_showMoreDetails && (tabs.SelectedTabPage == pageContext ||
                tabs.SelectedTabPage == pageCommit || tabs.SelectedTabPage == pageDetail))
                tabs.SelectedTabPage = pageSummary;
            pageContext.PageVisible = _showMoreDetails;
            pageCommit.PageVisible = _showMoreDetails;
            pageDetail.PageVisible = _showMoreDetails;
            btnHistoryDetails.Text = _showMoreDetails ? "추가 정보 접기" : "추가 정보 보기";
        }


        private void History_CustomColumnDisplayText(
            object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "SvnRevision" && e.ListSourceRowIndex < 0
                && (e.Value == null || e.Value == DBNull.Value)) return;
            string text = AiReviewHistoryPresenter.FormatColumnText(e.Column.FieldName, e.Value);
            if (text != null)
                e.DisplayText = text;
        }

        private void History_FocusedRowChanged(
            object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (!_updatingControls)
                _presenter.ShowSelectedEntry();
        }

        private void CurrentOnly_CheckedChanged(object sender, EventArgs e)
        {
            _presenter.ApplyFilter();
        }

        private async void Refresh_Click(object sender, EventArgs e)
        {
            await _presenter.ReloadAsync();
        }

        private async void View_Shown(object sender, EventArgs e)
        {
            await _presenter.ReloadAsync();
        }

        private void View_Disposed(object sender, EventArgs e)
        {
            _presenter.Close();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Issue_FocusedRowChanged(object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            ShowSelectedIssue();
        }

        private void Issue_CustomColumnDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Severity" && e.Value is IssueSeverity)
            {
                var severity = (IssueSeverity)e.Value;
                e.DisplayText = severity == IssueSeverity.Error ? "오류"
                    : severity == IssueSeverity.Warning ? "경고" : "정보";
            }
        }

        private void History_RowCellStyle(object sender,
            DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0 || e.Column.FieldName != "Status" || _view.IsRowSelected(e.RowHandle)) return;
            var entry = _view.GetRow(e.RowHandle) as AiReviewHistoryEntry;
            if (entry != null) e.Appearance.ForeColor = StatusColor(entry.Status);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!e.Cancel)
                _presenter.Close();
        }

        #endregion

        #region Control Methods

        public void ConfigureCurrentOnlyFilter(bool available)
        {
            _currentOnly.Enabled = available;
            _currentOnly.Checked = available;
        }

        public void SetLoading(bool loading, bool canFilter)
        {
            _refresh.Enabled = !loading;
            _currentOnly.Enabled = !loading && canFilter;
        }

        public void ShowEntries(List<AiReviewHistoryEntry> entries)
        {
            _updatingControls = true;
            try
            {
                _grid.DataSource = entries;
                _view.FocusedRowHandle = entries.Count == 0 ? GridControl.InvalidRowHandle : 0;
                grpHistory.Text = "검수 이력 / 표시 " + entries.Count + "건";
            }
            finally
            {
                _updatingControls = false;
            }
        }

        public void ShowErrors(string text, int count)
        {
            _errors.Text = text;
            pageErrors.Text = "조회 오류 (" + count + ")";
            if (count == 0 && tabs.SelectedTabPage == pageErrors) tabs.SelectedTabPage = pageSummary;
            pageErrors.PageVisible = count > 0;
        }

        public void ShowStatus(string text)
        {
            _status.Text = text;
            _status.ToolTip = text;
        }

        public void ShowDetail(AiReviewHistoryEntry entry, string text)
        {
            _detail.Text = text;
            txtSummary.Text = DisplayText(entry?.Summary);
            txtContext.Text = DisplayText(entry?.ChangeContext);
            txtCommitMessage.Text = DisplayText(entry?.SuggestedCommitMessage);
            var issues = entry?.Issues == null ? new List<AiReviewHistoryIssue>()
                : entry.Issues.Where(issue => issue != null).ToList();
            gridIssues.DataSource = issues;
            viewIssues.FocusedRowHandle = issues.Count == 0 ? GridControl.InvalidRowHandle : 0;
            ShowSelectedIssue();
            listFiles.Items.Clear();
            if (entry?.FilePaths != null)
                foreach (string path in entry.FilePaths) listFiles.Items.Add(path);
            pageIssues.Text = "이슈 (" + issues.Count + ")";
            pageFiles.Text = "대상 파일 (" + listFiles.ItemCount + ")";
            if (entry == null)
            {
                lblOutcome.Text = "이력을 선택하세요.";
                lblOutcome.Appearance.ForeColor = SystemColors.ControlText;
                lblOutcome.ToolTip = lblOutcome.Text;
                lblReviewInfo.Text = string.Empty;
                lblReviewInfo.ToolTip = string.Empty;
                lblTokenInfo.Text = string.Empty;
                lblTokenInfo.ToolTip = string.Empty;
                return;
            }
            lblOutcome.Text = AiReviewHistoryPresenter.FormatColumnText("Status", entry.Status)
                + " / 연결 리비전: " + AiReviewHistoryPresenter.FormatColumnText("SvnRevision", entry.SvnRevision);
            lblOutcome.Appearance.ForeColor = StatusColor(entry.Status);
            lblOutcome.ToolTip = lblOutcome.Text;
            lblReviewInfo.Text = entry.ReviewedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                + " / " + entry.ProviderName + " / " + entry.ModelName;
            lblReviewInfo.ToolTip = lblReviewInfo.Text;
            lblTokenInfo.Text = "입력 추정 " + entry.EstimatedInputTokens.ToString("N0")
                + " / 실제 입력 " + TokenText(entry.ActualInputTokens)
                + " / 실제 출력 " + TokenText(entry.ActualOutputTokens)
                + " / 소요 " + (entry.ElapsedMs / 1000D).ToString("N1") + "초";
            lblTokenInfo.ToolTip = lblTokenInfo.Text;
        }

        #endregion

        #region Operations

        /// <summary>선택한 이슈의 제목, 위치 및 설명을 읽기 전용 영역에 표시한다.</summary>
        private void ShowSelectedIssue()
        {
            var issue = viewIssues.GetFocusedRow() as AiReviewHistoryIssue;
            txtIssueDetail.Text = issue == null ? "표시할 이슈가 없습니다."
                : issue.Title + Environment.NewLine
                    + (issue.FilePath ?? "위치 미지정")
                    + (issue.LineNumber.HasValue ? " : " + issue.LineNumber.Value + "행" : string.Empty)
                    + Environment.NewLine + Environment.NewLine + issue.Description;
        }

        /// <summary>저장된 본문이 없으면 기록 없음으로 표시한다.</summary>
        private static string DisplayText(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? "기록 없음" : text;
        }

        /// <summary>사용량이 제공되지 않은 경우를 숫자 0과 구분한다.</summary>
        private static string TokenText(int? value)
        {
            return value.HasValue ? value.Value.ToString("N0") : "미제공";
        }

        /// <summary>성공, 부분 성공 및 실패를 구분하는 표시 색상을 반환한다.</summary>
        private static Color StatusColor(AiReviewStatus status)
        {
            return status == AiReviewStatus.Succeeded ? Color.DarkGreen
                : status == AiReviewStatus.Partial ? Color.DarkGoldenrod
                : status == AiReviewStatus.Failed ? Color.Firebrick : SystemColors.ControlText;
        }

        /// <summary>공유 글꼴을 본문, 버튼 및 DevExpress 표시 속성에 적용한다.</summary>
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
