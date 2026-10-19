namespace SvnAiClient.Views.Main
{
    partial class MainView
    {
        #region Fields

        /// <summary>디자이너 구성 요소의 수명을 관리한다.</summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>도킹한 입력란과 명령 버튼 사이의 공통 6px 간격.</summary>
        private System.Windows.Forms.Panel commitCommandGap;


        /// <summary>도킹한 입력란과 명령 버튼 사이의 공통 6px 간격.</summary>
        private System.Windows.Forms.Panel pathCommandGap;



        /// <summary>상단 명령과 상태바를 관리한다.</summary>
        private DevExpress.XtraBars.BarManager barManager;

        /// <summary>작업 복사본 선택과 앱 공통 기능의 명령 모음.</summary>
        private DevExpress.XtraBars.Bar barToolbar;

        /// <summary>작업 복사본 전체에 적용하는 SVN 명령 모음.</summary>
        private DevExpress.XtraBars.Bar barWorkingCopy;

        /// <summary>상단 SVN 명령이 작업 폴더 전체에 적용됨을 표시한다.</summary>
        private DevExpress.XtraBars.BarStaticItem barItemWorkingCopyScope;

        /// <summary>새로고침과 업데이트의 적용 범위를 안내한다.</summary>
        private DevExpress.XtraBars.BarStaticItem barItemWorkingCopyHint;

        /// <summary>파일 조회 명령과 SVN 변경 명령을 구분한다.</summary>
        private System.Windows.Forms.ToolStripSeparator menuFileActionsSeparator;

        /// <summary>작업 진행과 AI 및 저장소 정보를 표시한다.</summary>
        private DevExpress.XtraBars.Bar barStatus;

        /// <summary>작업 복사본 Update 명령.</summary>
        private DevExpress.XtraBars.BarButtonItem btnUpdate;

        /// <summary>작업 복사본 상태 새로고침 명령.</summary>
        private DevExpress.XtraBars.BarButtonItem btnRefresh;

        /// <summary>앱 설정 화면 진입 명령.</summary>
        private DevExpress.XtraBars.BarButtonItem btnSettings;

        /// <summary>로컬 AI 검수 이력 화면 진입 명령.</summary>
        private DevExpress.XtraBars.BarButtonItem btnReviewHistory;

        /// <summary>작업 복사본 등록 명령.</summary>
        private DevExpress.XtraBars.BarButtonItem btnAddWorkingCopy;

        /// <summary>등록된 작업 복사본 제거 명령.</summary>
        private DevExpress.XtraBars.BarButtonItem btnRemoveWorkingCopy;

        /// <summary>현재 작업 복사본을 선택하는 항목.</summary>
        private DevExpress.XtraBars.BarEditItem barEditWorkingCopy;

        /// <summary>등록된 작업 복사본 선택 목록.</summary>
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cboWorkingCopyRepo;

        /// <summary>현재 작업의 진행 또는 완료 상태.</summary>
        private DevExpress.XtraBars.BarStaticItem barItemOperation;

        /// <summary>설정된 AI 프로바이더 이름.</summary>
        private DevExpress.XtraBars.BarStaticItem barItemProvider;

        /// <summary>작업 복사본의 저장소 주소.</summary>
        private DevExpress.XtraBars.BarStaticItem barItemRepoUrl;

        /// <summary>상단 명령 모음의 도킹 영역.</summary>
        private DevExpress.XtraBars.BarDockControl barDockControlTop;

        /// <summary>하단 상태바의 도킹 영역.</summary>
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;

        /// <summary>왼쪽 명령 모음 도킹 영역.</summary>
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;

        /// <summary>오른쪽 명령 모음 도킹 영역.</summary>
        private DevExpress.XtraBars.BarDockControl barDockControlRight;

        /// <summary>폴더 탐색 영역의 작업 복사본 관리 메뉴.</summary>
        private DevExpress.XtraBars.PopupMenu popupTree;

        /// <summary>폴더 탐색과 파일 작업 영역을 나눈다.</summary>
        private DevExpress.XtraEditors.SplitContainerControl splitMain;

        /// <summary>작업 복사본의 폴더 계층을 표시한다.</summary>
        private DevExpress.XtraTreeList.TreeList treeWorkingCopy;

        /// <summary>폴더 탐색 영역의 제목.</summary>
        private System.Windows.Forms.Label lblFolderTitle;

        /// <summary>파일 목록과 커밋 대상 목록의 높이를 조절한다.</summary>
        private DevExpress.XtraEditors.SplitContainerControl splitRight;

        /// <summary>현재 폴더 경로와 해당 폴더 로그 명령을 배치한다.</summary>
        private System.Windows.Forms.Panel panelCurrentPath;

        /// <summary>현재 위치 입력란의 설명.</summary>
        private System.Windows.Forms.Label lblCurrentPath;

        /// <summary>현재 폴더 경로를 읽기 전용으로 표시하며 복사를 허용한다.</summary>
        private DevExpress.XtraEditors.TextEdit txtCurrentPath;

        /// <summary>현재 폴더의 SVN 로그 진입 버튼.</summary>
        private DevExpress.XtraEditors.SimpleButton btnLog;

        /// <summary>파일 목록의 필터와 선택 항목 명령을 배치한다.</summary>
        private System.Windows.Forms.Panel panelFileTools;

        /// <summary>변경된 항목만 표시하는 필터.</summary>
        private DevExpress.XtraEditors.CheckEdit chkOnlyChanged;

        /// <summary>변경 필터에서 하위 폴더를 포함하는 옵션.</summary>
        private DevExpress.XtraEditors.CheckEdit chkIncludeSubfolders;

        /// <summary>작업 명령에 적용할 선택 파일 수.</summary>
        private System.Windows.Forms.Label lblFileSelection;

        /// <summary>선택 파일의 변경을 되돌리는 명령.</summary>
        private DevExpress.XtraEditors.SimpleButton btnRevert;

        /// <summary>현재 폴더의 파일 목록.</summary>
        private DevExpress.XtraGrid.GridControl gridFiles;

        /// <summary>파일 목록이 비어 있는 이유와 다음 행동을 안내한다.</summary>
        private System.Windows.Forms.Label lblFilesEmpty;

        /// <summary>파일 선택과 커밋 체크를 구분하여 표시하는 그리드 뷰.</summary>
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewFiles;

        /// <summary>커밋 대상 개수와 관련 명령을 배치한다.</summary>
        private System.Windows.Forms.Panel panelCommitHeader;

        /// <summary>폴더 이동과 관계없이 누적한 커밋 대상의 개수.</summary>
        private System.Windows.Forms.Label lblCommitTargets;

        /// <summary>검수 및 커밋 준비 화면 진입 버튼.</summary>
        private DevExpress.XtraEditors.SimpleButton btnCommit;

        /// <summary>실제 파일을 삭제하지 않고 커밋 대상에서 제외하는 버튼.</summary>
        private DevExpress.XtraEditors.SimpleButton btnRemoveCommitTarget;

        /// <summary>누적한 커밋 대상 파일 목록.</summary>
        private DevExpress.XtraGrid.GridControl gridCommitTarget;

        /// <summary>커밋 대상의 상태와 경로를 표시하는 그리드 뷰.</summary>
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCommitTarget;

        /// <summary>파일 목록의 SVN 작업 메뉴.</summary>
        private System.Windows.Forms.ContextMenuStrip popupFiles;

        /// <summary>선택 파일의 SVN 추가 예약 메뉴.</summary>
        private System.Windows.Forms.ToolStripMenuItem menuSvnAdd;

        /// <summary>선택 파일의 SVN 삭제 예약 메뉴.</summary>
        private System.Windows.Forms.ToolStripMenuItem menuSvnDelete;

        /// <summary>우클릭한 파일의 Diff 진입 메뉴.</summary>
        private System.Windows.Forms.ToolStripMenuItem menuViewDiff;

        /// <summary>우클릭한 파일의 로그 진입 메뉴.</summary>
        private System.Windows.Forms.ToolStripMenuItem menuViewFileLog;

        #endregion

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Initialization

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            this.commitCommandGap = new System.Windows.Forms.Panel();
            this.pathCommandGap = new System.Windows.Forms.Panel();
            this.barManager = new DevExpress.XtraBars.BarManager();
            this.barToolbar = new DevExpress.XtraBars.Bar();
            this.barEditWorkingCopy = new DevExpress.XtraBars.BarEditItem();
            this.cboWorkingCopyRepo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.btnAddWorkingCopy = new DevExpress.XtraBars.BarButtonItem();
            this.btnReviewHistory = new DevExpress.XtraBars.BarButtonItem();
            this.btnSettings = new DevExpress.XtraBars.BarButtonItem();
            this.barWorkingCopy = new DevExpress.XtraBars.Bar();
            this.barItemWorkingCopyScope = new DevExpress.XtraBars.BarStaticItem();
            this.barItemWorkingCopyHint = new DevExpress.XtraBars.BarStaticItem();
            this.menuFileActionsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.btnUpdate = new DevExpress.XtraBars.BarButtonItem();
            this.barStatus = new DevExpress.XtraBars.Bar();
            this.barItemOperation = new DevExpress.XtraBars.BarStaticItem();
            this.barItemProvider = new DevExpress.XtraBars.BarStaticItem();
            this.barItemRepoUrl = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnRemoveWorkingCopy = new DevExpress.XtraBars.BarButtonItem();
            this.popupTree = new DevExpress.XtraBars.PopupMenu();
            this.splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            this.treeWorkingCopy = new DevExpress.XtraTreeList.TreeList();
            this.lblFolderTitle = new System.Windows.Forms.Label();
            this.splitRight = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridFiles = new DevExpress.XtraGrid.GridControl();
            this.gridViewFiles = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblFilesEmpty = new System.Windows.Forms.Label();
            this.panelFileTools = new System.Windows.Forms.Panel();
            this.lblFileSelection = new System.Windows.Forms.Label();
            this.btnRevert = new DevExpress.XtraEditors.SimpleButton();
            this.chkIncludeSubfolders = new DevExpress.XtraEditors.CheckEdit();
            this.chkOnlyChanged = new DevExpress.XtraEditors.CheckEdit();
            this.panelCurrentPath = new System.Windows.Forms.Panel();
            this.txtCurrentPath = new DevExpress.XtraEditors.TextEdit();
            this.btnLog = new DevExpress.XtraEditors.SimpleButton();
            this.lblCurrentPath = new System.Windows.Forms.Label();
            this.gridCommitTarget = new DevExpress.XtraGrid.GridControl();
            this.gridViewCommitTarget = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelCommitHeader = new System.Windows.Forms.Panel();
            this.lblCommitTargets = new System.Windows.Forms.Label();
            this.btnRemoveCommitTarget = new DevExpress.XtraEditors.SimpleButton();
            this.btnCommit = new DevExpress.XtraEditors.SimpleButton();
            this.popupFiles = new System.Windows.Forms.ContextMenuStrip();
            this.menuViewDiff = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewFileLog = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSvnAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSvnDelete = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkingCopyRepo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeWorkingCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).BeginInit();
            this.splitRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFiles)).BeginInit();
            this.panelFileTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIncludeSubfolders.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOnlyChanged.Properties)).BeginInit();
            this.panelCurrentPath.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCommitTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCommitTarget)).BeginInit();
            this.panelCommitHeader.SuspendLayout();
            this.popupFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // commitCommandGap
            // 
            this.commitCommandGap.Dock = System.Windows.Forms.DockStyle.Right;
            this.commitCommandGap.Font = new System.Drawing.Font("SUIT", 10F);
            this.commitCommandGap.Location = new System.Drawing.Point(877, 4);
            this.commitCommandGap.Margin = new System.Windows.Forms.Padding(0);
            this.commitCommandGap.Name = "commitCommandGap";
            this.commitCommandGap.Size = new System.Drawing.Size(6, 28);
            this.commitCommandGap.TabIndex = 1;
            // 
            // pathCommandGap
            // 
            this.pathCommandGap.Dock = System.Windows.Forms.DockStyle.Right;
            this.pathCommandGap.Font = new System.Drawing.Font("SUIT", 10F);
            this.pathCommandGap.Location = new System.Drawing.Point(877, 4);
            this.pathCommandGap.Margin = new System.Windows.Forms.Padding(0);
            this.pathCommandGap.Name = "pathCommandGap";
            this.pathCommandGap.Size = new System.Drawing.Size(6, 28);
            this.pathCommandGap.TabIndex = 1;
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barToolbar,
            this.barWorkingCopy,
            this.barStatus});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnUpdate,
            this.btnRefresh,
            this.btnSettings,
            this.btnReviewHistory,
            this.btnAddWorkingCopy,
            this.btnRemoveWorkingCopy,
            this.barEditWorkingCopy,
            this.barItemOperation,
            this.barItemProvider,
            this.barItemRepoUrl,
            this.barItemWorkingCopyScope,
            this.barItemWorkingCopyHint});
            this.barManager.MaxItemId = 15;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cboWorkingCopyRepo});
            this.barManager.StatusBar = this.barStatus;
            // 
            // barToolbar
            // 
            this.barToolbar.BarAppearance.Normal.Font = new System.Drawing.Font("SUIT", 9F);
            this.barToolbar.BarAppearance.Normal.Options.UseFont = true;
            this.barToolbar.BarName = "ApplicationToolbar";
            this.barToolbar.DockCol = 0;
            this.barToolbar.DockRow = 0;
            this.barToolbar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barToolbar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditWorkingCopy),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddWorkingCopy),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnReviewHistory, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSettings)});
            this.barToolbar.OptionsBar.AllowQuickCustomization = false;
            this.barToolbar.OptionsBar.DrawDragBorder = false;
            this.barToolbar.OptionsBar.UseWholeRow = true;
            this.barToolbar.Text = "앱 및 작업 폴더";
            // 
            // barEditWorkingCopy
            // 
            this.barEditWorkingCopy.Caption = "작업 폴더";
            this.barEditWorkingCopy.Edit = this.cboWorkingCopyRepo;
            this.barEditWorkingCopy.EditWidth = 420;
            this.barEditWorkingCopy.Id = 6;
            this.barEditWorkingCopy.Name = "barEditWorkingCopy";
            this.barEditWorkingCopy.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            // 
            // cboWorkingCopyRepo
            // 
            this.cboWorkingCopyRepo.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cboWorkingCopyRepo.Appearance.Options.UseFont = true;
            this.cboWorkingCopyRepo.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cboWorkingCopyRepo.AppearanceDropDown.Options.UseFont = true;
            this.cboWorkingCopyRepo.Name = "cboWorkingCopyRepo";
            // 
            // btnAddWorkingCopy
            // 
            this.btnAddWorkingCopy.Caption = "작업 폴더 등록…";
            this.btnAddWorkingCopy.Hint = "관리할 SVN 작업 폴더를 등록합니다.";
            this.btnAddWorkingCopy.Id = 9;
            this.btnAddWorkingCopy.Name = "btnAddWorkingCopy";
            this.btnAddWorkingCopy.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddWorkingCopy_Click);
            // 
            // btnReviewHistory
            // 
            this.btnReviewHistory.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnReviewHistory.Caption = "AI 검수 이력";
            this.btnReviewHistory.Hint = "로컬에 저장된 AI 검수 이력을 조회합니다.";
            this.btnReviewHistory.Id = 11;
            this.btnReviewHistory.Name = "btnReviewHistory";
            this.btnReviewHistory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ReviewHistoryButton_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnSettings.Caption = "설정";
            this.btnSettings.Hint = "AI 프로바이더와 앱 설정을 변경합니다.";
            this.btnSettings.Id = 5;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.menuSettings_Click);
            // 
            // barWorkingCopy
            // 
            this.barWorkingCopy.BarAppearance.Normal.Font = new System.Drawing.Font("SUIT", 9F);
            this.barWorkingCopy.BarAppearance.Normal.Options.UseFont = true;
            this.barWorkingCopy.BarName = "WorkingCopyToolbar";
            this.barWorkingCopy.DockCol = 0;
            this.barWorkingCopy.DockRow = 1;
            this.barWorkingCopy.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barWorkingCopy.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barItemWorkingCopyScope),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRefresh, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUpdate),
            new DevExpress.XtraBars.LinkPersistInfo(this.barItemWorkingCopyHint)});
            this.barWorkingCopy.OptionsBar.AllowQuickCustomization = false;
            this.barWorkingCopy.OptionsBar.DrawDragBorder = false;
            this.barWorkingCopy.OptionsBar.UseWholeRow = true;
            this.barWorkingCopy.Text = "작업 폴더 명령";
            this.barItemWorkingCopyScope.Caption = "작업 폴더 전체";
            this.barItemWorkingCopyScope.Id = 13;
            this.barItemWorkingCopyScope.Name = "barItemWorkingCopyScope";
            this.barItemWorkingCopyScope.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.barItemWorkingCopyScope.Hint = "새로고침과 업데이트는 현재 선택한 작업 폴더 전체에 적용됩니다.";
            this.barItemWorkingCopyScope.ItemAppearance.Normal.Font = new System.Drawing.Font("SUIT", 9F, System.Drawing.FontStyle.Bold);
            this.barItemWorkingCopyScope.ItemAppearance.Normal.Options.UseFont = true;
            this.barItemWorkingCopyHint.Caption = "현재 탐색 위치와 관계없이 전체 폴더에 적용";
            this.barItemWorkingCopyHint.Id = 14;
            this.barItemWorkingCopyHint.Name = "barItemWorkingCopyHint";
            this.barItemWorkingCopyHint.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.barItemWorkingCopyHint.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Caption = "상태 새로고침";
            this.btnRefresh.Hint = "작업 폴더 전체의 SVN 상태를 다시 조회합니다. (F5)";
            this.btnRefresh.Id = 4;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRefresh_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Caption = "최신 버전 받기";
            this.btnUpdate.Hint = "작업 폴더 전체를 최신 리비전으로 갱신합니다.";
            this.btnUpdate.Id = 0;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnUpdate_Click);
            // 
            // barStatus
            // 
            this.barStatus.BarAppearance.Normal.Font = new System.Drawing.Font("SUIT", 10F);
            this.barStatus.BarAppearance.Normal.Options.UseFont = true;
            this.barStatus.BarName = "StatusBar";
            this.barStatus.DockCol = 0;
            this.barStatus.DockRow = 0;
            this.barStatus.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.barStatus.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barItemOperation),
            new DevExpress.XtraBars.LinkPersistInfo(this.barItemProvider, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barItemRepoUrl, true)});
            this.barStatus.OptionsBar.AllowQuickCustomization = false;
            this.barStatus.OptionsBar.DrawDragBorder = false;
            this.barStatus.OptionsBar.UseWholeRow = true;
            this.barStatus.Text = "작업 상태";
            // 
            // barItemOperation
            // 
            this.barItemOperation.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
            this.barItemOperation.Caption = "준비 완료";
            this.barItemOperation.Id = 12;
            this.barItemOperation.Name = "barItemOperation";
            this.barItemOperation.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barItemProvider
            // 
            this.barItemProvider.AutoSize = DevExpress.XtraBars.BarStaticItemSize.None;
            this.barItemProvider.Caption = "AI: -";
            this.barItemProvider.Id = 7;
            this.barItemProvider.Name = "barItemProvider";
            this.barItemProvider.Size = new System.Drawing.Size(100, 0);
            this.barItemProvider.TextAlignment = System.Drawing.StringAlignment.Near;
            this.barItemProvider.Width = 100;
            // 
            // barItemRepoUrl
            // 
            this.barItemRepoUrl.AutoSize = DevExpress.XtraBars.BarStaticItemSize.None;
            this.barItemRepoUrl.Caption = "저장소: -";
            this.barItemRepoUrl.Id = 8;
            this.barItemRepoUrl.Name = "barItemRepoUrl";
            this.barItemRepoUrl.Size = new System.Drawing.Size(400, 0);
            this.barItemRepoUrl.TextAlignment = System.Drawing.StringAlignment.Near;
            this.barItemRepoUrl.Width = 400;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Font = new System.Drawing.Font("SUIT", 10F);
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(1280, 60);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Font = new System.Drawing.Font("SUIT", 10F);
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 772);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(1280, 28);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Font = new System.Drawing.Font("SUIT", 10F);
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 60);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 712);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Font = new System.Drawing.Font("SUIT", 10F);
            this.barDockControlRight.Location = new System.Drawing.Point(1280, 60);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 712);
            // 
            // btnRemoveWorkingCopy
            // 
            this.btnRemoveWorkingCopy.Caption = "목록에서 제거";
            this.btnRemoveWorkingCopy.Hint = "작업 폴더를 등록 목록에서 제거합니다. 로컬 파일은 삭제하지 않습니다.";
            this.btnRemoveWorkingCopy.Id = 10;
            this.btnRemoveWorkingCopy.Name = "btnRemoveWorkingCopy";
            this.btnRemoveWorkingCopy.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRemoveWorkingCopy_Click);
            // 
            // popupTree
            // 
            this.popupTree.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddWorkingCopy),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRemoveWorkingCopy)});
            this.popupTree.Manager = this.barManager;
            this.popupTree.Name = "popupTree";
            // 
            // splitMain
            // 
            this.splitMain.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitMain.AppearanceCaption.Options.UseFont = true;
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 60);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitMain.Panel1.AppearanceCaption.Options.UseFont = true;
            this.splitMain.Panel1.Controls.Add(this.treeWorkingCopy);
            this.splitMain.Panel1.Controls.Add(this.lblFolderTitle);
            this.splitMain.Panel1.Padding = new System.Windows.Forms.Padding(12);
            this.splitMain.Panel2.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitMain.Panel2.AppearanceCaption.Options.UseFont = true;
            this.splitMain.Panel2.Controls.Add(this.splitRight);
            this.splitMain.Panel2.Padding = new System.Windows.Forms.Padding(0, 12, 12, 12);
            this.splitMain.Size = new System.Drawing.Size(1280, 712);
            this.splitMain.SplitterPosition = 240;
            this.splitMain.TabIndex = 0;
            // 
            // treeWorkingCopy
            // 
            this.treeWorkingCopy.Appearance.HeaderPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.treeWorkingCopy.Appearance.HeaderPanel.Options.UseFont = true;
            this.treeWorkingCopy.Appearance.Row.Font = new System.Drawing.Font("SUIT", 10F);
            this.treeWorkingCopy.Appearance.Row.Options.UseFont = true;
            this.treeWorkingCopy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeWorkingCopy.Font = new System.Drawing.Font("SUIT", 10F);
            this.treeWorkingCopy.Location = new System.Drawing.Point(12, 40);
            this.treeWorkingCopy.Name = "treeWorkingCopy";
            this.treeWorkingCopy.OptionsBehavior.Editable = false;
            this.treeWorkingCopy.Size = new System.Drawing.Size(216, 660);
            this.treeWorkingCopy.TabIndex = 1;
            this.treeWorkingCopy.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeWorkingCopy_MouseUp);
            // 
            // lblFolderTitle
            // 
            this.lblFolderTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFolderTitle.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.lblFolderTitle.Location = new System.Drawing.Point(12, 12);
            this.lblFolderTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblFolderTitle.Name = "lblFolderTitle";
            this.lblFolderTitle.Size = new System.Drawing.Size(216, 28);
            this.lblFolderTitle.TabIndex = 2;
            this.lblFolderTitle.Text = "폴더 탐색";
            this.lblFolderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitRight
            // 
            this.splitRight.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitRight.AppearanceCaption.Options.UseFont = true;
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitRight.Horizontal = false;
            this.splitRight.Location = new System.Drawing.Point(0, 12);
            this.splitRight.Name = "splitRight";
            this.splitRight.Panel1.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitRight.Panel1.AppearanceCaption.Options.UseFont = true;
            this.splitRight.Panel1.Controls.Add(this.gridFiles);
            this.splitRight.Panel1.Controls.Add(this.lblFilesEmpty);
            this.splitRight.Panel1.Controls.Add(this.panelFileTools);
            this.splitRight.Panel1.Controls.Add(this.panelCurrentPath);
            this.splitRight.Panel2.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitRight.Panel2.AppearanceCaption.Options.UseFont = true;
            this.splitRight.Panel2.Controls.Add(this.gridCommitTarget);
            this.splitRight.Panel2.Controls.Add(this.panelCommitHeader);
            this.splitRight.Size = new System.Drawing.Size(1023, 688);
            this.splitRight.SplitterPosition = 210;
            this.splitRight.TabIndex = 0;
            // 
            // gridFiles
            // 
            this.gridFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridFiles.EmbeddedNavigator.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridFiles.EmbeddedNavigator.Appearance.Options.UseFont = true;
            this.gridFiles.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridFiles.Location = new System.Drawing.Point(0, 108);
            this.gridFiles.MainView = this.gridViewFiles;
            this.gridFiles.Margin = new System.Windows.Forms.Padding(0);
            this.gridFiles.Name = "gridFiles";
            this.gridFiles.Size = new System.Drawing.Size(1023, 365);
            this.gridFiles.TabIndex = 2;
            this.gridFiles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewFiles});
            this.gridFiles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridFiles_MouseUp);
            // 
            // gridViewFiles
            // 
            this.gridViewFiles.Appearance.Empty.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewFiles.Appearance.Empty.Options.UseFont = true;
            this.gridViewFiles.Appearance.HeaderPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewFiles.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewFiles.Appearance.Row.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewFiles.Appearance.Row.Options.UseFont = true;
            this.gridViewFiles.GridControl = this.gridFiles;
            this.gridViewFiles.Name = "gridViewFiles";
            this.gridViewFiles.OptionsBehavior.Editable = false;
            this.gridViewFiles.OptionsFind.AlwaysVisible = true;
            this.gridViewFiles.OptionsFind.FindFilterColumns = "FileName;RelativePath";
            this.gridViewFiles.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.Always;
            this.gridViewFiles.OptionsFind.FindNullPrompt = "현재 목록에서 파일명 또는 경로 검색 (Ctrl+F)";
            this.gridViewFiles.OptionsFind.ShowCloseButton = false;
            this.gridViewFiles.OptionsFind.ShowFindButton = false;
            this.gridViewFiles.OptionsView.ShowGroupPanel = false;
            // 
            // lblFilesEmpty
            // 
            this.lblFilesEmpty.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFilesEmpty.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblFilesEmpty.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFilesEmpty.Location = new System.Drawing.Point(0, 72);
            this.lblFilesEmpty.Margin = new System.Windows.Forms.Padding(0);
            this.lblFilesEmpty.Name = "lblFilesEmpty";
            this.lblFilesEmpty.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblFilesEmpty.Size = new System.Drawing.Size(1023, 36);
            this.lblFilesEmpty.TabIndex = 3;
            this.lblFilesEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFilesEmpty.Visible = false;
            // 
            // panelFileTools
            // 
            this.panelFileTools.Controls.Add(this.lblFileSelection);
            this.panelFileTools.Controls.Add(this.btnRevert);
            this.panelFileTools.Controls.Add(this.chkIncludeSubfolders);
            this.panelFileTools.Controls.Add(this.chkOnlyChanged);
            this.panelFileTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFileTools.Font = new System.Drawing.Font("SUIT", 10F);
            this.panelFileTools.Location = new System.Drawing.Point(0, 36);
            this.panelFileTools.Margin = new System.Windows.Forms.Padding(0);
            this.panelFileTools.Name = "panelFileTools";
            this.panelFileTools.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.panelFileTools.Size = new System.Drawing.Size(1023, 36);
            this.panelFileTools.TabIndex = 3;
            // 
            // lblFileSelection
            // 
            this.lblFileSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFileSelection.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblFileSelection.Location = new System.Drawing.Point(300, 4);
            this.lblFileSelection.Margin = new System.Windows.Forms.Padding(0);
            this.lblFileSelection.Name = "lblFileSelection";
            this.lblFileSelection.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblFileSelection.Size = new System.Drawing.Size(583, 28);
            this.lblFileSelection.TabIndex = 0;
            this.lblFileSelection.Text = "행 선택 · 0개";
            this.lblFileSelection.AutoEllipsis = true;
            this.lblFileSelection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRevert
            // 
            this.btnRevert.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnRevert.Appearance.Options.UseFont = true;
            this.btnRevert.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRevert.Location = new System.Drawing.Point(883, 4);
            this.btnRevert.Margin = new System.Windows.Forms.Padding(0);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(156, 28);
            this.btnRevert.TabIndex = 2;
            this.btnRevert.Text = "선택 파일 되돌리기…";
            this.btnRevert.ToolTip = "행으로 선택한 파일의 변경을 되돌립니다. 커밋 체크와는 별개의 선택입니다.";
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // chkIncludeSubfolders
            // 
            this.chkIncludeSubfolders.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkIncludeSubfolders.Enabled = false;
            this.chkIncludeSubfolders.Location = new System.Drawing.Point(140, 4);
            this.chkIncludeSubfolders.Margin = new System.Windows.Forms.Padding(0);
            this.chkIncludeSubfolders.Name = "chkIncludeSubfolders";
            this.chkIncludeSubfolders.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.chkIncludeSubfolders.Properties.Appearance.Options.UseFont = true;
            this.chkIncludeSubfolders.Properties.Caption = "하위 폴더 포함";
            this.chkIncludeSubfolders.Size = new System.Drawing.Size(140, 28);
            this.chkIncludeSubfolders.TabIndex = 1;
            // 
            // chkOnlyChanged
            // 
            this.chkOnlyChanged.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkOnlyChanged.Location = new System.Drawing.Point(0, 4);
            this.chkOnlyChanged.Margin = new System.Windows.Forms.Padding(0);
            this.chkOnlyChanged.Name = "chkOnlyChanged";
            this.chkOnlyChanged.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.chkOnlyChanged.Properties.Appearance.Options.UseFont = true;
            this.chkOnlyChanged.Properties.Caption = "변경 항목만";
            this.chkOnlyChanged.Size = new System.Drawing.Size(120, 28);
            this.chkOnlyChanged.TabIndex = 0;
            // 
            // panelCurrentPath
            // 
            this.panelCurrentPath.Controls.Add(this.txtCurrentPath);
            this.panelCurrentPath.Controls.Add(this.pathCommandGap);
            this.panelCurrentPath.Controls.Add(this.btnLog);
            this.panelCurrentPath.Controls.Add(this.lblCurrentPath);
            this.panelCurrentPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCurrentPath.Font = new System.Drawing.Font("SUIT", 10F);
            this.panelCurrentPath.Location = new System.Drawing.Point(0, 0);
            this.panelCurrentPath.Margin = new System.Windows.Forms.Padding(0);
            this.panelCurrentPath.Name = "panelCurrentPath";
            this.panelCurrentPath.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.panelCurrentPath.Size = new System.Drawing.Size(1023, 36);
            this.panelCurrentPath.TabIndex = 4;
            // 
            // txtCurrentPath
            // 
            this.txtCurrentPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCurrentPath.Location = new System.Drawing.Point(104, 4);
            this.txtCurrentPath.Margin = new System.Windows.Forms.Padding(0);
            this.txtCurrentPath.Name = "txtCurrentPath";
            this.txtCurrentPath.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtCurrentPath.Properties.Appearance.Options.UseFont = true;
            this.txtCurrentPath.Properties.AutoHeight = false;
            this.txtCurrentPath.Properties.ReadOnly = true;
            this.txtCurrentPath.Size = new System.Drawing.Size(773, 28);
            this.txtCurrentPath.TabIndex = 0;
            // 
            // btnLog
            // 
            this.btnLog.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnLog.Appearance.Options.UseFont = true;
            this.btnLog.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnLog.Location = new System.Drawing.Point(883, 4);
            this.btnLog.Margin = new System.Windows.Forms.Padding(0);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(156, 28);
            this.btnLog.TabIndex = 1;
            this.btnLog.Text = "현재 폴더 이력";
            this.btnLog.ToolTip = "현재 탐색 중인 폴더의 SVN 로그를 조회합니다.";
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // lblCurrentPath
            // 
            this.lblCurrentPath.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCurrentPath.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblCurrentPath.Location = new System.Drawing.Point(0, 4);
            this.lblCurrentPath.Margin = new System.Windows.Forms.Padding(0);
            this.lblCurrentPath.Name = "lblCurrentPath";
            this.lblCurrentPath.Size = new System.Drawing.Size(104, 28);
            this.lblCurrentPath.TabIndex = 2;
            this.lblCurrentPath.Text = "현재 위치";
            this.lblCurrentPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gridCommitTarget
            // 
            this.gridCommitTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCommitTarget.EmbeddedNavigator.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridCommitTarget.EmbeddedNavigator.Appearance.Options.UseFont = true;
            this.gridCommitTarget.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridCommitTarget.Location = new System.Drawing.Point(0, 36);
            this.gridCommitTarget.MainView = this.gridViewCommitTarget;
            this.gridCommitTarget.Margin = new System.Windows.Forms.Padding(0);
            this.gridCommitTarget.Name = "gridCommitTarget";
            this.gridCommitTarget.Size = new System.Drawing.Size(1023, 174);
            this.gridCommitTarget.TabIndex = 2;
            this.gridCommitTarget.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCommitTarget});
            // 
            // gridViewCommitTarget
            // 
            this.gridViewCommitTarget.Appearance.Empty.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewCommitTarget.Appearance.Empty.Options.UseFont = true;
            this.gridViewCommitTarget.Appearance.HeaderPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewCommitTarget.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewCommitTarget.Appearance.Row.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridViewCommitTarget.Appearance.Row.Options.UseFont = true;
            this.gridViewCommitTarget.GridControl = this.gridCommitTarget;
            this.gridViewCommitTarget.Name = "gridViewCommitTarget";
            this.gridViewCommitTarget.OptionsBehavior.Editable = false;
            this.gridViewCommitTarget.OptionsView.ShowGroupPanel = false;
            // 
            // panelCommitHeader
            // 
            this.panelCommitHeader.Controls.Add(this.lblCommitTargets);
            this.panelCommitHeader.Controls.Add(this.btnRemoveCommitTarget);
            this.panelCommitHeader.Controls.Add(this.commitCommandGap);
            this.panelCommitHeader.Controls.Add(this.btnCommit);
            this.panelCommitHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCommitHeader.Font = new System.Drawing.Font("SUIT", 10F);
            this.panelCommitHeader.Location = new System.Drawing.Point(0, 0);
            this.panelCommitHeader.Margin = new System.Windows.Forms.Padding(0);
            this.panelCommitHeader.Name = "panelCommitHeader";
            this.panelCommitHeader.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.panelCommitHeader.Size = new System.Drawing.Size(1023, 36);
            this.panelCommitHeader.TabIndex = 3;
            // 
            // lblCommitTargets
            // 
            this.lblCommitTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCommitTargets.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.lblCommitTargets.Location = new System.Drawing.Point(0, 4);
            this.lblCommitTargets.Margin = new System.Windows.Forms.Padding(0);
            this.lblCommitTargets.Name = "lblCommitTargets";
            this.lblCommitTargets.Size = new System.Drawing.Size(737, 28);
            this.lblCommitTargets.TabIndex = 0;
            this.lblCommitTargets.Text = "커밋 대상 · 0개";
            this.lblCommitTargets.AutoEllipsis = true;
            this.lblCommitTargets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemoveCommitTarget
            // 
            this.btnRemoveCommitTarget.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnRemoveCommitTarget.Appearance.Options.UseFont = true;
            this.btnRemoveCommitTarget.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRemoveCommitTarget.Location = new System.Drawing.Point(737, 4);
            this.btnRemoveCommitTarget.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemoveCommitTarget.Name = "btnRemoveCommitTarget";
            this.btnRemoveCommitTarget.Size = new System.Drawing.Size(140, 28);
            this.btnRemoveCommitTarget.TabIndex = 0;
            this.btnRemoveCommitTarget.Text = "커밋 대상에서 제외";
            this.btnRemoveCommitTarget.ToolTip = "커밋 대상에서만 제외합니다. 파일은 삭제하거나 되돌리지 않습니다.";
            // 
            // btnCommit
            // 
            this.btnCommit.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCommit.Location = new System.Drawing.Point(883, 4);
            this.btnCommit.Margin = new System.Windows.Forms.Padding(0);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(156, 28);
            this.btnCommit.TabIndex = 1;
            this.btnCommit.Text = "검수 및 커밋…";
            this.btnCommit.ToolTip = "체크한 커밋 대상의 변경 내용을 확인하고 AI 검수 및 커밋을 진행합니다.";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // popupFiles
            // 
            this.popupFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.popupFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewDiff,
            this.menuViewFileLog,
            this.menuFileActionsSeparator,
            this.menuSvnAdd,
            this.menuSvnDelete});
            this.popupFiles.Name = "popupFiles";
            this.popupFiles.Size = new System.Drawing.Size(255, 92);
            // 
            // menuViewDiff
            // 
            this.menuViewDiff.Name = "menuViewDiff";
            this.menuViewDiff.Size = new System.Drawing.Size(254, 22);
            this.menuViewDiff.Text = "변경 내용 보기";
            this.menuViewDiff.Click += new System.EventHandler(this.menuViewDiff_Click);
            // 
            // menuViewFileLog
            // 
            this.menuFileActionsSeparator.Name = "menuFileActionsSeparator";
            this.menuViewFileLog.ToolTipText = "여러 파일이 선택되어 있어도 우클릭한 파일의 이력을 조회합니다.";
            this.menuViewFileLog.Enabled = false;
            this.menuViewFileLog.Name = "menuViewFileLog";
            this.menuViewFileLog.Size = new System.Drawing.Size(254, 22);
            this.menuViewFileLog.Text = "이 파일의 이력 보기";
            this.menuViewFileLog.Click += new System.EventHandler(this.menuViewFileLog_Click);
            // 
            // menuSvnAdd
            // 
            this.menuSvnAdd.Name = "menuSvnAdd";
            this.menuSvnAdd.Size = new System.Drawing.Size(254, 22);
            this.menuSvnAdd.Text = "SVN 추가";
            this.menuSvnAdd.Click += new System.EventHandler(this.menuSvnAdd_Click);
            // 
            // menuSvnDelete
            // 
            this.menuSvnDelete.Name = "menuSvnDelete";
            this.menuSvnDelete.Size = new System.Drawing.Size(254, 22);
            this.menuSvnDelete.Text = "SVN 삭제 예약 (로컬 파일 유지)";
            this.menuSvnDelete.Click += new System.EventHandler(this.menuSvnDelete_Click);
            // 
            // MainView
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ShowIcon = true;
            this.Name = "MainView";
            this.Text = "EZ SVN";
            this.Load += new System.EventHandler(this.MainView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkingCopyRepo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeWorkingCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).EndInit();
            this.splitRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFiles)).EndInit();
            this.panelFileTools.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkIncludeSubfolders.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkOnlyChanged.Properties)).EndInit();
            this.panelCurrentPath.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCommitTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCommitTarget)).EndInit();
            this.panelCommitHeader.ResumeLayout(false);
            this.popupFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
