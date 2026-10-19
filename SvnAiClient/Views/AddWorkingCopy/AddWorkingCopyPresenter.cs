using System;
using System.IO;
using SvnAiClient.Models;

namespace SvnAiClient.Views.AddWorkingCopy
{
    /// <summary>작업 복사본 입력 검증, 경로 정규화 및 자격 증명 보존을 담당한다.</summary>
    public sealed class AddWorkingCopyPresenter
    {
        #region Fields

        /// <summary>입력값을 읽고 폴더 선택과 승인 결과 표시를 요청하는 화면 계약.</summary>
        private readonly AddWorkingCopyInterface _view;

        /// <summary>비밀번호를 새로 입력하지 않은 편집 요청에서 유지할 기존 암호문.</summary>
        private string _existingEncryptedPassword = string.Empty;

        #endregion

        #region Initialization

        public AddWorkingCopyPresenter(AddWorkingCopyInterface view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            _view = view;
        }

        public void LoadExisting(WorkingCopyEntry existing)
        {
            if (existing == null) throw new ArgumentNullException(nameof(existing));
            _existingEncryptedPassword = existing.EncryptedPassword;
            _view.ShowExisting(existing);
        }

        #endregion

        #region View Requests

        public void Browse()
        {
            string path = _view.SelectFolder(Directory.Exists(_view.LocalPath)
                ? _view.LocalPath : string.Empty);
            if (path == null) return;

            _view.LocalPath = path;
            if (string.IsNullOrWhiteSpace(_view.DisplayName))
                _view.DisplayName = Path.GetFileName(path);
        }

        public void Save()
        {
            string localPath = _view.LocalPath.Trim();
            if (string.IsNullOrWhiteSpace(localPath))
            {
                _view.ShowValidation("로컬 경로를 입력하세요.");
                _view.FocusLocalPath();
                return;
            }

            if (!Directory.Exists(localPath))
            {
                _view.ShowValidation("존재하지 않는 경로입니다.");
                _view.FocusLocalPath();
                return;
            }

            _view.Complete(CreateEntry(localPath));
        }

        public void Cancel()
        {
            _view.Cancel();
        }

        #endregion

        #region Operations

        /// <summary>정규화한 경로와 입력값으로 새 항목을 만들고 비밀번호 변경을 반영한다.</summary>
        private WorkingCopyEntry CreateEntry(string localPath)
        {
            var entry = new WorkingCopyEntry
            {
                LocalPath = NormalizeDirectory(localPath),
                DisplayName = _view.DisplayName.Trim(),
                Username = _view.Username.Trim(),
                EncryptedPassword = _existingEncryptedPassword
            };

            if (!string.IsNullOrEmpty(_view.Password))
                entry.SetPassword(_view.Password);

            return entry;
        }

        /// <summary>드라이브 루트의 구분자는 보존하고 일반 경로의 끝 구분자는 제거한다.</summary>
        private static string NormalizeDirectory(string path)
        {
            string fullPath = Path.GetFullPath(path);
            string root = Path.GetPathRoot(fullPath);
            return string.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase)
                ? fullPath : fullPath.TrimEnd('\\', '/');
        }

        #endregion
    }
}
