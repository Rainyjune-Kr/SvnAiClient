using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using SvnAiClient.Core.Settings;
using SvnAiClient.Helpers;
using SvnAiClient.Models;

namespace SvnAiClient.Views.Settings
{
    /// <summary>편집 중인 설정 상태, 검증 및 저장 흐름을 관리한다.</summary>
    public sealed class SettingsPresenter
    {
        #region Fields

        /// <summary>설정 저장 중 발생한 오류를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>컨트롤 형식에 의존하지 않고 화면 입력과 표시를 요청하는 계약.</summary>
        private readonly SettingsInterface _view;
        /// <summary>검증된 설정의 로컬 파일 저장을 담당한다.</summary>
        private readonly SettingsService _settingsService;
        /// <summary>원본 설정과 분리하여 관리하는 편집용 설정 복사본.</summary>
        private AppSettings _settings;

        #endregion

        #region Initialization

        public SettingsPresenter(
            SettingsInterface view, SettingsService settingsService, AppSettings settings)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _view = view;
            _settingsService = settingsService;
            _settings = SettingsService.CreateEditableCopy(settings);
        }

        public void Initialize()
        {
            _view.ShowSettings(CreateViewData());
            RefreshWorkingCopies();
            RefreshOpenAiEfforts(_settings.AiProviders.OpenAi.Model);
        }

        #endregion

        #region View Requests

        public void Save()
        {
            try
            {
                var candidate = CreateSaveSettings(_view.ReadSettings());
                string error = ValidateModels(candidate);
                if (error != null)
                    throw new InvalidOperationException(error);

                _settingsService.Save(candidate);
                _settings = candidate;
                _view.CompleteSave(candidate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "설정 저장 오류");
                _view.ShowSaveError(ex.Message);
            }
        }

        public void Cancel()
        {
            _view.CancelEdit();
        }

        public void AddWorkingCopy()
        {
            var entry = _view.EditWorkingCopy(null);
            if (entry == null) return;

            _settings.WorkingCopies.Add(CopyWorkingCopy(entry));
            RefreshWorkingCopies();
        }

        public void EditWorkingCopy()
        {
            int index = _view.SelectedWorkingCopyIndex;
            if (!IsWorkingCopyIndex(index)) return;

            var entry = _view.EditWorkingCopy(CopyWorkingCopy(_settings.WorkingCopies[index]));
            if (entry == null) return;

            _settings.WorkingCopies[index] = CopyWorkingCopy(entry);
            RefreshWorkingCopies();
        }

        public void RemoveWorkingCopy()
        {
            int index = _view.SelectedWorkingCopyIndex;
            if (!IsWorkingCopyIndex(index)) return;
            if (!_view.ConfirmWorkingCopyRemoval(_settings.WorkingCopies[index].GetDisplayName()))
                return;

            _settings.WorkingCopies.RemoveAt(index);
            RefreshWorkingCopies();
        }

        public void SelectModel(string provider, string model)
        {
            var preset = AiModelCatalog.Find(provider, model);
            if (preset == null) return;

            if (provider == "OpenAI")
                _settings.AiProviders.OpenAi.ReasoningEffort = preset.Effort ?? "medium";
            else if (provider == "Claude")
                _settings.AiProviders.Claude.TimeoutSeconds = preset.TimeoutSeconds;
            else if (provider == "Gemini")
                _settings.AiProviders.Gemini.TimeoutSeconds = preset.TimeoutSeconds;
            else
                return;

            _view.ApplyModelPreset(provider, preset);
            if (provider == "OpenAI")
                RefreshOpenAiEfforts(model);
        }

        public void SelectRecommendedModel()
        {
            string provider = _view.EditingProvider;
            if (provider == "Local") return;

            var preset = AiModelCatalog.ForProvider(provider)
                .FirstOrDefault(item => item.Recommended);
            if (preset == null) return;

            _view.SelectRecommendedModel(provider, preset);
            SelectModel(provider, preset.Id);
        }

        public void RefreshOpenAiEfforts(string model)
        {
            _view.ShowOpenAiEfforts(
                AiModelCatalog.Find("OpenAI", model),
                _settings.AiProviders.OpenAi.ReasoningEffort);
        }

        #endregion

        #region View Updates

        private void RefreshWorkingCopies()
        {
            _view.ShowWorkingCopies(_settings.WorkingCopies.Select((item, index) =>
                new SettingsWorkingCopyItem
                {
                    Index = index,
                    Name = item.GetDisplayName(),
                    LocalPath = item.LocalPath,
                    Username = item.Username
                }).ToList());
        }

        #endregion

        #region Operations

        /// <summary>편집용 설정에서 모델 목록과 복호화된 키를 포함한 화면 데이터를 만든다.</summary>
        private SettingsViewData CreateViewData()
        {
            var providers = _settings.AiProviders;
            return new SettingsViewData
            {
                ActiveProvider = _settings.ActiveAiProvider,
                AutoReviewBeforeCommit = _settings.AutoReviewBeforeCommit,
                ReviewItems = new Dictionary<string, bool>(_settings.ReviewItems),
                Claude = new SettingsProviderData
                {
                    ApiKey = DpapiHelper.DecryptSafe(providers.Claude.EncryptedApiKey),
                    Model = providers.Claude.Model,
                    InputTokenLimit = providers.Claude.InputTokenLimit,
                    MaxTokens = providers.Claude.MaxTokens,
                    Models = AiModelCatalog.ForProvider("Claude")
                },
                OpenAi = new SettingsProviderData
                {
                    ApiKey = DpapiHelper.DecryptSafe(providers.OpenAi.EncryptedApiKey),
                    Model = providers.OpenAi.Model,
                    InputTokenLimit = providers.OpenAi.InputTokenLimit,
                    MaxTokens = providers.OpenAi.MaxTokens,
                    TimeoutSeconds = providers.OpenAi.TimeoutSeconds,
                    Models = AiModelCatalog.ForProvider("OpenAI")
                },
                Gemini = new SettingsProviderData
                {
                    ApiKey = DpapiHelper.DecryptSafe(providers.Gemini.EncryptedApiKey),
                    Model = providers.Gemini.Model,
                    InputTokenLimit = providers.Gemini.InputTokenLimit,
                    MaxTokens = providers.Gemini.MaxTokens,
                    Models = AiModelCatalog.ForProvider("Gemini")
                },
                LocalLlm = new SettingsProviderData
                {
                    EndpointUrl = providers.LocalLlm.EndpointUrl,
                    Model = providers.LocalLlm.Model,
                    InputTokenLimit = providers.LocalLlm.InputTokenLimit,
                    MaxTokens = providers.LocalLlm.MaxTokens
                }
            };
        }

        /// <summary>입력값과 암호화된 API 키를 별도 저장 후보 설정에 반영한다.</summary>
        private AppSettings CreateSaveSettings(SettingsViewData data)
        {
            var settings = SettingsService.CreateEditableCopy(_settings);
            settings.ActiveAiProvider = data.ActiveProvider;
            settings.AutoReviewBeforeCommit = data.AutoReviewBeforeCommit;

            var providers = settings.AiProviders;
            providers.Claude.EncryptedApiKey = EncryptKey(data.Claude.ApiKey);
            providers.Claude.Model = data.Claude.Model;
            providers.Claude.InputTokenLimit = data.Claude.InputTokenLimit;
            providers.Claude.MaxTokens = data.Claude.MaxTokens;

            providers.OpenAi.EncryptedApiKey = EncryptKey(data.OpenAi.ApiKey);
            providers.OpenAi.Model = data.OpenAi.Model;
            providers.OpenAi.InputTokenLimit = data.OpenAi.InputTokenLimit;
            providers.OpenAi.MaxTokens = data.OpenAi.MaxTokens;
            providers.OpenAi.TimeoutSeconds = data.OpenAi.TimeoutSeconds;
            if (data.OpenAi.ReasoningEffort != null)
                providers.OpenAi.ReasoningEffort = data.OpenAi.ReasoningEffort;

            providers.Gemini.EncryptedApiKey = EncryptKey(data.Gemini.ApiKey);
            providers.Gemini.Model = data.Gemini.Model;
            providers.Gemini.InputTokenLimit = data.Gemini.InputTokenLimit;
            providers.Gemini.MaxTokens = data.Gemini.MaxTokens;

            providers.LocalLlm.EndpointUrl = data.LocalLlm.EndpointUrl;
            providers.LocalLlm.Model = data.LocalLlm.Model;
            providers.LocalLlm.InputTokenLimit = data.LocalLlm.InputTokenLimit;
            providers.LocalLlm.MaxTokens = data.LocalLlm.MaxTokens;

            foreach (var item in data.ReviewItems)
            {
                if (settings.ReviewItems.ContainsKey(item.Key))
                    settings.ReviewItems[item.Key] = item.Value;
            }

            return settings;
        }

        /// <summary>클라우드 모델별 입력·출력 토큰 설정을 검사하고 첫 오류를 반환한다.</summary>
        private static string ValidateModels(AppSettings settings)
        {
            var providers = settings.AiProviders;
            return AiModelCatalog.Validate("Claude", providers.Claude.Model,
                       providers.Claude.InputTokenLimit, providers.Claude.MaxTokens)
                   ?? AiModelCatalog.Validate("OpenAI", providers.OpenAi.Model,
                       providers.OpenAi.InputTokenLimit, providers.OpenAi.MaxTokens)
                   ?? AiModelCatalog.Validate("Gemini", providers.Gemini.Model,
                       providers.Gemini.InputTokenLimit, providers.Gemini.MaxTokens);
        }

        /// <summary>API 키의 앞뒤 공백을 제거하고 비어 있지 않으면 DPAPI로 암호화한다.</summary>
        private static string EncryptKey(string value)
        {
            string key = (value ?? string.Empty).Trim();
            return key.Length == 0 ? string.Empty : DpapiHelper.Encrypt(key);
        }

        /// <summary>선택 인덱스가 현재 편집 중인 작업 복사본 목록의 범위에 있는지 확인한다.</summary>
        private bool IsWorkingCopyIndex(int index)
        {
            return index >= 0 && index < _settings.WorkingCopies.Count;
        }

        /// <summary>편집 취소가 기존 항목에 영향을 주지 않도록 작업 복사본 정보를 복사한다.</summary>
        private static WorkingCopyEntry CopyWorkingCopy(WorkingCopyEntry entry)
        {
            return new WorkingCopyEntry
            {
                LocalPath = entry.LocalPath,
                DisplayName = entry.DisplayName,
                Username = entry.Username,
                EncryptedPassword = entry.EncryptedPassword
            };
        }

        #endregion
    }
}
