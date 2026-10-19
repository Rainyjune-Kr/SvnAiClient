using System;
using System.Collections.Generic;
using System.IO;
using SvnAiClient.Models;
using static SvnAiClient.Helpers.WorkingCopyPath;

namespace SvnAiClient.Helpers
{
    /// <summary>미등록 빌드 산출물을 숨기되 SVN 관리 항목과 탐색에 필요한 부모 폴더는 유지한다.</summary>
    internal sealed class WorkingCopyDisplayFilter
    {
        #region Fields

        /// <summary>C# 및 Java 프로젝트에서 일반적으로 사용하는 빌드 출력 폴더 이름.</summary>
        private static readonly HashSet<string> OutputFolderNames = new HashSet<string>(
            new[] { "bin", "obj", "classes", "target", "build", "out" }, StringComparer.OrdinalIgnoreCase);

        /// <summary>표시 여부를 판단할 작업 복사본의 정규화된 루트 경로.</summary>
        private readonly string _rootPath;

        /// <summary>SVN 관리 항목 및 해당 항목에 접근하는 데 필요한 모든 부모 경로.</summary>
        private readonly HashSet<string> _managedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Initialization

        public WorkingCopyDisplayFilter(string rootPath, IEnumerable<SvnFileStatus> statuses)
        {
            _rootPath = NormalizeFilePath(rootPath);
            foreach (var status in statuses)
            {
                if (!IsManaged(status) || string.IsNullOrWhiteSpace(status.FullPath)) continue;
                var path = NormalizeFilePath(status.FullPath);
                if (!IsSameFilePath(_rootPath, path) && !IsDescendantPath(_rootPath, path)) continue;

                while (_managedPaths.Add(path))
                {
                    if (IsSameFilePath(_rootPath, path)) break;
                    path = Path.GetDirectoryName(path);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>SVN 관리 여부를 우선하고 미등록 항목의 출력 폴더 포함 여부로 표시를 결정한다.</summary>
        public bool ShouldShow(string fullPath, bool isDirectory)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) return false;
            var path = NormalizeFilePath(fullPath);
            if (IsSameFilePath(_rootPath, path)) return true;
            if (!IsDescendantPath(_rootPath, path)) return false;

            var parts = GetRelativePath(_rootPath, path).Split(new[] { '\\', '/' },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
                if (string.Equals(part, ".svn", StringComparison.OrdinalIgnoreCase)) return false;

            if (_managedPaths.Contains(path)) return true;

            // 일부 미등록 폴더는 SVN 상태에서 종류를 알 수 없으므로 출력 이름인 경우만 디스크로 보완한다.
            if (!isDirectory && OutputFolderNames.Contains(Path.GetFileName(path)))
                isDirectory = Directory.Exists(path);

            int folderCount = isDirectory ? parts.Length : parts.Length - 1;
            for (int index = 0; index < folderCount; index++)
                if (OutputFolderNames.Contains(parts[index])) return false;
            return true;
        }

        #endregion

        #region Operations

        /// <summary>원래 SVN 상태와 기존 캐시 상태를 함께 사용하여 관리 및 추가 예약 항목을 구분한다.</summary>
        private static bool IsManaged(SvnFileStatus status)
        {
            if (status == null) return false;
            if (status.IsVersioned) return true;
            switch (status.Status)
            {
                case SvnStatusKind.Normal:
                case SvnStatusKind.Modified:
                case SvnStatusKind.Added:
                case SvnStatusKind.Deleted:
                case SvnStatusKind.Missing:
                case SvnStatusKind.Conflicted:
                case SvnStatusKind.Incomplete:
                case SvnStatusKind.External:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}
