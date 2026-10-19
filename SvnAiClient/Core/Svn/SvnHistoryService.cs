using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SharpSvn;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Svn
{
    /// <summary>경로별 SVN 로그와 리비전 간 변경 내용 및 파일 Diff를 조회한다.</summary>
    internal sealed class SvnHistoryService
    {
        #region Fields

        /// <summary>작업마다 인증 설정을 적용한 SVN 클라이언트를 생성한다.</summary>
        private readonly SvnClientFactory _clients;

        #endregion

        #region Constructors

        public SvnHistoryService(SvnClientFactory clients)
        {
            if (clients == null) throw new ArgumentNullException(nameof(clients));
            _clients = clients;
        }

        #endregion

        #region Public Methods

        /// <summary>지정한 파일 또는 폴더 범위의 커밋 로그와 변경 경로를 조회한다.</summary>
        public Task<List<SvnLogEntry>> GetLogAsync(
            string workingCopyPath,
            int maxCount = 100,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(workingCopyPath))
            {
                throw new ArgumentException(
                    "조회 경로가 필요합니다.",
                    nameof(workingCopyPath));
            }

            if (maxCount < 1 || maxCount > 1000)
                throw new ArgumentOutOfRangeException(nameof(maxCount));

            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = new List<SvnLogEntry>();

                using (var client = _clients.CreateClient())
                {
                    var args = new SvnLogArgs
                    {
                        Start = SvnRevision.Head,
                        End = new SvnRevision(0),
                        Limit = maxCount,
                        RetrieveChangedPaths = true
                    };

                    args.Cancel += (sender, e) =>
                        e.Cancel = cancellationToken.IsCancellationRequested;

                    try
                    {
                        bool success = client.Log(
                            workingCopyPath,
                            args,
                            (sender, e) =>
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                var entry = new SvnLogEntry
                                {
                                    Revision = e.Revision,
                                    Author = e.Author ?? string.Empty,
                                    Date = e.Time.ToLocalTime(),
                                    Message = e.LogMessage ?? string.Empty
                                };

                                // 이벤트 실행 중 변경 경로를 복사한다.
                                if (e.ChangedPaths != null)
                                {
                                    foreach (var change in e.ChangedPaths)
                                    {
                                        entry.Changes.Add(new SvnChangedPath
                                        {
                                            Path = change.Path,
                                            Action = change.Action.ToString(),
                                            IsDirectory = change.NodeKind == SvnNodeKind.Directory
                                        });

                                        entry.ChangedPaths.Add(
                                            change.Action + "  " + change.Path);
                                    }

                                    entry.ChangedPaths.Sort(StringComparer.Ordinal);
                                }

                                result.Add(entry);
                            });

                        cancellationToken.ThrowIfCancellationRequested();

                        if (!success)
                        {
                            throw new InvalidOperationException(
                                "SVN 이력을 가져오지 못했습니다.");
                        }
                    }
                    catch (Exception) when (
                        cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(cancellationToken);
                    }
                }

                return result;
            }, cancellationToken);
        }

        /// <summary>두 리비전 사이에서 조회 범위에 해당하는 변경 경로를 반환한다.</summary>
        public Task<List<SvnChangedPath>> GetRevisionChangesAsync(
            string localPath,
            long fromRevision,
            long toRevision,
            CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(localPath))
                throw new ArgumentException(
                    "조회 경로가 필요합니다.", nameof(localPath));

            if (fromRevision < 0 || toRevision <= fromRevision)
                throw new ArgumentOutOfRangeException(nameof(toRevision));

            return Task.Run(() =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    using (var client = _clients.CreateClient())
                    {
                        SvnInfoEventArgs info;

                        if (!client.GetInfo(localPath, out info) ||
                            info.RepositoryRoot == null ||
                            info.Uri == null)
                        {
                            throw new InvalidOperationException(
                                "저장소 주소를 확인할 수 없습니다.");
                        }

                        var root = new Uri(
                            info.RepositoryRoot.AbsoluteUri.TrimEnd('/') + "/");

                        string scope = "/" + Uri.UnescapeDataString(
                            root.MakeRelativeUri(info.Uri).ToString()).Trim('/');

                        var paths = new List<SvnChangedPath>();
                        var args = new SvnDiffSummaryArgs
                        {
                            Depth = SvnDepth.Infinity,
                            IgnoreAncestry = true
                        };

                        args.Cancel += (sender, e) =>
                            e.Cancel = token.IsCancellationRequested;

                        // 대상이 한쪽 시점에 없어도 비교하도록 루트에서 조회한다.
                        bool success = client.DiffSummary(
                            new SvnUriTarget(
                                root, new SvnRevision(fromRevision)),
                            new SvnUriTarget(
                                root, new SvnRevision(toRevision)),
                            args,
                            (sender, e) =>
                            {
                                token.ThrowIfCancellationRequested();

                                string path = "/" + e.Path.Trim('/');

                                if (scope != "/" &&
                                    path != scope &&
                                    !path.StartsWith(
                                        scope + "/",
                                        StringComparison.Ordinal))
                                {
                                    return;
                                }

                                paths.Add(new SvnChangedPath
                                {
                                    Path = path,
                                    IsDirectory =
                                        e.NodeKind == SvnNodeKind.Directory,
                                    Action =
                                        e.DiffKind == SvnDiffKind.Added ? "A"
                                        : e.DiffKind == SvnDiffKind.Deleted ? "D"
                                        : e.DiffKind == SvnDiffKind.Modified ? "M"
                                        : "P"
                                });
                            });

                        token.ThrowIfCancellationRequested();

                        if (!success)
                            throw new InvalidOperationException(
                                "리비전 간 변경 목록 조회에 실패했습니다.");

                        paths.Sort((left, right) =>
                            string.CompareOrdinal(left.Path, right.Path));

                        return paths;
                    }
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
            }, token);
        }

        /// <summary>선택한 리비전과 직전 리비전 사이의 파일 Diff를 조회한다.</summary>
        public Task<string> GetRevisionDiffAsync(
            string workingCopyPath,
            SvnChangedPath change,
            long revision,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (revision < 1)
                throw new ArgumentOutOfRangeException(nameof(revision));

            return GetRevisionDiffAsync(
                workingCopyPath,
                change,
                revision - 1,
                revision,
                cancellationToken);
        }

        /// <summary>두 리비전 사이의 선택 파일 Diff를 전체 문맥으로 조회한다.</summary>
        public Task<string> GetRevisionDiffAsync(
            string workingCopyPath,
            SvnChangedPath change,
            long fromRevision,
            long toRevision,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change));

            if (fromRevision < 0 || toRevision <= fromRevision)
                throw new ArgumentOutOfRangeException(nameof(toRevision));

            if (change.IsDirectory)
            {
                throw new InvalidOperationException(
                    "폴더 이력 비교는 아직 지원하지 않습니다.");
            }

            if (string.IsNullOrWhiteSpace(change.Path) ||
                !change.Path.StartsWith("/"))
            {
                throw new ArgumentException(
                    "저장소 기준 경로가 필요합니다.");
            }

            return Task.Run(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var client = _clients.CreateClient())
                    using (var stream = new MemoryStream())
                    {
                        SvnInfoEventArgs info;

                        if (!client.GetInfo(workingCopyPath, out info) ||
                            info.RepositoryRoot == null)
                        {
                            throw new InvalidOperationException(
                                "저장소 루트 주소를 확인할 수 없습니다.");
                        }

                        Uri root = info.RepositoryRoot;

                        int slash = change.Path.LastIndexOf('/');
                        string parentPath = change.Path.Substring(0, slash);

                        var relative = new StringBuilder();

                        foreach (string segment in
                            parentPath.Trim('/').Split('/'))
                        {
                            if (segment.Length > 0)
                            {
                                relative.Append(
                                    Uri.EscapeDataString(segment));

                                relative.Append('/');
                            }
                        }

                        Uri parent = new Uri(
                            root.AbsoluteUri.TrimEnd('/')
                            + "/"
                            + relative);

                        var oldRevision = new SvnRevision(fromRevision);
                        var newRevision = new SvnRevision(toRevision);

                        Collection<SvnInfoEventArgs> ignored;

                        var infoArgs = new SvnInfoArgs
                        {
                            ThrowOnError = false
                        };

                        infoArgs.Cancel += (sender, e) =>
                            e.Cancel =
                                cancellationToken.IsCancellationRequested;

                        bool parentExists =
                            client.GetInfo(
                                new SvnUriTarget(parent, oldRevision),
                                infoArgs,
                                out ignored)
                            &&
                            client.GetInfo(
                                new SvnUriTarget(parent, newRevision),
                                infoArgs,
                                out ignored);

                        cancellationToken.ThrowIfCancellationRequested();

                        Uri target = parentExists ? parent : root;

                        string expectedIndex = parentExists
                            ? change.Path.Substring(slash + 1)
                            : change.Path.TrimStart('/');

                        var args = new SvnDiffArgs
                        {
                            Depth = parentExists
                                ? SvnDepth.Children
                                : SvnDepth.Infinity,

                            IgnoreAncestry = true,
                            CopiesAsAdds = true
                        };

                        args.Cancel += (sender, e) =>
                            e.Cancel =
                                cancellationToken.IsCancellationRequested;

                        if (!client.Diff(
                            new SvnUriTarget(target, newRevision),
                            new SvnRevisionRange(oldRevision, newRevision),
                            args,
                            stream))
                        {
                            throw new InvalidOperationException(
                                "리비전 Diff 조회에 실패했습니다.");
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        stream.Position = 0;

                        string diff;

                        using (var reader =
                               new StreamReader(stream, Encoding.UTF8, true, 4096, true))
                        {
                            diff = reader.ReadToEnd();
                        }

                        // 여러 파일 중 선택한 파일의 Diff만 추출한다.
                        string selectedDiff = ExtractFileDiff(
                            diff, expectedIndex, change.Path);

                        // 선택 파일의 저장소 URL을 생성한다.
                        var fileRelative = new StringBuilder();

                        foreach (string segment in change.Path.TrimStart('/').Split('/'))
                        {
                            fileRelative.Append(Uri.EscapeDataString(segment));
                            fileRelative.Append('/');
                        }

                        Uri fileUri = new Uri(
                            root.AbsoluteUri.TrimEnd('/')
                            + "/"
                            + fileRelative.ToString().TrimEnd('/'));

                        // 삭제 파일은 삭제 전 리비전에서 내용을 읽는다.
                        var sourceRevision = change.Action == "D"
                            ? oldRevision
                            : newRevision;

                        int contextSize;

                        using (var content = new MemoryStream())
                        {
                            var writeArgs = new SvnWriteArgs
                            {
                                Revision = sourceRevision
                            };

                            writeArgs.Cancel += (sender, e) =>
                                e.Cancel = cancellationToken.IsCancellationRequested;

                            if (!client.Write(
                                    new SvnUriTarget(fileUri, sourceRevision),
                                    content,
                                    writeArgs))
                            {
                                throw new InvalidOperationException(
                                    "리비전의 파일 내용을 가져오지 못했습니다.");
                            }

                            content.Position = 0;

                            contextSize = SvnDiffContext.GetFullContextSize(
                                selectedDiff, content, cancellationToken);
                        }

                        // 전체 문맥을 포함하여 다시 조회한다.
                        args.DiffArguments.Add("--context=" + contextSize);

                        stream.SetLength(0);
                        stream.Position = 0;

                        if (!client.Diff(
                                new SvnUriTarget(target, newRevision),
                                new SvnRevisionRange(oldRevision, newRevision),
                                args,
                                stream))
                        {
                            throw new InvalidOperationException(
                                "전체 문맥 이력 Diff 조회에 실패했습니다.");
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        stream.Position = 0;

                        using (var reader =
                               new StreamReader(stream, Encoding.UTF8, true, 4096, true))
                        {
                            diff = reader.ReadToEnd();
                        }

                        return ExtractFileDiff(diff, expectedIndex, change.Path);
                    }
                }
                catch (Exception) when (
                    cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(
                        cancellationToken);
                }
            }, cancellationToken);
        }

        #endregion

        #region Private Methods

        /// <summary>저장소 Diff에서 선택한 파일의 Index 구간만 추출한다.</summary>
        private static string ExtractFileDiff(
            string diff,
            string expectedIndex,
            string path)
        {
            var headers = Regex.Matches(
                diff,
                @"^Index: (.+)\r?$",
                RegexOptions.Multiline);

            var selected = new StringBuilder();

            for (int i = 0; i < headers.Count; i++)
            {
                string indexPath =
                    headers[i].Groups[1].Value.TrimEnd('\r');

                if (!string.Equals(
                        indexPath,
                        expectedIndex,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                int start = headers[i].Index;

                int end = i + 1 < headers.Count
                    ? headers[i + 1].Index
                    : diff.Length;

                selected.Append(diff.Substring(start, end - start));
            }

            if (selected.Length == 0)
            {
                throw new InvalidOperationException(
                    "파일의 Diff를 찾지 못했습니다: " + path);
            }

            return selected.ToString();
        }

        #endregion
    }
}
