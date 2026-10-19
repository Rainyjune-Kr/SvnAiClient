using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
using SvnAiClient.Helpers;

namespace SvnAiClient.Views.Common
{
    /// <summary>실행 화면과 디자이너에 공통 글꼴, 버튼 및 영역 제목 스타일을 적용하는 기반 View.</summary>
    public class ThemedView : XtraForm
    {
        #region Initialization

        public ThemedView()
        {
            Font = ViewTheme.GetFont(10F);
        }

        #endregion

        #region Event Handlers

        protected override void OnLoad(System.EventArgs e)
        {
            ViewTheme.ApplyStyles(this);
            base.OnLoad(e);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || (Site != null && Site.DesignMode))
                ViewTheme.ApplyFonts(e.Control);
        }

        #endregion
    }
}

