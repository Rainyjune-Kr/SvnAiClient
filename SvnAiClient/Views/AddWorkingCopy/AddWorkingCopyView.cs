using System;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraEditors;
using SvnAiClient.Helpers;
using SvnAiClient.Models;
using SvnAiClient.Views.Common;

namespace SvnAiClient.Views.AddWorkingCopy
{
    /// <summary>작업 복사본 등록·편집을 위한 입력과 폴더 선택 UI.</summary>
    public partial class AddWorkingCopyView : ThemedView, AddWorkingCopyInterface
    {
        #region Fields

        /// <summary>입력 검증과 등록·편집 결과 생성을 위임하는 Presenter.</summary>
        private readonly AddWorkingCopyPresenter _presenter;

        /// <summary>화면의 본문과 버튼에 사용하는 공유 글꼴.</summary>
        private readonly ApplicationFonts _applicationFonts;

        /// <summary>호출자가 글꼴을 전달하지 않으면 화면 종료 시 직접 해제한다.</summary>
        private readonly bool _ownsFonts;

        #endregion

        #region Properties

        /// <summary>사용자가 승인한 작업 복사본 항목.</summary>
        public WorkingCopyEntry Result { get; private set; }

        /// <summary>작업 복사본의 로컬 경로 입력값.</summary>
        public string LocalPath
        {
            get { return txtLocalPath.Text; }
            set { txtLocalPath.Text = value; }
        }

        /// <summary>작업 복사본 목록에 사용할 표시 이름 입력값.</summary>
        public string DisplayName
        {
            get { return txtDisplayName.Text; }
            set { txtDisplayName.Text = value; }
        }

        /// <summary>SVN 인증 사용자명 입력값.</summary>
        public string Username
        {
            get { return txtUsername.Text; }
            set { txtUsername.Text = value; }
        }

        /// <summary>새 비밀번호 입력값. 기존 비밀번호를 화면에 복호화해 표시하지 않는다.</summary>
        public string Password { get { return txtPassword.Text; } }

        #endregion

        #region Initialization

        public AddWorkingCopyView() : this(null, null)
        {
        }

        public AddWorkingCopyView(WorkingCopyEntry existing, ApplicationFonts fonts)
        {
            InitializeComponent();
            _applicationFonts = fonts ?? new ApplicationFonts();
            _ownsFonts = fonts == null;
            InitializeFonts(_applicationFonts);
            _presenter = new AddWorkingCopyPresenter(this);
            btnBrowse.Click += btnBrowse_Click;
            btnOk.Click += btnOk_Click;
            btnCancel.Click += btnCancel_Click;
            ActiveControl = txtLocalPath;
            if (existing != null) _presenter.LoadExisting(existing);
        }

        public AddWorkingCopyView(WorkingCopyEntry existing) : this()
        {
            _presenter.LoadExisting(existing);
        }

        private void InitializeFonts(ApplicationFonts fonts)
        {
            ApplyControlFonts(this, fonts);
            foreach (var label in new[] { lblDisplayNameHint, lblPasswordHint })
                label.Appearance.Font = fonts.GetFont(9F);
            btnOk.Appearance.Font = fonts.GetFont(9F, FontStyle.Bold);
        }

        #endregion

        #region Event Handlers

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            _presenter.Browse();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _presenter.Save();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _presenter.Cancel();
        }

        #endregion

        #region Control Methods

        public string SelectFolder(string initialPath)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "SVN 작업 폴더 선택";
                if (!string.IsNullOrEmpty(initialPath))
                    dialog.SelectedPath = initialPath;
                return dialog.ShowDialog(this) == DialogResult.OK ? dialog.SelectedPath : null;
            }
        }

        public void ShowExisting(WorkingCopyEntry entry)
        {
            Text = "작업 폴더 편집";
            btnOk.Text = "저장";
            LocalPath = entry.LocalPath;
            DisplayName = entry.DisplayName;
            Username = entry.Username;
            txtPassword.Text = string.Empty;
            lblPasswordHint.Text = "비워두면 기존 비밀번호를 유지합니다.";
        }

        public void ShowValidation(string message)
        {
            XtraMessageBox.Show(this, message, "알림");
        }

        public void FocusLocalPath()
        {
            txtLocalPath.Focus();
        }

        public void Complete(WorkingCopyEntry entry)
        {
            Result = entry;
            DialogResult = DialogResult.OK;
            Close();
        }

        public void Cancel()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        #region Operations

        /// <summary>본문과 버튼 및 DevExpress 표시 속성에 공유 글꼴을 적용한다.</summary>
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
