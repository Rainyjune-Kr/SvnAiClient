using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace SvnAiClient.Views.AiReviewHistory
{
    partial class AiReviewHistoryView
    {
        #region Fields

        /// <summary>화면 구성 요소의 수명을 관리한다.</summary>
        private IContainer components = null;
        /// <summary>전체 화면의 여백과 주요 영역 배치.</summary>
        private Panel layoutRoot;
        /// <summary>현재 작업 복사본과 조회 조건 배치.</summary>
        private Panel header;
        /// <summary>작업 복사본 경로 표시.</summary>
        private Panel scopeRow;
        /// <summary>현재 작업 복사본 표시.</summary>
        private LabelControl lblScope;
        /// <summary>필터의 기준이 되는 작업 복사본 경로.</summary>
        private TextEdit txtWorkingCopy;
        /// <summary>새로고침과 현재 작업 복사본 필터 배치.</summary>
        private FlowLayoutPanel toolbar;
        /// <summary>새로고침 버튼.</summary>
        private SimpleButton _refresh;
        /// <summary>변경 배경, 제안 메시지 및 전체 기록 탭의 표시를 전환한다.</summary>
        private SimpleButton btnHistoryDetails;
        /// <summary>현재 작업 복사본만 표시하는 필터.</summary>
        private CheckEdit _currentOnly;
        /// <summary>과거 검수 이력은 조회 전용입니다. 표시.</summary>
        private LabelControl lblReadOnly;
        /// <summary>이력 목록과 선택한 이력 상세의 높이 조절.</summary>
        private SplitContainerControl split;
        /// <summary>검수 이력 목록과 표시 건수.</summary>
        private GroupControl grpHistory;
        /// <summary>검수 시각, 상태 및 연결 리비전별 이력 목록.</summary>
        private GridControl _grid;
        /// <summary>이력 검색과 단일 항목 선택.</summary>
        private GridView _view;
        /// <summary>선택 이력의 개요와 상세 탭 배치.</summary>
        private Panel detailRoot;
        /// <summary>검수 상태, 연결 리비전 및 사용량 개요.</summary>
        private Panel selectionHeader;
        /// <summary>이력을 선택하세요. 표시.</summary>
        private LabelControl lblOutcome;
        /// <summary> 표시.</summary>
        private LabelControl lblReviewInfo;
        /// <summary> 표시.</summary>
        private LabelControl lblTokenInfo;
        /// <summary>검수 결과를 항목별로 구분하는 상세 탭.</summary>
        private XtraTabControl tabs;
        /// <summary>요약 탭.</summary>
        private XtraTabPage pageSummary;
        /// <summary>검수 요약 전문.</summary>
        private MemoEdit txtSummary;
        /// <summary>변경 배경 탭.</summary>
        private XtraTabPage pageContext;
        /// <summary>변경 배경과 검수 요청 내용.</summary>
        private MemoEdit txtContext;
        /// <summary>이슈 (0) 탭.</summary>
        private XtraTabPage pageIssues;
        /// <summary>대상 파일 (0) 탭.</summary>
        private XtraTabPage pageFiles;
        /// <summary>제안 메시지 탭.</summary>
        private XtraTabPage pageCommit;
        /// <summary>저장 당시 제안한 커밋 메시지.</summary>
        private MemoEdit txtCommitMessage;
        /// <summary>전체 기록 탭.</summary>
        private XtraTabPage pageDetail;
        /// <summary>배치별 처리 상태를 포함한 전체 이력 기록.</summary>
        private MemoEdit _detail;
        /// <summary>조회 오류 (0) 탭.</summary>
        private XtraTabPage pageErrors;
        /// <summary>이력 파일을 읽는 중 발생한 오류.</summary>
        private MemoEdit _errors;
        /// <summary>검수 대상 파일의 경로 목록.</summary>
        private ListBoxControl listFiles;
        /// <summary>이슈 목록과 선택 이슈 설명의 높이 조절.</summary>
        private SplitContainerControl splitIssues;
        /// <summary>저장 당시 검수 이슈 목록.</summary>
        private GridControl gridIssues;
        /// <summary>이슈의 심각도, 제목과 위치 표시.</summary>
        private GridView viewIssues;
        /// <summary>선택한 이슈의 제목, 위치 및 설명 전문.</summary>
        private MemoEdit txtIssueDetail;
        /// <summary>조회 상태와 닫기 버튼 배치.</summary>
        private Panel footer;
        /// <summary>조회 준비 표시.</summary>
        private LabelControl _status;
        /// <summary>닫기 버튼.</summary>
        private SimpleButton btnClose;

        #endregion

        #region Initialization

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.layoutRoot = new Panel();
            this.header = new Panel();
            this.scopeRow = new Panel();
            this.lblScope = new LabelControl();
            this.txtWorkingCopy = new TextEdit();
            this.toolbar = new FlowLayoutPanel();
            this._refresh = new SimpleButton();
            this.btnHistoryDetails = new SimpleButton();
            this._currentOnly = new CheckEdit();
            this.lblReadOnly = new LabelControl();
            this.split = new SplitContainerControl();
            this.grpHistory = new GroupControl();
            this._grid = new GridControl();
            this._view = new GridView();
            this.detailRoot = new Panel();
            this.selectionHeader = new Panel();
            this.lblOutcome = new LabelControl();
            this.lblReviewInfo = new LabelControl();
            this.lblTokenInfo = new LabelControl();
            this.tabs = new XtraTabControl();
            this.pageSummary = new XtraTabPage();
            this.txtSummary = new MemoEdit();
            this.pageContext = new XtraTabPage();
            this.txtContext = new MemoEdit();
            this.pageIssues = new XtraTabPage();
            this.pageFiles = new XtraTabPage();
            this.pageCommit = new XtraTabPage();
            this.txtCommitMessage = new MemoEdit();
            this.pageDetail = new XtraTabPage();
            this._detail = new MemoEdit();
            this.pageErrors = new XtraTabPage();
            this._errors = new MemoEdit();
            this.listFiles = new ListBoxControl();
            this.splitIssues = new SplitContainerControl();
            this.gridIssues = new GridControl();
            this.viewIssues = new GridView();
            this.txtIssueDetail = new MemoEdit();
            this.footer = new Panel();
            this._status = new LabelControl();
            this.btnClose = new SimpleButton();
            ((ISupportInitialize)(this.txtWorkingCopy.Properties)).BeginInit();
            ((ISupportInitialize)(this._currentOnly.Properties)).BeginInit();
            ((ISupportInitialize)(this.split)).BeginInit();
            ((ISupportInitialize)(this.grpHistory)).BeginInit();
            ((ISupportInitialize)(this._grid)).BeginInit();
            ((ISupportInitialize)(this._view)).BeginInit();
            ((ISupportInitialize)(this.tabs)).BeginInit();
            ((ISupportInitialize)(this.txtSummary.Properties)).BeginInit();
            ((ISupportInitialize)(this.txtContext.Properties)).BeginInit();
            ((ISupportInitialize)(this.txtCommitMessage.Properties)).BeginInit();
            ((ISupportInitialize)(this._detail.Properties)).BeginInit();
            ((ISupportInitialize)(this._errors.Properties)).BeginInit();
            ((ISupportInitialize)(this.listFiles)).BeginInit();
            ((ISupportInitialize)(this.splitIssues)).BeginInit();
            ((ISupportInitialize)(this.gridIssues)).BeginInit();
            ((ISupportInitialize)(this.viewIssues)).BeginInit();
            ((ISupportInitialize)(this.txtIssueDetail.Properties)).BeginInit();
            this.layoutRoot.SuspendLayout();
            this.header.SuspendLayout();
            this.scopeRow.SuspendLayout();
            this.toolbar.SuspendLayout();
            this.split.SuspendLayout();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.grpHistory.SuspendLayout();
            this.detailRoot.SuspendLayout();
            this.selectionHeader.SuspendLayout();
            this.tabs.SuspendLayout();
            this.pageSummary.SuspendLayout();
            this.pageContext.SuspendLayout();
            this.pageIssues.SuspendLayout();
            this.pageFiles.SuspendLayout();
            this.pageCommit.SuspendLayout();
            this.pageDetail.SuspendLayout();
            this.pageErrors.SuspendLayout();
            this.splitIssues.SuspendLayout();
            this.splitIssues.Panel1.SuspendLayout();
            this.splitIssues.Panel2.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.Dock = DockStyle.Fill;
            this.header.Name = "header";
            this.header.Dock = DockStyle.Top;
            this.scopeRow.Name = "scopeRow";
            this.scopeRow.Dock = DockStyle.Top;
            this.lblScope.Name = "lblScope";
            this.lblScope.Dock = DockStyle.Left;
            this.lblScope.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblScope.Text = "현재 작업 폴더";
            this.lblScope.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.txtWorkingCopy.Name = "txtWorkingCopy";
            this.txtWorkingCopy.Dock = DockStyle.Fill;
            this.txtWorkingCopy.Properties.ReadOnly = true;
            this.toolbar.Name = "toolbar";
            this.toolbar.Dock = DockStyle.Bottom;
            this.toolbar.WrapContents = false;
            this._refresh.Name = "_refresh";
            this._refresh.Text = "새로고침";
            this._currentOnly.Name = "_currentOnly";
            this._currentOnly.Properties.Caption = "현재 작업 폴더만";
            this._currentOnly.Properties.AutoHeight = false;
            this.lblReadOnly.Name = "lblReadOnly";
            this.lblReadOnly.Dock = DockStyle.None;
            this.lblReadOnly.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblReadOnly.Text = "조회 전용 · 현재 검수에 재사용되지 않습니다.";
            this.lblReadOnly.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblReadOnly.Size = new Size(430, 32);
            this.split.Name = "split";
            this.split.Dock = DockStyle.Fill;
            this.split.Size = new Size(1096, 660);
            this.split.Horizontal = false;
            this.split.FixedPanel = SplitFixedPanel.None;
            this.split.SplitterPosition = 300;
            this.grpHistory.Name = "grpHistory";
            this.grpHistory.Dock = DockStyle.Fill;
            this.grpHistory.Text = "검수 이력";
            this._grid.Name = "_grid";
            this._grid.Dock = DockStyle.Fill;
            this._view.Name = "_view";
            this._grid.MainView = this._view;
            this._view.GridControl = this._grid;
            this.detailRoot.Name = "detailRoot";
            this.detailRoot.Dock = DockStyle.Fill;
            this.selectionHeader.Name = "selectionHeader";
            this.selectionHeader.Dock = DockStyle.Top;
            this.selectionHeader.Padding = new Padding(2, 2, 0, 4);
            this.lblOutcome.Name = "lblOutcome";
            this.lblOutcome.Dock = DockStyle.Top;
            this.lblOutcome.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblOutcome.Text = "이력을 선택하세요.";
            this.lblOutcome.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblReviewInfo.Name = "lblReviewInfo";
            this.lblReviewInfo.Dock = DockStyle.Top;
            this.lblReviewInfo.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblReviewInfo.Text = "";
            this.lblReviewInfo.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblTokenInfo.Name = "lblTokenInfo";
            this.lblTokenInfo.Dock = DockStyle.Fill;
            this.lblTokenInfo.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblTokenInfo.Text = "";
            this.lblTokenInfo.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.tabs.Name = "tabs";
            this.tabs.Dock = DockStyle.Fill;
            this.pageSummary.Name = "pageSummary";
            this.pageSummary.Text = "요약";
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.Dock = DockStyle.Fill;
            this.txtSummary.Properties.ReadOnly = true;
            this.pageContext.Name = "pageContext";
            this.pageContext.Text = "변경 배경";
            this.pageContext.PageVisible = false;
            this.txtContext.Name = "txtContext";
            this.txtContext.Dock = DockStyle.Fill;
            this.txtContext.Properties.ReadOnly = true;
            this.pageIssues.Name = "pageIssues";
            this.pageIssues.Text = "이슈 (0)";
            this.pageFiles.Name = "pageFiles";
            this.pageFiles.Text = "대상 파일 (0)";
            this.pageCommit.Name = "pageCommit";
            this.pageCommit.Text = "제안 메시지";
            this.pageCommit.PageVisible = false;
            this.txtCommitMessage.Name = "txtCommitMessage";
            this.txtCommitMessage.Dock = DockStyle.Fill;
            this.txtCommitMessage.Properties.ReadOnly = true;
            this.pageDetail.Name = "pageDetail";
            this.pageDetail.Text = "전체 기록";
            this.pageDetail.PageVisible = false;
            this._detail.Name = "_detail";
            this._detail.Dock = DockStyle.Fill;
            this._detail.Properties.ReadOnly = true;
            this.pageErrors.Name = "pageErrors";
            this.pageErrors.Text = "조회 오류 (0)";
            this.pageErrors.PageVisible = false;
            this._errors.Name = "_errors";
            this._errors.Dock = DockStyle.Fill;
            this._errors.Properties.ReadOnly = true;
            this.tabs.SelectedTabPage = this.pageSummary;
            this.listFiles.Name = "listFiles";
            this.listFiles.Dock = DockStyle.Fill;
            this.listFiles.HorizontalScrollbar = true;
            this.splitIssues.Name = "splitIssues";
            this.splitIssues.Dock = DockStyle.Fill;
            this.splitIssues.Size = new Size(1050, 260);
            this.splitIssues.Horizontal = false;
            this.splitIssues.FixedPanel = SplitFixedPanel.None;
            this.splitIssues.SplitterPosition = 130;
            this.gridIssues.Name = "gridIssues";
            this.gridIssues.Dock = DockStyle.Fill;
            this.viewIssues.Name = "viewIssues";
            this.gridIssues.MainView = this.viewIssues;
            this.viewIssues.GridControl = this.gridIssues;
            this.txtIssueDetail.Name = "txtIssueDetail";
            this.txtIssueDetail.Dock = DockStyle.Fill;
            this.txtIssueDetail.Properties.ReadOnly = true;
            this.footer.Name = "footer";
            this.footer.Dock = DockStyle.Bottom;
            this._status.Name = "_status";
            this._status.Dock = DockStyle.Fill;
            this._status.AutoSizeMode = LabelAutoSizeMode.None;
            this._status.Text = "조회 준비";
            this._status.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.btnClose.Name = "btnClose";
            this.btnClose.Text = "닫기";
            this.btnClose.Dock = DockStyle.Right;
            this.btnClose.DialogResult = DialogResult.Cancel;
            this._grid.ViewCollection.Add(this._view);
            this.pageSummary.Controls.Add(this.txtSummary);
            this.tabs.TabPages.Add(this.pageSummary);
            this.pageContext.Controls.Add(this.txtContext);

            this.tabs.TabPages.Add(this.pageIssues);
            this.tabs.TabPages.Add(this.pageFiles);
            this.tabs.TabPages.Add(this.pageContext);
            this.pageCommit.Controls.Add(this.txtCommitMessage);
            this.tabs.TabPages.Add(this.pageCommit);
            this.pageDetail.Controls.Add(this._detail);
            this.tabs.TabPages.Add(this.pageDetail);
            this.pageErrors.Controls.Add(this._errors);
            this.tabs.TabPages.Add(this.pageErrors);
            this.pageFiles.Controls.Add(this.listFiles);
            this.gridIssues.ViewCollection.Add(this.viewIssues);
            this.splitIssues.Panel1.Controls.Add(this.gridIssues);
            this.splitIssues.Panel2.Controls.Add(this.txtIssueDetail);
            this.pageIssues.Controls.Add(this.splitIssues);
            this.scopeRow.Controls.Add(this.txtWorkingCopy);
            this.scopeRow.Controls.Add(this.lblScope);
            this.toolbar.Controls.Add(this._refresh);
            this.toolbar.Controls.Add(this._currentOnly);
            this.btnHistoryDetails.Name = "btnHistoryDetails";
            this.btnHistoryDetails.Size = new Size(144, 28);
            this.btnHistoryDetails.Margin = new Padding(0, 0, 6, 0);
            this.btnHistoryDetails.Text = "추가 정보 보기";
            this.btnHistoryDetails.ToolTip = "변경 배경·제안 메시지·전체 기록을 표시합니다.";
            this.toolbar.Controls.Add(this.btnHistoryDetails);
            this.toolbar.Controls.Add(this.lblReadOnly);
            this.header.Controls.Add(this.scopeRow);
            this.header.Controls.Add(this.toolbar);
            this.grpHistory.Controls.Add(this._grid);
            this.split.Panel1.Controls.Add(this.grpHistory);
            this.selectionHeader.Controls.Add(this.lblTokenInfo);
            this.selectionHeader.Controls.Add(this.lblReviewInfo);
            this.selectionHeader.Controls.Add(this.lblOutcome);
            this.detailRoot.Controls.Add(this.tabs);
            this.detailRoot.Controls.Add(this.selectionHeader);
            this.split.Panel2.Controls.Add(this.detailRoot);
            this.footer.Controls.Add(this._status);
            this.footer.Controls.Add(this.btnClose);
            this.layoutRoot.Controls.Add(this.split);
            this.layoutRoot.Controls.Add(this.header);
            this.layoutRoot.Controls.Add(this.footer);
            // Common theme: outer 12, group 8, gap 6, input/button 28.
            this.pageDetail.Margin = new System.Windows.Forms.Padding(0);
            this.pageDetail.Padding = new System.Windows.Forms.Padding(8);
            this.toolbar.Margin = new System.Windows.Forms.Padding(0);
            this._refresh.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._currentOnly.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblReadOnly.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.layoutRoot.Margin = new System.Windows.Forms.Padding(0);
            this.footer.Margin = new System.Windows.Forms.Padding(0);
            this.lblReviewInfo.Margin = new System.Windows.Forms.Padding(0);
            this.scopeRow.Margin = new System.Windows.Forms.Padding(0);
            this.txtCommitMessage.Margin = new System.Windows.Forms.Padding(0);
            this.detailRoot.Margin = new System.Windows.Forms.Padding(0);
            this.lblOutcome.Margin = new System.Windows.Forms.Padding(0);
            this.txtContext.Margin = new System.Windows.Forms.Padding(0);
            this.pageFiles.Margin = new System.Windows.Forms.Padding(0);
            this.pageFiles.Padding = new System.Windows.Forms.Padding(8);
            this.lblScope.Margin = new System.Windows.Forms.Padding(0);
            this.grpHistory.Margin = new System.Windows.Forms.Padding(0);
            this.grpHistory.Padding = new System.Windows.Forms.Padding(8);
            this.txtSummary.Margin = new System.Windows.Forms.Padding(0);
            this.gridIssues.Margin = new System.Windows.Forms.Padding(0);
            this._refresh.Size = new System.Drawing.Size(88, 28);
            this._refresh.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this._refresh.Appearance.Options.UseFont = true;
            this.pageIssues.Margin = new System.Windows.Forms.Padding(0);
            this.pageIssues.Padding = new System.Windows.Forms.Padding(8);
            this.pageSummary.Margin = new System.Windows.Forms.Padding(0);
            this.pageSummary.Padding = new System.Windows.Forms.Padding(8);
            this.lblTokenInfo.Margin = new System.Windows.Forms.Padding(0);
            this.txtIssueDetail.Margin = new System.Windows.Forms.Padding(0);
            this.tabs.Margin = new System.Windows.Forms.Padding(0);
            this._errors.Margin = new System.Windows.Forms.Padding(0);
            this.pageErrors.Margin = new System.Windows.Forms.Padding(0);
            this.pageErrors.Padding = new System.Windows.Forms.Padding(8);
            this._detail.Margin = new System.Windows.Forms.Padding(0);
            this._status.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Size = new System.Drawing.Size(88, 28);
            this.btnClose.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnClose.Appearance.Options.UseFont = true;
            this.header.Margin = new System.Windows.Forms.Padding(0);
            this.selectionHeader.Margin = new System.Windows.Forms.Padding(0);
            this._grid.Margin = new System.Windows.Forms.Padding(0);
            this.listFiles.Margin = new System.Windows.Forms.Padding(0);
            this.pageContext.Margin = new System.Windows.Forms.Padding(0);
            this.pageContext.Padding = new System.Windows.Forms.Padding(8);
            this.pageCommit.Margin = new System.Windows.Forms.Padding(0);
            this.pageCommit.Padding = new System.Windows.Forms.Padding(8);
            this.txtWorkingCopy.Margin = new System.Windows.Forms.Padding(0);
            this.txtWorkingCopy.Properties.AutoHeight = false;
            this.layoutRoot.Padding = new System.Windows.Forms.Padding(12);
            this.header.Height = 72;
            this.scopeRow.Height = 34;
            this.scopeRow.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.lblScope.Width = 104;
            this.toolbar.Height = 36;
            this.toolbar.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.footer.Height = 40;
            this.footer.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.lblReadOnly.Height = 28;
            this._currentOnly.Size = new System.Drawing.Size(200, 28);
            this.selectionHeader.Height = 74;
            this.lblOutcome.Height = 26;
            this.lblReviewInfo.Height = 22;
            this.Controls.Add(this.layoutRoot);
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(1120, 820);
            this.MinimumSize = new Size(960, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            this.Name = "AiReviewHistoryView";
            this.Text = "AI 검수 이력";
            this.CancelButton = this.btnClose;
            ((System.ComponentModel.ISupportInitialize)(this.txtIssueDetail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewIssues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridIssues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitIssues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._errors.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._detail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommitMessage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContext.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSummary.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._view)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._currentOnly.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkingCopy.Properties)).EndInit();
            this.footer.ResumeLayout(false);
            this.splitIssues.Panel1.ResumeLayout(false);
            this.splitIssues.Panel2.ResumeLayout(false);
            this.splitIssues.ResumeLayout(false);
            this.pageErrors.ResumeLayout(false);
            this.pageDetail.ResumeLayout(false);
            this.pageCommit.ResumeLayout(false);
            this.pageFiles.ResumeLayout(false);
            this.pageIssues.ResumeLayout(false);
            this.pageContext.ResumeLayout(false);
            this.pageSummary.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.selectionHeader.ResumeLayout(false);
            this.detailRoot.ResumeLayout(false);
            this.grpHistory.ResumeLayout(false);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            this.split.ResumeLayout(false);
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.scopeRow.ResumeLayout(false);
            this.header.ResumeLayout(false);
            this.layoutRoot.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}

