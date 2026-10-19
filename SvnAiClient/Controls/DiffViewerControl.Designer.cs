namespace SvnAiClient.Controls
{
    partial class DiffViewerControl
    {
        #region Fields

        /// <summary>디자이너가 생성한 구성 요소의 수명을 관리한다.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>원본과 수정본 영역을 나누는 분할 컨테이너.</summary>
        private System.Windows.Forms.SplitContainer mainSplit;

        /// <summary>원본 라벨과 편집기를 배치할 패널.</summary>
        private System.Windows.Forms.Panel panelLeft;

        /// <summary>수정본 라벨과 편집기를 배치할 패널.</summary>
        private System.Windows.Forms.Panel panelRight;

        /// <summary>원본 소스와 Diff 마커를 표시할 편집기.</summary>
        private DiffScintilla scintLeft;

        /// <summary>수정본 소스와 Diff 마커를 표시할 편집기.</summary>
        private DiffScintilla scintRight;

        /// <summary>원본 또는 이전 리비전의 안내 라벨.</summary>
        private System.Windows.Forms.Label lblLeft;

        /// <summary>수정본 또는 이후 리비전의 안내 라벨.</summary>
        private System.Windows.Forms.Label lblRight;

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
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.scintLeft = new SvnAiClient.Controls.DiffScintilla();
            this.lblLeft = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.scintRight = new SvnAiClient.Controls.DiffScintilla();
            this.lblRight = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.Location = new System.Drawing.Point(0, 0);
            this.mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.Controls.Add(this.panelLeft);
            this.mainSplit.Panel1MinSize = 100;
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.Controls.Add(this.panelRight);
            this.mainSplit.Panel2MinSize = 100;
            this.mainSplit.Size = new System.Drawing.Size(1758, 868);
            this.mainSplit.SplitterDistance = 879;
            this.mainSplit.TabIndex = 0;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.scintLeft);
            this.panelLeft.Controls.Add(this.lblLeft);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(879, 868);
            this.panelLeft.TabIndex = 0;
            // 
            // scintLeft
            // 
            this.scintLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintLeft.Location = new System.Drawing.Point(0, 22);
            this.scintLeft.Name = "scintLeft";
            this.scintLeft.ReadOnly = true;
            this.scintLeft.ScrollWidthTracking = false;
            this.scintLeft.Size = new System.Drawing.Size(879, 846);
            this.scintLeft.TabIndex = 0;
            // 
            // lblLeft
            // 
            this.lblLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(215)))), ((int)(((byte)(238)))));
            this.lblLeft.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLeft.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.lblLeft.Location = new System.Drawing.Point(0, 0);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(879, 22);
            this.lblLeft.TabIndex = 1;
            this.lblLeft.Text = "Base (원본)";
            this.lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.scintRight);
            this.panelRight.Controls.Add(this.lblRight);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(875, 868);
            this.panelRight.TabIndex = 0;
            // 
            // scintRight
            // 
            this.scintRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintRight.Location = new System.Drawing.Point(0, 22);
            this.scintRight.Name = "scintRight";
            this.scintRight.ReadOnly = true;
            this.scintRight.ScrollWidthTracking = false;
            this.scintRight.Size = new System.Drawing.Size(875, 846);
            this.scintRight.TabIndex = 0;
            // 
            // lblRight
            // 
            this.lblRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(239)))), ((int)(((byte)(206)))));
            this.lblRight.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRight.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.lblRight.Location = new System.Drawing.Point(0, 0);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(875, 22);
            this.lblRight.TabIndex = 1;
            this.lblRight.Text = "Working (수정본)";
            this.lblRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DiffViewerControl
            // 
            this.Controls.Add(this.mainSplit);
            this.Name = "DiffViewerControl";
            this.Size = new System.Drawing.Size(1758, 868);
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
