using System.Collections.Generic;
using SvnAiClient.Models;

namespace SvnAiClient.Views.Settings
{
    /// <summary>설정 화면의 입력과 표시 기능. UI 라이브러리 형식은 노출하지 않는다.</summary>
    public interface SettingsInterface
    {
        #region Properties

        /// <summary>화면에서 선택한 기본 AI 프로바이더 이름.</summary>
        string ActiveProvider { get; }
        /// <summary>작업 복사본 목록의 선택 인덱스. 선택하지 않았으면 -1이다.</summary>
        int SelectedWorkingCopyIndex { get; }
        /// <summary>현재 편집 중인 설정 탭의 프로바이더 이름.</summary>
        string EditingProvider { get; }

        #endregion

        #region View Contract

        SettingsViewData ReadSettings();
        void ShowSettings(SettingsViewData data);
        void ShowWorkingCopies(IEnumerable<SettingsWorkingCopyItem> items);
        void ApplyModelPreset(string provider, AiModelPreset preset);
        void SelectRecommendedModel(string provider, AiModelPreset preset);
        void ShowOpenAiEfforts(AiModelPreset preset, string saved);
        WorkingCopyEntry EditWorkingCopy(WorkingCopyEntry existing);
        bool ConfirmWorkingCopyRemoval(string name);
        void ShowSaveError(string message);
        void CompleteSave(AppSettings settings);
        void CancelEdit();

        #endregion
    }
}
