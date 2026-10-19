using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace SvnAiClient.Views.Commit
{
    partial class CommitView
    {
        #region Fields

        /// <summary>디자이너 구성 요소의 수명을 관리한다.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>커밋 화면의 주요 영역을 세로로 배치한다.</summary>
        private TableLayoutPanel layoutRoot;

        /// <summary>높이 조절이 가능한 파일 목록과 나머지 화면을 배치하는 외곽 영역.</summary>
        private Panel layoutFrame;
        /// <summary>파일 목록의 높이를 조절하는 가로 분할 막대.</summary>
        private Splitter filesSplitter;
        /// <summary>변경 배경의 접기·펼치기 도구와 입력란을 배치한다.</summary>
        private Panel contextPanel;
        /// <summary>변경 배경 표시 전환 버튼과 입력 요약을 배치한다.</summary>
        private Panel contextTools;
        /// <summary>변경 배경 입력란을 접거나 펼친다.</summary>
        private SimpleButton btnToggleContext;
        /// <summary>접힌 변경 배경의 입력 요약.</summary>
        private LabelControl lblContextHint;
        /// <summary>최종 메시지 안내와 AI 제안 표시 전환 버튼을 배치한다.</summary>
        private Panel messageTools;
        /// <summary>최종 메시지 작성 안내.</summary>
        private LabelControl lblMessageHint;
        /// <summary>AI 제안 메시지를 접거나 펼친다.</summary>
        private SimpleButton btnToggleSuggestion;

        /// <summary>커밋 대상과 선택 개수를 표시한다.</summary>
        private GroupControl grpCommitFiles;

        /// <summary>커밋 대상 파일 목록.</summary>
        private GridControl gridCommitFiles;

        /// <summary>파일 상태와 커밋 체크를 표시한다.</summary>
        private GridView gridViewCommitFiles;

        /// <summary>선택 파일 비교 안내와 실행 버튼을 배치한다.</summary>
        private Panel fileTools;
        /// <summary>더블클릭으로 최신 저장소와 비교할 수 있음을 안내한다.</summary>
        private LabelControl lblFileDiffHint;
        /// <summary>포커스된 파일을 저장소 HEAD와 비교한다.</summary>
        private SimpleButton btnFileDiff;

        /// <summary>변경 배경과 검수 요청 영역.</summary>
        private GroupControl grpChangeContext;

        /// <summary>개발자가 입력한 변경 배경과 요청 내용.</summary>
        private MemoEdit txtChangeContext;

        /// <summary>프로바이더와 검수 명령 및 진행 상태를 배치한다.</summary>
        private TableLayoutPanel layoutReviewTools;

        /// <summary>프로바이더 선택 안내.</summary>
        private LabelControl lblAiProvider;

        /// <summary>검수에 사용할 프로바이더.</summary>
        private ComboBoxEdit cboAiProvider;

        /// <summary>현재 대상의 AI 검수를 요청한다.</summary>
        private SimpleButton btnAiReview;

        /// <summary>진행 상태와 검수 무효화 사유.</summary>
        private LabelControl lblAiStatus;

        /// <summary>검수 결과를 종류별로 표시한다.</summary>
        private GroupControl grpAiResult;

        /// <summary>요약과 이슈 및 오류를 구분한다.</summary>
        private XtraTabControl tabsReview;

        /// <summary>검수 결과 요약 탭.</summary>
        private XtraTabPage pageSummary;

        /// <summary>요약 헤더와 본문을 배치한다.</summary>
        private TableLayoutPanel layoutSummary;

        /// <summary>처리 상태와 심각도별 이슈 개수.</summary>
        private LabelControl lblReviewOutcome;

        /// <summary>토큰 사용량과 검수 소요 시간.</summary>
        private LabelControl lblTokenUsage;

        /// <summary>읽기 전용 검수 요약.</summary>
        private MemoEdit txtReviewSummary;

        /// <summary>이슈 목록과 선택 상세 탭.</summary>
        private XtraTabPage pageIssues;

        /// <summary>이슈 목록과 상세의 분할 영역.</summary>
        private SplitContainerControl splitIssues;

        /// <summary>검수 이슈 목록.</summary>
        private GridControl gridIssues;

        /// <summary>이슈의 심각도와 제목 및 파일을 표시한다.</summary>
        private GridView gridViewIssues;

        /// <summary>선택한 이슈의 상세를 배치한다.</summary>
        private TableLayoutPanel layoutIssueDetail;

        /// <summary>선택한 이슈 제목.</summary>
        private MemoEdit txtIssueTitle;

        /// <summary>복사 가능한 파일 경로와 행 번호.</summary>
        private TextEdit txtIssueLocation;

        /// <summary>선택한 이슈 설명.</summary>
        private MemoEdit txtIssueDescription;

        /// <summary>선택한 이슈의 검수 당시 Diff를 연다.</summary>
        private SimpleButton btnIssueDiff;

        /// <summary>검수 실패 또는 부분 성공 오류 탭.</summary>
        private XtraTabPage pageErrors;

        /// <summary>읽기 전용 오류 상세.</summary>
        private MemoEdit txtReviewErrors;

        /// <summary>최종 메시지와 AI 제안을 나란히 배치한다.</summary>
        private TableLayoutPanel layoutMessages;

        /// <summary>최종 SVN 커밋 메시지 영역.</summary>
        private GroupControl grpCommitMessage;

        /// <summary>사용자가 편집하는 최종 커밋 메시지.</summary>
        private MemoEdit txtCommitMessage;

        /// <summary>AI 제안과 적용 명령 영역.</summary>
        private GroupControl grpSuggestedMessage;

        /// <summary>제안 메시지와 적용 버튼을 배치한다.</summary>
        private TableLayoutPanel layoutSuggestion;

        /// <summary>읽기 전용 AI 제안 메시지.</summary>
        private MemoEdit txtSuggestedMsg;

        /// <summary>제안을 최종 커밋 메시지에 적용한다.</summary>
        private SimpleButton btnCopyCommitMsg;

        /// <summary>최종 확인 안내와 명령을 배치한다.</summary>
        private TableLayoutPanel layoutFooter;

        /// <summary>커밋 전에 확인할 내용을 안내한다.</summary>
        private LabelControl lblFooterHint;

        /// <summary>기존 승인 절차를 거쳐 커밋을 요청한다.</summary>
        private SimpleButton btnCommit;

        /// <summary>진행 작업을 취소하고 화면을 닫는다.</summary>
        private SimpleButton btnCancel;

        #endregion

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Initialization

        private void InitializeComponent()
        {
            this.layoutRoot = new System.Windows.Forms.TableLayoutPanel();
            this.contextPanel = new System.Windows.Forms.Panel();
            this.grpChangeContext = new DevExpress.XtraEditors.GroupControl();
            this.txtChangeContext = new DevExpress.XtraEditors.MemoEdit();
            this.contextTools = new System.Windows.Forms.Panel();
            this.lblContextHint = new DevExpress.XtraEditors.LabelControl();
            this.btnToggleContext = new DevExpress.XtraEditors.SimpleButton();
            this.layoutReviewTools = new System.Windows.Forms.TableLayoutPanel();
            this.lblAiProvider = new DevExpress.XtraEditors.LabelControl();
            this.cboAiProvider = new DevExpress.XtraEditors.ComboBoxEdit();
            this.btnAiReview = new DevExpress.XtraEditors.SimpleButton();
            this.lblFooterHint = new DevExpress.XtraEditors.LabelControl();
            this.grpAiResult = new DevExpress.XtraEditors.GroupControl();
            this.tabsReview = new DevExpress.XtraTab.XtraTabControl();
            this.pageSummary = new DevExpress.XtraTab.XtraTabPage();
            this.layoutSummary = new System.Windows.Forms.TableLayoutPanel();
            this.lblReviewOutcome = new DevExpress.XtraEditors.LabelControl();
            this.lblTokenUsage = new DevExpress.XtraEditors.LabelControl();
            this.txtReviewSummary = new DevExpress.XtraEditors.MemoEdit();
            this.pageIssues = new DevExpress.XtraTab.XtraTabPage();
            this.splitIssues = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridIssues = new DevExpress.XtraGrid.GridControl();
            this.gridViewIssues = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutIssueDetail = new System.Windows.Forms.TableLayoutPanel();
            this.txtIssueTitle = new DevExpress.XtraEditors.MemoEdit();
            this.txtIssueLocation = new DevExpress.XtraEditors.TextEdit();
            this.txtIssueDescription = new DevExpress.XtraEditors.MemoEdit();
            this.btnIssueDiff = new DevExpress.XtraEditors.SimpleButton();
            this.pageErrors = new DevExpress.XtraTab.XtraTabPage();
            this.txtReviewErrors = new DevExpress.XtraEditors.MemoEdit();
            this.layoutMessages = new System.Windows.Forms.TableLayoutPanel();
            this.grpCommitMessage = new DevExpress.XtraEditors.GroupControl();
            this.txtCommitMessage = new DevExpress.XtraEditors.MemoEdit();
            this.messageTools = new System.Windows.Forms.Panel();
            this.lblMessageHint = new DevExpress.XtraEditors.LabelControl();
            this.btnToggleSuggestion = new DevExpress.XtraEditors.SimpleButton();
            this.grpSuggestedMessage = new DevExpress.XtraEditors.GroupControl();
            this.layoutSuggestion = new System.Windows.Forms.TableLayoutPanel();
            this.txtSuggestedMsg = new DevExpress.XtraEditors.MemoEdit();
            this.btnCopyCommitMsg = new DevExpress.XtraEditors.SimpleButton();
            this.layoutFooter = new System.Windows.Forms.TableLayoutPanel();
            this.lblAiStatus = new DevExpress.XtraEditors.LabelControl();
            this.btnCommit = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.layoutFrame = new System.Windows.Forms.Panel();
            this.filesSplitter = new System.Windows.Forms.Splitter();
            this.grpCommitFiles = new DevExpress.XtraEditors.GroupControl();
            this.gridCommitFiles = new DevExpress.XtraGrid.GridControl();
            this.gridViewCommitFiles = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.fileTools = new System.Windows.Forms.Panel();
            this.lblFileDiffHint = new DevExpress.XtraEditors.LabelControl();
            this.btnFileDiff = new DevExpress.XtraEditors.SimpleButton();
            this.layoutRoot.SuspendLayout();
            this.contextPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpChangeContext)).BeginInit();
            this.grpChangeContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChangeContext.Properties)).BeginInit();
            this.contextTools.SuspendLayout();
            this.layoutReviewTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAiProvider.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpAiResult)).BeginInit();
            this.grpAiResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabsReview)).BeginInit();
            this.tabsReview.SuspendLayout();
            this.pageSummary.SuspendLayout();
            this.layoutSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReviewSummary.Properties)).BeginInit();
            this.pageIssues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitIssues)).BeginInit();
            this.splitIssues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridIssues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewIssues)).BeginInit();
            this.layoutIssueDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtIssueTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIssueLocation.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIssueDescription.Properties)).BeginInit();
            this.pageErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReviewErrors.Properties)).BeginInit();
            this.layoutMessages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpCommitMessage)).BeginInit();
            this.grpCommitMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommitMessage.Properties)).BeginInit();
            this.messageTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpSuggestedMessage)).BeginInit();
            this.grpSuggestedMessage.SuspendLayout();
            this.layoutSuggestion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSuggestedMsg.Properties)).BeginInit();
            this.layoutFooter.SuspendLayout();
            this.layoutFrame.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpCommitFiles)).BeginInit();
            this.grpCommitFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCommitFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCommitFiles)).BeginInit();
            this.fileTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutRoot
            // 
            this.layoutRoot.ColumnCount = 1;
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Controls.Add(this.contextPanel, 0, 0);
            this.layoutRoot.Controls.Add(this.layoutReviewTools, 0, 1);
            this.layoutRoot.Controls.Add(this.grpAiResult, 0, 2);
            this.layoutRoot.Controls.Add(this.layoutMessages, 0, 3);
            this.layoutRoot.Controls.Add(this.layoutFooter, 0, 4);
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutRoot.Location = new System.Drawing.Point(12, 196);
            this.layoutRoot.Margin = new System.Windows.Forms.Padding(0);
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.RowCount = 5;
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layoutRoot.Size = new System.Drawing.Size(1156, 632);
            this.layoutRoot.TabIndex = 0;
            // 
            // contextPanel
            // 
            this.contextPanel.Controls.Add(this.grpChangeContext);
            this.contextPanel.Controls.Add(this.contextTools);
            this.contextPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contextPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.contextPanel.Location = new System.Drawing.Point(0, 0);
            this.contextPanel.Margin = new System.Windows.Forms.Padding(0);
            this.contextPanel.Name = "contextPanel";
            this.contextPanel.Size = new System.Drawing.Size(1156, 34);
            this.contextPanel.TabIndex = 0;
            // 
            // grpChangeContext
            // 
            this.grpChangeContext.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpChangeContext.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.grpChangeContext.AppearanceCaption.Options.UseFont = true;
            this.grpChangeContext.AppearanceCaption.Options.UseForeColor = true;
            this.grpChangeContext.Controls.Add(this.txtChangeContext);
            this.grpChangeContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpChangeContext.Location = new System.Drawing.Point(0, 34);
            this.grpChangeContext.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.grpChangeContext.Name = "grpChangeContext";
            this.grpChangeContext.Padding = new System.Windows.Forms.Padding(8);
            this.grpChangeContext.ShowCaption = false;
            this.grpChangeContext.Size = new System.Drawing.Size(1156, 0);
            this.grpChangeContext.TabIndex = 0;
            this.grpChangeContext.Text = "변경 배경 및 검수 요청 (선택)";
            this.grpChangeContext.Visible = false;
            // 
            // txtChangeContext
            // 
            this.txtChangeContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtChangeContext.Location = new System.Drawing.Point(10, 9);
            this.txtChangeContext.Margin = new System.Windows.Forms.Padding(0);
            this.txtChangeContext.Name = "txtChangeContext";
            this.txtChangeContext.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtChangeContext.Properties.Appearance.Options.UseFont = true;
            this.txtChangeContext.Properties.NullValuePrompt = "변경 목적 / 문제 원인 / 수정 방법 / 집중 검수 항목";
            this.txtChangeContext.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtChangeContext.Size = new System.Drawing.Size(1136, 0);
            this.txtChangeContext.TabIndex = 0;
            // 
            // contextTools
            // 
            this.contextTools.Controls.Add(this.lblContextHint);
            this.contextTools.Controls.Add(this.btnToggleContext);
            this.contextTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextTools.Font = new System.Drawing.Font("SUIT", 10F);
            this.contextTools.Location = new System.Drawing.Point(0, 0);
            this.contextTools.Name = "contextTools";
            this.contextTools.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.contextTools.Size = new System.Drawing.Size(1156, 34);
            this.contextTools.TabIndex = 1;
            // 
            // lblContextHint
            // 
            this.lblContextHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblContextHint.Appearance.Options.UseFont = true;
            this.lblContextHint.Appearance.Options.UseTextOptions = true;
            this.lblContextHint.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblContextHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblContextHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblContextHint.Location = new System.Drawing.Point(168, 0);
            this.lblContextHint.Name = "lblContextHint";
            this.lblContextHint.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblContextHint.Size = new System.Drawing.Size(988, 28);
            this.lblContextHint.TabIndex = 0;
            this.lblContextHint.Text = "선택 사항 · 변경 배경과 검수 요청을 입력할 수 있습니다.";
            // 
            // btnToggleContext
            // 
            this.btnToggleContext.Appearance.BackColor = System.Drawing.Color.White;
            this.btnToggleContext.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnToggleContext.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnToggleContext.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnToggleContext.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnToggleContext.Appearance.Options.UseBackColor = true;
            this.btnToggleContext.Appearance.Options.UseBorderColor = true;
            this.btnToggleContext.Appearance.Options.UseFont = true;
            this.btnToggleContext.Appearance.Options.UseForeColor = true;
            this.btnToggleContext.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnToggleContext.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnToggleContext.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnToggleContext.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnToggleContext.AppearanceDisabled.Options.UseBackColor = true;
            this.btnToggleContext.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnToggleContext.AppearanceDisabled.Options.UseForeColor = true;
            this.btnToggleContext.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnToggleContext.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnToggleContext.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnToggleContext.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnToggleContext.AppearanceHovered.Options.UseBackColor = true;
            this.btnToggleContext.AppearanceHovered.Options.UseBorderColor = true;
            this.btnToggleContext.AppearanceHovered.Options.UseForeColor = true;
            this.btnToggleContext.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnToggleContext.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnToggleContext.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnToggleContext.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnToggleContext.AppearancePressed.Options.UseBackColor = true;
            this.btnToggleContext.AppearancePressed.Options.UseBorderColor = true;
            this.btnToggleContext.AppearancePressed.Options.UseForeColor = true;
            this.btnToggleContext.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnToggleContext.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnToggleContext.Location = new System.Drawing.Point(0, 0);
            this.btnToggleContext.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnToggleContext.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnToggleContext.Name = "btnToggleContext";
            this.btnToggleContext.Size = new System.Drawing.Size(168, 28);
            this.btnToggleContext.TabIndex = 1;
            this.btnToggleContext.Text = "변경 배경 펼치기";
            this.btnToggleContext.ToolTip = "변경 배경을 접어도 입력한 내용은 AI 검수에 전달됩니다.";
            // 
            // layoutReviewTools
            // 
            this.layoutReviewTools.ColumnCount = 4;
            this.layoutReviewTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.layoutReviewTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.layoutReviewTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.layoutReviewTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutReviewTools.Controls.Add(this.lblAiProvider, 0, 0);
            this.layoutReviewTools.Controls.Add(this.cboAiProvider, 1, 0);
            this.layoutReviewTools.Controls.Add(this.btnAiReview, 2, 0);
            this.layoutReviewTools.Controls.Add(this.lblFooterHint, 3, 0);
            this.layoutReviewTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutReviewTools.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutReviewTools.Location = new System.Drawing.Point(0, 34);
            this.layoutReviewTools.Margin = new System.Windows.Forms.Padding(0);
            this.layoutReviewTools.Name = "layoutReviewTools";
            this.layoutReviewTools.RowCount = 1;
            this.layoutReviewTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutReviewTools.Size = new System.Drawing.Size(1156, 34);
            this.layoutReviewTools.TabIndex = 1;
            // 
            // lblAiProvider
            // 
            this.lblAiProvider.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblAiProvider.Appearance.Options.UseFont = true;
            this.lblAiProvider.Appearance.Options.UseTextOptions = true;
            this.lblAiProvider.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblAiProvider.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAiProvider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAiProvider.Location = new System.Drawing.Point(0, 0);
            this.lblAiProvider.Margin = new System.Windows.Forms.Padding(0);
            this.lblAiProvider.Name = "lblAiProvider";
            this.lblAiProvider.Size = new System.Drawing.Size(96, 34);
            this.lblAiProvider.TabIndex = 0;
            this.lblAiProvider.Text = "AI 프로바이더";
            // 
            // cboAiProvider
            // 
            this.cboAiProvider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboAiProvider.Location = new System.Drawing.Point(96, 3);
            this.cboAiProvider.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.cboAiProvider.Name = "cboAiProvider";
            this.cboAiProvider.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.cboAiProvider.Properties.Appearance.Options.UseFont = true;
            this.cboAiProvider.Properties.AppearanceDropDown.Font = new System.Drawing.Font("SUIT", 9F);
            this.cboAiProvider.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cboAiProvider.Properties.AutoHeight = false;
            this.cboAiProvider.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAiProvider.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboAiProvider.Size = new System.Drawing.Size(154, 28);
            this.cboAiProvider.TabIndex = 1;
            // 
            // btnAiReview
            // 
            this.btnAiReview.Appearance.BackColor = System.Drawing.Color.White;
            this.btnAiReview.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnAiReview.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnAiReview.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnAiReview.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAiReview.Appearance.Options.UseBackColor = true;
            this.btnAiReview.Appearance.Options.UseBorderColor = true;
            this.btnAiReview.Appearance.Options.UseFont = true;
            this.btnAiReview.Appearance.Options.UseForeColor = true;
            this.btnAiReview.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnAiReview.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnAiReview.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnAiReview.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnAiReview.AppearanceDisabled.Options.UseBackColor = true;
            this.btnAiReview.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnAiReview.AppearanceDisabled.Options.UseForeColor = true;
            this.btnAiReview.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnAiReview.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnAiReview.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnAiReview.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAiReview.AppearanceHovered.Options.UseBackColor = true;
            this.btnAiReview.AppearanceHovered.Options.UseBorderColor = true;
            this.btnAiReview.AppearanceHovered.Options.UseForeColor = true;
            this.btnAiReview.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnAiReview.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnAiReview.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnAiReview.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAiReview.AppearancePressed.Options.UseBackColor = true;
            this.btnAiReview.AppearancePressed.Options.UseBorderColor = true;
            this.btnAiReview.AppearancePressed.Options.UseForeColor = true;
            this.btnAiReview.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnAiReview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAiReview.Location = new System.Drawing.Point(256, 3);
            this.btnAiReview.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnAiReview.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAiReview.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.btnAiReview.Name = "btnAiReview";
            this.btnAiReview.Size = new System.Drawing.Size(118, 28);
            this.btnAiReview.TabIndex = 2;
            this.btnAiReview.Text = "AI 검수 실행";
            // 
            // lblFooterHint
            // 
            this.lblFooterHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblFooterHint.Appearance.Options.UseFont = true;
            this.lblFooterHint.Appearance.Options.UseTextOptions = true;
            this.lblFooterHint.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblFooterHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblFooterHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFooterHint.Location = new System.Drawing.Point(380, 0);
            this.lblFooterHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblFooterHint.Name = "lblFooterHint";
            this.lblFooterHint.Size = new System.Drawing.Size(776, 34);
            this.lblFooterHint.TabIndex = 3;
            this.lblFooterHint.Text = "검수 후 요약과 이슈를 확인하세요.";
            // 
            // grpAiResult
            // 
            this.grpAiResult.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpAiResult.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.grpAiResult.AppearanceCaption.Options.UseFont = true;
            this.grpAiResult.AppearanceCaption.Options.UseForeColor = true;
            this.grpAiResult.Controls.Add(this.tabsReview);
            this.grpAiResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAiResult.Location = new System.Drawing.Point(0, 68);
            this.grpAiResult.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.grpAiResult.Name = "grpAiResult";
            this.grpAiResult.Padding = new System.Windows.Forms.Padding(8);
            this.grpAiResult.Size = new System.Drawing.Size(1156, 362);
            this.grpAiResult.TabIndex = 2;
            this.grpAiResult.Text = "AI 검수 결과";
            // 
            // tabsReview
            // 
            this.tabsReview.AppearancePage.Header.Font = new System.Drawing.Font("SUIT", 10F);
            this.tabsReview.AppearancePage.Header.Options.UseFont = true;
            this.tabsReview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsReview.Location = new System.Drawing.Point(10, 31);
            this.tabsReview.Margin = new System.Windows.Forms.Padding(0);
            this.tabsReview.Name = "tabsReview";
            this.tabsReview.SelectedTabPage = this.pageSummary;
            this.tabsReview.Size = new System.Drawing.Size(1136, 321);
            this.tabsReview.TabIndex = 0;
            this.tabsReview.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.pageSummary,
            this.pageIssues,
            this.pageErrors});
            // 
            // pageSummary
            // 
            this.pageSummary.Controls.Add(this.layoutSummary);
            this.pageSummary.Margin = new System.Windows.Forms.Padding(0);
            this.pageSummary.Name = "pageSummary";
            this.pageSummary.Padding = new System.Windows.Forms.Padding(8);
            this.pageSummary.Size = new System.Drawing.Size(1130, 288);
            this.pageSummary.Text = "요약";
            // 
            // layoutSummary
            // 
            this.layoutSummary.ColumnCount = 1;
            this.layoutSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutSummary.Controls.Add(this.lblReviewOutcome, 0, 0);
            this.layoutSummary.Controls.Add(this.lblTokenUsage, 0, 1);
            this.layoutSummary.Controls.Add(this.txtReviewSummary, 0, 2);
            this.layoutSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutSummary.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutSummary.Location = new System.Drawing.Point(8, 8);
            this.layoutSummary.Margin = new System.Windows.Forms.Padding(0);
            this.layoutSummary.Name = "layoutSummary";
            this.layoutSummary.RowCount = 3;
            this.layoutSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.layoutSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.layoutSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutSummary.Size = new System.Drawing.Size(1114, 272);
            this.layoutSummary.TabIndex = 0;
            // 
            // lblReviewOutcome
            // 
            this.lblReviewOutcome.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblReviewOutcome.Appearance.Options.UseFont = true;
            this.lblReviewOutcome.Appearance.Options.UseTextOptions = true;
            this.lblReviewOutcome.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblReviewOutcome.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblReviewOutcome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReviewOutcome.Location = new System.Drawing.Point(0, 0);
            this.lblReviewOutcome.Margin = new System.Windows.Forms.Padding(0);
            this.lblReviewOutcome.Name = "lblReviewOutcome";
            this.lblReviewOutcome.Size = new System.Drawing.Size(1114, 28);
            this.lblReviewOutcome.TabIndex = 0;
            this.lblReviewOutcome.Text = "검수 결과 없음";
            // 
            // lblTokenUsage
            // 
            this.lblTokenUsage.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblTokenUsage.Appearance.Options.UseFont = true;
            this.lblTokenUsage.Appearance.Options.UseTextOptions = true;
            this.lblTokenUsage.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblTokenUsage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTokenUsage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTokenUsage.Location = new System.Drawing.Point(0, 28);
            this.lblTokenUsage.Margin = new System.Windows.Forms.Padding(0);
            this.lblTokenUsage.Name = "lblTokenUsage";
            this.lblTokenUsage.Size = new System.Drawing.Size(1114, 28);
            this.lblTokenUsage.TabIndex = 1;
            this.lblTokenUsage.Text = "검수 실행 후 토큰 사용량이 표시됩니다.";
            // 
            // txtReviewSummary
            // 
            this.txtReviewSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReviewSummary.Location = new System.Drawing.Point(0, 56);
            this.txtReviewSummary.Margin = new System.Windows.Forms.Padding(0);
            this.txtReviewSummary.Name = "txtReviewSummary";
            this.txtReviewSummary.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtReviewSummary.Properties.Appearance.Options.UseFont = true;
            this.txtReviewSummary.Properties.ReadOnly = true;
            this.txtReviewSummary.Size = new System.Drawing.Size(1114, 216);
            this.txtReviewSummary.TabIndex = 2;
            // 
            // pageIssues
            // 
            this.pageIssues.Controls.Add(this.splitIssues);
            this.pageIssues.Margin = new System.Windows.Forms.Padding(0);
            this.pageIssues.Name = "pageIssues";
            this.pageIssues.Padding = new System.Windows.Forms.Padding(8);
            this.pageIssues.Size = new System.Drawing.Size(1130, 288);
            this.pageIssues.Text = "이슈 (0)";
            // 
            // splitIssues
            // 
            this.splitIssues.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitIssues.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.splitIssues.AppearanceCaption.Options.UseFont = true;
            this.splitIssues.AppearanceCaption.Options.UseForeColor = true;
            this.splitIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitIssues.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.splitIssues.Location = new System.Drawing.Point(8, 8);
            this.splitIssues.Name = "splitIssues";
            this.splitIssues.Panel1.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitIssues.Panel1.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.splitIssues.Panel1.AppearanceCaption.Options.UseFont = true;
            this.splitIssues.Panel1.AppearanceCaption.Options.UseForeColor = true;
            this.splitIssues.Panel1.Controls.Add(this.gridIssues);
            this.splitIssues.Panel2.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitIssues.Panel2.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.splitIssues.Panel2.AppearanceCaption.Options.UseFont = true;
            this.splitIssues.Panel2.AppearanceCaption.Options.UseForeColor = true;
            this.splitIssues.Panel2.Controls.Add(this.layoutIssueDetail);
            this.splitIssues.Size = new System.Drawing.Size(1114, 272);
            this.splitIssues.SplitterPosition = 449;
            this.splitIssues.TabIndex = 0;
            // 
            // gridIssues
            // 
            this.gridIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridIssues.EmbeddedNavigator.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridIssues.EmbeddedNavigator.Appearance.Options.UseFont = true;
            this.gridIssues.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridIssues.Location = new System.Drawing.Point(0, 0);
            this.gridIssues.MainView = this.gridViewIssues;
            this.gridIssues.Margin = new System.Windows.Forms.Padding(0);
            this.gridIssues.Name = "gridIssues";
            this.gridIssues.Size = new System.Drawing.Size(449, 272);
            this.gridIssues.TabIndex = 0;
            this.gridIssues.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewIssues});
            // 
            // gridViewIssues
            // 
            this.gridViewIssues.Appearance.Empty.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewIssues.Appearance.Empty.Options.UseFont = true;
            this.gridViewIssues.Appearance.HeaderPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewIssues.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewIssues.Appearance.Row.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewIssues.Appearance.Row.Options.UseFont = true;
            this.gridViewIssues.GridControl = this.gridIssues;
            this.gridViewIssues.Name = "gridViewIssues";
            // 
            // layoutIssueDetail
            // 
            this.layoutIssueDetail.ColumnCount = 1;
            this.layoutIssueDetail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutIssueDetail.Controls.Add(this.txtIssueTitle, 0, 0);
            this.layoutIssueDetail.Controls.Add(this.txtIssueLocation, 0, 1);
            this.layoutIssueDetail.Controls.Add(this.txtIssueDescription, 0, 2);
            this.layoutIssueDetail.Controls.Add(this.btnIssueDiff, 0, 3);
            this.layoutIssueDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutIssueDetail.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutIssueDetail.Location = new System.Drawing.Point(0, 0);
            this.layoutIssueDetail.Margin = new System.Windows.Forms.Padding(0);
            this.layoutIssueDetail.Name = "layoutIssueDetail";
            this.layoutIssueDetail.Padding = new System.Windows.Forms.Padding(8);
            this.layoutIssueDetail.RowCount = 4;
            this.layoutIssueDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.layoutIssueDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.layoutIssueDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutIssueDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layoutIssueDetail.Size = new System.Drawing.Size(660, 272);
            this.layoutIssueDetail.TabIndex = 0;
            // 
            // txtIssueTitle
            // 
            this.txtIssueTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIssueTitle.Location = new System.Drawing.Point(8, 8);
            this.txtIssueTitle.Margin = new System.Windows.Forms.Padding(0);
            this.txtIssueTitle.Name = "txtIssueTitle";
            this.txtIssueTitle.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtIssueTitle.Properties.Appearance.Options.UseFont = true;
            this.txtIssueTitle.Properties.ReadOnly = true;
            this.txtIssueTitle.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtIssueTitle.Size = new System.Drawing.Size(644, 54);
            this.txtIssueTitle.TabIndex = 0;
            // 
            // txtIssueLocation
            // 
            this.txtIssueLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIssueLocation.Location = new System.Drawing.Point(8, 62);
            this.txtIssueLocation.Margin = new System.Windows.Forms.Padding(0);
            this.txtIssueLocation.Name = "txtIssueLocation";
            this.txtIssueLocation.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtIssueLocation.Properties.Appearance.Options.UseFont = true;
            this.txtIssueLocation.Properties.AutoHeight = false;
            this.txtIssueLocation.Properties.ReadOnly = true;
            this.txtIssueLocation.Size = new System.Drawing.Size(644, 28);
            this.txtIssueLocation.TabIndex = 1;
            // 
            // txtIssueDescription
            // 
            this.txtIssueDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIssueDescription.Location = new System.Drawing.Point(8, 90);
            this.txtIssueDescription.Margin = new System.Windows.Forms.Padding(0);
            this.txtIssueDescription.Name = "txtIssueDescription";
            this.txtIssueDescription.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtIssueDescription.Properties.Appearance.Options.UseFont = true;
            this.txtIssueDescription.Properties.ReadOnly = true;
            this.txtIssueDescription.Size = new System.Drawing.Size(644, 138);
            this.txtIssueDescription.TabIndex = 2;
            // 
            // btnIssueDiff
            // 
            this.btnIssueDiff.Appearance.BackColor = System.Drawing.Color.White;
            this.btnIssueDiff.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnIssueDiff.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnIssueDiff.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnIssueDiff.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnIssueDiff.Appearance.Options.UseBackColor = true;
            this.btnIssueDiff.Appearance.Options.UseBorderColor = true;
            this.btnIssueDiff.Appearance.Options.UseFont = true;
            this.btnIssueDiff.Appearance.Options.UseForeColor = true;
            this.btnIssueDiff.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnIssueDiff.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnIssueDiff.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnIssueDiff.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnIssueDiff.AppearanceDisabled.Options.UseBackColor = true;
            this.btnIssueDiff.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnIssueDiff.AppearanceDisabled.Options.UseForeColor = true;
            this.btnIssueDiff.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnIssueDiff.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnIssueDiff.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnIssueDiff.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnIssueDiff.AppearanceHovered.Options.UseBackColor = true;
            this.btnIssueDiff.AppearanceHovered.Options.UseBorderColor = true;
            this.btnIssueDiff.AppearanceHovered.Options.UseForeColor = true;
            this.btnIssueDiff.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnIssueDiff.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnIssueDiff.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnIssueDiff.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnIssueDiff.AppearancePressed.Options.UseBackColor = true;
            this.btnIssueDiff.AppearancePressed.Options.UseBorderColor = true;
            this.btnIssueDiff.AppearancePressed.Options.UseForeColor = true;
            this.btnIssueDiff.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnIssueDiff.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnIssueDiff.Enabled = false;
            this.btnIssueDiff.Location = new System.Drawing.Point(593, 232);
            this.btnIssueDiff.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnIssueDiff.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnIssueDiff.Margin = new System.Windows.Forms.Padding(0, 4, 6, 4);
            this.btnIssueDiff.Name = "btnIssueDiff";
            this.btnIssueDiff.Size = new System.Drawing.Size(53, 28);
            this.btnIssueDiff.TabIndex = 3;
            this.btnIssueDiff.Text = "선택 이슈 Diff 보기";
            this.btnIssueDiff.ToolTip = "선택한 이슈에 연결된 검수 당시 Diff를 엽니다.";
            // 
            // pageErrors
            // 
            this.pageErrors.Controls.Add(this.txtReviewErrors);
            this.pageErrors.Margin = new System.Windows.Forms.Padding(0);
            this.pageErrors.Name = "pageErrors";
            this.pageErrors.Padding = new System.Windows.Forms.Padding(8);
            this.pageErrors.PageVisible = false;
            this.pageErrors.Size = new System.Drawing.Size(1130, 288);
            this.pageErrors.Text = "오류 상세";
            // 
            // txtReviewErrors
            // 
            this.txtReviewErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReviewErrors.Location = new System.Drawing.Point(8, 8);
            this.txtReviewErrors.Margin = new System.Windows.Forms.Padding(0);
            this.txtReviewErrors.Name = "txtReviewErrors";
            this.txtReviewErrors.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtReviewErrors.Properties.Appearance.Options.UseFont = true;
            this.txtReviewErrors.Properties.ReadOnly = true;
            this.txtReviewErrors.Size = new System.Drawing.Size(1114, 272);
            this.txtReviewErrors.TabIndex = 0;
            // 
            // layoutMessages
            // 
            this.layoutMessages.ColumnCount = 2;
            this.layoutMessages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutMessages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutMessages.Controls.Add(this.grpCommitMessage, 0, 0);
            this.layoutMessages.Controls.Add(this.grpSuggestedMessage, 1, 0);
            this.layoutMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutMessages.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutMessages.Location = new System.Drawing.Point(0, 436);
            this.layoutMessages.Margin = new System.Windows.Forms.Padding(0);
            this.layoutMessages.Name = "layoutMessages";
            this.layoutMessages.RowCount = 1;
            this.layoutMessages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutMessages.Size = new System.Drawing.Size(1156, 160);
            this.layoutMessages.TabIndex = 3;
            // 
            // grpCommitMessage
            // 
            this.grpCommitMessage.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpCommitMessage.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.grpCommitMessage.AppearanceCaption.Options.UseFont = true;
            this.grpCommitMessage.AppearanceCaption.Options.UseForeColor = true;
            this.grpCommitMessage.Controls.Add(this.txtCommitMessage);
            this.grpCommitMessage.Controls.Add(this.messageTools);
            this.grpCommitMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCommitMessage.Location = new System.Drawing.Point(0, 0);
            this.grpCommitMessage.Margin = new System.Windows.Forms.Padding(0);
            this.grpCommitMessage.Name = "grpCommitMessage";
            this.grpCommitMessage.Padding = new System.Windows.Forms.Padding(8);
            this.grpCommitMessage.Size = new System.Drawing.Size(1156, 160);
            this.grpCommitMessage.TabIndex = 0;
            this.grpCommitMessage.Text = "최종 커밋 메시지";
            // 
            // txtCommitMessage
            // 
            this.txtCommitMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommitMessage.Location = new System.Drawing.Point(10, 65);
            this.txtCommitMessage.Margin = new System.Windows.Forms.Padding(0);
            this.txtCommitMessage.Name = "txtCommitMessage";
            this.txtCommitMessage.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtCommitMessage.Properties.Appearance.Options.UseFont = true;
            this.txtCommitMessage.Properties.NullValuePrompt = "실제로 SVN 커밋에 사용할 메시지";
            this.txtCommitMessage.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtCommitMessage.Size = new System.Drawing.Size(1136, 85);
            this.txtCommitMessage.TabIndex = 0;
            // 
            // messageTools
            // 
            this.messageTools.Controls.Add(this.lblMessageHint);
            this.messageTools.Controls.Add(this.btnToggleSuggestion);
            this.messageTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.messageTools.Font = new System.Drawing.Font("SUIT", 10F);
            this.messageTools.Location = new System.Drawing.Point(10, 31);
            this.messageTools.Name = "messageTools";
            this.messageTools.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.messageTools.Size = new System.Drawing.Size(1136, 34);
            this.messageTools.TabIndex = 1;
            // 
            // lblMessageHint
            // 
            this.lblMessageHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblMessageHint.Appearance.Options.UseFont = true;
            this.lblMessageHint.Appearance.Options.UseTextOptions = true;
            this.lblMessageHint.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblMessageHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMessageHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessageHint.Location = new System.Drawing.Point(0, 0);
            this.lblMessageHint.Name = "lblMessageHint";
            this.lblMessageHint.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblMessageHint.Size = new System.Drawing.Size(980, 28);
            this.lblMessageHint.TabIndex = 0;
            this.lblMessageHint.Text = "최종 메시지를 직접 작성하거나 AI 제안을 적용하세요.";
            // 
            // btnToggleSuggestion
            // 
            this.btnToggleSuggestion.Appearance.BackColor = System.Drawing.Color.White;
            this.btnToggleSuggestion.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnToggleSuggestion.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnToggleSuggestion.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnToggleSuggestion.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnToggleSuggestion.Appearance.Options.UseBackColor = true;
            this.btnToggleSuggestion.Appearance.Options.UseBorderColor = true;
            this.btnToggleSuggestion.Appearance.Options.UseFont = true;
            this.btnToggleSuggestion.Appearance.Options.UseForeColor = true;
            this.btnToggleSuggestion.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnToggleSuggestion.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnToggleSuggestion.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnToggleSuggestion.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnToggleSuggestion.AppearanceDisabled.Options.UseBackColor = true;
            this.btnToggleSuggestion.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnToggleSuggestion.AppearanceDisabled.Options.UseForeColor = true;
            this.btnToggleSuggestion.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnToggleSuggestion.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnToggleSuggestion.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnToggleSuggestion.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnToggleSuggestion.AppearanceHovered.Options.UseBackColor = true;
            this.btnToggleSuggestion.AppearanceHovered.Options.UseBorderColor = true;
            this.btnToggleSuggestion.AppearanceHovered.Options.UseForeColor = true;
            this.btnToggleSuggestion.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnToggleSuggestion.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnToggleSuggestion.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnToggleSuggestion.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnToggleSuggestion.AppearancePressed.Options.UseBackColor = true;
            this.btnToggleSuggestion.AppearancePressed.Options.UseBorderColor = true;
            this.btnToggleSuggestion.AppearancePressed.Options.UseForeColor = true;
            this.btnToggleSuggestion.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnToggleSuggestion.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnToggleSuggestion.Enabled = false;
            this.btnToggleSuggestion.Location = new System.Drawing.Point(980, 0);
            this.btnToggleSuggestion.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnToggleSuggestion.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnToggleSuggestion.Name = "btnToggleSuggestion";
            this.btnToggleSuggestion.Size = new System.Drawing.Size(156, 28);
            this.btnToggleSuggestion.TabIndex = 1;
            this.btnToggleSuggestion.Text = "AI 제안 없음";
            // 
            // grpSuggestedMessage
            // 
            this.grpSuggestedMessage.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpSuggestedMessage.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.grpSuggestedMessage.AppearanceCaption.Options.UseFont = true;
            this.grpSuggestedMessage.AppearanceCaption.Options.UseForeColor = true;
            this.grpSuggestedMessage.Controls.Add(this.layoutSuggestion);
            this.grpSuggestedMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSuggestedMessage.Location = new System.Drawing.Point(1156, 0);
            this.grpSuggestedMessage.Margin = new System.Windows.Forms.Padding(0);
            this.grpSuggestedMessage.Name = "grpSuggestedMessage";
            this.grpSuggestedMessage.Padding = new System.Windows.Forms.Padding(8);
            this.grpSuggestedMessage.Size = new System.Drawing.Size(1, 160);
            this.grpSuggestedMessage.TabIndex = 1;
            this.grpSuggestedMessage.Text = "AI 제안 메시지";
            this.grpSuggestedMessage.Visible = false;
            // 
            // layoutSuggestion
            // 
            this.layoutSuggestion.ColumnCount = 1;
            this.layoutSuggestion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutSuggestion.Controls.Add(this.txtSuggestedMsg, 0, 0);
            this.layoutSuggestion.Controls.Add(this.btnCopyCommitMsg, 0, 1);
            this.layoutSuggestion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutSuggestion.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutSuggestion.Location = new System.Drawing.Point(9, 31);
            this.layoutSuggestion.Margin = new System.Windows.Forms.Padding(0);
            this.layoutSuggestion.Name = "layoutSuggestion";
            this.layoutSuggestion.RowCount = 2;
            this.layoutSuggestion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutSuggestion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutSuggestion.Size = new System.Drawing.Size(0, 119);
            this.layoutSuggestion.TabIndex = 0;
            // 
            // txtSuggestedMsg
            // 
            this.txtSuggestedMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSuggestedMsg.Location = new System.Drawing.Point(0, 0);
            this.txtSuggestedMsg.Margin = new System.Windows.Forms.Padding(0);
            this.txtSuggestedMsg.Name = "txtSuggestedMsg";
            this.txtSuggestedMsg.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtSuggestedMsg.Properties.Appearance.Options.UseFont = true;
            this.txtSuggestedMsg.Properties.NullValuePrompt = "검수가 완료되면 AI 제안 메시지가 표시됩니다.";
            this.txtSuggestedMsg.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSuggestedMsg.Properties.ReadOnly = true;
            this.txtSuggestedMsg.Size = new System.Drawing.Size(1, 85);
            this.txtSuggestedMsg.TabIndex = 0;
            // 
            // btnCopyCommitMsg
            // 
            this.btnCopyCommitMsg.Appearance.BackColor = System.Drawing.Color.White;
            this.btnCopyCommitMsg.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnCopyCommitMsg.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnCopyCommitMsg.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnCopyCommitMsg.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnCopyCommitMsg.Appearance.Options.UseBackColor = true;
            this.btnCopyCommitMsg.Appearance.Options.UseBorderColor = true;
            this.btnCopyCommitMsg.Appearance.Options.UseFont = true;
            this.btnCopyCommitMsg.Appearance.Options.UseForeColor = true;
            this.btnCopyCommitMsg.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnCopyCommitMsg.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnCopyCommitMsg.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnCopyCommitMsg.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnCopyCommitMsg.AppearanceDisabled.Options.UseBackColor = true;
            this.btnCopyCommitMsg.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnCopyCommitMsg.AppearanceDisabled.Options.UseForeColor = true;
            this.btnCopyCommitMsg.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnCopyCommitMsg.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnCopyCommitMsg.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnCopyCommitMsg.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnCopyCommitMsg.AppearanceHovered.Options.UseBackColor = true;
            this.btnCopyCommitMsg.AppearanceHovered.Options.UseBorderColor = true;
            this.btnCopyCommitMsg.AppearanceHovered.Options.UseForeColor = true;
            this.btnCopyCommitMsg.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnCopyCommitMsg.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnCopyCommitMsg.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnCopyCommitMsg.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnCopyCommitMsg.AppearancePressed.Options.UseBackColor = true;
            this.btnCopyCommitMsg.AppearancePressed.Options.UseBorderColor = true;
            this.btnCopyCommitMsg.AppearancePressed.Options.UseForeColor = true;
            this.btnCopyCommitMsg.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnCopyCommitMsg.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCopyCommitMsg.Enabled = false;
            this.btnCopyCommitMsg.Location = new System.Drawing.Point(0, 88);
            this.btnCopyCommitMsg.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnCopyCommitMsg.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCopyCommitMsg.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.btnCopyCommitMsg.Name = "btnCopyCommitMsg";
            this.btnCopyCommitMsg.Size = new System.Drawing.Size(1, 28);
            this.btnCopyCommitMsg.TabIndex = 1;
            this.btnCopyCommitMsg.Text = "커밋 메시지에 적용";
            this.btnCopyCommitMsg.ToolTip = "현재 검수 결과를 확인한 뒤 왼쪽의 최종 커밋 메시지를 제안 메시지로 바꿉니다.";
            // 
            // layoutFooter
            // 
            this.layoutFooter.ColumnCount = 3;
            this.layoutFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.layoutFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.layoutFooter.Controls.Add(this.lblAiStatus, 0, 0);
            this.layoutFooter.Controls.Add(this.btnCommit, 1, 0);
            this.layoutFooter.Controls.Add(this.btnCancel, 2, 0);
            this.layoutFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutFooter.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutFooter.Location = new System.Drawing.Point(0, 596);
            this.layoutFooter.Margin = new System.Windows.Forms.Padding(0);
            this.layoutFooter.Name = "layoutFooter";
            this.layoutFooter.RowCount = 1;
            this.layoutFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutFooter.Size = new System.Drawing.Size(1156, 36);
            this.layoutFooter.TabIndex = 4;
            // 
            // lblAiStatus
            // 
            this.lblAiStatus.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblAiStatus.Appearance.Options.UseFont = true;
            this.lblAiStatus.Appearance.Options.UseTextOptions = true;
            this.lblAiStatus.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblAiStatus.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblAiStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAiStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAiStatus.Location = new System.Drawing.Point(0, 0);
            this.lblAiStatus.Margin = new System.Windows.Forms.Padding(0);
            this.lblAiStatus.Name = "lblAiStatus";
            this.lblAiStatus.Size = new System.Drawing.Size(924, 36);
            this.lblAiStatus.TabIndex = 0;
            this.lblAiStatus.Text = "검수 실행 전";
            // 
            // btnCommit
            // 
            this.btnCommit.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnCommit.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnCommit.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnCommit.Appearance.Font = new System.Drawing.Font("SUIT", 9F, System.Drawing.FontStyle.Bold);
            this.btnCommit.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnCommit.Appearance.Options.UseBackColor = true;
            this.btnCommit.Appearance.Options.UseBorderColor = true;
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Appearance.Options.UseForeColor = true;
            this.btnCommit.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnCommit.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnCommit.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnCommit.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnCommit.AppearanceDisabled.Options.UseBackColor = true;
            this.btnCommit.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnCommit.AppearanceDisabled.Options.UseForeColor = true;
            this.btnCommit.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(72)))), ((int)(((byte)(104)))));
            this.btnCommit.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(72)))), ((int)(((byte)(104)))));
            this.btnCommit.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(72)))), ((int)(((byte)(104)))));
            this.btnCommit.AppearanceHovered.ForeColor = System.Drawing.Color.White;
            this.btnCommit.AppearanceHovered.Options.UseBackColor = true;
            this.btnCommit.AppearanceHovered.Options.UseBorderColor = true;
            this.btnCommit.AppearanceHovered.Options.UseForeColor = true;
            this.btnCommit.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.btnCommit.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.btnCommit.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.btnCommit.AppearancePressed.ForeColor = System.Drawing.Color.White;
            this.btnCommit.AppearancePressed.Options.UseBackColor = true;
            this.btnCommit.AppearancePressed.Options.UseBorderColor = true;
            this.btnCommit.AppearancePressed.Options.UseForeColor = true;
            this.btnCommit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnCommit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCommit.Location = new System.Drawing.Point(924, 4);
            this.btnCommit.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnCommit.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCommit.Margin = new System.Windows.Forms.Padding(0, 4, 6, 4);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(126, 28);
            this.btnCommit.TabIndex = 1;
            this.btnCommit.Text = "커밋 실행";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BackColor = System.Drawing.Color.White;
            this.btnCancel.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnCancel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Appearance.Options.UseForeColor = true;
            this.btnCancel.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnCancel.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnCancel.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnCancel.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnCancel.AppearanceDisabled.Options.UseBackColor = true;
            this.btnCancel.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnCancel.AppearanceDisabled.Options.UseForeColor = true;
            this.btnCancel.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnCancel.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnCancel.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnCancel.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnCancel.AppearanceHovered.Options.UseBackColor = true;
            this.btnCancel.AppearanceHovered.Options.UseBorderColor = true;
            this.btnCancel.AppearanceHovered.Options.UseForeColor = true;
            this.btnCancel.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnCancel.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnCancel.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnCancel.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnCancel.AppearancePressed.Options.UseBackColor = true;
            this.btnCancel.AppearancePressed.Options.UseBorderColor = true;
            this.btnCancel.AppearancePressed.Options.UseForeColor = true;
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Location = new System.Drawing.Point(1056, 4);
            this.btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0, 4, 6, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "취소";
            // 
            // layoutFrame
            // 
            this.layoutFrame.Controls.Add(this.layoutRoot);
            this.layoutFrame.Controls.Add(this.filesSplitter);
            this.layoutFrame.Controls.Add(this.grpCommitFiles);
            this.layoutFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutFrame.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutFrame.Location = new System.Drawing.Point(0, 0);
            this.layoutFrame.Margin = new System.Windows.Forms.Padding(0);
            this.layoutFrame.Name = "layoutFrame";
            this.layoutFrame.Padding = new System.Windows.Forms.Padding(12);
            this.layoutFrame.Size = new System.Drawing.Size(1180, 840);
            this.layoutFrame.TabIndex = 0;
            // 
            // filesSplitter
            // 
            this.filesSplitter.AccessibleName = "커밋 대상 목록 높이 조절";
            this.filesSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.filesSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.filesSplitter.Font = new System.Drawing.Font("SUIT", 10F);
            this.filesSplitter.Location = new System.Drawing.Point(12, 190);
            this.filesSplitter.MinExtra = 480;
            this.filesSplitter.MinSize = 120;
            this.filesSplitter.Name = "filesSplitter";
            this.filesSplitter.Size = new System.Drawing.Size(1156, 6);
            this.filesSplitter.TabIndex = 1;
            this.filesSplitter.TabStop = false;
            // 
            // grpCommitFiles
            // 
            this.grpCommitFiles.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpCommitFiles.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.grpCommitFiles.AppearanceCaption.Options.UseFont = true;
            this.grpCommitFiles.AppearanceCaption.Options.UseForeColor = true;
            this.grpCommitFiles.Controls.Add(this.gridCommitFiles);
            this.grpCommitFiles.Controls.Add(this.fileTools);
            this.grpCommitFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCommitFiles.Location = new System.Drawing.Point(12, 12);
            this.grpCommitFiles.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.grpCommitFiles.Name = "grpCommitFiles";
            this.grpCommitFiles.Padding = new System.Windows.Forms.Padding(8);
            this.grpCommitFiles.Size = new System.Drawing.Size(1156, 178);
            this.grpCommitFiles.TabIndex = 2;
            this.grpCommitFiles.Text = "커밋 대상";
            // 
            // gridCommitFiles
            // 
            this.gridCommitFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCommitFiles.EmbeddedNavigator.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridCommitFiles.EmbeddedNavigator.Appearance.Options.UseFont = true;
            this.gridCommitFiles.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridCommitFiles.Location = new System.Drawing.Point(10, 65);
            this.gridCommitFiles.MainView = this.gridViewCommitFiles;
            this.gridCommitFiles.Margin = new System.Windows.Forms.Padding(0);
            this.gridCommitFiles.Name = "gridCommitFiles";
            this.gridCommitFiles.Size = new System.Drawing.Size(1136, 103);
            this.gridCommitFiles.TabIndex = 0;
            this.gridCommitFiles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCommitFiles});
            // 
            // gridViewCommitFiles
            // 
            this.gridViewCommitFiles.Appearance.Empty.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewCommitFiles.Appearance.Empty.Options.UseFont = true;
            this.gridViewCommitFiles.Appearance.HeaderPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewCommitFiles.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewCommitFiles.Appearance.Row.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewCommitFiles.Appearance.Row.Options.UseFont = true;
            this.gridViewCommitFiles.GridControl = this.gridCommitFiles;
            this.gridViewCommitFiles.Name = "gridViewCommitFiles";
            // 
            // fileTools
            // 
            this.fileTools.Controls.Add(this.lblFileDiffHint);
            this.fileTools.Controls.Add(this.btnFileDiff);
            this.fileTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.fileTools.Font = new System.Drawing.Font("SUIT", 10F);
            this.fileTools.Location = new System.Drawing.Point(10, 31);
            this.fileTools.Margin = new System.Windows.Forms.Padding(0);
            this.fileTools.Name = "fileTools";
            this.fileTools.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.fileTools.Size = new System.Drawing.Size(1136, 34);
            this.fileTools.TabIndex = 1;
            // 
            // lblFileDiffHint
            // 
            this.lblFileDiffHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblFileDiffHint.Appearance.Options.UseFont = true;
            this.lblFileDiffHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblFileDiffHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFileDiffHint.Location = new System.Drawing.Point(0, 0);
            this.lblFileDiffHint.Name = "lblFileDiffHint";
            this.lblFileDiffHint.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblFileDiffHint.Size = new System.Drawing.Size(980, 28);
            this.lblFileDiffHint.TabIndex = 0;
            this.lblFileDiffHint.Text = "파일을 더블클릭하면 저장소 최신 버전과 비교합니다.";
            // 
            // btnFileDiff
            // 
            this.btnFileDiff.Appearance.BackColor = System.Drawing.Color.White;
            this.btnFileDiff.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnFileDiff.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnFileDiff.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnFileDiff.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnFileDiff.Appearance.Options.UseBackColor = true;
            this.btnFileDiff.Appearance.Options.UseBorderColor = true;
            this.btnFileDiff.Appearance.Options.UseFont = true;
            this.btnFileDiff.Appearance.Options.UseForeColor = true;
            this.btnFileDiff.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnFileDiff.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnFileDiff.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnFileDiff.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnFileDiff.AppearanceDisabled.Options.UseBackColor = true;
            this.btnFileDiff.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnFileDiff.AppearanceDisabled.Options.UseForeColor = true;
            this.btnFileDiff.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnFileDiff.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnFileDiff.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnFileDiff.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnFileDiff.AppearanceHovered.Options.UseBackColor = true;
            this.btnFileDiff.AppearanceHovered.Options.UseBorderColor = true;
            this.btnFileDiff.AppearanceHovered.Options.UseForeColor = true;
            this.btnFileDiff.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnFileDiff.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnFileDiff.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnFileDiff.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnFileDiff.AppearancePressed.Options.UseBackColor = true;
            this.btnFileDiff.AppearancePressed.Options.UseBorderColor = true;
            this.btnFileDiff.AppearancePressed.Options.UseForeColor = true;
            this.btnFileDiff.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnFileDiff.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnFileDiff.Enabled = false;
            this.btnFileDiff.Location = new System.Drawing.Point(980, 0);
            this.btnFileDiff.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnFileDiff.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnFileDiff.Margin = new System.Windows.Forms.Padding(0);
            this.btnFileDiff.Name = "btnFileDiff";
            this.btnFileDiff.Size = new System.Drawing.Size(156, 28);
            this.btnFileDiff.TabIndex = 0;
            this.btnFileDiff.Text = "최신 버전과 비교";
            this.btnFileDiff.ToolTip = "선택한 파일의 저장소 최신(HEAD)과 현재 작업 파일을 비교합니다.";
            // 
            // CommitView
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1180, 840);
            this.Controls.Add(this.layoutFrame);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1040, 760);
            this.Name = "CommitView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AI 검수 및 커밋";
            this.layoutRoot.ResumeLayout(false);
            this.contextPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpChangeContext)).EndInit();
            this.grpChangeContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtChangeContext.Properties)).EndInit();
            this.contextTools.ResumeLayout(false);
            this.layoutReviewTools.ResumeLayout(false);
            this.layoutReviewTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAiProvider.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpAiResult)).EndInit();
            this.grpAiResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabsReview)).EndInit();
            this.tabsReview.ResumeLayout(false);
            this.pageSummary.ResumeLayout(false);
            this.layoutSummary.ResumeLayout(false);
            this.layoutSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReviewSummary.Properties)).EndInit();
            this.pageIssues.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitIssues)).EndInit();
            this.splitIssues.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridIssues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewIssues)).EndInit();
            this.layoutIssueDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtIssueTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIssueLocation.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIssueDescription.Properties)).EndInit();
            this.pageErrors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtReviewErrors.Properties)).EndInit();
            this.layoutMessages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpCommitMessage)).EndInit();
            this.grpCommitMessage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCommitMessage.Properties)).EndInit();
            this.messageTools.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpSuggestedMessage)).EndInit();
            this.grpSuggestedMessage.ResumeLayout(false);
            this.layoutSuggestion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtSuggestedMsg.Properties)).EndInit();
            this.layoutFooter.ResumeLayout(false);
            this.layoutFooter.PerformLayout();
            this.layoutFrame.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpCommitFiles)).EndInit();
            this.grpCommitFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridCommitFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCommitFiles)).EndInit();
            this.fileTools.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
