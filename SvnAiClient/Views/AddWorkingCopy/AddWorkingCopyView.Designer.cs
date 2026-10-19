using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SvnAiClient.Views.AddWorkingCopy
{
    partial class AddWorkingCopyView
    {
        #region Fields

        /// <summary>디자이너 구성 요소의 수명을 관리한다.</summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>기본 정보와 인증 그룹 사이의 공통 간격.</summary>
        private System.Windows.Forms.Panel groupGap;
        /// <summary>기본 정보, 인증 정보 및 하단 버튼 배치.</summary>
        private Panel layoutRoot;
        /// <summary>작업 복사본 경로와 표시 이름 입력 영역.</summary>
        private GroupControl grpWorkingCopy;
        /// <summary>경로와 표시 이름 입력 행 배치.</summary>
        private Panel layoutWorkingCopy;
        /// <summary>경로 입력과 찾아보기 버튼 배치.</summary>
        private Panel rowLocalPath;
        /// <summary>로컬 경로 표시.</summary>
        private LabelControl lblLocalPath;
        /// <summary>등록할 작업 복사본의 로컬 경로.</summary>
        private TextEdit txtLocalPath;
        /// <summary>찾아보기 버튼.</summary>
        private SimpleButton btnBrowse;
        /// <summary>경로 입력과 찾아보기 버튼 사이의 여백.</summary>
        private Panel browseSpacing;
        /// <summary>목록 표시 이름 입력 행.</summary>
        private Panel rowDisplayName;
        /// <summary>표시 이름 표시.</summary>
        private LabelControl lblDisplayName;
        /// <summary>목록과 폴더 트리에 표시할 이름.</summary>
        private TextEdit txtDisplayName;
        /// <summary>비워두면 폴더 이름으로 표시합니다. 표시.</summary>
        private LabelControl lblDisplayNameHint;
        /// <summary>SVN 인증 사용자명과 비밀번호 입력 영역.</summary>
        private GroupControl grpAuthentication;
        /// <summary>인증 정보 입력 행 배치.</summary>
        private Panel layoutAuthentication;
        /// <summary>사용자명 입력 행.</summary>
        private Panel rowUsername;
        /// <summary>사용자명 표시.</summary>
        private LabelControl lblUsername;
        /// <summary>SVN 인증 사용자명.</summary>
        private TextEdit txtUsername;
        /// <summary>비밀번호 입력 행.</summary>
        private Panel rowPassword;
        /// <summary>비밀번호 표시.</summary>
        private LabelControl lblPassword;
        /// <summary>새로 입력할 비밀번호.</summary>
        private TextEdit txtPassword;
        /// <summary>SVN 인증에 사용할 비밀번호를 입력하세요. 표시.</summary>
        private LabelControl lblPasswordHint;
        /// <summary>등록 또는 저장 및 취소 버튼 배치.</summary>
        private FlowLayoutPanel footer;
        /// <summary>등록 버튼.</summary>
        private SimpleButton btnOk;
        /// <summary>취소 버튼.</summary>
        private SimpleButton btnCancel;

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
            this.groupGap = new System.Windows.Forms.Panel();
            this.layoutRoot = new Panel();
            this.grpWorkingCopy = new GroupControl();
            this.layoutWorkingCopy = new Panel();
            this.rowLocalPath = new Panel();
            this.lblLocalPath = new LabelControl();
            this.txtLocalPath = new TextEdit();
            this.btnBrowse = new SimpleButton();
            this.browseSpacing = new Panel();
            this.rowDisplayName = new Panel();
            this.lblDisplayName = new LabelControl();
            this.txtDisplayName = new TextEdit();
            this.lblDisplayNameHint = new LabelControl();
            this.grpAuthentication = new GroupControl();
            this.layoutAuthentication = new Panel();
            this.rowUsername = new Panel();
            this.lblUsername = new LabelControl();
            this.txtUsername = new TextEdit();
            this.rowPassword = new Panel();
            this.lblPassword = new LabelControl();
            this.txtPassword = new TextEdit();
            this.lblPasswordHint = new LabelControl();
            this.footer = new FlowLayoutPanel();
            this.btnOk = new SimpleButton();
            this.btnCancel = new SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpWorkingCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpAuthentication)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            this.layoutRoot.SuspendLayout();
            this.grpWorkingCopy.SuspendLayout();
            this.layoutWorkingCopy.SuspendLayout();
            this.rowLocalPath.SuspendLayout();
            this.browseSpacing.SuspendLayout();
            this.rowDisplayName.SuspendLayout();
            this.grpAuthentication.SuspendLayout();
            this.layoutAuthentication.SuspendLayout();
            this.rowUsername.SuspendLayout();
            this.rowPassword.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.Dock = DockStyle.Fill;
            this.grpWorkingCopy.Name = "grpWorkingCopy";
            this.grpWorkingCopy.Dock = DockStyle.Top;
            this.grpWorkingCopy.Text = "작업 폴더";
            this.grpWorkingCopy.TabIndex = 0;
            this.layoutWorkingCopy.Name = "layoutWorkingCopy";
            this.layoutWorkingCopy.Dock = DockStyle.Fill;
            this.rowLocalPath.Name = "rowLocalPath";
            this.rowLocalPath.Dock = DockStyle.Top;
            this.rowLocalPath.TabIndex = 0;
            this.lblLocalPath.Name = "lblLocalPath";
            this.lblLocalPath.Text = "로컬 경로";
            this.lblLocalPath.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblLocalPath.Dock = DockStyle.Left;
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.Dock = DockStyle.Fill;
            this.txtLocalPath.TabIndex = 0;
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Text = "찾아보기";
            this.btnBrowse.Dock = DockStyle.Right;
            this.btnBrowse.TabIndex = 1;
            this.browseSpacing.Name = "browseSpacing";
            this.browseSpacing.Dock = DockStyle.Right;
            this.browseSpacing.Width = 6;
            this.rowDisplayName.Name = "rowDisplayName";
            this.rowDisplayName.Dock = DockStyle.Top;
            this.rowDisplayName.TabIndex = 1;
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Text = "표시 이름";
            this.lblDisplayName.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblDisplayName.Dock = DockStyle.Left;
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Dock = DockStyle.Fill;
            this.txtDisplayName.TabIndex = 0;
            this.lblDisplayNameHint.Name = "lblDisplayNameHint";
            this.lblDisplayNameHint.Text = "비워두면 폴더 이름으로 표시합니다.";
            this.lblDisplayNameHint.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblDisplayNameHint.Dock = DockStyle.Top;
            this.grpAuthentication.Name = "grpAuthentication";
            this.grpAuthentication.Dock = DockStyle.Fill;
            this.grpAuthentication.Text = "SVN 인증";
            this.grpAuthentication.TabIndex = 1;
            this.layoutAuthentication.Name = "layoutAuthentication";
            this.layoutAuthentication.Dock = DockStyle.Fill;
            this.rowUsername.Name = "rowUsername";
            this.rowUsername.Dock = DockStyle.Top;
            this.rowUsername.TabIndex = 0;
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Text = "사용자명";
            this.lblUsername.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblUsername.Dock = DockStyle.Left;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Dock = DockStyle.Fill;
            this.txtUsername.TabIndex = 0;
            this.rowPassword.Name = "rowPassword";
            this.rowPassword.Dock = DockStyle.Top;
            this.rowPassword.TabIndex = 1;
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Text = "비밀번호";
            this.lblPassword.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblPassword.Dock = DockStyle.Left;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Dock = DockStyle.Fill;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.TabIndex = 0;
            this.lblPasswordHint.Name = "lblPasswordHint";
            this.lblPasswordHint.Text = "SVN 인증에 사용할 비밀번호를 입력하세요.";
            this.lblPasswordHint.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblPasswordHint.Dock = DockStyle.Top;
            this.lblPasswordHint.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.footer.Name = "footer";
            this.footer.Dock = DockStyle.Bottom;
            this.footer.FlowDirection = FlowDirection.RightToLeft;
            this.footer.WrapContents = false;
            this.footer.TabIndex = 2;
            this.btnOk.Name = "btnOk";
            this.btnOk.Text = "등록";
            this.btnOk.TabIndex = 0;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Text = "취소";
            this.btnCancel.TabIndex = 1;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.rowLocalPath.Controls.Add(this.txtLocalPath);
            this.rowLocalPath.Controls.Add(this.browseSpacing);
            this.rowLocalPath.Controls.Add(this.btnBrowse);
            this.rowLocalPath.Controls.Add(this.lblLocalPath);
            this.rowDisplayName.Controls.Add(this.txtDisplayName);
            this.rowDisplayName.Controls.Add(this.lblDisplayName);
            this.layoutWorkingCopy.Controls.Add(this.lblDisplayNameHint);
            this.layoutWorkingCopy.Controls.Add(this.rowDisplayName);
            this.layoutWorkingCopy.Controls.Add(this.rowLocalPath);
            this.grpWorkingCopy.Controls.Add(this.layoutWorkingCopy);
            this.rowUsername.Controls.Add(this.txtUsername);
            this.rowUsername.Controls.Add(this.lblUsername);
            this.rowPassword.Controls.Add(this.txtPassword);
            this.rowPassword.Controls.Add(this.lblPassword);
            this.layoutAuthentication.Controls.Add(this.lblPasswordHint);
            this.layoutAuthentication.Controls.Add(this.rowPassword);
            this.layoutAuthentication.Controls.Add(this.rowUsername);
            this.grpAuthentication.Controls.Add(this.layoutAuthentication);
            this.footer.Controls.Add(this.btnCancel);
            this.footer.Controls.Add(this.btnOk);
            this.layoutRoot.Controls.Add(this.grpAuthentication);
            this.groupGap.Name = "groupGap";
            this.groupGap.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupGap.Height = 6;
            this.groupGap.Margin = new System.Windows.Forms.Padding(0);
            this.layoutRoot.Controls.Add(this.groupGap);
            this.layoutRoot.Controls.Add(this.grpWorkingCopy);
            this.layoutRoot.Controls.Add(this.footer);
            // Common theme: outer 12, group 8, gap 6, input/button 28.
            this.layoutWorkingCopy.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Size = new System.Drawing.Size(88, 28);
            this.btnCancel.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.rowPassword.Margin = new System.Windows.Forms.Padding(0);
            this.lblDisplayNameHint.Margin = new System.Windows.Forms.Padding(0);
            this.rowLocalPath.Margin = new System.Windows.Forms.Padding(0);
            this.lblDisplayName.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowse.Size = new System.Drawing.Size(88, 28);
            this.btnBrowse.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnBrowse.Appearance.Options.UseFont = true;
            this.lblUsername.Margin = new System.Windows.Forms.Padding(0);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(0);
            this.txtUsername.Properties.AutoHeight = false;
            this.browseSpacing.Margin = new System.Windows.Forms.Padding(0);
            this.lblPasswordHint.Margin = new System.Windows.Forms.Padding(0);
            this.layoutRoot.Margin = new System.Windows.Forms.Padding(0);
            this.layoutAuthentication.Margin = new System.Windows.Forms.Padding(0);
            this.txtDisplayName.Margin = new System.Windows.Forms.Padding(0);
            this.txtDisplayName.Properties.AutoHeight = false;
            this.rowDisplayName.Margin = new System.Windows.Forms.Padding(0);
            this.grpAuthentication.Margin = new System.Windows.Forms.Padding(0);
            this.grpAuthentication.Padding = new System.Windows.Forms.Padding(8);
            this.rowUsername.Margin = new System.Windows.Forms.Padding(0);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(0);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(0);
            this.txtPassword.Properties.AutoHeight = false;
            this.btnOk.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnOk.Size = new System.Drawing.Size(88, 28);
            this.btnOk.Appearance.Font = new System.Drawing.Font("SUIT", 9F);
            this.btnOk.Appearance.Options.UseFont = true;
            this.grpWorkingCopy.Margin = new System.Windows.Forms.Padding(0);
            this.grpWorkingCopy.Padding = new System.Windows.Forms.Padding(8);
            this.txtLocalPath.Margin = new System.Windows.Forms.Padding(0);
            this.txtLocalPath.Properties.AutoHeight = false;
            this.footer.Margin = new System.Windows.Forms.Padding(0);
            this.lblLocalPath.Margin = new System.Windows.Forms.Padding(0);
            this.layoutRoot.Padding = new System.Windows.Forms.Padding(12);
            this.lblLocalPath.Width = 96;
            this.lblDisplayName.Width = 96;
            this.lblUsername.Width = 96;
            this.lblPassword.Width = 96;
            this.footer.Height = 40;
            this.footer.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.rowLocalPath.Height = 34;
            this.rowLocalPath.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.rowDisplayName.Height = 34;
            this.rowDisplayName.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.rowUsername.Height = 34;
            this.rowUsername.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.rowPassword.Height = 34;
            this.rowPassword.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.grpWorkingCopy.Height = 132;
            this.lblDisplayNameHint.Height = 22;
            this.lblPasswordHint.Height = 26;
            this.lblDisplayNameHint.Padding = new System.Windows.Forms.Padding(96, 0, 0, 0);
            this.lblPasswordHint.Padding = new System.Windows.Forms.Padding(96, 0, 0, 0);
            this.Controls.Add(this.layoutRoot);
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(620, 340);
            this.MinimumSize = new System.Drawing.Size(636, 376);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.Name = "AddWorkingCopyView";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "작업 폴더 등록";
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpAuthentication)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLocalPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpWorkingCopy)).EndInit();
            this.footer.ResumeLayout(false);
            this.footer.PerformLayout();
            this.rowPassword.ResumeLayout(false);
            this.rowUsername.ResumeLayout(false);
            this.layoutAuthentication.ResumeLayout(false);
            this.grpAuthentication.ResumeLayout(false);
            this.rowDisplayName.ResumeLayout(false);
            this.browseSpacing.ResumeLayout(false);
            this.rowLocalPath.ResumeLayout(false);
            this.layoutWorkingCopy.ResumeLayout(false);
            this.grpWorkingCopy.ResumeLayout(false);
            this.layoutRoot.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}

