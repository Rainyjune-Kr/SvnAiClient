using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraBars;
using SvnAiClient.Controls;

namespace SvnAiClient.Helpers
{
    /// <summary>실행 화면과 Visual Studio 디자이너에 공통 글꼴, 버튼 및 영역 제목 스타일을 제공한다.</summary>
    internal static class ViewTheme
    {
        #region Fields

        /// <summary>주요 실행 버튼과 영역 제목에 사용하는 EZ SVN 남색.</summary>
        private static readonly Color PrimaryColor = Color.FromArgb(22, 50, 79);
        /// <summary>주요 버튼에 마우스를 올렸을 때의 배경색.</summary>
        private static readonly Color PrimaryHoverColor = Color.FromArgb(34, 72, 104);
        /// <summary>주요 버튼을 누르고 있을 때의 배경색.</summary>
        private static readonly Color PrimaryPressedColor = Color.FromArgb(15, 37, 59);
        /// <summary>보조 버튼의 기본 글자색.</summary>
        private static readonly Color TextColor = Color.FromArgb(36, 52, 71);
        /// <summary>보조 버튼의 경계선 색상.</summary>
        private static readonly Color BorderColor = Color.FromArgb(200, 209, 220);
        /// <summary>비활성 버튼의 글자색.</summary>
        private static readonly Color DisabledTextColor = Color.FromArgb(124, 135, 150);

        /// <summary>메모리에서 등록한 GDI+ 글꼴 모음.</summary>
        private static readonly PrivateFontCollection Collection = new PrivateFontCollection();
        /// <summary>글꼴이 사용하는 메모리를 등록 해제까지 보관한다.</summary>
        private static readonly List<IntPtr> Buffers = new List<IntPtr>();
        /// <summary>프로세스 전용 GDI 글꼴 등록 핸들.</summary>
        private static readonly List<IntPtr> Handles = new List<IntPtr>();
        /// <summary>패밀리별로 유지하는 GDI+ 글꼴 패밀리.</summary>
        private static readonly Dictionary<string, FontFamily> Families = new Dictionary<string, FontFamily>();
        /// <summary>화면에서 공유하는 크기·스타일별 글꼴.</summary>
        private static readonly Dictionary<Tuple<string, float, FontStyle>, Font> Fonts =
            new Dictionary<Tuple<string, float, FontStyle>, Font>();
        /// <summary>코드 글꼴 등록을 한 번만 시도한다.</summary>
        private static bool _codeLoaded;

        /// <summary>프로세스 종료와 도메인 종료가 겹쳐도 자원을 한 번만 해제한다.</summary>
        private static bool _released;

        #endregion

        #region Initialization

        static ViewTheme()
        {
            LoadFont("SUIT-Regular.ttf");
            LoadFont("SUIT-Bold.ttf");
            AppDomain.CurrentDomain.DomainUnload += ReleaseFonts;
            AppDomain.CurrentDomain.ProcessExit += ReleaseFonts;
        }

        #endregion

        #region Operations

        /// <summary>폰트 이름으로 재생성하지 않고 등록한 패밀리로 SUIT 글꼴을 반환한다.</summary>
        public static Font GetFont(float size, FontStyle style = FontStyle.Regular)
        {
            return GetFamilyFont("SUIT", size, style, "맑은 고딕");
        }

        /// <summary>코드 영역에 사용할 D2Coding을 등록하여 반환한다.</summary>
        public static Font GetCodeFont(float size = 10F)
        {
            if (!_codeLoaded)
            {
                _codeLoaded = true;
                LoadFont("D2Coding.ttf");
            }
            return GetFamilyFont("D2Coding", size, FontStyle.Regular, "Consolas");
        }

        /// <summary>자식 컨트롤과 DevExpress 표시 속성에 같은 역할별 글꼴을 적용한다.</summary>
        public static void ApplyFonts(Control control)
        {
            if (control is ScintillaNET.Scintilla) return;
            bool button = control is SimpleButton;
            bool hint = control.Name.IndexOf("Hint", StringComparison.OrdinalIgnoreCase) >= 0
                || control.Name == "_status" || control.Name == "lblStatus"
                || control.Name == "lblReadOnly" || control.Name == "lblTokenInfo"
                || control.Name == "lblAiProvider" || control.Name == "lblAiStatus"
                || control.Name == "cboAiProvider";
            float size = button || hint ? 9F : 10F;
            var action = control as SimpleButton;
            bool primary = action != null && IsPrimaryAction(action);
            bool bold = control.Font.Bold || primary || (action != null && action.Appearance.Font.Bold);
            control.Font = GetFont(size, bold ? FontStyle.Bold : FontStyle.Regular);
            var edit = control as BaseEdit;
            if (edit != null)
            {
                edit.Properties.Appearance.Font = control.Font;
                edit.Properties.Appearance.Options.UseFont = true;
            }
            var combo = control as ComboBoxEdit;
            if (combo != null)
                combo.Properties.AppearanceDropDown.Font = control.Font;
            var label = control as LabelControl;
            if (label != null)
            {
                label.Appearance.Font = control.Font;
                label.Appearance.Options.UseFont = true;
            }
            if (action != null)
            {
                action.Appearance.Font = control.Font;
                action.Appearance.Options.UseFont = true;
            }
            var group = control as GroupControl;
            if (group != null)
            {
                group.AppearanceCaption.Font = GetFont(10F, FontStyle.Bold);
                group.AppearanceCaption.Options.UseFont = true;
            }
            var tabs = control as XtraTabControl;
            if (tabs != null)
            {
                tabs.AppearancePage.Header.Font = GetFont(10F);
                tabs.AppearancePage.Header.Options.UseFont = true;
            }
            var grid = control as GridControl;
            if (grid != null)
                foreach (var item in grid.ViewCollection)
                {
                    var view = item as GridView;
                    if (view == null) continue;
                    view.Appearance.Row.Font = GetFont(10F);
                    view.Appearance.Row.Options.UseFont = true;
                    view.Appearance.HeaderPanel.Font = GetFont(10F);
                    view.Appearance.HeaderPanel.Options.UseFont = true;
                    view.Appearance.Empty.Font = GetFont(10F);
                    view.Appearance.Empty.Options.UseFont = true;
                }
            var tree = control as TreeList;
            if (tree != null)
            {
                tree.Appearance.Row.Font = GetFont(10F);
                tree.Appearance.Row.Options.UseFont = true;
                tree.Appearance.HeaderPanel.Font = GetFont(10F);
                tree.Appearance.HeaderPanel.Options.UseFont = true;
            }
            var dock = control as BarDockControl;
            if (dock != null && dock.Manager != null)
            {
                foreach (Bar bar in dock.Manager.Bars)
                {
                    bar.BarAppearance.Normal.Font = GetFont(bar == dock.Manager.StatusBar ? 10F : 9F);
                    bar.BarAppearance.Normal.Options.UseFont = true;
                }
                foreach (DevExpress.XtraEditors.Repository.RepositoryItem item in dock.Manager.RepositoryItems)
                {
                    item.Appearance.Font = GetFont(9F);
                    item.Appearance.Options.UseFont = true;
                    var selection = item as DevExpress.XtraEditors.Repository.RepositoryItemComboBox;
                    if (selection != null) selection.AppearanceDropDown.Font = GetFont(9F);
                }
            }
            ApplyControlStyle(control);
            control.ControlAdded -= ChildControlAdded;
            control.ControlAdded += ChildControlAdded;
            foreach (Control child in control.Controls) ApplyFonts(child);
            var diff = control as DiffViewerControl;
            if (diff != null) diff.SetCodeFont(GetCodeFont());
            if (control.Name == "txtRawDiff" && edit != null)
            {
                control.Font = GetCodeFont();
                edit.Properties.Appearance.Font = control.Font;
            }
        }

        /// <summary>배치와 본문 글꼴을 바꾸지 않고 화면의 버튼 및 영역 제목 스타일을 통일한다.</summary>
        public static void ApplyStyles(Control control)
        {
            if (control is ScintillaNET.Scintilla) return;
            ApplyControlStyle(control);
            foreach (Control child in control.Controls) ApplyStyles(child);
        }

        /// <summary>컨트롤 역할에 따라 버튼의 강조와 제목 글꼴을 적용하며 상태 표시 색상은 유지한다.</summary>
        private static void ApplyControlStyle(Control control)
        {
            var button = control as SimpleButton;
            if (button != null)
            {
                bool primary = IsPrimaryAction(button);
                button.Font = GetFont(9F, primary ? FontStyle.Bold : FontStyle.Regular);
                button.Appearance.Font = button.Font;
                button.Appearance.Options.UseFont = true;
                if (!SystemInformation.HighContrast) ApplyButtonColors(button, primary);
            }
            var group = control as GroupControl;
            if (group != null)
            {
                group.AppearanceCaption.Font = GetFont(10F, FontStyle.Bold);
                group.AppearanceCaption.Options.UseFont = true;
                if (!SystemInformation.HighContrast)
                {
                    group.AppearanceCaption.ForeColor = TextColor;
                    group.AppearanceCaption.Options.UseForeColor = true;
                }
            }
            if (control.Name == "lblFolderTitle" || control.Name == "lblCommitTargets")
            {
                control.Font = GetFont(10F, FontStyle.Bold);
                if (!SystemInformation.HighContrast) control.ForeColor = TextColor;
            }
        }

        /// <summary>등록, 저장 및 커밋 버튼을 화면의 주요 실행 명령으로 식별한다.</summary>
        private static bool IsPrimaryAction(SimpleButton button)
        {
            return button.Name == "btnOk" || button.Name == "btnSave" || button.Name == "btnCommit";
        }

        /// <summary>스킨에 의존하지 않고 기본, 마우스 진입, 누름 및 비활성 상태의 색상을 적용한다.</summary>
        private static void ApplyButtonColors(SimpleButton button, bool primary)
        {
            button.LookAndFeel.UseDefaultLookAndFeel = false;
            button.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            button.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            SetButtonAppearance(button.Appearance, primary ? PrimaryColor : Color.White,
                primary ? Color.White : TextColor, primary ? PrimaryColor : BorderColor);
            SetButtonAppearance(button.AppearanceHovered, primary ? PrimaryHoverColor : Color.FromArgb(240, 244, 248),
                primary ? Color.White : TextColor, primary ? PrimaryHoverColor : PrimaryColor);
            SetButtonAppearance(button.AppearancePressed, primary ? PrimaryPressedColor : Color.FromArgb(226, 234, 242),
                primary ? Color.White : TextColor, primary ? PrimaryPressedColor : PrimaryColor);
            SetButtonAppearance(button.AppearanceDisabled, Color.FromArgb(239, 242, 246),
                DisabledTextColor, Color.FromArgb(214, 221, 230));
        }

        /// <summary>버튼의 배경, 글자 및 테두리 색상을 하나의 표시 상태에 적용한다.</summary>
        private static void SetButtonAppearance(DevExpress.Utils.AppearanceObject appearance,
            Color background, Color foreground, Color border)
        {
            appearance.BackColor = background;
            appearance.BackColor2 = background;
            appearance.ForeColor = foreground;
            appearance.BorderColor = border;
            appearance.Options.UseBackColor = true;
            appearance.Options.UseForeColor = true;
            appearance.Options.UseBorderColor = true;
        }

        /// <summary>디자이너에서 기존 컨테이너 안에 새로 넣은 컨트롤에도 역할별 글꼴을 적용한다.</summary>
        private static void ChildControlAdded(object sender, ControlEventArgs e)
        {
            ApplyFonts(e.Control);
        }

        /// <summary>어셈블리 리소스의 글꼴을 GDI와 GDI+에 등록하고 메모리를 보관한다.</summary>
        private static void LoadFont(string name)
        {
            IntPtr buffer = IntPtr.Zero;
            IntPtr handle = IntPtr.Zero;
            try
            {
                using (var stream = typeof(ViewTheme).Assembly.GetManifestResourceStream("SvnAiClient.Theme." + name))
                {
                    if (stream == null) throw new FileNotFoundException("테마 글꼴 리소스 없음: " + name);
                    using (var bytes = new MemoryStream())
                    {
                        stream.CopyTo(bytes);
                        byte[] data = bytes.ToArray();
                        buffer = Marshal.AllocHGlobal(data.Length);
                        Marshal.Copy(data, 0, buffer, data.Length);
                        uint count = 0;
                        handle = AddFontMemResourceEx(buffer, (uint)data.Length, IntPtr.Zero, ref count);
                        if (handle == IntPtr.Zero) throw new InvalidOperationException("테마 글꼴 등록 실패: " + name);
                        Collection.AddMemoryFont(buffer, data.Length);
                        Buffers.Add(buffer);
                        Handles.Add(handle);
                    }
                }
            }
            catch (Exception ex)
            {
                if (handle != IntPtr.Zero) RemoveFontMemResourceEx(handle);
                if (buffer != IntPtr.Zero) Marshal.FreeHGlobal(buffer);
                Trace.TraceWarning(ex.Message);
            }
        }

        /// <summary>등록한 패밀리를 이용해 글꼴을 만들고 크기·스타일별로 재사용한다.</summary>
        private static Font GetFamilyFont(string name, float size, FontStyle style, string fallback)
        {
            var key = Tuple.Create(name, size, style);
            Font font;
            if (Fonts.TryGetValue(key, out font)) return font;
            FontFamily family;
            if (!Families.TryGetValue(name, out family))
            {
                foreach (var candidate in Collection.Families)
                {
                    if (family == null && candidate.Name == name) family = candidate;
                    else candidate.Dispose();
                }
                if (family != null) Families[name] = family;
            }
            font = family != null && family.IsStyleAvailable(style)
                ? new Font(family, size, style) : new Font(fallback, size, style);
            Fonts.Add(key, font);
            return font;
        }

        /// <summary>프로세스 또는 디자이너 도메인 종료 시 글꼴과 등록 메모리를 한 번 해제한다.</summary>
        private static void ReleaseFonts(object sender, EventArgs e)
        {
            if (_released) return;
            _released = true;
            foreach (var font in Fonts.Values) font.Dispose();
            Fonts.Clear();
            foreach (var family in Families.Values) family.Dispose();
            Families.Clear();
            Collection.Dispose();
            foreach (var handle in Handles) RemoveFontMemResourceEx(handle);
            Handles.Clear();
            foreach (var buffer in Buffers) Marshal.FreeHGlobal(buffer);
            Buffers.Clear();
        }

        #endregion

        #region Native Methods

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr data, uint length, IntPtr reserved, ref uint count);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveFontMemResourceEx(IntPtr handle);

        #endregion
    }
}

