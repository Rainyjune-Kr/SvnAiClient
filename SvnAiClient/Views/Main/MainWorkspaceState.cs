using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SvnAiClient.Helpers;
using SvnAiClient.Models;
using static SvnAiClient.Helpers.WorkingCopyPath;

namespace SvnAiClient.Views.Main
{
    /// <summary>메인 화면의 작업 복사본 상태 캐시, 표시 목록 및 커밋 선택을 관리한다.</summary>
    internal sealed class MainWorkspaceState
    {
        #region Fields

        /// <summary>현재 작업 복사본의 전체 SVN 상태 캐시.</summary>
        private List<SvnFileStatus> _allStatuses = new List<SvnFileStatus>();

        /// <summary>폴더 이동과 관계없이 유지하는 커밋 대상 목록.</summary>
        private readonly List<SvnFileStatus> _commitTargets = new List<SvnFileStatus>();

        /// <summary>상태 캐시를 기준으로 미등록 빌드 산출물과 관리 항목을 구분하는 표시 필터.</summary>
        private WorkingCopyDisplayFilter _displayFilter;

        #endregion

        #region Properties

        /// <summary>기존 화면과 커밋 대화상자가 공유하는 커밋 대상 목록.</summary>
        public List<SvnFileStatus> CommitTargets { get { return _commitTargets; } }

        #endregion

        #region Public Methods

        /// <summary>현재 체크한 커밋 대상의 정규화된 경로 집합을 반환한다.</summary>
        public HashSet<string> GetCheckedPaths()
        {
            return new HashSet<string>(
                _commitTargets.Where(file => file.IsChecked)
                    .Select(file => NormalizeFilePath(file.FullPath)),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>선택 가능 여부를 확인하고 파일의 체크 상태와 누적 커밋 대상을 갱신한다.</summary>
        public bool SetCommitChecked(SvnFileStatus file, bool requestedCheck)
        {
            if (file == null) return false;
            file.IsChecked = requestedCheck && CanSelectForCommit(file);
            _commitTargets.RemoveAll(target => IsSameFilePath(target.FullPath, file.FullPath));
            if (file.IsChecked)
                _commitTargets.Add(file);
            return file.IsChecked;
        }

        /// <summary>선택 항목을 커밋 대상에서 제거하고 체크를 해제한다.</summary>
        public void RemoveCommitTarget(SvnFileStatus selected)
        {
            if (selected == null)
                return;

            _commitTargets.RemoveAll(target => IsSameFilePath(target.FullPath, selected.FullPath));
            selected.IsChecked = false;
        }

        /// <summary>커밋이 성공한 경로를 누적 커밋 대상에서 제거한다.</summary>
        public void RemoveCommittedTargets(List<string> paths)
        {
            _commitTargets.RemoveAll(target => paths.Any(path => IsSameFilePath(target.FullPath, path)));
        }

        /// <summary>전체 조회 결과로 캐시를 교체하고 이전에 체크한 파일의 선택을 복원한다.</summary>
        public void ReplaceStatuses(string rootPath, List<SvnFileStatus> statuses,
            HashSet<string> selectedPaths)
        {
            _allStatuses = statuses;
            _displayFilter = new WorkingCopyDisplayFilter(rootPath, _allStatuses);
            _commitTargets.Clear();
            foreach (var status in statuses)
            {
                if (!CanSelectForCommit(status) ||
                    !IsDescendantPath(rootPath, status.FullPath) ||
                    !selectedPaths.Remove(NormalizeFilePath(status.FullPath)))
                    continue;
                var target = CreateGridItem(rootPath, status);
                target.IsChecked = true;
                _commitTargets.Add(target);
            }
        }

        /// <summary>조회한 경로만 캐시와 커밋 대상에 병합하고 조회 범위 밖의 항목은 유지한다.</summary>
        public void MergeStatuses(string rootPath, HashSet<string> queryPaths,
            List<SvnFileStatus> statuses, HashSet<string> checkedPaths)
        {
            // 모든 지정 경로 조회가 성공한 뒤 캐시를 교체한다.
            var cache = new Dictionary<string, SvnFileStatus>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var status in _allStatuses)
                cache[NormalizeFilePath(status.FullPath)] = status;

            foreach (var path in queryPaths)
                cache.Remove(path);

            foreach (var status in statuses)
                cache[NormalizeFilePath(status.FullPath)] = status;

            _allStatuses = cache.Values.ToList();
            _displayFilter = new WorkingCopyDisplayFilter(rootPath, _allStatuses);

            _commitTargets.RemoveAll(file =>
                queryPaths.Contains(NormalizeFilePath(file.FullPath)));

            foreach (var status in statuses)
            {
                if (!CanSelectForCommit(status) ||
                    !checkedPaths.Contains(
                        NormalizeFilePath(status.FullPath)))
                {
                    continue;
                }

                var target = CreateGridItem(rootPath, status);
                target.IsChecked = true;
                _commitTargets.Add(target);
            }
        }

        /// <summary>삭제된 폴더의 SVN 상태도 고려하여 유지 가능한 탐색 위치를 구한다.</summary>
        public string GetExistingRelativePath(string rootPath, string relativePath)
        {
            var fullPath = Path.Combine(rootPath, relativePath);
            while (!IsSameFilePath(fullPath, rootPath) &&
                   (!ShouldShowPath(fullPath, true) || (!Directory.Exists(fullPath) && !_allStatuses.Any(status =>
                       IsSameFilePath(fullPath, status.FullPath) ||
                       IsDescendantPath(fullPath, status.FullPath)))))
                fullPath = Path.GetDirectoryName(fullPath);
            return GetRelativePath(rootPath, fullPath);
        }

        /// <summary>디스크와 SVN 상태를 병합하고 필터, 정렬 및 누적 커밋 선택을 적용한다.</summary>
        public List<SvnFileStatus> GetFiles(string rootPath, string fullPath,
            bool onlyChanged, bool includeDescendants, Action<Exception> reportDiskError)
        {
            var merged = new Dictionary<string, SvnFileStatus>(
                StringComparer.OrdinalIgnoreCase);

            // SVN 상태를 경로 기준으로 구성한다.
            var statusByPath = new Dictionary<string, SvnFileStatus>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var status in _allStatuses)
            {
                if (string.IsNullOrWhiteSpace(status.FullPath))
                    continue;

                if (!ShouldShowPath(status.FullPath, status.IsDirectory))
                    continue;

                var path = NormalizeFilePath(status.FullPath);

                if (!IsSameFilePath(rootPath, path) &&
                    !IsDescendantPath(rootPath, path))
                    continue;

                statusByPath[path] = status;
            }

            try
            {
                // 삭제된 폴더는 디스크에 없으므로 탐색을 생략한다.
                if (Directory.Exists(fullPath))
                {
                    foreach (var path in
                             Directory.EnumerateFileSystemEntries(fullPath))
                    {
                        if (string.Equals(
                                Path.GetFileName(path),
                                ".svn",
                                StringComparison.OrdinalIgnoreCase))
                            continue;

                        var normalizedPath = NormalizeFilePath(path);
                        bool isDirectory = Directory.Exists(path);
                        if (!ShouldShowPath(normalizedPath, isDirectory)) continue;

                        SvnFileStatus status;

                        if (statusByPath.TryGetValue(
                                normalizedPath, out status))
                        {
                            merged[normalizedPath] = CreateGridItem(rootPath, status);
                        }
                        else
                        {
                            // 조회되지 않은 항목을 정상 상태로 추정하지 않는다.
                            merged[normalizedPath] = new SvnFileStatus
                            {
                                FullPath = normalizedPath,
                                RelativePath = GetRelativePath(
                                    rootPath, normalizedPath),
                                FileName = Path.GetFileName(normalizedPath),
                                Status = SvnStatusKind.None,
                                IsDirectory = isDirectory,
                                LastModified = isDirectory
                                    ? Directory.GetLastWriteTime(path)
                                    : File.GetLastWriteTime(path)
                            };
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                reportDiskError(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                reportDiskError(ex);
            }

            // SVN 항목을 병합한다.
            // 디스크에 없는 Deleted/Missing 항목도 여기서 추가된다.
            foreach (var pair in statusByPath)
            {
                var path = pair.Key;

                bool inScope = includeDescendants
                    ? IsDescendantPath(fullPath, path)
                    : IsDirectChildPath(fullPath, path);

                if (!inScope)
                    continue;

                merged[path] = CreateGridItem(rootPath, pair.Value);
            }

            // 디렉터리 자체의 상태는 유지하고 하위 변경만 별도로 표시한다.
            foreach (var item in merged.Values)
            {
                if (!item.IsDirectory)
                    continue;

                item.HasDescendantChanges = statusByPath.Values.Any(status =>
                    HasSvnChange(status) &&
                    IsDescendantPath(item.FullPath, status.FullPath));
            }

            var items = merged.Values
                .Where(item =>
                    !onlyChanged ||
                    HasSvnChange(item) ||
                    item.HasDescendantChanges)
                .OrderByDescending(item => item.IsDirectory)
                .ThenBy(
                    item => item.RelativePath,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

            // 변경 필터와 관계없이 상위 폴더 이동을 제공한다.
            if (!IsSameFilePath(rootPath, fullPath))
            {
                var parentPath = Path.GetDirectoryName(fullPath);

                items.Insert(0, new SvnFileStatus
                {
                    FullPath = parentPath,
                    RelativePath = IsSameFilePath(rootPath, parentPath)
                        ? string.Empty
                        : GetRelativePath(rootPath, parentPath),
                    FileName = "..",
                    Status = SvnStatusKind.None,
                    IsDirectory = true,
                    IsChecked = false
                });
            }

            RestoreCommitChecks(items);

            return items;
        }

        /// <summary>누적 커밋 대상에 맞춰 표시 항목의 체크를 복원한다.</summary>
        public void RestoreCommitChecks(IEnumerable<SvnFileStatus> files)
        {
            foreach (var file in files)
            {
                file.IsChecked =
                    CanSelectForCommit(file) &&
                    _commitTargets.Exists(target =>
                        target.IsChecked &&
                        IsSameFilePath(target.FullPath, file.FullPath));
            }
        }

        /// <summary>폴더 자체 또는 하위 항목에 SVN 변경이 있는지 확인한다.</summary>
        public bool HasFolderChanges(string folderPath)
        {
            return _allStatuses.Any(item => HasSvnChange(item) && ShouldShowPath(item.FullPath, item.IsDirectory) &&
                (IsSameFilePath(folderPath, item.FullPath) || IsDescendantPath(folderPath, item.FullPath)));
        }

        /// <summary>트리와 파일 목록에서 동일한 빌드 산출물 제외 기준을 적용한다.</summary>
        public bool ShouldShowPath(string fullPath, bool isDirectory)
        {
            return _displayFilter == null || _displayFilter.ShouldShow(fullPath, isDirectory);
        }

        /// <summary>작업 복사본의 상태 캐시와 커밋 대상을 비운다.</summary>
        public void Clear()
        {
            _allStatuses.Clear();
            _displayFilter = null;
            _commitTargets.Clear();
        }

        #endregion

        #region Private Methods

        /// <summary>속성 변경을 포함하여 변경 목록에 표시할 SVN 상태인지 판단한다.</summary>
        private static bool HasSvnChange(SvnFileStatus item)
        {
            if (item.HasPropertyChanges)
                return true;

            switch (item.Status)
            {
                case SvnStatusKind.Modified:
                case SvnStatusKind.Added:
                case SvnStatusKind.Deleted:
                case SvnStatusKind.Missing:
                case SvnStatusKind.Unversioned:
                case SvnStatusKind.Conflicted:
                case SvnStatusKind.Incomplete:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>기존 메인 화면의 커밋 선택 허용 기준을 적용한다.</summary>
        private static bool CanSelectForCommit(SvnFileStatus file)
        {
            if (file == null || file.IsDirectory)
                return false;

            return file.Status == SvnStatusKind.Modified ||
                   file.Status == SvnStatusKind.Added ||
                   file.Status == SvnStatusKind.Deleted ||
                   (file.Status == SvnStatusKind.Normal &&
                    file.HasPropertyChanges);
        }

        /// <summary>상태 캐시와 독립된 표시용 파일 객체를 생성한다.</summary>
        private SvnFileStatus CreateGridItem(string rootPath, SvnFileStatus source)
        {
            var path = NormalizeFilePath(source.FullPath);

            return new SvnFileStatus
            {
                FullPath = path,
                RelativePath = GetRelativePath(
                    rootPath, path),
                FileName = Path.GetFileName(path),
                Status = source.Status,
                IsVersioned = source.IsVersioned,
                LastModified = source.LastModified,
                IsDirectory = source.IsDirectory,
                HasPropertyChanges = source.HasPropertyChanges,
                IsChecked = false
            };
        }

        #endregion
    }
}
