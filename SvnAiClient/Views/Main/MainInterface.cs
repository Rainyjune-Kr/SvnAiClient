using System.Collections.Generic;
using SvnAiClient.Core.Settings;
using SvnAiClient.Core.Svn;
using SvnAiClient.Models;

namespace SvnAiClient.Views.Main
{
    /// <summary>메인 화면의 입력, 표시 및 하위 화면 호출에 대한 UI 독립 계약.</summary>
    public interface MainInterface
    {
        #region Properties

        /// <summary>화면이 폐기되지 않아 조회 결과를 표시할 수 있는지 여부.</summary>
        bool IsAvailable { get; }

        /// <summary>변경된 항목만 표시하는 필터의 활성화 여부.</summary>
        bool OnlyChanged { get; }

        /// <summary>변경 필터에 하위 폴더의 파일까지 포함할지 여부.</summary>
        bool IncludeSubfolders { get; }

        /// <summary>파일 그리드에서 선택한 파일들. 폴더는 제외한다.</summary>
        List<SvnFileStatus> SelectedFiles { get; }

        /// <summary>현재 파일 그리드에 바인딩된 표시 객체 목록.</summary>
        List<SvnFileStatus> DisplayedFiles { get; }

        #endregion

        #region View Contract

        void SetWorkingCopies(List<string> names, int selectedIndex);
        void UpdateWorkingCopyRoot(WorkingCopyEntry entry);
        void RebuildFolderTree(WorkingCopyEntry entry, string relativePath);
        void SyncTreeToPath(string fullPath);
        void PostFileEdits();
        void ShowFiles(List<SvnFileStatus> files);
        void ShowCommitTargets(List<SvnFileStatus> targets);
        void RefreshFiles();
        void ClearWorkspace();
        void InvalidateTree();
        void SetBusy(bool busy);
        void SetStatusText(string message);
        void SetStatusBar(string provider, string repository);
        void ShowMessage(string message, string title, MainNotice kind);
        bool Confirm(string message, string title, bool warning);
        WorkingCopyEntry CreateWorkingCopy();
        AppSettings EditSettings(SettingsService service, AppSettings settings);
        MainCommitRequest EditCommit(List<SvnFileStatus> targets,
            AppSettings settings, SvnService service, string workingCopyPath);
        void ShowLogViewer(string path, SvnService service);
        void ShowReviewHistory(string workingCopyPath);
        void ShowDiff(string path, string diff);

        #endregion
    }

    /// <summary>UI 라이브러리와 독립적인 메인 화면 알림의 종류.</summary>
    public enum MainNotice
    {
        Information,
        Warning,
        Error
    }
}
