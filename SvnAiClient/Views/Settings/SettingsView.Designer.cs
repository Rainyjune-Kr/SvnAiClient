using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;

namespace SvnAiClient.Views.Settings
{
    partial class SettingsView
    {
        #region Fields

        /// <summary>화면 구성 요소의 수명을 관리한다.</summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>토큰 한도와 실행 옵션의 표시를 전환한다.</summary>
        private DevExpress.XtraEditors.SimpleButton btnAdvancedSettings;
        /// <summary>고급 설정과 권장 모델 명령 사이의 공통 간격.</summary>
        private System.Windows.Forms.Panel advancedHeaderGap;
        /// <summary>설정 탭과 하단 작업 버튼 배치.</summary>
        private Panel layoutRoot;
        /// <summary>AI 설정과 작업 복사본 탭.</summary>
        private XtraTabControl tabControl;
        /// <summary>AI 검수 설정 탭.</summary>
        private XtraTabPage tabAi;
        /// <summary>작업 복사본 탭.</summary>
        private XtraTabPage tabWorkingCopy;
        /// <summary>기본 프로바이더, 연결 정보 및 검수 항목 배치.</summary>
        private Panel layoutAi;
        /// <summary>기본 검수 프로바이더 선택 영역.</summary>
        private GroupControl grpProvider;
        /// <summary>기본 프로바이더 선택 버튼 배치.</summary>
        private FlowLayoutPanel providerChoices;
        /// <summary>기본 Claude 검수 선택.</summary>
        private RadioButton rbClaude;
        /// <summary>기본 OpenAI 검수 선택.</summary>
        private RadioButton rbOpenAi;
        /// <summary>기본 Gemini 검수 선택.</summary>
        private RadioButton rbGemini;
        /// <summary>기본 Local 검수 선택.</summary>
        private RadioButton rbLocal;
        /// <summary>편집 대상 설명과 권장 모델 버튼 배치.</summary>
        private Panel layoutProviderHeader;
        /// <summary>편집 중인 설정 라벨.</summary>
        private LabelControl lblEditingProvider;
        /// <summary>권장 모델 적용 버튼.</summary>
        private SimpleButton btnRecommendedModel;
        /// <summary>편집할 프로바이더별 연결 설정 탭.</summary>
        private XtraTabControl tabProviders;
        /// <summary>Claude 연결 설정 탭.</summary>
        private XtraTabPage tabClaude;
        /// <summary>Claude 입력 필드 배치.</summary>
        private TableLayoutPanel layoutClaude;
        /// <summary>API 키 라벨.</summary>
        private LabelControl lblClaudeApiKey;
        /// <summary>설정 입력 컨트롤.</summary>
        private TextEdit txtClaudeApiKey;
        /// <summary>모델 라벨.</summary>
        private LabelControl lblClaudeModel;
        /// <summary>설정 입력 컨트롤.</summary>
        private ComboBoxEdit txtClaudeModel;
        /// <summary>출력 토큰 한도 라벨.</summary>
        private LabelControl lblClaudeTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numClaudeTokens;
        /// <summary>입력 토큰 한도 라벨.</summary>
        private LabelControl lblClaudeInputTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numClaudeInputTokens;
        /// <summary>모델명을 직접 입력할 수 있습니다. 권장 모델 적용 시 토큰 한도도 변경됩니다. 라벨.</summary>
        private LabelControl lblClaudeHint;
        /// <summary>OpenAI 연결 설정 탭.</summary>
        private XtraTabPage tabOpenAi;
        /// <summary>OpenAI 입력 필드 배치.</summary>
        private TableLayoutPanel layoutOpenAi;
        /// <summary>API 키 라벨.</summary>
        private LabelControl lblOpenAiApiKey;
        /// <summary>설정 입력 컨트롤.</summary>
        private TextEdit txtOpenAiApiKey;
        /// <summary>모델 라벨.</summary>
        private LabelControl lblOpenAiModel;
        /// <summary>설정 입력 컨트롤.</summary>
        private ComboBoxEdit txtOpenAiModel;
        /// <summary>출력 토큰 한도 라벨.</summary>
        private LabelControl lblOpenAiTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numOpenAiTokens;
        /// <summary>입력 토큰 한도 라벨.</summary>
        private LabelControl lblOpenAiInputTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numOpenAiInputTokens;
        /// <summary>요청 제한 시간(초) 라벨.</summary>
        private LabelControl lblOpenAiTimeout;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numOpenAiTimeout;
        /// <summary>추론 강도 라벨.</summary>
        private LabelControl lblOpenAiEffort;
        /// <summary>설정 입력 컨트롤.</summary>
        private ComboBoxEdit cboOpenAiEffort;
        /// <summary>모델명을 직접 입력할 수 있습니다. 권장 모델 적용 시 토큰 한도도 변경됩니다. 라벨.</summary>
        private LabelControl lblOpenAiHint;
        /// <summary>Gemini 연결 설정 탭.</summary>
        private XtraTabPage tabGemini;
        /// <summary>Gemini 입력 필드 배치.</summary>
        private TableLayoutPanel layoutGemini;
        /// <summary>API 키 라벨.</summary>
        private LabelControl lblGeminiApiKey;
        /// <summary>설정 입력 컨트롤.</summary>
        private TextEdit txtGeminiApiKey;
        /// <summary>모델 라벨.</summary>
        private LabelControl lblGeminiModel;
        /// <summary>설정 입력 컨트롤.</summary>
        private ComboBoxEdit txtGeminiModel;
        /// <summary>출력 토큰 한도 라벨.</summary>
        private LabelControl lblGeminiTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numGeminiTokens;
        /// <summary>입력 토큰 한도 라벨.</summary>
        private LabelControl lblGeminiInputTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numGeminiInputTokens;
        /// <summary>모델명을 직접 입력할 수 있습니다. 권장 모델 적용 시 토큰 한도도 변경됩니다. 라벨.</summary>
        private LabelControl lblGeminiHint;
        /// <summary>Local 연결 설정 탭.</summary>
        private XtraTabPage tabLocal;
        /// <summary>Local 입력 필드 배치.</summary>
        private TableLayoutPanel layoutLocal;
        /// <summary>서버 주소 라벨.</summary>
        private LabelControl lblLocalEndpoint;
        /// <summary>설정 입력 컨트롤.</summary>
        private TextEdit txtLocalEndpoint;
        /// <summary>모델 라벨.</summary>
        private LabelControl lblLocalModel;
        /// <summary>설정 입력 컨트롤.</summary>
        private TextEdit txtLocalModel;
        /// <summary>출력 토큰 한도 라벨.</summary>
        private LabelControl lblLocalTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numLocalTokens;
        /// <summary>입력 토큰 한도 라벨.</summary>
        private LabelControl lblLocalInputTokens;
        /// <summary>설정 입력 컨트롤.</summary>
        private SpinEdit numLocalInputTokens;
        /// <summary>로컬 서버의 주소와 모델명을 직접 입력하세요. 라벨.</summary>
        private LabelControl lblLocalHint;
        /// <summary>설정 입력 컨트롤.</summary>
        private CheckEdit chkAutoReview;
        /// <summary>검수에 포함할 항목 선택 영역.</summary>
        private GroupControl grpReviewItems;
        /// <summary>검수 항목별 사용 여부.</summary>
        private CheckedListBoxControl clbReviewItems;
        /// <summary>작업 복사본 안내, 목록 및 작업 버튼 배치.</summary>
        private Panel layoutWorkingCopies;
        /// <summary>등록한 작업 복사본입니다. 목록에서 제거해도 실제 폴더는 삭제되지 않습니다. 라벨.</summary>
        private LabelControl lblWorkingCopiesHint;
        /// <summary>이름, 경로 및 사용자별 작업 복사본 목록.</summary>
        private DevExpress.XtraGrid.GridControl gridWorkingCopies;
        /// <summary>작업 복사본 선택을 제공하는 읽기 전용 표.</summary>
        private DevExpress.XtraGrid.Views.Grid.GridView viewWorkingCopies;
        /// <summary>이름 열.</summary>
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        /// <summary>로컬 경로 열.</summary>
        private DevExpress.XtraGrid.Columns.GridColumn colPath;
        /// <summary>사용자 열.</summary>
        private DevExpress.XtraGrid.Columns.GridColumn colUsername;
        /// <summary>작업 복사본 추가, 수정 및 제거 버튼 배치.</summary>
        private FlowLayoutPanel layoutWorkingCopyButtons;
        /// <summary>추가 버튼.</summary>
        private SimpleButton btnAddWC;
        /// <summary>수정 버튼.</summary>
        private SimpleButton btnEditWC;
        /// <summary>제거 버튼.</summary>
        private SimpleButton btnRemoveWC;
        /// <summary>설정 저장 및 취소 버튼 배치.</summary>
        private FlowLayoutPanel panelButtons;
        /// <summary>저장 버튼.</summary>
        private SimpleButton btnSave;
        /// <summary>취소 버튼.</summary>
        private SimpleButton btnCancel;

        #endregion

        #region Initialization

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnAdvancedSettings = new DevExpress.XtraEditors.SimpleButton();
            this.advancedHeaderGap = new System.Windows.Forms.Panel();
            this.layoutRoot = new System.Windows.Forms.Panel();
            this.tabControl = new DevExpress.XtraTab.XtraTabControl();
            this.tabAi = new DevExpress.XtraTab.XtraTabPage();
            this.layoutAi = new System.Windows.Forms.Panel();
            this.grpReviewItems = new DevExpress.XtraEditors.GroupControl();
            this.clbReviewItems = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.chkAutoReview = new DevExpress.XtraEditors.CheckEdit();
            this.tabProviders = new DevExpress.XtraTab.XtraTabControl();
            this.tabClaude = new DevExpress.XtraTab.XtraTabPage();
            this.layoutClaude = new System.Windows.Forms.TableLayoutPanel();
            this.lblClaudeApiKey = new DevExpress.XtraEditors.LabelControl();
            this.txtClaudeApiKey = new DevExpress.XtraEditors.TextEdit();
            this.lblClaudeModel = new DevExpress.XtraEditors.LabelControl();
            this.txtClaudeModel = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblClaudeTokens = new DevExpress.XtraEditors.LabelControl();
            this.numClaudeTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblClaudeInputTokens = new DevExpress.XtraEditors.LabelControl();
            this.numClaudeInputTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblClaudeHint = new DevExpress.XtraEditors.LabelControl();
            this.tabOpenAi = new DevExpress.XtraTab.XtraTabPage();
            this.layoutOpenAi = new System.Windows.Forms.TableLayoutPanel();
            this.lblOpenAiApiKey = new DevExpress.XtraEditors.LabelControl();
            this.txtOpenAiApiKey = new DevExpress.XtraEditors.TextEdit();
            this.lblOpenAiModel = new DevExpress.XtraEditors.LabelControl();
            this.txtOpenAiModel = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblOpenAiTokens = new DevExpress.XtraEditors.LabelControl();
            this.numOpenAiTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblOpenAiInputTokens = new DevExpress.XtraEditors.LabelControl();
            this.numOpenAiInputTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblOpenAiTimeout = new DevExpress.XtraEditors.LabelControl();
            this.numOpenAiTimeout = new DevExpress.XtraEditors.SpinEdit();
            this.lblOpenAiEffort = new DevExpress.XtraEditors.LabelControl();
            this.cboOpenAiEffort = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblOpenAiHint = new DevExpress.XtraEditors.LabelControl();
            this.tabGemini = new DevExpress.XtraTab.XtraTabPage();
            this.layoutGemini = new System.Windows.Forms.TableLayoutPanel();
            this.lblGeminiApiKey = new DevExpress.XtraEditors.LabelControl();
            this.txtGeminiApiKey = new DevExpress.XtraEditors.TextEdit();
            this.lblGeminiModel = new DevExpress.XtraEditors.LabelControl();
            this.txtGeminiModel = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblGeminiTokens = new DevExpress.XtraEditors.LabelControl();
            this.numGeminiTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblGeminiInputTokens = new DevExpress.XtraEditors.LabelControl();
            this.numGeminiInputTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblGeminiHint = new DevExpress.XtraEditors.LabelControl();
            this.tabLocal = new DevExpress.XtraTab.XtraTabPage();
            this.layoutLocal = new System.Windows.Forms.TableLayoutPanel();
            this.lblLocalEndpoint = new DevExpress.XtraEditors.LabelControl();
            this.txtLocalEndpoint = new DevExpress.XtraEditors.TextEdit();
            this.lblLocalModel = new DevExpress.XtraEditors.LabelControl();
            this.txtLocalModel = new DevExpress.XtraEditors.TextEdit();
            this.lblLocalTokens = new DevExpress.XtraEditors.LabelControl();
            this.numLocalTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblLocalInputTokens = new DevExpress.XtraEditors.LabelControl();
            this.numLocalInputTokens = new DevExpress.XtraEditors.SpinEdit();
            this.lblLocalHint = new DevExpress.XtraEditors.LabelControl();
            this.layoutProviderHeader = new System.Windows.Forms.Panel();
            this.lblEditingProvider = new DevExpress.XtraEditors.LabelControl();
            this.btnRecommendedModel = new DevExpress.XtraEditors.SimpleButton();
            this.grpProvider = new DevExpress.XtraEditors.GroupControl();
            this.providerChoices = new System.Windows.Forms.FlowLayoutPanel();
            this.rbClaude = new System.Windows.Forms.RadioButton();
            this.rbOpenAi = new System.Windows.Forms.RadioButton();
            this.rbGemini = new System.Windows.Forms.RadioButton();
            this.rbLocal = new System.Windows.Forms.RadioButton();
            this.tabWorkingCopy = new DevExpress.XtraTab.XtraTabPage();
            this.layoutWorkingCopies = new System.Windows.Forms.Panel();
            this.gridWorkingCopies = new DevExpress.XtraGrid.GridControl();
            this.viewWorkingCopies = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPath = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUsername = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutWorkingCopyButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRemoveWC = new DevExpress.XtraEditors.SimpleButton();
            this.btnEditWC = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddWC = new DevExpress.XtraEditors.SimpleButton();
            this.lblWorkingCopiesHint = new DevExpress.XtraEditors.LabelControl();
            this.panelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabAi.SuspendLayout();
            this.layoutAi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpReviewItems)).BeginInit();
            this.grpReviewItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clbReviewItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoReview.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabProviders)).BeginInit();
            this.tabProviders.SuspendLayout();
            this.tabClaude.SuspendLayout();
            this.layoutClaude.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtClaudeApiKey.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtClaudeModel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClaudeTokens.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClaudeInputTokens.Properties)).BeginInit();
            this.tabOpenAi.SuspendLayout();
            this.layoutOpenAi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenAiApiKey.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenAiModel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpenAiTokens.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpenAiInputTokens.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpenAiTimeout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOpenAiEffort.Properties)).BeginInit();
            this.tabGemini.SuspendLayout();
            this.layoutGemini.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeminiApiKey.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeminiModel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGeminiTokens.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGeminiInputTokens.Properties)).BeginInit();
            this.tabLocal.SuspendLayout();
            this.layoutLocal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalEndpoint.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalModel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocalTokens.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocalInputTokens.Properties)).BeginInit();
            this.layoutProviderHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpProvider)).BeginInit();
            this.grpProvider.SuspendLayout();
            this.providerChoices.SuspendLayout();
            this.tabWorkingCopy.SuspendLayout();
            this.layoutWorkingCopies.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridWorkingCopies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewWorkingCopies)).BeginInit();
            this.layoutWorkingCopyButtons.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdvancedSettings
            // 
            this.btnAdvancedSettings.Appearance.BackColor = System.Drawing.Color.White;
            this.btnAdvancedSettings.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnAdvancedSettings.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnAdvancedSettings.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnAdvancedSettings.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAdvancedSettings.Appearance.Options.UseBackColor = true;
            this.btnAdvancedSettings.Appearance.Options.UseBorderColor = true;
            this.btnAdvancedSettings.Appearance.Options.UseFont = true;
            this.btnAdvancedSettings.Appearance.Options.UseForeColor = true;
            this.btnAdvancedSettings.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnAdvancedSettings.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnAdvancedSettings.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnAdvancedSettings.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnAdvancedSettings.AppearanceDisabled.Options.UseBackColor = true;
            this.btnAdvancedSettings.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnAdvancedSettings.AppearanceDisabled.Options.UseForeColor = true;
            this.btnAdvancedSettings.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnAdvancedSettings.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnAdvancedSettings.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnAdvancedSettings.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAdvancedSettings.AppearanceHovered.Options.UseBackColor = true;
            this.btnAdvancedSettings.AppearanceHovered.Options.UseBorderColor = true;
            this.btnAdvancedSettings.AppearanceHovered.Options.UseForeColor = true;
            this.btnAdvancedSettings.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnAdvancedSettings.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnAdvancedSettings.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnAdvancedSettings.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAdvancedSettings.AppearancePressed.Options.UseBackColor = true;
            this.btnAdvancedSettings.AppearancePressed.Options.UseBorderColor = true;
            this.btnAdvancedSettings.AppearancePressed.Options.UseForeColor = true;
            this.btnAdvancedSettings.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnAdvancedSettings.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAdvancedSettings.Location = new System.Drawing.Point(486, 3);
            this.btnAdvancedSettings.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnAdvancedSettings.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAdvancedSettings.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdvancedSettings.Name = "btnAdvancedSettings";
            this.btnAdvancedSettings.Size = new System.Drawing.Size(144, 28);
            this.btnAdvancedSettings.TabIndex = 1;
            this.btnAdvancedSettings.Text = "고급 설정 펼치기";
            this.btnAdvancedSettings.ToolTip = "토큰 한도·시간 제한·추론 강도를 표시합니다. 접어도 설정값은 유지됩니다.";
            // 
            // advancedHeaderGap
            // 
            this.advancedHeaderGap.Dock = System.Windows.Forms.DockStyle.Right;
            this.advancedHeaderGap.Font = new System.Drawing.Font("SUIT", 10F);
            this.advancedHeaderGap.Location = new System.Drawing.Point(630, 3);
            this.advancedHeaderGap.Name = "advancedHeaderGap";
            this.advancedHeaderGap.Size = new System.Drawing.Size(6, 28);
            this.advancedHeaderGap.TabIndex = 2;
            // 
            // layoutRoot
            // 
            this.layoutRoot.Controls.Add(this.tabControl);
            this.layoutRoot.Controls.Add(this.panelButtons);
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutRoot.Margin = new System.Windows.Forms.Padding(0);
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.Padding = new System.Windows.Forms.Padding(12);
            this.layoutRoot.Size = new System.Drawing.Size(900, 680);
            this.layoutRoot.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.AppearancePage.Header.Font = new System.Drawing.Font("SUIT", 10F);
            this.tabControl.AppearancePage.Header.Options.UseFont = true;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedTabPage = this.tabAi;
            this.tabControl.Size = new System.Drawing.Size(876, 616);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabAi,
            this.tabWorkingCopy});
            // 
            // tabAi
            // 
            this.tabAi.Controls.Add(this.layoutAi);
            this.tabAi.Margin = new System.Windows.Forms.Padding(0);
            this.tabAi.Name = "tabAi";
            this.tabAi.Padding = new System.Windows.Forms.Padding(8);
            this.tabAi.Size = new System.Drawing.Size(870, 583);
            this.tabAi.Text = "AI 검수 설정";
            // 
            // layoutAi
            // 
            this.layoutAi.Controls.Add(this.grpReviewItems);
            this.layoutAi.Controls.Add(this.chkAutoReview);
            this.layoutAi.Controls.Add(this.tabProviders);
            this.layoutAi.Controls.Add(this.layoutProviderHeader);
            this.layoutAi.Controls.Add(this.grpProvider);
            this.layoutAi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutAi.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutAi.Location = new System.Drawing.Point(8, 8);
            this.layoutAi.Margin = new System.Windows.Forms.Padding(0);
            this.layoutAi.Name = "layoutAi";
            this.layoutAi.Size = new System.Drawing.Size(854, 567);
            this.layoutAi.TabIndex = 0;
            // 
            // grpReviewItems
            // 
            this.grpReviewItems.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpReviewItems.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.grpReviewItems.AppearanceCaption.Options.UseFont = true;
            this.grpReviewItems.AppearanceCaption.Options.UseForeColor = true;
            this.grpReviewItems.Controls.Add(this.clbReviewItems);
            this.grpReviewItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpReviewItems.Location = new System.Drawing.Point(0, 284);
            this.grpReviewItems.Margin = new System.Windows.Forms.Padding(0);
            this.grpReviewItems.Name = "grpReviewItems";
            this.grpReviewItems.Padding = new System.Windows.Forms.Padding(8);
            this.grpReviewItems.Size = new System.Drawing.Size(854, 283);
            this.grpReviewItems.TabIndex = 0;
            this.grpReviewItems.Text = "검수 항목";
            // 
            // clbReviewItems
            // 
            this.clbReviewItems.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.clbReviewItems.Appearance.Options.UseFont = true;
            this.clbReviewItems.CheckOnClick = true;
            this.clbReviewItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbReviewItems.Location = new System.Drawing.Point(10, 31);
            this.clbReviewItems.Margin = new System.Windows.Forms.Padding(0);
            this.clbReviewItems.Name = "clbReviewItems";
            this.clbReviewItems.Size = new System.Drawing.Size(834, 242);
            this.clbReviewItems.TabIndex = 0;
            // 
            // chkAutoReview
            // 
            this.chkAutoReview.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAutoReview.Location = new System.Drawing.Point(0, 250);
            this.chkAutoReview.Margin = new System.Windows.Forms.Padding(0);
            this.chkAutoReview.Name = "chkAutoReview";
            this.chkAutoReview.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.chkAutoReview.Properties.Appearance.Options.UseFont = true;
            this.chkAutoReview.Properties.AutoHeight = false;
            this.chkAutoReview.Properties.Caption = "커밋 전 AI 검수를 자동으로 실행";
            this.chkAutoReview.Size = new System.Drawing.Size(854, 34);
            this.chkAutoReview.TabIndex = 1;
            // 
            // tabProviders
            // 
            this.tabProviders.AppearancePage.Header.Font = new System.Drawing.Font("SUIT", 10F);
            this.tabProviders.AppearancePage.Header.Options.UseFont = true;
            this.tabProviders.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabProviders.Location = new System.Drawing.Point(0, 98);
            this.tabProviders.Margin = new System.Windows.Forms.Padding(0);
            this.tabProviders.Name = "tabProviders";
            this.tabProviders.Size = new System.Drawing.Size(854, 152);
            this.tabProviders.TabIndex = 2;
            this.tabProviders.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabClaude,
            this.tabOpenAi,
            this.tabGemini,
            this.tabLocal});
            // 
            // tabClaude
            // 
            this.tabClaude.Controls.Add(this.layoutClaude);
            this.tabClaude.Margin = new System.Windows.Forms.Padding(0);
            this.tabClaude.Name = "tabClaude";
            this.tabClaude.Padding = new System.Windows.Forms.Padding(8);
            this.tabClaude.Size = new System.Drawing.Size(848, 121);
            this.tabClaude.Text = "Claude";
            // 
            // layoutClaude
            // 
            this.layoutClaude.ColumnCount = 4;
            this.layoutClaude.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutClaude.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutClaude.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutClaude.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutClaude.Controls.Add(this.lblClaudeApiKey, 0, 0);
            this.layoutClaude.Controls.Add(this.txtClaudeApiKey, 1, 0);
            this.layoutClaude.Controls.Add(this.lblClaudeModel, 0, 1);
            this.layoutClaude.Controls.Add(this.txtClaudeModel, 1, 1);
            this.layoutClaude.Controls.Add(this.lblClaudeTokens, 0, 2);
            this.layoutClaude.Controls.Add(this.numClaudeTokens, 1, 2);
            this.layoutClaude.Controls.Add(this.lblClaudeInputTokens, 2, 2);
            this.layoutClaude.Controls.Add(this.numClaudeInputTokens, 3, 2);
            this.layoutClaude.Controls.Add(this.lblClaudeHint, 0, 4);
            this.layoutClaude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutClaude.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutClaude.Location = new System.Drawing.Point(8, 8);
            this.layoutClaude.Margin = new System.Windows.Forms.Padding(0);
            this.layoutClaude.Name = "layoutClaude";
            this.layoutClaude.RowCount = 5;
            this.layoutClaude.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutClaude.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutClaude.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutClaude.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutClaude.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutClaude.Size = new System.Drawing.Size(832, 105);
            this.layoutClaude.TabIndex = 0;
            // 
            // lblClaudeApiKey
            // 
            this.lblClaudeApiKey.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblClaudeApiKey.Appearance.Options.UseFont = true;
            this.lblClaudeApiKey.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblClaudeApiKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClaudeApiKey.Location = new System.Drawing.Point(0, 3);
            this.lblClaudeApiKey.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblClaudeApiKey.Name = "lblClaudeApiKey";
            this.lblClaudeApiKey.Size = new System.Drawing.Size(114, 28);
            this.lblClaudeApiKey.TabIndex = 0;
            this.lblClaudeApiKey.Text = "API 키";
            // 
            // txtClaudeApiKey
            // 
            this.layoutClaude.SetColumnSpan(this.txtClaudeApiKey, 3);
            this.txtClaudeApiKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtClaudeApiKey.Location = new System.Drawing.Point(120, 3);
            this.txtClaudeApiKey.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtClaudeApiKey.Name = "txtClaudeApiKey";
            this.txtClaudeApiKey.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtClaudeApiKey.Properties.Appearance.Options.UseFont = true;
            this.txtClaudeApiKey.Properties.AutoHeight = false;
            this.txtClaudeApiKey.Properties.PasswordChar = '*';
            this.txtClaudeApiKey.Size = new System.Drawing.Size(706, 28);
            this.txtClaudeApiKey.TabIndex = 1;
            // 
            // lblClaudeModel
            // 
            this.lblClaudeModel.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblClaudeModel.Appearance.Options.UseFont = true;
            this.lblClaudeModel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblClaudeModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClaudeModel.Location = new System.Drawing.Point(0, 37);
            this.lblClaudeModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblClaudeModel.Name = "lblClaudeModel";
            this.lblClaudeModel.Size = new System.Drawing.Size(114, 28);
            this.lblClaudeModel.TabIndex = 2;
            this.lblClaudeModel.Text = "모델";
            // 
            // txtClaudeModel
            // 
            this.layoutClaude.SetColumnSpan(this.txtClaudeModel, 3);
            this.txtClaudeModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtClaudeModel.Location = new System.Drawing.Point(120, 37);
            this.txtClaudeModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtClaudeModel.Name = "txtClaudeModel";
            this.txtClaudeModel.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtClaudeModel.Properties.Appearance.Options.UseFont = true;
            this.txtClaudeModel.Properties.AppearanceDropDown.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtClaudeModel.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txtClaudeModel.Properties.AutoHeight = false;
            this.txtClaudeModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtClaudeModel.Size = new System.Drawing.Size(706, 28);
            this.txtClaudeModel.TabIndex = 3;
            // 
            // lblClaudeTokens
            // 
            this.lblClaudeTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblClaudeTokens.Appearance.Options.UseFont = true;
            this.lblClaudeTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblClaudeTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClaudeTokens.Location = new System.Drawing.Point(0, 71);
            this.lblClaudeTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblClaudeTokens.Name = "lblClaudeTokens";
            this.lblClaudeTokens.Size = new System.Drawing.Size(114, 1);
            this.lblClaudeTokens.TabIndex = 4;
            this.lblClaudeTokens.Text = "출력 토큰 한도";
            this.lblClaudeTokens.Visible = false;
            // 
            // numClaudeTokens
            // 
            this.numClaudeTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numClaudeTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numClaudeTokens.Location = new System.Drawing.Point(120, 71);
            this.numClaudeTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numClaudeTokens.Name = "numClaudeTokens";
            this.numClaudeTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numClaudeTokens.Properties.Appearance.Options.UseFont = true;
            this.numClaudeTokens.Properties.AutoHeight = false;
            this.numClaudeTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numClaudeTokens.Properties.IsFloatValue = false;
            this.numClaudeTokens.Properties.MaxValue = new decimal(new int[] {
            128000,
            0,
            0,
            0});
            this.numClaudeTokens.Properties.MinValue = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numClaudeTokens.Size = new System.Drawing.Size(290, 1);
            this.numClaudeTokens.TabIndex = 5;
            this.numClaudeTokens.Visible = false;
            // 
            // lblClaudeInputTokens
            // 
            this.lblClaudeInputTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblClaudeInputTokens.Appearance.Options.UseFont = true;
            this.lblClaudeInputTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblClaudeInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClaudeInputTokens.Location = new System.Drawing.Point(416, 71);
            this.lblClaudeInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblClaudeInputTokens.Name = "lblClaudeInputTokens";
            this.lblClaudeInputTokens.Size = new System.Drawing.Size(114, 1);
            this.lblClaudeInputTokens.TabIndex = 6;
            this.lblClaudeInputTokens.Text = "입력 토큰 한도";
            this.lblClaudeInputTokens.Visible = false;
            // 
            // numClaudeInputTokens
            // 
            this.numClaudeInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numClaudeInputTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numClaudeInputTokens.Location = new System.Drawing.Point(536, 71);
            this.numClaudeInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numClaudeInputTokens.Name = "numClaudeInputTokens";
            this.numClaudeInputTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numClaudeInputTokens.Properties.Appearance.Options.UseFont = true;
            this.numClaudeInputTokens.Properties.AutoHeight = false;
            this.numClaudeInputTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numClaudeInputTokens.Properties.IsFloatValue = false;
            this.numClaudeInputTokens.Properties.MaxValue = new decimal(new int[] {
            2000000,
            0,
            0,
            0});
            this.numClaudeInputTokens.Properties.MinValue = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numClaudeInputTokens.Size = new System.Drawing.Size(290, 1);
            this.numClaudeInputTokens.TabIndex = 7;
            this.numClaudeInputTokens.Visible = false;
            // 
            // lblClaudeHint
            // 
            this.lblClaudeHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblClaudeHint.Appearance.Options.UseFont = true;
            this.lblClaudeHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.layoutClaude.SetColumnSpan(this.lblClaudeHint, 4);
            this.lblClaudeHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClaudeHint.Location = new System.Drawing.Point(0, 71);
            this.lblClaudeHint.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblClaudeHint.Name = "lblClaudeHint";
            this.lblClaudeHint.Size = new System.Drawing.Size(826, 31);
            this.lblClaudeHint.TabIndex = 8;
            this.lblClaudeHint.Text = "모델명을 직접 입력할 수 있습니다. 권장 모델 적용 시 토큰 한도도 변경됩니다.";
            // 
            // tabOpenAi
            // 
            this.tabOpenAi.Controls.Add(this.layoutOpenAi);
            this.tabOpenAi.Margin = new System.Windows.Forms.Padding(0);
            this.tabOpenAi.Name = "tabOpenAi";
            this.tabOpenAi.Padding = new System.Windows.Forms.Padding(8);
            this.tabOpenAi.Size = new System.Drawing.Size(848, 121);
            this.tabOpenAi.Text = "OpenAI";
            // 
            // layoutOpenAi
            // 
            this.layoutOpenAi.ColumnCount = 4;
            this.layoutOpenAi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutOpenAi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutOpenAi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutOpenAi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutOpenAi.Controls.Add(this.lblOpenAiApiKey, 0, 0);
            this.layoutOpenAi.Controls.Add(this.txtOpenAiApiKey, 1, 0);
            this.layoutOpenAi.Controls.Add(this.lblOpenAiModel, 0, 1);
            this.layoutOpenAi.Controls.Add(this.txtOpenAiModel, 1, 1);
            this.layoutOpenAi.Controls.Add(this.lblOpenAiTokens, 0, 2);
            this.layoutOpenAi.Controls.Add(this.numOpenAiTokens, 1, 2);
            this.layoutOpenAi.Controls.Add(this.lblOpenAiInputTokens, 2, 2);
            this.layoutOpenAi.Controls.Add(this.numOpenAiInputTokens, 3, 2);
            this.layoutOpenAi.Controls.Add(this.lblOpenAiTimeout, 0, 3);
            this.layoutOpenAi.Controls.Add(this.numOpenAiTimeout, 1, 3);
            this.layoutOpenAi.Controls.Add(this.lblOpenAiEffort, 2, 3);
            this.layoutOpenAi.Controls.Add(this.cboOpenAiEffort, 3, 3);
            this.layoutOpenAi.Controls.Add(this.lblOpenAiHint, 0, 4);
            this.layoutOpenAi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutOpenAi.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutOpenAi.Location = new System.Drawing.Point(8, 8);
            this.layoutOpenAi.Margin = new System.Windows.Forms.Padding(0);
            this.layoutOpenAi.Name = "layoutOpenAi";
            this.layoutOpenAi.RowCount = 5;
            this.layoutOpenAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutOpenAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutOpenAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutOpenAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutOpenAi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutOpenAi.Size = new System.Drawing.Size(832, 105);
            this.layoutOpenAi.TabIndex = 0;
            // 
            // lblOpenAiApiKey
            // 
            this.lblOpenAiApiKey.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblOpenAiApiKey.Appearance.Options.UseFont = true;
            this.lblOpenAiApiKey.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOpenAiApiKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenAiApiKey.Location = new System.Drawing.Point(0, 3);
            this.lblOpenAiApiKey.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblOpenAiApiKey.Name = "lblOpenAiApiKey";
            this.lblOpenAiApiKey.Size = new System.Drawing.Size(114, 28);
            this.lblOpenAiApiKey.TabIndex = 0;
            this.lblOpenAiApiKey.Text = "API 키";
            // 
            // txtOpenAiApiKey
            // 
            this.layoutOpenAi.SetColumnSpan(this.txtOpenAiApiKey, 3);
            this.txtOpenAiApiKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOpenAiApiKey.Location = new System.Drawing.Point(120, 3);
            this.txtOpenAiApiKey.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtOpenAiApiKey.Name = "txtOpenAiApiKey";
            this.txtOpenAiApiKey.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtOpenAiApiKey.Properties.Appearance.Options.UseFont = true;
            this.txtOpenAiApiKey.Properties.AutoHeight = false;
            this.txtOpenAiApiKey.Properties.PasswordChar = '*';
            this.txtOpenAiApiKey.Size = new System.Drawing.Size(706, 28);
            this.txtOpenAiApiKey.TabIndex = 1;
            // 
            // lblOpenAiModel
            // 
            this.lblOpenAiModel.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblOpenAiModel.Appearance.Options.UseFont = true;
            this.lblOpenAiModel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOpenAiModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenAiModel.Location = new System.Drawing.Point(0, 37);
            this.lblOpenAiModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblOpenAiModel.Name = "lblOpenAiModel";
            this.lblOpenAiModel.Size = new System.Drawing.Size(114, 28);
            this.lblOpenAiModel.TabIndex = 2;
            this.lblOpenAiModel.Text = "모델";
            // 
            // txtOpenAiModel
            // 
            this.layoutOpenAi.SetColumnSpan(this.txtOpenAiModel, 3);
            this.txtOpenAiModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOpenAiModel.Location = new System.Drawing.Point(120, 37);
            this.txtOpenAiModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtOpenAiModel.Name = "txtOpenAiModel";
            this.txtOpenAiModel.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtOpenAiModel.Properties.Appearance.Options.UseFont = true;
            this.txtOpenAiModel.Properties.AppearanceDropDown.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtOpenAiModel.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txtOpenAiModel.Properties.AutoHeight = false;
            this.txtOpenAiModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtOpenAiModel.Size = new System.Drawing.Size(706, 28);
            this.txtOpenAiModel.TabIndex = 3;
            // 
            // lblOpenAiTokens
            // 
            this.lblOpenAiTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblOpenAiTokens.Appearance.Options.UseFont = true;
            this.lblOpenAiTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOpenAiTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenAiTokens.Location = new System.Drawing.Point(0, 71);
            this.lblOpenAiTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblOpenAiTokens.Name = "lblOpenAiTokens";
            this.lblOpenAiTokens.Size = new System.Drawing.Size(114, 1);
            this.lblOpenAiTokens.TabIndex = 4;
            this.lblOpenAiTokens.Text = "출력 토큰 한도";
            this.lblOpenAiTokens.Visible = false;
            // 
            // numOpenAiTokens
            // 
            this.numOpenAiTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numOpenAiTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numOpenAiTokens.Location = new System.Drawing.Point(120, 71);
            this.numOpenAiTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numOpenAiTokens.Name = "numOpenAiTokens";
            this.numOpenAiTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numOpenAiTokens.Properties.Appearance.Options.UseFont = true;
            this.numOpenAiTokens.Properties.AutoHeight = false;
            this.numOpenAiTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numOpenAiTokens.Properties.IsFloatValue = false;
            this.numOpenAiTokens.Properties.MaxValue = new decimal(new int[] {
            128000,
            0,
            0,
            0});
            this.numOpenAiTokens.Properties.MinValue = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numOpenAiTokens.Size = new System.Drawing.Size(290, 1);
            this.numOpenAiTokens.TabIndex = 5;
            this.numOpenAiTokens.Visible = false;
            // 
            // lblOpenAiInputTokens
            // 
            this.lblOpenAiInputTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblOpenAiInputTokens.Appearance.Options.UseFont = true;
            this.lblOpenAiInputTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOpenAiInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenAiInputTokens.Location = new System.Drawing.Point(416, 71);
            this.lblOpenAiInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblOpenAiInputTokens.Name = "lblOpenAiInputTokens";
            this.lblOpenAiInputTokens.Size = new System.Drawing.Size(114, 1);
            this.lblOpenAiInputTokens.TabIndex = 6;
            this.lblOpenAiInputTokens.Text = "입력 토큰 한도";
            this.lblOpenAiInputTokens.Visible = false;
            // 
            // numOpenAiInputTokens
            // 
            this.numOpenAiInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numOpenAiInputTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numOpenAiInputTokens.Location = new System.Drawing.Point(536, 71);
            this.numOpenAiInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numOpenAiInputTokens.Name = "numOpenAiInputTokens";
            this.numOpenAiInputTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numOpenAiInputTokens.Properties.Appearance.Options.UseFont = true;
            this.numOpenAiInputTokens.Properties.AutoHeight = false;
            this.numOpenAiInputTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numOpenAiInputTokens.Properties.IsFloatValue = false;
            this.numOpenAiInputTokens.Properties.MaxValue = new decimal(new int[] {
            2000000,
            0,
            0,
            0});
            this.numOpenAiInputTokens.Properties.MinValue = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numOpenAiInputTokens.Size = new System.Drawing.Size(290, 1);
            this.numOpenAiInputTokens.TabIndex = 7;
            this.numOpenAiInputTokens.Visible = false;
            // 
            // lblOpenAiTimeout
            // 
            this.lblOpenAiTimeout.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblOpenAiTimeout.Appearance.Options.UseFont = true;
            this.lblOpenAiTimeout.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOpenAiTimeout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenAiTimeout.Location = new System.Drawing.Point(0, 71);
            this.lblOpenAiTimeout.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblOpenAiTimeout.Name = "lblOpenAiTimeout";
            this.lblOpenAiTimeout.Size = new System.Drawing.Size(114, 1);
            this.lblOpenAiTimeout.TabIndex = 8;
            this.lblOpenAiTimeout.Text = "요청 제한 시간(초)";
            this.lblOpenAiTimeout.Visible = false;
            // 
            // numOpenAiTimeout
            // 
            this.numOpenAiTimeout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numOpenAiTimeout.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numOpenAiTimeout.Location = new System.Drawing.Point(120, 71);
            this.numOpenAiTimeout.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numOpenAiTimeout.Name = "numOpenAiTimeout";
            this.numOpenAiTimeout.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numOpenAiTimeout.Properties.Appearance.Options.UseFont = true;
            this.numOpenAiTimeout.Properties.AutoHeight = false;
            this.numOpenAiTimeout.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numOpenAiTimeout.Properties.IsFloatValue = false;
            this.numOpenAiTimeout.Properties.MaxValue = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numOpenAiTimeout.Properties.MinValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numOpenAiTimeout.Size = new System.Drawing.Size(290, 1);
            this.numOpenAiTimeout.TabIndex = 9;
            this.numOpenAiTimeout.Visible = false;
            // 
            // lblOpenAiEffort
            // 
            this.lblOpenAiEffort.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblOpenAiEffort.Appearance.Options.UseFont = true;
            this.lblOpenAiEffort.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOpenAiEffort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenAiEffort.Location = new System.Drawing.Point(416, 71);
            this.lblOpenAiEffort.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblOpenAiEffort.Name = "lblOpenAiEffort";
            this.lblOpenAiEffort.Size = new System.Drawing.Size(114, 1);
            this.lblOpenAiEffort.TabIndex = 10;
            this.lblOpenAiEffort.Text = "추론 강도";
            this.lblOpenAiEffort.Visible = false;
            // 
            // cboOpenAiEffort
            // 
            this.cboOpenAiEffort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboOpenAiEffort.Location = new System.Drawing.Point(536, 71);
            this.cboOpenAiEffort.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.cboOpenAiEffort.Name = "cboOpenAiEffort";
            this.cboOpenAiEffort.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.cboOpenAiEffort.Properties.Appearance.Options.UseFont = true;
            this.cboOpenAiEffort.Properties.AppearanceDropDown.Font = new System.Drawing.Font("SUIT", 10F);
            this.cboOpenAiEffort.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cboOpenAiEffort.Properties.AutoHeight = false;
            this.cboOpenAiEffort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboOpenAiEffort.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboOpenAiEffort.Size = new System.Drawing.Size(290, 1);
            this.cboOpenAiEffort.TabIndex = 11;
            this.cboOpenAiEffort.Visible = false;
            // 
            // lblOpenAiHint
            // 
            this.lblOpenAiHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblOpenAiHint.Appearance.Options.UseFont = true;
            this.lblOpenAiHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.layoutOpenAi.SetColumnSpan(this.lblOpenAiHint, 4);
            this.lblOpenAiHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenAiHint.Location = new System.Drawing.Point(0, 71);
            this.lblOpenAiHint.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblOpenAiHint.Name = "lblOpenAiHint";
            this.lblOpenAiHint.Size = new System.Drawing.Size(826, 31);
            this.lblOpenAiHint.TabIndex = 12;
            this.lblOpenAiHint.Text = "모델명을 직접 입력할 수 있습니다. 권장 모델 적용 시 토큰 한도도 변경됩니다.";
            // 
            // tabGemini
            // 
            this.tabGemini.Controls.Add(this.layoutGemini);
            this.tabGemini.Margin = new System.Windows.Forms.Padding(0);
            this.tabGemini.Name = "tabGemini";
            this.tabGemini.Padding = new System.Windows.Forms.Padding(8);
            this.tabGemini.Size = new System.Drawing.Size(848, 121);
            this.tabGemini.Text = "Gemini";
            // 
            // layoutGemini
            // 
            this.layoutGemini.ColumnCount = 4;
            this.layoutGemini.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutGemini.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutGemini.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutGemini.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutGemini.Controls.Add(this.lblGeminiApiKey, 0, 0);
            this.layoutGemini.Controls.Add(this.txtGeminiApiKey, 1, 0);
            this.layoutGemini.Controls.Add(this.lblGeminiModel, 0, 1);
            this.layoutGemini.Controls.Add(this.txtGeminiModel, 1, 1);
            this.layoutGemini.Controls.Add(this.lblGeminiTokens, 0, 2);
            this.layoutGemini.Controls.Add(this.numGeminiTokens, 1, 2);
            this.layoutGemini.Controls.Add(this.lblGeminiInputTokens, 2, 2);
            this.layoutGemini.Controls.Add(this.numGeminiInputTokens, 3, 2);
            this.layoutGemini.Controls.Add(this.lblGeminiHint, 0, 4);
            this.layoutGemini.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutGemini.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutGemini.Location = new System.Drawing.Point(8, 8);
            this.layoutGemini.Margin = new System.Windows.Forms.Padding(0);
            this.layoutGemini.Name = "layoutGemini";
            this.layoutGemini.RowCount = 5;
            this.layoutGemini.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutGemini.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutGemini.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutGemini.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutGemini.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutGemini.Size = new System.Drawing.Size(832, 105);
            this.layoutGemini.TabIndex = 0;
            // 
            // lblGeminiApiKey
            // 
            this.lblGeminiApiKey.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblGeminiApiKey.Appearance.Options.UseFont = true;
            this.lblGeminiApiKey.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGeminiApiKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGeminiApiKey.Location = new System.Drawing.Point(0, 3);
            this.lblGeminiApiKey.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblGeminiApiKey.Name = "lblGeminiApiKey";
            this.lblGeminiApiKey.Size = new System.Drawing.Size(114, 28);
            this.lblGeminiApiKey.TabIndex = 0;
            this.lblGeminiApiKey.Text = "API 키";
            // 
            // txtGeminiApiKey
            // 
            this.layoutGemini.SetColumnSpan(this.txtGeminiApiKey, 3);
            this.txtGeminiApiKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGeminiApiKey.Location = new System.Drawing.Point(120, 3);
            this.txtGeminiApiKey.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtGeminiApiKey.Name = "txtGeminiApiKey";
            this.txtGeminiApiKey.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtGeminiApiKey.Properties.Appearance.Options.UseFont = true;
            this.txtGeminiApiKey.Properties.AutoHeight = false;
            this.txtGeminiApiKey.Properties.PasswordChar = '*';
            this.txtGeminiApiKey.Size = new System.Drawing.Size(706, 28);
            this.txtGeminiApiKey.TabIndex = 1;
            // 
            // lblGeminiModel
            // 
            this.lblGeminiModel.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblGeminiModel.Appearance.Options.UseFont = true;
            this.lblGeminiModel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGeminiModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGeminiModel.Location = new System.Drawing.Point(0, 37);
            this.lblGeminiModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblGeminiModel.Name = "lblGeminiModel";
            this.lblGeminiModel.Size = new System.Drawing.Size(114, 28);
            this.lblGeminiModel.TabIndex = 2;
            this.lblGeminiModel.Text = "모델";
            // 
            // txtGeminiModel
            // 
            this.layoutGemini.SetColumnSpan(this.txtGeminiModel, 3);
            this.txtGeminiModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGeminiModel.Location = new System.Drawing.Point(120, 37);
            this.txtGeminiModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtGeminiModel.Name = "txtGeminiModel";
            this.txtGeminiModel.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtGeminiModel.Properties.Appearance.Options.UseFont = true;
            this.txtGeminiModel.Properties.AppearanceDropDown.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtGeminiModel.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txtGeminiModel.Properties.AutoHeight = false;
            this.txtGeminiModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtGeminiModel.Size = new System.Drawing.Size(706, 28);
            this.txtGeminiModel.TabIndex = 3;
            // 
            // lblGeminiTokens
            // 
            this.lblGeminiTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblGeminiTokens.Appearance.Options.UseFont = true;
            this.lblGeminiTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGeminiTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGeminiTokens.Location = new System.Drawing.Point(0, 71);
            this.lblGeminiTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblGeminiTokens.Name = "lblGeminiTokens";
            this.lblGeminiTokens.Size = new System.Drawing.Size(114, 1);
            this.lblGeminiTokens.TabIndex = 4;
            this.lblGeminiTokens.Text = "출력 토큰 한도";
            this.lblGeminiTokens.Visible = false;
            // 
            // numGeminiTokens
            // 
            this.numGeminiTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numGeminiTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numGeminiTokens.Location = new System.Drawing.Point(120, 71);
            this.numGeminiTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numGeminiTokens.Name = "numGeminiTokens";
            this.numGeminiTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numGeminiTokens.Properties.Appearance.Options.UseFont = true;
            this.numGeminiTokens.Properties.AutoHeight = false;
            this.numGeminiTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numGeminiTokens.Properties.IsFloatValue = false;
            this.numGeminiTokens.Properties.MaxValue = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numGeminiTokens.Properties.MinValue = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numGeminiTokens.Size = new System.Drawing.Size(290, 1);
            this.numGeminiTokens.TabIndex = 5;
            this.numGeminiTokens.Visible = false;
            // 
            // lblGeminiInputTokens
            // 
            this.lblGeminiInputTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblGeminiInputTokens.Appearance.Options.UseFont = true;
            this.lblGeminiInputTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGeminiInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGeminiInputTokens.Location = new System.Drawing.Point(416, 71);
            this.lblGeminiInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblGeminiInputTokens.Name = "lblGeminiInputTokens";
            this.lblGeminiInputTokens.Size = new System.Drawing.Size(114, 1);
            this.lblGeminiInputTokens.TabIndex = 6;
            this.lblGeminiInputTokens.Text = "입력 토큰 한도";
            this.lblGeminiInputTokens.Visible = false;
            // 
            // numGeminiInputTokens
            // 
            this.numGeminiInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numGeminiInputTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numGeminiInputTokens.Location = new System.Drawing.Point(536, 71);
            this.numGeminiInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numGeminiInputTokens.Name = "numGeminiInputTokens";
            this.numGeminiInputTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numGeminiInputTokens.Properties.Appearance.Options.UseFont = true;
            this.numGeminiInputTokens.Properties.AutoHeight = false;
            this.numGeminiInputTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numGeminiInputTokens.Properties.IsFloatValue = false;
            this.numGeminiInputTokens.Properties.MaxValue = new decimal(new int[] {
            2000000,
            0,
            0,
            0});
            this.numGeminiInputTokens.Properties.MinValue = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numGeminiInputTokens.Size = new System.Drawing.Size(290, 1);
            this.numGeminiInputTokens.TabIndex = 7;
            this.numGeminiInputTokens.Visible = false;
            // 
            // lblGeminiHint
            // 
            this.lblGeminiHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblGeminiHint.Appearance.Options.UseFont = true;
            this.lblGeminiHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.layoutGemini.SetColumnSpan(this.lblGeminiHint, 4);
            this.lblGeminiHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGeminiHint.Location = new System.Drawing.Point(0, 71);
            this.lblGeminiHint.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblGeminiHint.Name = "lblGeminiHint";
            this.lblGeminiHint.Size = new System.Drawing.Size(826, 31);
            this.lblGeminiHint.TabIndex = 8;
            this.lblGeminiHint.Text = "모델명을 직접 입력할 수 있습니다. 권장 모델 적용 시 토큰 한도도 변경됩니다.";
            // 
            // tabLocal
            // 
            this.tabLocal.Controls.Add(this.layoutLocal);
            this.tabLocal.Margin = new System.Windows.Forms.Padding(0);
            this.tabLocal.Name = "tabLocal";
            this.tabLocal.Padding = new System.Windows.Forms.Padding(8);
            this.tabLocal.Size = new System.Drawing.Size(848, 121);
            this.tabLocal.Text = "Local";
            // 
            // layoutLocal
            // 
            this.layoutLocal.ColumnCount = 4;
            this.layoutLocal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutLocal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutLocal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layoutLocal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutLocal.Controls.Add(this.lblLocalEndpoint, 0, 0);
            this.layoutLocal.Controls.Add(this.txtLocalEndpoint, 1, 0);
            this.layoutLocal.Controls.Add(this.lblLocalModel, 0, 1);
            this.layoutLocal.Controls.Add(this.txtLocalModel, 1, 1);
            this.layoutLocal.Controls.Add(this.lblLocalTokens, 0, 2);
            this.layoutLocal.Controls.Add(this.numLocalTokens, 1, 2);
            this.layoutLocal.Controls.Add(this.lblLocalInputTokens, 2, 2);
            this.layoutLocal.Controls.Add(this.numLocalInputTokens, 3, 2);
            this.layoutLocal.Controls.Add(this.lblLocalHint, 0, 4);
            this.layoutLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutLocal.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutLocal.Location = new System.Drawing.Point(8, 8);
            this.layoutLocal.Margin = new System.Windows.Forms.Padding(0);
            this.layoutLocal.Name = "layoutLocal";
            this.layoutLocal.RowCount = 5;
            this.layoutLocal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutLocal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutLocal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutLocal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.layoutLocal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutLocal.Size = new System.Drawing.Size(832, 105);
            this.layoutLocal.TabIndex = 0;
            // 
            // lblLocalEndpoint
            // 
            this.lblLocalEndpoint.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblLocalEndpoint.Appearance.Options.UseFont = true;
            this.lblLocalEndpoint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLocalEndpoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLocalEndpoint.Location = new System.Drawing.Point(0, 3);
            this.lblLocalEndpoint.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblLocalEndpoint.Name = "lblLocalEndpoint";
            this.lblLocalEndpoint.Size = new System.Drawing.Size(114, 28);
            this.lblLocalEndpoint.TabIndex = 0;
            this.lblLocalEndpoint.Text = "서버 주소";
            // 
            // txtLocalEndpoint
            // 
            this.layoutLocal.SetColumnSpan(this.txtLocalEndpoint, 3);
            this.txtLocalEndpoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLocalEndpoint.Location = new System.Drawing.Point(120, 3);
            this.txtLocalEndpoint.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtLocalEndpoint.Name = "txtLocalEndpoint";
            this.txtLocalEndpoint.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtLocalEndpoint.Properties.Appearance.Options.UseFont = true;
            this.txtLocalEndpoint.Properties.AutoHeight = false;
            this.txtLocalEndpoint.Size = new System.Drawing.Size(706, 28);
            this.txtLocalEndpoint.TabIndex = 1;
            // 
            // lblLocalModel
            // 
            this.lblLocalModel.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblLocalModel.Appearance.Options.UseFont = true;
            this.lblLocalModel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLocalModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLocalModel.Location = new System.Drawing.Point(0, 37);
            this.lblLocalModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblLocalModel.Name = "lblLocalModel";
            this.lblLocalModel.Size = new System.Drawing.Size(114, 28);
            this.lblLocalModel.TabIndex = 2;
            this.lblLocalModel.Text = "모델";
            // 
            // txtLocalModel
            // 
            this.layoutLocal.SetColumnSpan(this.txtLocalModel, 3);
            this.txtLocalModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLocalModel.Location = new System.Drawing.Point(120, 37);
            this.txtLocalModel.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.txtLocalModel.Name = "txtLocalModel";
            this.txtLocalModel.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtLocalModel.Properties.Appearance.Options.UseFont = true;
            this.txtLocalModel.Properties.AutoHeight = false;
            this.txtLocalModel.Size = new System.Drawing.Size(706, 28);
            this.txtLocalModel.TabIndex = 3;
            // 
            // lblLocalTokens
            // 
            this.lblLocalTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblLocalTokens.Appearance.Options.UseFont = true;
            this.lblLocalTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLocalTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLocalTokens.Location = new System.Drawing.Point(0, 71);
            this.lblLocalTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblLocalTokens.Name = "lblLocalTokens";
            this.lblLocalTokens.Size = new System.Drawing.Size(114, 1);
            this.lblLocalTokens.TabIndex = 4;
            this.lblLocalTokens.Text = "출력 토큰 한도";
            this.lblLocalTokens.Visible = false;
            // 
            // numLocalTokens
            // 
            this.numLocalTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numLocalTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numLocalTokens.Location = new System.Drawing.Point(120, 71);
            this.numLocalTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numLocalTokens.Name = "numLocalTokens";
            this.numLocalTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numLocalTokens.Properties.Appearance.Options.UseFont = true;
            this.numLocalTokens.Properties.AutoHeight = false;
            this.numLocalTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numLocalTokens.Properties.IsFloatValue = false;
            this.numLocalTokens.Properties.MaxValue = new decimal(new int[] {
            32000,
            0,
            0,
            0});
            this.numLocalTokens.Properties.MinValue = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numLocalTokens.Size = new System.Drawing.Size(290, 1);
            this.numLocalTokens.TabIndex = 5;
            this.numLocalTokens.Visible = false;
            // 
            // lblLocalInputTokens
            // 
            this.lblLocalInputTokens.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblLocalInputTokens.Appearance.Options.UseFont = true;
            this.lblLocalInputTokens.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLocalInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLocalInputTokens.Location = new System.Drawing.Point(416, 71);
            this.lblLocalInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblLocalInputTokens.Name = "lblLocalInputTokens";
            this.lblLocalInputTokens.Size = new System.Drawing.Size(114, 1);
            this.lblLocalInputTokens.TabIndex = 6;
            this.lblLocalInputTokens.Text = "입력 토큰 한도";
            this.lblLocalInputTokens.Visible = false;
            // 
            // numLocalInputTokens
            // 
            this.numLocalInputTokens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numLocalInputTokens.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numLocalInputTokens.Location = new System.Drawing.Point(536, 71);
            this.numLocalInputTokens.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.numLocalInputTokens.Name = "numLocalInputTokens";
            this.numLocalInputTokens.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numLocalInputTokens.Properties.Appearance.Options.UseFont = true;
            this.numLocalInputTokens.Properties.AutoHeight = false;
            this.numLocalInputTokens.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numLocalInputTokens.Properties.IsFloatValue = false;
            this.numLocalInputTokens.Properties.MaxValue = new decimal(new int[] {
            2000000,
            0,
            0,
            0});
            this.numLocalInputTokens.Properties.MinValue = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numLocalInputTokens.Size = new System.Drawing.Size(290, 1);
            this.numLocalInputTokens.TabIndex = 7;
            this.numLocalInputTokens.Visible = false;
            // 
            // lblLocalHint
            // 
            this.lblLocalHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblLocalHint.Appearance.Options.UseFont = true;
            this.lblLocalHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.layoutLocal.SetColumnSpan(this.lblLocalHint, 4);
            this.lblLocalHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLocalHint.Location = new System.Drawing.Point(0, 71);
            this.lblLocalHint.Margin = new System.Windows.Forms.Padding(0, 3, 6, 3);
            this.lblLocalHint.Name = "lblLocalHint";
            this.lblLocalHint.Size = new System.Drawing.Size(826, 31);
            this.lblLocalHint.TabIndex = 8;
            this.lblLocalHint.Text = "로컬 서버의 주소와 모델명을 직접 입력하세요.";
            // 
            // layoutProviderHeader
            // 
            this.layoutProviderHeader.Controls.Add(this.lblEditingProvider);
            this.layoutProviderHeader.Controls.Add(this.btnAdvancedSettings);
            this.layoutProviderHeader.Controls.Add(this.advancedHeaderGap);
            this.layoutProviderHeader.Controls.Add(this.btnRecommendedModel);
            this.layoutProviderHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutProviderHeader.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutProviderHeader.Location = new System.Drawing.Point(0, 64);
            this.layoutProviderHeader.Margin = new System.Windows.Forms.Padding(0);
            this.layoutProviderHeader.Name = "layoutProviderHeader";
            this.layoutProviderHeader.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.layoutProviderHeader.Size = new System.Drawing.Size(854, 34);
            this.layoutProviderHeader.TabIndex = 3;
            // 
            // lblEditingProvider
            // 
            this.lblEditingProvider.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblEditingProvider.Appearance.Options.UseFont = true;
            this.lblEditingProvider.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblEditingProvider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEditingProvider.Location = new System.Drawing.Point(0, 3);
            this.lblEditingProvider.Margin = new System.Windows.Forms.Padding(0);
            this.lblEditingProvider.Name = "lblEditingProvider";
            this.lblEditingProvider.Size = new System.Drawing.Size(486, 28);
            this.lblEditingProvider.TabIndex = 0;
            this.lblEditingProvider.Text = "편집 중인 설정";
            // 
            // btnRecommendedModel
            // 
            this.btnRecommendedModel.Appearance.BackColor = System.Drawing.Color.White;
            this.btnRecommendedModel.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnRecommendedModel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnRecommendedModel.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnRecommendedModel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnRecommendedModel.Appearance.Options.UseBackColor = true;
            this.btnRecommendedModel.Appearance.Options.UseBorderColor = true;
            this.btnRecommendedModel.Appearance.Options.UseFont = true;
            this.btnRecommendedModel.Appearance.Options.UseForeColor = true;
            this.btnRecommendedModel.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnRecommendedModel.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnRecommendedModel.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnRecommendedModel.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnRecommendedModel.AppearanceDisabled.Options.UseBackColor = true;
            this.btnRecommendedModel.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnRecommendedModel.AppearanceDisabled.Options.UseForeColor = true;
            this.btnRecommendedModel.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnRecommendedModel.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnRecommendedModel.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnRecommendedModel.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnRecommendedModel.AppearanceHovered.Options.UseBackColor = true;
            this.btnRecommendedModel.AppearanceHovered.Options.UseBorderColor = true;
            this.btnRecommendedModel.AppearanceHovered.Options.UseForeColor = true;
            this.btnRecommendedModel.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnRecommendedModel.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnRecommendedModel.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnRecommendedModel.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnRecommendedModel.AppearancePressed.Options.UseBackColor = true;
            this.btnRecommendedModel.AppearancePressed.Options.UseBorderColor = true;
            this.btnRecommendedModel.AppearancePressed.Options.UseForeColor = true;
            this.btnRecommendedModel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnRecommendedModel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRecommendedModel.Location = new System.Drawing.Point(636, 3);
            this.btnRecommendedModel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnRecommendedModel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnRecommendedModel.Margin = new System.Windows.Forms.Padding(0);
            this.btnRecommendedModel.Name = "btnRecommendedModel";
            this.btnRecommendedModel.Size = new System.Drawing.Size(218, 28);
            this.btnRecommendedModel.TabIndex = 1;
            this.btnRecommendedModel.Text = "권장 모델 적용";
            // 
            // grpProvider
            // 
            this.grpProvider.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpProvider.AppearanceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.grpProvider.AppearanceCaption.Options.UseFont = true;
            this.grpProvider.AppearanceCaption.Options.UseForeColor = true;
            this.grpProvider.Controls.Add(this.providerChoices);
            this.grpProvider.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpProvider.Location = new System.Drawing.Point(0, 0);
            this.grpProvider.Margin = new System.Windows.Forms.Padding(0);
            this.grpProvider.Name = "grpProvider";
            this.grpProvider.Padding = new System.Windows.Forms.Padding(8);
            this.grpProvider.Size = new System.Drawing.Size(854, 64);
            this.grpProvider.TabIndex = 4;
            this.grpProvider.Text = "기본 검수 프로바이더";
            // 
            // providerChoices
            // 
            this.providerChoices.Controls.Add(this.rbClaude);
            this.providerChoices.Controls.Add(this.rbOpenAi);
            this.providerChoices.Controls.Add(this.rbGemini);
            this.providerChoices.Controls.Add(this.rbLocal);
            this.providerChoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.providerChoices.Font = new System.Drawing.Font("SUIT", 10F);
            this.providerChoices.Location = new System.Drawing.Point(10, 31);
            this.providerChoices.Margin = new System.Windows.Forms.Padding(0);
            this.providerChoices.Name = "providerChoices";
            this.providerChoices.Size = new System.Drawing.Size(834, 23);
            this.providerChoices.TabIndex = 0;
            this.providerChoices.WrapContents = false;
            // 
            // rbClaude
            // 
            this.rbClaude.AutoSize = true;
            this.rbClaude.Font = new System.Drawing.Font("SUIT", 10F);
            this.rbClaude.Location = new System.Drawing.Point(0, 2);
            this.rbClaude.Margin = new System.Windows.Forms.Padding(0, 2, 12, 0);
            this.rbClaude.Name = "rbClaude";
            this.rbClaude.Size = new System.Drawing.Size(70, 22);
            this.rbClaude.TabIndex = 0;
            this.rbClaude.Text = "Claude";
            // 
            // rbOpenAi
            // 
            this.rbOpenAi.AutoSize = true;
            this.rbOpenAi.Font = new System.Drawing.Font("SUIT", 10F);
            this.rbOpenAi.Location = new System.Drawing.Point(82, 2);
            this.rbOpenAi.Margin = new System.Windows.Forms.Padding(0, 2, 12, 0);
            this.rbOpenAi.Name = "rbOpenAi";
            this.rbOpenAi.Size = new System.Drawing.Size(76, 22);
            this.rbOpenAi.TabIndex = 1;
            this.rbOpenAi.Text = "OpenAI";
            // 
            // rbGemini
            // 
            this.rbGemini.AutoSize = true;
            this.rbGemini.Font = new System.Drawing.Font("SUIT", 10F);
            this.rbGemini.Location = new System.Drawing.Point(170, 2);
            this.rbGemini.Margin = new System.Windows.Forms.Padding(0, 2, 12, 0);
            this.rbGemini.Name = "rbGemini";
            this.rbGemini.Size = new System.Drawing.Size(70, 22);
            this.rbGemini.TabIndex = 2;
            this.rbGemini.Text = "Gemini";
            // 
            // rbLocal
            // 
            this.rbLocal.AutoSize = true;
            this.rbLocal.Font = new System.Drawing.Font("SUIT", 10F);
            this.rbLocal.Location = new System.Drawing.Point(252, 2);
            this.rbLocal.Margin = new System.Windows.Forms.Padding(0, 2, 12, 0);
            this.rbLocal.Name = "rbLocal";
            this.rbLocal.Size = new System.Drawing.Size(60, 22);
            this.rbLocal.TabIndex = 3;
            this.rbLocal.Text = "Local";
            // 
            // tabWorkingCopy
            // 
            this.tabWorkingCopy.Controls.Add(this.layoutWorkingCopies);
            this.tabWorkingCopy.Margin = new System.Windows.Forms.Padding(0);
            this.tabWorkingCopy.Name = "tabWorkingCopy";
            this.tabWorkingCopy.Padding = new System.Windows.Forms.Padding(8);
            this.tabWorkingCopy.Size = new System.Drawing.Size(870, 655);
            this.tabWorkingCopy.Text = "작업 폴더";
            // 
            // layoutWorkingCopies
            // 
            this.layoutWorkingCopies.Controls.Add(this.gridWorkingCopies);
            this.layoutWorkingCopies.Controls.Add(this.layoutWorkingCopyButtons);
            this.layoutWorkingCopies.Controls.Add(this.lblWorkingCopiesHint);
            this.layoutWorkingCopies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutWorkingCopies.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutWorkingCopies.Location = new System.Drawing.Point(8, 8);
            this.layoutWorkingCopies.Margin = new System.Windows.Forms.Padding(0);
            this.layoutWorkingCopies.Name = "layoutWorkingCopies";
            this.layoutWorkingCopies.Size = new System.Drawing.Size(854, 639);
            this.layoutWorkingCopies.TabIndex = 0;
            // 
            // gridWorkingCopies
            // 
            this.gridWorkingCopies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridWorkingCopies.EmbeddedNavigator.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridWorkingCopies.EmbeddedNavigator.Appearance.Options.UseFont = true;
            this.gridWorkingCopies.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridWorkingCopies.Location = new System.Drawing.Point(0, 28);
            this.gridWorkingCopies.MainView = this.viewWorkingCopies;
            this.gridWorkingCopies.Margin = new System.Windows.Forms.Padding(0);
            this.gridWorkingCopies.Name = "gridWorkingCopies";
            this.gridWorkingCopies.Size = new System.Drawing.Size(854, 575);
            this.gridWorkingCopies.TabIndex = 0;
            this.gridWorkingCopies.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewWorkingCopies});
            // 
            // viewWorkingCopies
            // 
            this.viewWorkingCopies.Appearance.Empty.Font = new System.Drawing.Font("SUIT", 10F);
            this.viewWorkingCopies.Appearance.Empty.Options.UseFont = true;
            this.viewWorkingCopies.Appearance.HeaderPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.viewWorkingCopies.Appearance.HeaderPanel.Options.UseFont = true;
            this.viewWorkingCopies.Appearance.Row.Font = new System.Drawing.Font("SUIT", 10F);
            this.viewWorkingCopies.Appearance.Row.Options.UseFont = true;
            this.viewWorkingCopies.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colName,
            this.colPath,
            this.colUsername});
            this.viewWorkingCopies.GridControl = this.gridWorkingCopies;
            this.viewWorkingCopies.Name = "viewWorkingCopies";
            this.viewWorkingCopies.OptionsBehavior.AutoPopulateColumns = false;
            this.viewWorkingCopies.OptionsBehavior.Editable = false;
            this.viewWorkingCopies.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.viewWorkingCopies.OptionsView.ShowGroupPanel = false;
            this.viewWorkingCopies.OptionsView.ShowIndicator = false;
            // 
            // colName
            // 
            this.colName.Caption = "이름";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 180;
            // 
            // colPath
            // 
            this.colPath.Caption = "로컬 경로";
            this.colPath.FieldName = "LocalPath";
            this.colPath.Name = "colPath";
            this.colPath.Visible = true;
            this.colPath.VisibleIndex = 1;
            this.colPath.Width = 420;
            // 
            // colUsername
            // 
            this.colUsername.Caption = "사용자";
            this.colUsername.FieldName = "Username";
            this.colUsername.Name = "colUsername";
            this.colUsername.Visible = true;
            this.colUsername.VisibleIndex = 2;
            this.colUsername.Width = 140;
            // 
            // layoutWorkingCopyButtons
            // 
            this.layoutWorkingCopyButtons.Controls.Add(this.btnRemoveWC);
            this.layoutWorkingCopyButtons.Controls.Add(this.btnEditWC);
            this.layoutWorkingCopyButtons.Controls.Add(this.btnAddWC);
            this.layoutWorkingCopyButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.layoutWorkingCopyButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.layoutWorkingCopyButtons.Font = new System.Drawing.Font("SUIT", 10F);
            this.layoutWorkingCopyButtons.Location = new System.Drawing.Point(0, 603);
            this.layoutWorkingCopyButtons.Margin = new System.Windows.Forms.Padding(0);
            this.layoutWorkingCopyButtons.Name = "layoutWorkingCopyButtons";
            this.layoutWorkingCopyButtons.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.layoutWorkingCopyButtons.Size = new System.Drawing.Size(854, 36);
            this.layoutWorkingCopyButtons.TabIndex = 1;
            this.layoutWorkingCopyButtons.WrapContents = false;
            // 
            // btnRemoveWC
            // 
            this.btnRemoveWC.Appearance.BackColor = System.Drawing.Color.White;
            this.btnRemoveWC.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnRemoveWC.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnRemoveWC.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnRemoveWC.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnRemoveWC.Appearance.Options.UseBackColor = true;
            this.btnRemoveWC.Appearance.Options.UseBorderColor = true;
            this.btnRemoveWC.Appearance.Options.UseFont = true;
            this.btnRemoveWC.Appearance.Options.UseForeColor = true;
            this.btnRemoveWC.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnRemoveWC.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnRemoveWC.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnRemoveWC.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnRemoveWC.AppearanceDisabled.Options.UseBackColor = true;
            this.btnRemoveWC.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnRemoveWC.AppearanceDisabled.Options.UseForeColor = true;
            this.btnRemoveWC.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnRemoveWC.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnRemoveWC.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnRemoveWC.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnRemoveWC.AppearanceHovered.Options.UseBackColor = true;
            this.btnRemoveWC.AppearanceHovered.Options.UseBorderColor = true;
            this.btnRemoveWC.AppearanceHovered.Options.UseForeColor = true;
            this.btnRemoveWC.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnRemoveWC.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnRemoveWC.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnRemoveWC.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnRemoveWC.AppearancePressed.Options.UseBackColor = true;
            this.btnRemoveWC.AppearancePressed.Options.UseBorderColor = true;
            this.btnRemoveWC.AppearancePressed.Options.UseForeColor = true;
            this.btnRemoveWC.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnRemoveWC.Enabled = false;
            this.btnRemoveWC.Location = new System.Drawing.Point(766, 4);
            this.btnRemoveWC.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnRemoveWC.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnRemoveWC.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemoveWC.Name = "btnRemoveWC";
            this.btnRemoveWC.Size = new System.Drawing.Size(88, 28);
            this.btnRemoveWC.TabIndex = 0;
            this.btnRemoveWC.Text = "제거";
            // 
            // btnEditWC
            // 
            this.btnEditWC.Appearance.BackColor = System.Drawing.Color.White;
            this.btnEditWC.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnEditWC.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnEditWC.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnEditWC.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnEditWC.Appearance.Options.UseBackColor = true;
            this.btnEditWC.Appearance.Options.UseBorderColor = true;
            this.btnEditWC.Appearance.Options.UseFont = true;
            this.btnEditWC.Appearance.Options.UseForeColor = true;
            this.btnEditWC.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnEditWC.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnEditWC.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnEditWC.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnEditWC.AppearanceDisabled.Options.UseBackColor = true;
            this.btnEditWC.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnEditWC.AppearanceDisabled.Options.UseForeColor = true;
            this.btnEditWC.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnEditWC.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnEditWC.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnEditWC.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnEditWC.AppearanceHovered.Options.UseBackColor = true;
            this.btnEditWC.AppearanceHovered.Options.UseBorderColor = true;
            this.btnEditWC.AppearanceHovered.Options.UseForeColor = true;
            this.btnEditWC.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnEditWC.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnEditWC.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnEditWC.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnEditWC.AppearancePressed.Options.UseBackColor = true;
            this.btnEditWC.AppearancePressed.Options.UseBorderColor = true;
            this.btnEditWC.AppearancePressed.Options.UseForeColor = true;
            this.btnEditWC.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnEditWC.Enabled = false;
            this.btnEditWC.Location = new System.Drawing.Point(672, 4);
            this.btnEditWC.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnEditWC.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnEditWC.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnEditWC.Name = "btnEditWC";
            this.btnEditWC.Size = new System.Drawing.Size(88, 28);
            this.btnEditWC.TabIndex = 1;
            this.btnEditWC.Text = "수정";
            // 
            // btnAddWC
            // 
            this.btnAddWC.Appearance.BackColor = System.Drawing.Color.White;
            this.btnAddWC.Appearance.BackColor2 = System.Drawing.Color.White;
            this.btnAddWC.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(209)))), ((int)(((byte)(220)))));
            this.btnAddWC.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnAddWC.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAddWC.Appearance.Options.UseBackColor = true;
            this.btnAddWC.Appearance.Options.UseBorderColor = true;
            this.btnAddWC.Appearance.Options.UseFont = true;
            this.btnAddWC.Appearance.Options.UseForeColor = true;
            this.btnAddWC.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnAddWC.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnAddWC.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnAddWC.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnAddWC.AppearanceDisabled.Options.UseBackColor = true;
            this.btnAddWC.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnAddWC.AppearanceDisabled.Options.UseForeColor = true;
            this.btnAddWC.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnAddWC.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.btnAddWC.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnAddWC.AppearanceHovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAddWC.AppearanceHovered.Options.UseBackColor = true;
            this.btnAddWC.AppearanceHovered.Options.UseBorderColor = true;
            this.btnAddWC.AppearanceHovered.Options.UseForeColor = true;
            this.btnAddWC.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnAddWC.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(234)))), ((int)(((byte)(242)))));
            this.btnAddWC.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnAddWC.AppearancePressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.btnAddWC.AppearancePressed.Options.UseBackColor = true;
            this.btnAddWC.AppearancePressed.Options.UseBorderColor = true;
            this.btnAddWC.AppearancePressed.Options.UseForeColor = true;
            this.btnAddWC.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnAddWC.Location = new System.Drawing.Point(578, 4);
            this.btnAddWC.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnAddWC.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAddWC.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnAddWC.Name = "btnAddWC";
            this.btnAddWC.Size = new System.Drawing.Size(88, 28);
            this.btnAddWC.TabIndex = 2;
            this.btnAddWC.Text = "추가";
            // 
            // lblWorkingCopiesHint
            // 
            this.lblWorkingCopiesHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblWorkingCopiesHint.Appearance.Options.UseFont = true;
            this.lblWorkingCopiesHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblWorkingCopiesHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWorkingCopiesHint.Location = new System.Drawing.Point(0, 0);
            this.lblWorkingCopiesHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblWorkingCopiesHint.Name = "lblWorkingCopiesHint";
            this.lblWorkingCopiesHint.Size = new System.Drawing.Size(854, 28);
            this.lblWorkingCopiesHint.TabIndex = 2;
            this.lblWorkingCopiesHint.Text = "등록한 작업 폴더입니다. 목록에서 제거해도 실제 폴더는 삭제되지 않습니다.";
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnSave);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panelButtons.Font = new System.Drawing.Font("SUIT", 10F);
            this.panelButtons.Location = new System.Drawing.Point(12, 628);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.panelButtons.Size = new System.Drawing.Size(876, 40);
            this.panelButtons.TabIndex = 1;
            this.panelButtons.WrapContents = false;
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
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(788, 6);
            this.btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 28);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "취소";
            // 
            // btnSave
            // 
            this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnSave.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnSave.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(50)))), ((int)(((byte)(79)))));
            this.btnSave.Appearance.Font = new System.Drawing.Font("SUIT", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseBorderColor = true;
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnSave.AppearanceDisabled.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.btnSave.AppearanceDisabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(221)))), ((int)(((byte)(230)))));
            this.btnSave.AppearanceDisabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(135)))), ((int)(((byte)(150)))));
            this.btnSave.AppearanceDisabled.Options.UseBackColor = true;
            this.btnSave.AppearanceDisabled.Options.UseBorderColor = true;
            this.btnSave.AppearanceDisabled.Options.UseForeColor = true;
            this.btnSave.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(72)))), ((int)(((byte)(104)))));
            this.btnSave.AppearanceHovered.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(72)))), ((int)(((byte)(104)))));
            this.btnSave.AppearanceHovered.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(72)))), ((int)(((byte)(104)))));
            this.btnSave.AppearanceHovered.ForeColor = System.Drawing.Color.White;
            this.btnSave.AppearanceHovered.Options.UseBackColor = true;
            this.btnSave.AppearanceHovered.Options.UseBorderColor = true;
            this.btnSave.AppearanceHovered.Options.UseForeColor = true;
            this.btnSave.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.btnSave.AppearancePressed.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.btnSave.AppearancePressed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.btnSave.AppearancePressed.ForeColor = System.Drawing.Color.White;
            this.btnSave.AppearancePressed.Options.UseBackColor = true;
            this.btnSave.AppearancePressed.Options.UseBorderColor = true;
            this.btnSave.AppearancePressed.Options.UseForeColor = true;
            this.btnSave.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnSave.Location = new System.Drawing.Point(694, 6);
            this.btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnSave.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 28);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "저장";
            // 
            // SettingsView
            // 
            this.AcceptButton = this.btnSave;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(900, 680);
            this.Controls.Add(this.layoutRoot);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(820, 660);
            this.Name = "SettingsView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "설정";
            this.layoutRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabAi.ResumeLayout(false);
            this.layoutAi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpReviewItems)).EndInit();
            this.grpReviewItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.clbReviewItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoReview.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabProviders)).EndInit();
            this.tabProviders.ResumeLayout(false);
            this.tabClaude.ResumeLayout(false);
            this.layoutClaude.ResumeLayout(false);
            this.layoutClaude.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtClaudeApiKey.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtClaudeModel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClaudeTokens.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClaudeInputTokens.Properties)).EndInit();
            this.tabOpenAi.ResumeLayout(false);
            this.layoutOpenAi.ResumeLayout(false);
            this.layoutOpenAi.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenAiApiKey.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenAiModel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpenAiTokens.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpenAiInputTokens.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpenAiTimeout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOpenAiEffort.Properties)).EndInit();
            this.tabGemini.ResumeLayout(false);
            this.layoutGemini.ResumeLayout(false);
            this.layoutGemini.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeminiApiKey.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeminiModel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGeminiTokens.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGeminiInputTokens.Properties)).EndInit();
            this.tabLocal.ResumeLayout(false);
            this.layoutLocal.ResumeLayout(false);
            this.layoutLocal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalEndpoint.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalModel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocalTokens.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocalInputTokens.Properties)).EndInit();
            this.layoutProviderHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpProvider)).EndInit();
            this.grpProvider.ResumeLayout(false);
            this.providerChoices.ResumeLayout(false);
            this.providerChoices.PerformLayout();
            this.tabWorkingCopy.ResumeLayout(false);
            this.layoutWorkingCopies.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridWorkingCopies)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewWorkingCopies)).EndInit();
            this.layoutWorkingCopyButtons.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
