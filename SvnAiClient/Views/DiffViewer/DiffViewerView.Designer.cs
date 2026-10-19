using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;

namespace SvnAiClient.Views.DiffViewer
{
    partial class DiffViewerView
    {
        #region Fields

        /// <summary>디자이너 구성 요소의 수명을 관리한다.</summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>경로, 이동 도구, 비교 탭 및 하단 상태 배치.</summary>
        private Panel layoutRoot;
        /// <summary>비교 파일 경로와 변경 위치 이동 도구.</summary>
        private Panel header;
        /// <summary>비교 파일 경로 배치.</summary>
        private Panel pathRow;
        /// <summary>비교 파일 표시.</summary>
        private LabelControl lblPathCaption;
        /// <summary>복사 가능한 비교 대상의 전체 경로.</summary>
        private TextEdit txtFilePath;
        /// <summary>변경 구간 이동 버튼과 색상 범례 배치.</summary>
        private FlowLayoutPanel navigationBar;
        /// <summary>현재 비교의 원본과 수정본 기준을 표시한다.</summary>
        private LabelControl lblComparisonScope;
        /// <summary>색상 범례의 표시를 전환한다.</summary>
        private SimpleButton btnToggleLegend;
        /// <summary>필요할 때만 펼쳐 표시하는 색상 범례.</summary>
        private FlowLayoutPanel legendBar;
        /// <summary>이전 변경 (Shift+F7) 버튼.</summary>
        private SimpleButton btnPreviousChange;
        /// <summary>다음 변경 (F7) 버튼.</summary>
        private SimpleButton btnNextChange;
        /// <summary>변경 0 / 0 표시.</summary>
        private LabelControl lblChangePosition;
        /// <summary>삭제 표시.</summary>
        private LabelControl lblRemoved;
        /// <summary>추가 표시.</summary>
        private LabelControl lblAdded;
        /// <summary>수정 표시.</summary>
        private LabelControl lblModified;
        /// <summary>정렬 빈 줄 표시.</summary>
        private LabelControl lblEmpty;
        /// <summary>좌우 비교와 Diff 원문 표시 탭.</summary>
        private XtraTabControl tabs;
        /// <summary>좌우 비교 탭.</summary>
        private XtraTabPage tabCompare;
        /// <summary>Diff 원문 탭.</summary>
        private XtraTabPage tabRaw;
        /// <summary>원본과 수정본 코드 및 실제 줄번호 표시.</summary>
        private SvnAiClient.Controls.DiffViewerControl diffViewer;
        /// <summary>읽기 전용 Diff 원문.</summary>
        private MemoEdit txtRawDiff;
        /// <summary>Diff 종류와 위치 안내 및 닫기 버튼.</summary>
        private Panel footer;
        /// <summary>변경 비교 준비 표시.</summary>
        private LabelControl lblStatus;
        /// <summary>닫기 버튼.</summary>
        private SimpleButton btnClose;

        #endregion

        #region Initialization

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null) components.Dispose();
                base.Dispose(disposing);
            }
            finally
            {
                if (disposing && _ownsFonts && _applicationFonts != null) _applicationFonts.Dispose();
            }
        }

        private void InitializeComponent()
        {
            this.layoutRoot = new Panel();
            this.header = new Panel();
            this.pathRow = new Panel();
            this.lblPathCaption = new LabelControl();
            this.txtFilePath = new TextEdit();
            this.navigationBar = new FlowLayoutPanel();
            this.lblComparisonScope = new LabelControl();
            this.btnToggleLegend = new SimpleButton();
            this.legendBar = new FlowLayoutPanel();
            this.btnPreviousChange = new SimpleButton();
            this.btnNextChange = new SimpleButton();
            this.lblChangePosition = new LabelControl();
            this.lblRemoved = new LabelControl();
            this.lblAdded = new LabelControl();
            this.lblModified = new LabelControl();
            this.lblEmpty = new LabelControl();
            this.tabs = new XtraTabControl();
            this.tabCompare = new XtraTabPage();
            this.tabRaw = new XtraTabPage();
            this.diffViewer = new SvnAiClient.Controls.DiffViewerControl();
            this.txtRawDiff = new MemoEdit();
            this.footer = new Panel();
            this.lblStatus = new LabelControl();
            this.btnClose = new SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRawDiff.Properties)).BeginInit();
            this.layoutRoot.SuspendLayout();
            this.header.SuspendLayout();
            this.pathRow.SuspendLayout();
            this.navigationBar.SuspendLayout();
            this.legendBar.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabCompare.SuspendLayout();
            this.tabRaw.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.Dock = DockStyle.Fill;
            this.header.Name = "header";
            this.header.Dock = DockStyle.Top;
            this.pathRow.Name = "pathRow";
            this.pathRow.Dock = DockStyle.Top;
            this.lblPathCaption.Name = "lblPathCaption";
            this.lblPathCaption.Text = "비교 파일";
            this.lblPathCaption.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblPathCaption.Size = new Size(124, 32);
            this.lblPathCaption.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblPathCaption.Dock = DockStyle.Left;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Dock = DockStyle.Fill;
            this.txtFilePath.Properties.ReadOnly = true;
            this.navigationBar.Name = "navigationBar";
            this.navigationBar.Dock = DockStyle.Bottom;
            this.navigationBar.WrapContents = false;
            this.btnPreviousChange.Name = "btnPreviousChange";
            this.btnPreviousChange.Text = "이전 변경 (Shift+F7)";
            this.btnNextChange.Name = "btnNextChange";
            this.btnNextChange.Text = "다음 변경 (F7)";
            this.lblChangePosition.Name = "lblChangePosition";
            this.lblChangePosition.Text = "변경 0 / 0";
            this.lblChangePosition.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblChangePosition.Size = new Size(124, 32);
            this.lblChangePosition.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblRemoved.Name = "lblRemoved";
            this.lblRemoved.Text = "삭제";
            this.lblRemoved.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblRemoved.Size = new Size(124, 32);
            this.lblRemoved.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblRemoved.Size = new Size(58, 26);
            this.lblRemoved.Appearance.BackColor = Color.FromArgb(255, 199, 206);
            this.lblRemoved.Appearance.Options.UseBackColor = true;
            this.lblRemoved.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblAdded.Name = "lblAdded";
            this.lblAdded.Text = "추가";
            this.lblAdded.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblAdded.Size = new Size(124, 32);
            this.lblAdded.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblAdded.Size = new Size(58, 26);
            this.lblAdded.Appearance.BackColor = Color.FromArgb(198, 239, 206);
            this.lblAdded.Appearance.Options.UseBackColor = true;
            this.lblAdded.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblModified.Name = "lblModified";
            this.lblModified.Text = "수정";
            this.lblModified.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblModified.Size = new Size(124, 32);
            this.lblModified.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblModified.Size = new Size(58, 26);
            this.lblModified.Appearance.BackColor = Color.FromArgb(255, 235, 156);
            this.lblModified.Appearance.Options.UseBackColor = true;
            this.lblModified.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Text = "정렬 빈 줄";
            this.lblEmpty.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblEmpty.Size = new Size(124, 32);
            this.lblEmpty.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblEmpty.Size = new Size(94, 26);
            this.lblEmpty.Appearance.BackColor = Color.FromArgb(220, 220, 220);
            this.lblEmpty.Appearance.Options.UseBackColor = true;
            this.lblEmpty.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tabs.Name = "tabs";
            this.tabs.Dock = DockStyle.Fill;
            this.tabCompare.Name = "tabCompare";
            this.tabCompare.Text = "좌우 비교";
            this.tabRaw.Name = "tabRaw";
            this.tabRaw.Text = "변경 내용 원문";
            this.tabs.SelectedTabPage = this.tabCompare;
            this.diffViewer.Name = "diffViewer";
            this.diffViewer.Dock = DockStyle.Fill;
            this.txtRawDiff.Name = "txtRawDiff";
            this.txtRawDiff.Dock = DockStyle.Fill;
            this.txtRawDiff.Properties.ReadOnly = true;
            this.txtRawDiff.Properties.WordWrap = false;
            this.txtRawDiff.Properties.ScrollBars = ScrollBars.Both;
            this.footer.Name = "footer";
            this.footer.Dock = DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "변경 비교 준비";
            this.lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblStatus.Size = new Size(124, 32);
            this.lblStatus.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblStatus.Dock = DockStyle.Fill;
            this.btnClose.Name = "btnClose";
            this.btnClose.Text = "닫기";
            this.btnClose.Dock = DockStyle.Right;
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.tabs.TabPages.Add(this.tabCompare);
            this.tabs.TabPages.Add(this.tabRaw);
            this.pathRow.Controls.Add(this.txtFilePath);
            this.pathRow.Controls.Add(this.lblPathCaption);
            this.lblComparisonScope.Name = "lblComparisonScope";
            this.lblComparisonScope.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblComparisonScope.Size = new Size(380, 28);
            this.lblComparisonScope.Padding = new Padding(8, 0, 8, 0);
            this.lblComparisonScope.Margin = new Padding(0, 0, 6, 0);
            this.lblComparisonScope.Appearance.BackColor = Color.FromArgb(234, 240, 247);
            this.lblComparisonScope.Appearance.ForeColor = Color.FromArgb(22, 50, 79);
            this.lblComparisonScope.Appearance.Options.UseBackColor = true;
            this.lblComparisonScope.Appearance.Options.UseForeColor = true;
            this.lblComparisonScope.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblComparisonScope.Text = "원본 (BASE) → 작업 파일";
            this.btnToggleLegend.Name = "btnToggleLegend";
            this.btnToggleLegend.Size = new Size(88, 28);
            this.btnToggleLegend.Margin = new Padding(0);
            this.btnToggleLegend.Text = "범례 보기";
            this.legendBar.Name = "legendBar";
            this.legendBar.Dock = DockStyle.Bottom;
            this.legendBar.Height = 30;
            this.legendBar.WrapContents = false;
            this.legendBar.Padding = new Padding(0, 3, 0, 3);
            this.legendBar.Visible = false;
            this.navigationBar.Controls.Add(this.lblComparisonScope);
            this.navigationBar.Controls.Add(this.btnPreviousChange);
            this.navigationBar.Controls.Add(this.btnNextChange);
            this.navigationBar.Controls.Add(this.lblChangePosition);
            this.legendBar.Controls.Add(this.lblRemoved);
            this.legendBar.Controls.Add(this.lblAdded);
            this.legendBar.Controls.Add(this.lblModified);
            this.legendBar.Controls.Add(this.lblEmpty);
            this.header.Controls.Add(this.pathRow);
            this.navigationBar.Controls.Add(this.btnToggleLegend);
            this.header.Controls.Add(this.navigationBar);
            this.header.Controls.Add(this.legendBar);
            this.tabCompare.Controls.Add(this.diffViewer);
            this.tabRaw.Controls.Add(this.txtRawDiff);
            this.footer.Controls.Add(this.lblStatus);
            this.footer.Controls.Add(this.btnClose);
            this.layoutRoot.Controls.Add(this.tabs);
            this.layoutRoot.Controls.Add(this.header);
            this.layoutRoot.Controls.Add(this.footer);
            // Common theme: outer 12, group 8, gap 6, input/button 28.
            this.pathRow.Margin = new System.Windows.Forms.Padding(0);
            this.lblModified.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0);
            this.diffViewer.Margin = new System.Windows.Forms.Padding(0);
            this.tabCompare.Margin = new System.Windows.Forms.Padding(0);
            this.tabCompare.Padding = new System.Windows.Forms.Padding(8);
            this.lblPathCaption.Margin = new System.Windows.Forms.Padding(0);
            this.btnPreviousChange.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnPreviousChange.Size = new System.Drawing.Size(156, 28);
            this.btnPreviousChange.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnPreviousChange.Appearance.Options.UseFont = true;
            this.btnNextChange.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnNextChange.Size = new System.Drawing.Size(136, 28);
            this.btnNextChange.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnNextChange.Appearance.Options.UseFont = true;
            this.lblRemoved.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.lblEmpty.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.tabRaw.Margin = new System.Windows.Forms.Padding(0);
            this.tabRaw.Padding = new System.Windows.Forms.Padding(8);
            this.lblAdded.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.txtFilePath.Margin = new System.Windows.Forms.Padding(0);
            this.txtFilePath.Properties.AutoHeight = false;
            this.layoutRoot.Margin = new System.Windows.Forms.Padding(0);
            this.lblChangePosition.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.header.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Size = new System.Drawing.Size(88, 28);
            this.btnClose.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnClose.Appearance.Options.UseFont = true;
            this.tabs.Margin = new System.Windows.Forms.Padding(0);
            this.footer.Margin = new System.Windows.Forms.Padding(0);
            this.navigationBar.Margin = new System.Windows.Forms.Padding(0);
            this.txtRawDiff.Margin = new System.Windows.Forms.Padding(0);
            this.layoutRoot.Padding = new System.Windows.Forms.Padding(12);
            this.header.Height = 72;
            this.pathRow.Height = 34;
            this.pathRow.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.lblPathCaption.Width = 104;
            this.navigationBar.Height = 36;
            this.navigationBar.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.footer.Height = 40;
            this.footer.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.lblChangePosition.Height = 28;
            this.lblRemoved.Height = 24;
            this.lblAdded.Height = 24;
            this.lblModified.Height = 24;
            this.lblEmpty.Height = 24;
            this.lblChangePosition.Width = 112;
            this.Controls.Add(this.layoutRoot);
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(1200, 800);
            this.MinimumSize = new Size(1000, 640);
            this.ShowIcon = false;
            this.Name = "DiffViewerView";
            this.Text = "변경 비교";
            this.StartPosition = FormStartPosition.CenterParent;
            this.CancelButton = this.btnClose;
            ((System.ComponentModel.ISupportInitialize)(this.txtRawDiff.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath.Properties)).EndInit();
            this.footer.ResumeLayout(false);
            this.tabRaw.ResumeLayout(false);
            this.tabCompare.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.navigationBar.ResumeLayout(false);
            this.legendBar.ResumeLayout(false);
            this.navigationBar.PerformLayout();
            this.pathRow.ResumeLayout(false);
            this.header.ResumeLayout(false);
            this.layoutRoot.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}

