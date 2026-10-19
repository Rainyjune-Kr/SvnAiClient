using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using SvnAiClient.Helpers;
using DevExpress.XtraEditors;
using SvnAiClient.Core.Settings;
using SvnAiClient.Models;
using SvnAiClient.Views.AddWorkingCopy;
using SvnAiClient.Views.Common;

namespace SvnAiClient.Views.Settings
{
    /// <summary>설정 화면의 컨트롤 입출력과 사용자 요청 전달을 담당한다.</summary>
    public partial class SettingsView : ThemedView, SettingsInterface
    {
        #region Fields

        /// <summary>사용자 요청을 전달하고 설정 처리 흐름을 위임하는 Presenter.</summary>
        private readonly SettingsPresenter _presenter;

        /// <summary>호출 화면에서 빌려 쓰며 작업 복사본 편집 대화상자에도 전달하는 글꼴.</summary>
        private readonly ApplicationFonts _applicationFonts;
        /// <summary>컨트롤 값 표시 중 모델 선택 이벤트의 재진입을 방지한다.</summary>
        private bool _updatingControls;
        /// <summary>현재 편집 화면에서 고급 설정을 표시하는지 여부.</summary>
        private bool _advancedExpanded;
        /// <summary>설정 한 행의 표시 높이.</summary>
        private readonly float _providerRowHeight;
        /// <summary>프로바이더 탭 제목과 여백에 필요한 높이.</summary>
        private readonly int _providerFrameHeight;

        #endregion

        #region Properties

        /// <summary>파일 저장에 성공한 설정을 호출 화면에 전달한다.</summary>
        public AppSettings SavedSettings { get; private set; }

        /// <summary>화면에서 선택한 기본 AI 프로바이더 이름.</summary>
        public string ActiveProvider
        {
            get
            {
                return rbClaude.Checked ? "Claude" : rbOpenAi.Checked ? "OpenAI"
                    : rbGemini.Checked ? "Gemini" : "Local";
            }
        }

        /// <summary>작업 복사본 목록의 선택 인덱스. 선택하지 않았으면 -1이다.</summary>
        public int SelectedWorkingCopyIndex
        {
            get
            {
                var row = viewWorkingCopies.GetFocusedRow() as SettingsWorkingCopyItem;
                return row == null ? -1 : row.Index;
            }
        }

        /// <summary>현재 편집 중인 설정 탭의 프로바이더 이름.</summary>
        public string EditingProvider
        {
            get
            {
                return tabProviders.SelectedTabPage == tabOpenAi ? "OpenAI"
                    : tabProviders.SelectedTabPage == tabGemini ? "Gemini"
                    : tabProviders.SelectedTabPage == tabLocal ? "Local" : "Claude";
            }
        }

        #endregion

        #region Initialization

        public SettingsView(SettingsService settingsService, AppSettings settings, ApplicationFonts fonts = null)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            InitializeComponent();
            _providerRowHeight = layoutOpenAi.RowStyles[0].Height;
            _providerFrameHeight = Math.Max(0, tabProviders.Height - (int)(_providerRowHeight * 3));
            _applicationFonts = fonts;
            if (fonts != null) InitializeFonts(fonts);
            _presenter = new SettingsPresenter(this, settingsService, settings);
            AddEventHandlers();
            _presenter.Initialize();
            UpdateAdvancedLayout();
        }

        private void InitializeFonts(ApplicationFonts fonts)
        {
            ApplyControlFonts(this, fonts);
            btnSave.Appearance.Font = fonts.GetFont(9F, FontStyle.Bold);
            foreach (var tabs in new[] { tabControl, tabProviders })
            {
                tabs.AppearancePage.Header.Font = fonts.GetFont(10F);
                tabs.AppearancePage.Header.Options.UseFont = true;
            }
            viewWorkingCopies.Appearance.Row.Font = fonts.GetFont(10F);
            viewWorkingCopies.Appearance.Row.Options.UseFont = true;
            viewWorkingCopies.Appearance.HeaderPanel.Font = fonts.GetFont(10F);
            viewWorkingCopies.Appearance.HeaderPanel.Options.UseFont = true;
            viewWorkingCopies.Appearance.Empty.Font = fonts.GetFont(10F);
            viewWorkingCopies.Appearance.Empty.Options.UseFont = true;
            foreach (var combo in new[] { txtClaudeModel, txtOpenAiModel, txtGeminiModel, cboOpenAiEffort })
                combo.Properties.AppearanceDropDown.Font = fonts.GetFont(10F);
        }

        private void AddEventHandlers()
        {
            btnSave.Click += Save_Click;
            btnCancel.Click += Cancel_Click;
            btnAddWC.Click += AddWorkingCopy_Click;
            btnEditWC.Click += EditWorkingCopy_Click;
            btnRemoveWC.Click += RemoveWorkingCopy_Click;
            btnRecommendedModel.Click += RecommendedModel_Click;
            btnAdvancedSettings.Click += AdvancedSettings_Click;
            rbClaude.CheckedChanged += Provider_CheckedChanged;
            rbOpenAi.CheckedChanged += Provider_CheckedChanged;
            rbGemini.CheckedChanged += Provider_CheckedChanged;
            rbLocal.CheckedChanged += Provider_CheckedChanged;
            tabProviders.SelectedPageChanged += ProviderTab_SelectedPageChanged;
            viewWorkingCopies.FocusedRowChanged += WorkingCopy_FocusedRowChanged;
            viewWorkingCopies.DoubleClick += WorkingCopy_DoubleClick;
            txtOpenAiModel.SelectedIndexChanged += OpenAiModel_SelectedIndexChanged;
            txtClaudeModel.SelectedIndexChanged += ClaudeModel_SelectedIndexChanged;
            txtGeminiModel.SelectedIndexChanged += GeminiModel_SelectedIndexChanged;
            txtOpenAiModel.EditValueChanged += OpenAiModel_EditValueChanged;
        }

        #endregion

        #region Event Handlers

        private void Save_Click(object sender, EventArgs e)
        {
            _presenter.Save();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _presenter.Cancel();
        }

        private void AddWorkingCopy_Click(object sender, EventArgs e)
        {
            _presenter.AddWorkingCopy();
        }

        private void EditWorkingCopy_Click(object sender, EventArgs e)
        {
            _presenter.EditWorkingCopy();
        }

        private void RemoveWorkingCopy_Click(object sender, EventArgs e)
        {
            _presenter.RemoveWorkingCopy();
        }

        private void RecommendedModel_Click(object sender, EventArgs e)
        {
            _presenter.SelectRecommendedModel();
        }

        private void Provider_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updatingControls) UpdateProviderHeader();
        }

        private void AdvancedSettings_Click(object sender, EventArgs e)
        {
            _advancedExpanded = !_advancedExpanded;
            UpdateAdvancedLayout();
        }

        private void ProviderTab_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (!_updatingControls) UpdateProviderHeader();
            UpdateAdvancedLayout();
        }

        private void WorkingCopy_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateWorkingCopyButtons();
        }

        private void WorkingCopy_DoubleClick(object sender, EventArgs e)
        {
            var hit = viewWorkingCopies.CalcHitInfo(gridWorkingCopies.PointToClient(Cursor.Position));
            if (hit.InRow && hit.RowHandle >= 0) _presenter.EditWorkingCopy();
        }

        private void OpenAiModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updatingControls)
                _presenter.SelectModel("OpenAI", GetModelId(txtOpenAiModel));
        }

        private void ClaudeModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updatingControls)
                _presenter.SelectModel("Claude", GetModelId(txtClaudeModel));
        }

        private void GeminiModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updatingControls)
                _presenter.SelectModel("Gemini", GetModelId(txtGeminiModel));
        }

        private void OpenAiModel_EditValueChanged(object sender, EventArgs e)
        {
            if (!_updatingControls)
                _presenter.RefreshOpenAiEfforts(GetModelId(txtOpenAiModel));
        }

        #endregion

        #region Control Methods

        public void ShowSettings(SettingsViewData data)
        {
            _updatingControls = true;
            try
            {
                rbClaude.Checked = data.ActiveProvider == "Claude";
                rbOpenAi.Checked = data.ActiveProvider == "OpenAI";
                rbGemini.Checked = data.ActiveProvider == "Gemini";
                rbLocal.Checked = data.ActiveProvider == "Local";
                tabProviders.SelectedTabPage = data.ActiveProvider == "OpenAI" ? tabOpenAi
                    : data.ActiveProvider == "Gemini" ? tabGemini
                    : data.ActiveProvider == "Local" ? tabLocal : tabClaude;

                txtClaudeApiKey.Text = data.Claude.ApiKey;
                LoadModelCombo(txtClaudeModel, "Claude", data.Claude);
                numClaudeTokens.Value = data.Claude.MaxTokens;
                numClaudeInputTokens.Value = data.Claude.InputTokenLimit;

                txtOpenAiApiKey.Text = data.OpenAi.ApiKey;
                LoadModelCombo(txtOpenAiModel, "OpenAI", data.OpenAi);
                numOpenAiTokens.Value = data.OpenAi.MaxTokens;
                numOpenAiInputTokens.Value = data.OpenAi.InputTokenLimit;
                numOpenAiTimeout.Value = data.OpenAi.TimeoutSeconds;

                txtGeminiApiKey.Text = data.Gemini.ApiKey;
                LoadModelCombo(txtGeminiModel, "Gemini", data.Gemini);
                numGeminiTokens.Value = data.Gemini.MaxTokens;
                numGeminiInputTokens.Value = data.Gemini.InputTokenLimit;

                txtLocalEndpoint.Text = data.LocalLlm.EndpointUrl;
                txtLocalModel.Text = data.LocalLlm.Model;
                numLocalTokens.Value = data.LocalLlm.MaxTokens;
                numLocalInputTokens.Value = data.LocalLlm.InputTokenLimit;

                chkAutoReview.Checked = data.AutoReviewBeforeCommit;
                clbReviewItems.Items.Clear();
                foreach (var item in data.ReviewItems)
                    clbReviewItems.Items.Add(item.Key, item.Value);
            }
            finally
            {
                _updatingControls = false;
            }
            UpdateProviderHeader();
        }

        public SettingsViewData ReadSettings()
        {
            var data = new SettingsViewData
            {
                ActiveProvider = ActiveProvider,
                AutoReviewBeforeCommit = chkAutoReview.Checked,
                Claude = ReadProvider(txtClaudeModel, txtClaudeApiKey,
                    numClaudeTokens, numClaudeInputTokens),
                OpenAi = ReadProvider(txtOpenAiModel, txtOpenAiApiKey,
                    numOpenAiTokens, numOpenAiInputTokens),
                Gemini = ReadProvider(txtGeminiModel, txtGeminiApiKey,
                    numGeminiTokens, numGeminiInputTokens),
                LocalLlm = new SettingsProviderData
                {
                    EndpointUrl = txtLocalEndpoint.Text.Trim(),
                    Model = txtLocalModel.Text.Trim(),
                    MaxTokens = (int)numLocalTokens.Value,
                    InputTokenLimit = (int)numLocalInputTokens.Value
                }
            };

            data.OpenAi.TimeoutSeconds = (int)numOpenAiTimeout.Value;
            data.OpenAi.ReasoningEffort = cboOpenAiEffort.Enabled
                ? cboOpenAiEffort.SelectedItem?.ToString() ?? "medium" : null;

            for (int i = 0; i < clbReviewItems.Items.Count; i++)
                data.ReviewItems[clbReviewItems.Items[i].ToString()] = clbReviewItems.GetItemChecked(i);

            return data;
        }

        public void ShowWorkingCopies(IEnumerable<SettingsWorkingCopyItem> items)
        {
            int selected = SelectedWorkingCopyIndex;
            var rows = items.ToList();
            gridWorkingCopies.DataSource = rows;
            int sourceIndex = rows.FindIndex(item => item.Index == selected);
            if (sourceIndex < 0 && rows.Count > 0)
                sourceIndex = Math.Min(Math.Max(selected, 0), rows.Count - 1);
            viewWorkingCopies.FocusedRowHandle = sourceIndex < 0
                ? DevExpress.XtraGrid.GridControl.InvalidRowHandle
                : viewWorkingCopies.GetRowHandle(sourceIndex);
            UpdateWorkingCopyButtons();
        }

        public void ApplyModelPreset(string provider, AiModelPreset preset)
        {
            var output = provider == "OpenAI" ? numOpenAiTokens
                : provider == "Claude" ? numClaudeTokens : numGeminiTokens;
            var input = provider == "OpenAI" ? numOpenAiInputTokens
                : provider == "Claude" ? numClaudeInputTokens : numGeminiInputTokens;
            output.Properties.MaxValue = preset.MaxOutputTokens;
            output.Value = preset.OutputBudget;
            input.Value = preset.InputBudget;
            if (provider == "OpenAI")
                numOpenAiTimeout.Value = preset.TimeoutSeconds;
        }

        public void SelectRecommendedModel(string provider, AiModelPreset preset)
        {
            _updatingControls = true;
            try
            {
                var combo = provider == "OpenAI" ? txtOpenAiModel
                    : provider == "Gemini" ? txtGeminiModel : txtClaudeModel;
                combo.SelectedItem = preset;
                tabProviders.SelectedTabPage = provider == "OpenAI"
                    ? tabOpenAi : provider == "Gemini" ? tabGemini : tabClaude;
            }
            finally
            {
                _updatingControls = false;
            }
            UpdateProviderHeader();
        }

        public void ShowOpenAiEfforts(AiModelPreset preset, string saved)
        {
            cboOpenAiEffort.Properties.Items.Clear();
            cboOpenAiEffort.Enabled = preset != null && preset.Efforts.Length > 0;
            if (!cboOpenAiEffort.Enabled) return;

            foreach (string effort in preset.Efforts)
                cboOpenAiEffort.Properties.Items.Add(effort);
            cboOpenAiEffort.SelectedItem = Array.IndexOf(preset.Efforts, saved) >= 0
                ? saved : preset.Effort;
        }

        public WorkingCopyEntry EditWorkingCopy(WorkingCopyEntry existing)
        {
            using (var dialog = new AddWorkingCopyView(existing, _applicationFonts))
            {
                return dialog.ShowDialog(this) == DialogResult.OK ? dialog.Result : null;
            }
        }

        public bool ConfirmWorkingCopyRemoval(string name)
        {
            return XtraMessageBox.Show(this, $"'{name}'을(를) 제거하겠습니까?",
                "제거 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public void ShowSaveError(string message)
        {
            _advancedExpanded = true;
            UpdateAdvancedLayout();
            XtraMessageBox.Show(this, "설정 저장 실패:\n" + message,
                "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void CompleteSave(AppSettings settings)
        {
            SavedSettings = settings;
            DialogResult = DialogResult.OK;
            Close();
        }

        public void CancelEdit()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static void LoadModelCombo(
            ComboBoxEdit combo, string provider, SettingsProviderData data)
        {
            combo.Properties.Items.Clear();
            AiModelPreset selected = null;
            foreach (var preset in data.Models)
            {
                combo.Properties.Items.Add(preset);
                if (preset.Id == data.Model) selected = preset;
            }

            if (selected == null && !string.IsNullOrWhiteSpace(data.Model))
            {
                selected = new AiModelPreset
                {
                    Provider = provider, Id = data.Model, Name = "저장된 / 사용자 모델"
                };
                combo.Properties.Items.Add(selected);
            }

            combo.SelectedItem = selected;
        }

        #endregion

        #region Operations

        /// <summary>공유 글꼴을 본문과 버튼에 적용한다. 글꼴의 수명은 호출 화면에서 관리한다.</summary>
        private static void ApplyControlFonts(Control control, ApplicationFonts fonts)
        {
            control.Font = fonts.GetFont(control is SimpleButton ? 9F : 10F);
            var edit = control as BaseEdit;
            if (edit != null)
            {
                edit.Properties.Appearance.Font = control.Font;
                edit.Properties.Appearance.Options.UseFont = true;
            }
            var button = control as SimpleButton;
            if (button != null)
            {
                button.Appearance.Font = control.Font;
                button.Appearance.Options.UseFont = true;
            }
            var label = control as LabelControl;
            if (label != null)
            {
                label.Appearance.Font = control.Font;
                label.Appearance.Options.UseFont = true;
            }
            var group = control as GroupControl;
            if (group != null)
            {
                group.AppearanceCaption.Font = fonts.GetFont(10F, FontStyle.Bold);
                group.AppearanceCaption.Options.UseFont = true;
            }
            foreach (Control child in control.Controls) ApplyControlFonts(child, fonts);
        }

        /// <summary>기본 입력을 유지하고 고급 옵션 행의 표시와 탭 높이만 조정한다.</summary>
        private void UpdateAdvancedLayout()
        {
            if (_providerRowHeight <= 0) return;
            foreach (var table in new[] { layoutClaude, layoutOpenAi, layoutGemini, layoutLocal })
            {
                table.SuspendLayout();
                try
                {
                    table.RowStyles[2].Height = _advancedExpanded ? _providerRowHeight : 0;
                    table.RowStyles[3].Height = _advancedExpanded && table == layoutOpenAi ? _providerRowHeight : 0;
                    foreach (Control control in table.Controls)
                    {
                        int row = table.GetRow(control);
                        if (row == 2 || row == 3) control.Visible = _advancedExpanded;
                    }
                }
                finally { table.ResumeLayout(true); }
            }
            int rows = _advancedExpanded ? (tabProviders.SelectedTabPage == tabOpenAi ? 5 : 4) : 3;
            tabProviders.Height = _providerFrameHeight + (int)(_providerRowHeight * rows);
            btnAdvancedSettings.Text = _advancedExpanded ? "고급 설정 접기" : "고급 설정 펼치기";
        }

        /// <summary>편집 대상과 기본 검수 대상을 구분하고 권장 모델 버튼 상태를 갱신한다.</summary>
        private void UpdateProviderHeader()
        {
            lblEditingProvider.Text = "편집: " + EditingProvider + " / 기본 검수: " + ActiveProvider;
            btnRecommendedModel.Text = EditingProvider + " 권장 모델 적용";
            btnRecommendedModel.Enabled = EditingProvider != "Local";
        }

        /// <summary>현재 선택한 작업 복사본이 있을 때 수정 및 제거 버튼을 활성화한다.</summary>
        private void UpdateWorkingCopyButtons()
        {
            bool selected = SelectedWorkingCopyIndex >= 0;
            btnEditWC.Enabled = selected;
            btnRemoveWC.Enabled = selected;
        }

        /// <summary>목록 항목의 모델 ID 또는 직접 입력한 모델명을 반환한다.</summary>
        private static string GetModelId(ComboBoxEdit combo)
        {
            var selected = combo.SelectedItem as AiModelPreset;
            return selected != null && combo.Text == selected.ToString()
                ? selected.Id : combo.Text.Trim();
        }

        /// <summary>프로바이더별 공통 입력 컨트롤을 화면 전달 데이터로 변환한다.</summary>
        private static SettingsProviderData ReadProvider(
            ComboBoxEdit model, TextEdit apiKey, SpinEdit output, SpinEdit input)
        {
            return new SettingsProviderData
            {
                ApiKey = apiKey.Text.Trim(),
                Model = GetModelId(model),
                MaxTokens = (int)output.Value,
                InputTokenLimit = (int)input.Value
            };
        }

        #endregion
    }
}
