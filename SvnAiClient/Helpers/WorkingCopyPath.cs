using System;
using System.Collections.Generic;
using System.IO;

namespace SvnAiClient.Helpers
{
    /// <summary>작업 복사본 내부 경로의 비교, 상대 경로 및 부분 갱신 범위를 계산한다.</summary>
    internal static class WorkingCopyPath
    {
        #region Public Methods

        /// <summary>드라이브 루트를 보존하면서 비교용 절대 경로를 만든다.</summary>
        public static string NormalizeFilePath(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var root = Path.GetPathRoot(fullPath);
            return string.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase)
                ? fullPath
                : fullPath.TrimEnd('\\', '/');
        }

        /// <summary>정규화한 Windows 경로를 대소문자 구분 없이 비교한다.</summary>
        public static bool IsSameFilePath(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) ||
                string.IsNullOrWhiteSpace(second))
                return false;

            return string.Equals(
                NormalizeFilePath(first),
                NormalizeFilePath(second),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>작업 복사본 루트를 기준으로 상대 경로를 계산한다.</summary>
        public static string GetRelativePath(string root, string fullPath)
        {
            if (IsSameFilePath(root, fullPath))
                return string.Empty;
            var prefix = NormalizeFilePath(root).TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
            var path = NormalizeFilePath(fullPath);
            return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? path.Substring(prefix.Length)
                : path;
        }

        /// <summary>지정 폴더의 실제 하위 경로인지 확인한다.</summary>
        public static bool IsDescendantPath(
            string directoryPath,
            string candidatePath)
        {
            var directory = NormalizeFilePath(directoryPath)
                .TrimEnd('\\', '/') + Path.DirectorySeparatorChar;

            var candidate = NormalizeFilePath(candidatePath);

            return candidate.StartsWith(
                directory,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>지정 폴더의 바로 아래에 있는 경로인지 확인한다.</summary>
        public static bool IsDirectChildPath(
            string directoryPath,
            string candidatePath)
        {
            var parent = Path.GetDirectoryName(
                NormalizeFilePath(candidatePath));

            return IsSameFilePath(directoryPath, parent);
        }

        /// <summary>대상 경로가 작업 복사본 안에 있는지 확인하고 루트까지의 부모 경로를 포함한다.</summary>
        public static HashSet<string> GetRefreshPaths(
            string rootPath, IEnumerable<string> paths)
        {
            string root = NormalizeFilePath(rootPath);

            var queryPaths = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var target in paths)
            {
                string path = NormalizeFilePath(target);

                if (!IsSameFilePath(root, path) &&
                    !IsDescendantPath(root, path))
                {
                    throw new InvalidOperationException(
                        "작업 폴더 내부 경로만 갱신할 수 있습니다.");
                }

                // 부모 폴더는 자체 상태만 조회한다.
                while (!IsSameFilePath(root, path))
                {
                    queryPaths.Add(path);
                    path = Path.GetDirectoryName(path);
                }

                queryPaths.Add(root);
            }

            return queryPaths;
        }

        #endregion
    }
}
