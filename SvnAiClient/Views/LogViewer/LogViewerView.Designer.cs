using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace SvnAiClient.Views.LogViewer
{
    partial class LogViewerView
    {
        #region Fields

        /// <summary>화면 구성 요소의 수명을 관리한다.</summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>화면 전체 배치.</summary>
        private Panel layout;
        /// <summary>조회 대상과 조회 조건 배치.</summary>
        private Panel header;
        /// <summary>조회 대상 경로 배치.</summary>
        private Panel targetRow;
        /// <summary>조회 대상 안내.</summary>
        private LabelControl lblTarget;
        /// <summary>로그 조회 범위를 표시하는 복사 가능한 경로.</summary>
        private TextEdit txtTargetPath;
        /// <summary>조회 건수와 조회·중단 버튼 배치.</summary>
        private FlowLayoutPanel toolbar;
        /// <summary>조회 건수 안내.</summary>
        private LabelControl lblCount;
        /// <summary>최대 로그 조회 건수.</summary>
        private SpinEdit numCount;
        /// <summary>새로고침 버튼.</summary>
        private SimpleButton btnRefresh;
        /// <summary>조회 중단 버튼.</summary>
        private SimpleButton btnStop;
        /// <summary>목록의 필터 행에서 작성자와 메시지를 검색할 수 있습니다. 안내.</summary>
        private LabelControl lblGridHint;
        /// <summary>리비전 목록과 상세 영역의 높이 조절.</summary>
        private SplitContainerControl splitMain;
        /// <summary>리비전 목록과 비교 선택 상태 표시.</summary>
        private GroupControl grpLogs;
        /// <summary>선택 상태와 비교 버튼 배치.</summary>
        private Panel selectionBar;
        /// <summary>현재 변경 경로 목록에 적용된 리비전 비교 범위를 표시한다.</summary>
        private LabelControl lblComparisonScope;
        /// <summary>리비전 2개를 선택하면 두 시점을 비교할 수 있습니다. 안내.</summary>
        private LabelControl lblSelection;
        /// <summary>선택 리비전 비교 버튼.</summary>
        private SimpleButton btnCompare;
        /// <summary>조회된 SVN 리비전 목록.</summary>
        private GridControl gridLogs;
        /// <summary>리비전 검색과 다중 행 선택.</summary>
        private GridView viewLogs;
        /// <summary>메시지와 변경 경로 영역의 너비 조절.</summary>
        private SplitContainerControl details;
        /// <summary>선택한 리비전의 전체 커밋 메시지.</summary>
        private GroupControl grpMessage;
        /// <summary>리비전을 선택하세요. 안내.</summary>
        private LabelControl lblLogInfo;
        /// <summary>목록에서 생략된 내용을 포함한 전체 메시지.</summary>
        private MemoEdit txtMessage;
        /// <summary>리비전 또는 비교 범위의 변경 경로.</summary>
        private GroupControl grpPaths;
        /// <summary>작업 종류와 저장소 경로별 변경 목록.</summary>
        private ListBoxControl listPaths;
        /// <summary>변경 파일 Diff 버튼과 사용 안내.</summary>
        private Panel pathTools;
        /// <summary>파일을 더블클릭하면 Diff를 엽니다. 안내.</summary>
        private LabelControl lblPathHint;
        /// <summary>파일 Diff 열기 버튼.</summary>
        private SimpleButton btnOpenDiff;
        /// <summary>작업 상태와 닫기 버튼 배치.</summary>
        private Panel footer;
        /// <summary>조회 준비 안내.</summary>
        private LabelControl lblStatus;
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
            this.layout = new System.Windows.Forms.Panel();
            this.splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            this.grpLogs = new DevExpress.XtraEditors.GroupControl();
            this.gridLogs = new DevExpress.XtraGrid.GridControl();
            this.viewLogs = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.selectionBar = new System.Windows.Forms.Panel();
            this.lblComparisonScope = new DevExpress.XtraEditors.LabelControl();
            this.lblSelection = new DevExpress.XtraEditors.LabelControl();
            this.btnCompare = new DevExpress.XtraEditors.SimpleButton();
            this.details = new DevExpress.XtraEditors.SplitContainerControl();
            this.grpMessage = new DevExpress.XtraEditors.GroupControl();
            this.txtMessage = new DevExpress.XtraEditors.MemoEdit();
            this.lblLogInfo = new DevExpress.XtraEditors.LabelControl();
            this.grpPaths = new DevExpress.XtraEditors.GroupControl();
            this.listPaths = new DevExpress.XtraEditors.ListBoxControl();
            this.pathTools = new System.Windows.Forms.Panel();
            this.lblPathHint = new DevExpress.XtraEditors.LabelControl();
            this.btnOpenDiff = new DevExpress.XtraEditors.SimpleButton();
            this.header = new System.Windows.Forms.Panel();
            this.targetRow = new System.Windows.Forms.Panel();
            this.txtTargetPath = new DevExpress.XtraEditors.TextEdit();
            this.lblTarget = new DevExpress.XtraEditors.LabelControl();
            this.toolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCount = new DevExpress.XtraEditors.LabelControl();
            this.numCount = new DevExpress.XtraEditors.SpinEdit();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnStop = new DevExpress.XtraEditors.SimpleButton();
            this.lblGridHint = new DevExpress.XtraEditors.LabelControl();
            this.footer = new System.Windows.Forms.Panel();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpLogs)).BeginInit();
            this.grpLogs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLogs)).BeginInit();
            this.selectionBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.details)).BeginInit();
            this.details.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpMessage)).BeginInit();
            this.grpMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMessage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpPaths)).BeginInit();
            this.grpPaths.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listPaths)).BeginInit();
            this.pathTools.SuspendLayout();
            this.header.SuspendLayout();
            this.targetRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetPath.Properties)).BeginInit();
            this.toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount.Properties)).BeginInit();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.Controls.Add(this.splitMain);
            this.layout.Controls.Add(this.header);
            this.layout.Controls.Add(this.footer);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Font = new System.Drawing.Font("SUIT", 10F);
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Margin = new System.Windows.Forms.Padding(0);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(12);
            this.layout.Size = new System.Drawing.Size(1120, 800);
            this.layout.TabIndex = 0;
            // 
            // splitMain
            // 
            this.splitMain.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitMain.AppearanceCaption.Options.UseFont = true;
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.splitMain.Horizontal = false;
            this.splitMain.Location = new System.Drawing.Point(12, 84);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitMain.Panel1.AppearanceCaption.Options.UseFont = true;
            this.splitMain.Panel1.Controls.Add(this.grpLogs);
            this.splitMain.Panel2.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.splitMain.Panel2.AppearanceCaption.Options.UseFont = true;
            this.splitMain.Panel2.Controls.Add(this.details);
            this.splitMain.Size = new System.Drawing.Size(1096, 664);
            this.splitMain.SplitterPosition = 364;
            this.splitMain.TabIndex = 0;
            // 
            // grpLogs
            // 
            this.grpLogs.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpLogs.AppearanceCaption.Options.UseFont = true;
            this.grpLogs.Controls.Add(this.gridLogs);
            this.lblComparisonScope.Name = "lblComparisonScope";
            this.lblComparisonScope.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblComparisonScope.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblComparisonScope.Height = 28;
            this.lblComparisonScope.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblComparisonScope.Appearance.BackColor = System.Drawing.Color.FromArgb(234, 240, 247);
            this.lblComparisonScope.Appearance.ForeColor = System.Drawing.Color.FromArgb(22, 50, 79);
            this.lblComparisonScope.Appearance.Options.UseBackColor = true;
            this.lblComparisonScope.Appearance.Options.UseForeColor = true;
            this.lblComparisonScope.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblComparisonScope.Text = "표시 범위 · 리비전을 선택하세요.";
            this.grpLogs.Controls.Add(this.lblComparisonScope);
            this.grpLogs.Controls.Add(this.selectionBar);
            this.grpLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLogs.Location = new System.Drawing.Point(0, 0);
            this.grpLogs.Margin = new System.Windows.Forms.Padding(0);
            this.grpLogs.Name = "grpLogs";
            this.grpLogs.Padding = new System.Windows.Forms.Padding(8);
            this.grpLogs.Size = new System.Drawing.Size(1096, 364);
            this.grpLogs.TabIndex = 0;
            this.grpLogs.Text = "리비전 목록";
            // 
            // gridLogs
            // 
            this.gridLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLogs.EmbeddedNavigator.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridLogs.EmbeddedNavigator.Appearance.Options.UseFont = true;
            this.gridLogs.Font = new System.Drawing.Font("SUIT", 10F);
            this.gridLogs.Location = new System.Drawing.Point(10, 65);
            this.gridLogs.MainView = this.viewLogs;
            this.gridLogs.Margin = new System.Windows.Forms.Padding(0);
            this.gridLogs.Name = "gridLogs";
            this.gridLogs.Size = new System.Drawing.Size(1076, 289);
            this.gridLogs.TabIndex = 0;
            this.gridLogs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewLogs});
            // 
            // viewLogs
            // 
            this.viewLogs.Appearance.Empty.Font = new System.Drawing.Font("SUIT", 10F);
            this.viewLogs.Appearance.Empty.Options.UseFont = true;
            this.viewLogs.Appearance.HeaderPanel.Font = new System.Drawing.Font("SUIT", 10F);
            this.viewLogs.Appearance.HeaderPanel.Options.UseFont = true;
            this.viewLogs.Appearance.Row.Font = new System.Drawing.Font("SUIT", 10F);
            this.viewLogs.Appearance.Row.Options.UseFont = true;
            this.viewLogs.GridControl = this.gridLogs;
            this.viewLogs.Name = "viewLogs";
            // 
            // selectionBar
            // 
            this.selectionBar.Controls.Add(this.lblSelection);
            this.selectionBar.Controls.Add(this.btnCompare);
            this.selectionBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectionBar.Font = new System.Drawing.Font("SUIT", 10F);
            this.selectionBar.Location = new System.Drawing.Point(10, 31);
            this.selectionBar.Margin = new System.Windows.Forms.Padding(0);
            this.selectionBar.Name = "selectionBar";
            this.selectionBar.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.selectionBar.Size = new System.Drawing.Size(1076, 34);
            this.selectionBar.TabIndex = 1;
            // 
            // lblSelection
            // 
            this.lblSelection.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblSelection.Appearance.Options.UseFont = true;
            this.lblSelection.Appearance.Options.UseTextOptions = true;
            this.lblSelection.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblSelection.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelection.Location = new System.Drawing.Point(0, 3);
            this.lblSelection.Margin = new System.Windows.Forms.Padding(0);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(988, 28);
            this.lblSelection.TabIndex = 0;
            this.lblSelection.Text = "리비전 2개를 선택하면 두 시점을 비교할 수 있습니다.";
            // 
            // btnCompare
            // 
            this.btnCompare.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnCompare.Appearance.Options.UseFont = true;
            this.btnCompare.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCompare.Enabled = false;
            this.btnCompare.Location = new System.Drawing.Point(988, 3);
            this.btnCompare.Margin = new System.Windows.Forms.Padding(0);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(168, 28);
            this.btnCompare.TabIndex = 1;
            this.btnCompare.Text = "선택한 두 시점 비교";
            // 
            // details
            // 
            this.details.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.details.AppearanceCaption.Options.UseFont = true;
            this.details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.details.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.details.Location = new System.Drawing.Point(0, 0);
            this.details.Name = "details";
            this.details.Panel1.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.details.Panel1.AppearanceCaption.Options.UseFont = true;
            this.details.Panel1.Controls.Add(this.grpMessage);
            this.details.Panel2.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.details.Panel2.AppearanceCaption.Options.UseFont = true;
            this.details.Panel2.Controls.Add(this.grpPaths);
            this.details.Size = new System.Drawing.Size(1096, 295);
            this.details.SplitterPosition = 513;
            this.details.TabIndex = 0;
            // 
            // grpMessage
            // 
            this.grpMessage.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpMessage.AppearanceCaption.Options.UseFont = true;
            this.grpMessage.Controls.Add(this.txtMessage);
            this.grpMessage.Controls.Add(this.lblLogInfo);
            this.grpMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMessage.Location = new System.Drawing.Point(0, 0);
            this.grpMessage.Margin = new System.Windows.Forms.Padding(0);
            this.grpMessage.Name = "grpMessage";
            this.grpMessage.Padding = new System.Windows.Forms.Padding(8);
            this.grpMessage.Size = new System.Drawing.Size(513, 295);
            this.grpMessage.TabIndex = 0;
            this.grpMessage.Text = "커밋 메시지";
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(10, 63);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(0);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtMessage.Properties.Appearance.Options.UseFont = true;
            this.txtMessage.Properties.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(493, 222);
            this.txtMessage.TabIndex = 0;
            // 
            // lblLogInfo
            // 
            this.lblLogInfo.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblLogInfo.Appearance.Options.UseFont = true;
            this.lblLogInfo.Appearance.Options.UseTextOptions = true;
            this.lblLogInfo.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblLogInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLogInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLogInfo.Location = new System.Drawing.Point(10, 31);
            this.lblLogInfo.Margin = new System.Windows.Forms.Padding(0);
            this.lblLogInfo.Name = "lblLogInfo";
            this.lblLogInfo.Size = new System.Drawing.Size(493, 32);
            this.lblLogInfo.TabIndex = 1;
            this.lblLogInfo.Text = "리비전을 선택하세요.";
            // 
            // grpPaths
            // 
            this.grpPaths.AppearanceCaption.Font = new System.Drawing.Font("SUIT", 10F, System.Drawing.FontStyle.Bold);
            this.grpPaths.AppearanceCaption.Options.UseFont = true;
            this.grpPaths.Controls.Add(this.listPaths);
            this.grpPaths.Controls.Add(this.pathTools);
            this.grpPaths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPaths.Location = new System.Drawing.Point(0, 0);
            this.grpPaths.Margin = new System.Windows.Forms.Padding(0);
            this.grpPaths.Name = "grpPaths";
            this.grpPaths.Padding = new System.Windows.Forms.Padding(8);
            this.grpPaths.Size = new System.Drawing.Size(578, 295);
            this.grpPaths.TabIndex = 0;
            this.grpPaths.Text = "변경 경로 (저장소 기준)";
            // 
            // listPaths
            // 
            this.listPaths.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.listPaths.Appearance.Options.UseFont = true;
            this.listPaths.Cursor = System.Windows.Forms.Cursors.Default;
            this.listPaths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPaths.HorizontalScrollbar = true;
            this.listPaths.Location = new System.Drawing.Point(10, 31);
            this.listPaths.Margin = new System.Windows.Forms.Padding(0);
            this.listPaths.Name = "listPaths";
            this.listPaths.Size = new System.Drawing.Size(558, 220);
            this.listPaths.TabIndex = 0;
            // 
            // pathTools
            // 
            this.pathTools.Controls.Add(this.lblPathHint);
            this.pathTools.Controls.Add(this.btnOpenDiff);
            this.pathTools.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pathTools.Font = new System.Drawing.Font("SUIT", 10F);
            this.pathTools.Location = new System.Drawing.Point(10, 251);
            this.pathTools.Margin = new System.Windows.Forms.Padding(0);
            this.pathTools.Name = "pathTools";
            this.pathTools.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pathTools.Size = new System.Drawing.Size(558, 34);
            this.pathTools.TabIndex = 1;
            // 
            // lblPathHint
            // 
            this.lblPathHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblPathHint.Appearance.Options.UseFont = true;
            this.lblPathHint.Appearance.Options.UseTextOptions = true;
            this.lblPathHint.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblPathHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPathHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPathHint.Location = new System.Drawing.Point(0, 3);
            this.lblPathHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblPathHint.Name = "lblPathHint";
            this.lblPathHint.Size = new System.Drawing.Size(470, 28);
            this.lblPathHint.TabIndex = 0;
            this.lblPathHint.Text = "파일을 더블클릭하면 변경 내용을 볼 수 있습니다.";
            // 
            // btnOpenDiff
            // 
            this.btnOpenDiff.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnOpenDiff.Appearance.Options.UseFont = true;
            this.btnOpenDiff.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOpenDiff.Enabled = false;
            this.btnOpenDiff.Location = new System.Drawing.Point(470, 3);
            this.btnOpenDiff.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpenDiff.Name = "btnOpenDiff";
            this.btnOpenDiff.Size = new System.Drawing.Size(132, 28);
            this.btnOpenDiff.TabIndex = 1;
            this.btnOpenDiff.Text = "변경 내용 보기";
            // 
            // header
            // 
            this.header.Controls.Add(this.targetRow);
            this.header.Controls.Add(this.toolbar);
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Font = new System.Drawing.Font("SUIT", 10F);
            this.header.Location = new System.Drawing.Point(12, 12);
            this.header.Margin = new System.Windows.Forms.Padding(0);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(1096, 72);
            this.header.TabIndex = 1;
            // 
            // targetRow
            // 
            this.targetRow.Controls.Add(this.txtTargetPath);
            this.targetRow.Controls.Add(this.lblTarget);
            this.targetRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.targetRow.Font = new System.Drawing.Font("SUIT", 10F);
            this.targetRow.Location = new System.Drawing.Point(0, 0);
            this.targetRow.Margin = new System.Windows.Forms.Padding(0);
            this.targetRow.Name = "targetRow";
            this.targetRow.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.targetRow.Size = new System.Drawing.Size(1096, 34);
            this.targetRow.TabIndex = 0;
            // 
            // txtTargetPath
            // 
            this.txtTargetPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTargetPath.Location = new System.Drawing.Point(104, 0);
            this.txtTargetPath.Margin = new System.Windows.Forms.Padding(0);
            this.txtTargetPath.Name = "txtTargetPath";
            this.txtTargetPath.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.txtTargetPath.Properties.Appearance.Options.UseFont = true;
            this.txtTargetPath.Properties.AutoHeight = false;
            this.txtTargetPath.Properties.ReadOnly = true;
            this.txtTargetPath.Size = new System.Drawing.Size(992, 28);
            this.txtTargetPath.TabIndex = 0;
            // 
            // lblTarget
            // 
            this.lblTarget.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblTarget.Appearance.Options.UseFont = true;
            this.lblTarget.Appearance.Options.UseTextOptions = true;
            this.lblTarget.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblTarget.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTarget.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTarget.Location = new System.Drawing.Point(0, 0);
            this.lblTarget.Margin = new System.Windows.Forms.Padding(0);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(104, 28);
            this.lblTarget.TabIndex = 1;
            this.lblTarget.Text = "조회 대상";
            // 
            // toolbar
            // 
            this.toolbar.Controls.Add(this.lblCount);
            this.toolbar.Controls.Add(this.numCount);
            this.toolbar.Controls.Add(this.btnRefresh);
            this.toolbar.Controls.Add(this.btnStop);
            this.toolbar.Controls.Add(this.lblGridHint);
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolbar.Font = new System.Drawing.Font("SUIT", 10F);
            this.toolbar.Location = new System.Drawing.Point(0, 36);
            this.toolbar.Margin = new System.Windows.Forms.Padding(0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.toolbar.Size = new System.Drawing.Size(1096, 36);
            this.toolbar.TabIndex = 1;
            this.toolbar.WrapContents = false;
            // 
            // lblCount
            // 
            this.lblCount.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.lblCount.Appearance.Options.UseFont = true;
            this.lblCount.Appearance.Options.UseTextOptions = true;
            this.lblCount.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblCount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCount.Location = new System.Drawing.Point(0, 4);
            this.lblCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(104, 28);
            this.lblCount.TabIndex = 0;
            this.lblCount.Text = "조회 건수";
            // 
            // numCount
            // 
            this.numCount.EditValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numCount.Location = new System.Drawing.Point(76, 4);
            this.numCount.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.numCount.Name = "numCount";
            this.numCount.Properties.Appearance.Font = new System.Drawing.Font("SUIT", 10F);
            this.numCount.Properties.Appearance.Options.UseFont = true;
            this.numCount.Properties.AutoHeight = false;
            this.numCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numCount.Properties.IsFloatValue = false;
            this.numCount.Properties.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numCount.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCount.Size = new System.Drawing.Size(88, 28);
            this.numCount.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.Location = new System.Drawing.Point(164, 4);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(88, 28);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "새로고침";
            // 
            // btnStop
            // 
            this.btnStop.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnStop.Appearance.Options.UseFont = true;
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(252, 4);
            this.btnStop.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(88, 28);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "조회 중단";
            // 
            // lblGridHint
            // 
            this.lblGridHint.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblGridHint.Appearance.Options.UseFont = true;
            this.lblGridHint.Appearance.Options.UseTextOptions = true;
            this.lblGridHint.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblGridHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGridHint.Location = new System.Drawing.Point(340, 4);
            this.lblGridHint.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblGridHint.Name = "lblGridHint";
            this.lblGridHint.Size = new System.Drawing.Size(440, 28);
            this.lblGridHint.TabIndex = 4;
            this.lblGridHint.Text = "목록의 필터 행에서 작성자와 메시지를 검색할 수 있습니다.";
            // 
            // footer
            // 
            this.footer.Controls.Add(this.lblStatus);
            this.footer.Controls.Add(this.btnClose);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Font = new System.Drawing.Font("SUIT", 10F);
            this.footer.Location = new System.Drawing.Point(12, 748);
            this.footer.Margin = new System.Windows.Forms.Padding(0);
            this.footer.Name = "footer";
            this.footer.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.footer.Size = new System.Drawing.Size(1096, 40);
            this.footer.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.lblStatus.Appearance.Options.UseFont = true;
            this.lblStatus.Appearance.Options.UseTextOptions = true;
            this.lblStatus.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(0, 6);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1008, 28);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "조회 준비";
            // 
            // btnClose
            // 
            this.btnClose.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.Location = new System.Drawing.Point(1008, 6);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 28);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "닫기";
            // 
            // LogViewerView
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1120, 800);
            this.Controls.Add(this.layout);
            this.MinimumSize = new System.Drawing.Size(940, 680);
            this.ShowIcon = false;
            this.Name = "LogViewerView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SVN 로그";
            this.layout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpLogs)).EndInit();
            this.grpLogs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLogs)).EndInit();
            this.selectionBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.details)).EndInit();
            this.details.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpMessage)).EndInit();
            this.grpMessage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMessage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpPaths)).EndInit();
            this.grpPaths.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listPaths)).EndInit();
            this.pathTools.ResumeLayout(false);
            this.header.ResumeLayout(false);
            this.targetRow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTargetPath.Properties)).EndInit();
            this.toolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCount.Properties)).EndInit();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

