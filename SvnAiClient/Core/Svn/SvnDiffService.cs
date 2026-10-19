using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SharpSvn;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Svn
{
    /// <summary>작업 복사본의 Diff 및 검수에 사용할 변경 전후 전체 소스를 조회한다.</summary>
    internal sealed class SvnDiffService
    {
        #region Fields

        /// <summary>SVN 작업의 진행 및 오류 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>작업마다 인증 설정을 적용한 SVN 클라이언트를 생성한다.</summary>
        private readonly SvnClientFactory _clients;

        #endregion

        #region Constructors

        public SvnDiffService(SvnClientFactory clients)
        {
            if (clients == null) throw new ArgumentNullException(nameof(clients));
            _clients = clients;
        }

        #endregion

        #region Public Methods

        /// <summary>파일의 BASE와 작업본을 비교하여 필요 시 전체 문맥을 포함한 Diff를 반환한다.</summary>
        public Task<string> GetDiffAsync(
            string filePath,
            CancellationToken cancellationToken = default(CancellationToken),
            bool fullContext = false)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using (var client = _clients.CreateClient())
                    using (var stream = new MemoryStream())
                    {
                        var args = new SvnDiffArgs();

                        args.Cancel += (sender, e) => e.Cancel = cancellationToken.IsCancellationRequested;

                        if (!client.Diff(new SvnPathTarget(filePath, SvnRevision.Base), new SvnPathTarget(filePath, SvnRevision.Working), args, stream))
                        {
                            throw new InvalidOperationException("Diff 조회가 완료되지 않았습니다.");
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        stream.Position = 0;

                        string diff;

                        using (var reader = new StreamReader(
                                   stream, Encoding.UTF8, true, 4096, true))
                        {
                            diff = reader.ReadToEnd();
                        }

                        if (!fullContext || string.IsNullOrWhiteSpace(diff))
                            return diff;

                        int contextSize;

                        using (Stream source = File.Exists(filePath)
                                   ? (Stream)new FileStream(
                                       filePath,
                                       FileMode.Open,
                                       FileAccess.Read,
                                       FileShare.ReadWrite | FileShare.Delete)
                                   : new MemoryStream())
                        {
                            contextSize = SvnDiffContext.GetFullContextSize(
                                diff, source, cancellationToken);
                        }

                        args.DiffArguments.Add("--context=" + contextSize);

                        stream.SetLength(0);
                        stream.Position = 0;

                        if (!client.Diff(
                                new SvnPathTarget(filePath, SvnRevision.Base),
                                new SvnPathTarget(filePath, SvnRevision.Working),
                                args,
                                stream))
                        {
                            throw new InvalidOperationException(
                                "전체 문맥 Diff 조회에 실패했습니다.");
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        stream.Position = 0;

                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                            return reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Logger.Error(ex, "GetDiff 오류: " + filePath);
                    throw;
                }
            }, cancellationToken);
        }

        /// <summary>저장소 HEAD를 조회 시점의 리비전으로 고정하고 선택 파일을 현재 작업본과 비교한다.</summary>
        public Task<string> GetHeadWorkingDiffAsync(string filePath,
            CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("비교할 파일 경로가 필요합니다.", nameof(filePath));
            filePath = Path.GetFullPath(filePath);
            return Task.Run(() =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    using (var client = _clients.CreateClient())
                    using (var stream = new MemoryStream())
                    {
                        string parent = Path.GetDirectoryName(filePath);
                        Collection<SvnInfoEventArgs> localInfos;
                        var localArgs = new SvnInfoArgs();
                        localArgs.Cancel += (sender, e) => e.Cancel = token.IsCancellationRequested;
                        if (!client.GetInfo(parent, localArgs, out localInfos) || localInfos.Count == 0 || localInfos[0].RepositoryRoot == null)
                            throw new InvalidOperationException("비교 대상의 저장소 주소를 확인할 수 없습니다.");
                        Collection<SvnInfoEventArgs> headInfos;
                        var headArgs = new SvnInfoArgs();
                        headArgs.Cancel += (sender, e) => e.Cancel = token.IsCancellationRequested;
                        if (!client.GetInfo(new SvnUriTarget(localInfos[0].RepositoryRoot, SvnRevision.Head), headArgs, out headInfos) || headInfos.Count == 0)
                            throw new InvalidOperationException("저장소 최신 리비전을 확인할 수 없습니다.");
                        var revision = new SvnRevision(headInfos[0].Revision);
                        string scope = parent;
                        // 새 폴더가 HEAD에 없으면 공통 부모에서 비교한 뒤 선택 파일만 추출한다.
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            Collection<SvnInfoEventArgs> scopeInfos;
                            if (!client.GetInfo(scope, localArgs, out scopeInfos) || scopeInfos.Count == 0 || scopeInfos[0].Uri == null)
                                throw new InvalidOperationException("비교 기준 폴더를 확인할 수 없습니다.");
                            var existsArgs = new SvnInfoArgs { ThrowOnError = false };
                            existsArgs.Cancel += (sender, e) => e.Cancel = token.IsCancellationRequested;
                            Collection<SvnInfoEventArgs> ignored;
                            if (client.GetInfo(new SvnUriTarget(scopeInfos[0].Uri, revision), existsArgs, out ignored)) break;
                            scope = Path.GetDirectoryName(scope);
                            if (string.IsNullOrEmpty(scope))
                                throw new InvalidOperationException("최신 저장소에서 비교 기준 폴더를 찾지 못했습니다.");
                        }
                        string index = filePath.Substring(scope.TrimEnd('\\', '/').Length + 1).Replace('\\', '/');
                        var args = new SvnDiffArgs
                        {
                            Depth = string.Equals(scope, parent, StringComparison.OrdinalIgnoreCase) ? SvnDepth.Children : SvnDepth.Infinity,
                            IgnoreAncestry = true,
                            CopiesAsAdds = true
                        };
                        args.Cancel += (sender, e) => e.Cancel = token.IsCancellationRequested;
                        var from = new SvnPathTarget(scope, revision);
                        var to = new SvnPathTarget(scope, SvnRevision.Working);
                        if (!client.Diff(from, to, args, stream))
                            throw new InvalidOperationException("최신 저장소와의 변경 내용을 조회하지 못했습니다.");
                        token.ThrowIfCancellationRequested();
                        stream.Position = 0;
                        string selected;
                        using (var reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true))
                            selected = ExtractWorkingFileDiff(reader.ReadToEnd(), index, filePath);
                        if (string.IsNullOrWhiteSpace(selected) ||
                            selected.IndexOf("file marked as a binary type", StringComparison.OrdinalIgnoreCase) >= 0)
                            return selected;
                        int context;
                        using (Stream content = File.Exists(filePath)
                            ? (Stream)new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)
                            : new MemoryStream())
                            context = SvnDiffContext.GetFullContextSize(selected, content, token);
                        args.DiffArguments.Add("--context=" + context);
                        stream.SetLength(0);
                        stream.Position = 0;
                        if (!client.Diff(from, to, args, stream))
                            throw new InvalidOperationException("최신 저장소와의 전체 문맥 비교를 조회하지 못했습니다.");
                        token.ThrowIfCancellationRequested();
                        stream.Position = 0;
                        using (var reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true))
                            return ExtractWorkingFileDiff(reader.ReadToEnd(), index, filePath);
                    }
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
            }, token);
        }

        /// <summary>선택 파일의 Diff를 순서대로 조회하여 파일 경로별로 반환한다.</summary>
        public async Task<Dictionary<string, string>> GetDiffsAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken),
            bool fullContext = false)
        {
            var result = new Dictionary<string, string>();
            foreach (var path in filePaths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var diff = await GetDiffAsync(path, cancellationToken, fullContext);
                if (!string.IsNullOrEmpty(diff))
                    result[path] = diff;
            }

            return result;
        }

        /// <summary>바이너리를 제외한 선택 파일의 BASE와 작업본 전체 소스 및 검수용 Diff를 구성한다.</summary>
        public async Task<List<AiReviewFile>> GetReviewFilesAsync(
            IEnumerable<string> filePaths,
            CancellationToken token = default(CancellationToken))
        {
            if (filePaths == null) throw new ArgumentNullException(nameof(filePaths));
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in filePaths) paths.Add(Path.GetFullPath(path));
            var diffs = await GetDiffsAsync(paths, token);
            var files = await Task.Run(() =>
            {
                var result = new List<AiReviewFile>();
                try
                {
                    using (var client = _clients.CreateClient())
                    {
                        foreach (var path in paths)
                        {
                            token.ThrowIfCancellationRequested();
                            var args = new SvnStatusArgs
                            {
                                Depth = SvnDepth.Empty,
                                RetrieveAllEntries = true
                            };
                            args.Cancel += (sender, e) => e.Cancel = token.IsCancellationRequested;
                            Collection<SvnStatusEventArgs> statuses;
                            if (!client.GetStatus(path, args, out statuses))
                                throw new InvalidOperationException("SVN 상태 확인 실패: " + path);
                            SvnStatusEventArgs status = null;
                            foreach (var item in statuses)
                            {
                                if (string.Equals(Path.GetFullPath(item.FullPath), path,
                                    StringComparison.OrdinalIgnoreCase))
                                    status = item;
                            }
                            if (status == null || status.NodeKind == SvnNodeKind.Directory ||
                                status.LocalContentStatus == SvnStatus.NotVersioned ||
                                status.LocalContentStatus == SvnStatus.Missing ||
                                status.LocalContentStatus == SvnStatus.Conflicted)
                                throw new InvalidOperationException("전체 소스를 검수할 수 없는 상태입니다: " + path);
                            string diff;
                            if (!diffs.TryGetValue(path, out diff)) diff = string.Empty;
                            if (diff.IndexOf("file marked as a binary type", StringComparison.OrdinalIgnoreCase) >= 0)
                                continue;
                            string original = string.Empty;
                            if (status.LocalContentStatus != SvnStatus.Added)
                            {
                                using (var content = new MemoryStream())
                                {
                                    var write = new SvnWriteArgs { Revision = SvnRevision.Base };
                                    write.Cancel += (sender, e) => e.Cancel = token.IsCancellationRequested;
                                    if (!client.Write(new SvnPathTarget(path, SvnRevision.Base), content, write))
                                        throw new InvalidOperationException("BASE 소스 조회 실패: " + path);
                                    original = TryDecodeReviewSource(content.ToArray());
                                }
                            }
                            string modified = string.Empty;
                            if (status.LocalContentStatus != SvnStatus.Deleted)
                                modified = TryDecodeReviewSource(File.ReadAllBytes(path));
                            if (original == null || modified == null) continue;
                            result.Add(new AiReviewFile
                            {
                                FilePath = path,
                                OriginalSource = original,
                                ModifiedSource = modified,
                                Diff = diff
                            });
                        }
                    }
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
                return result;
            }, token);
            return files;
        }

        #endregion

        #region Private Methods

        /// <summary>폴더 단위 Diff에서 선택 파일의 Index 구간만 반환하며 차이가 없으면 빈 문자열을 반환한다.</summary>
        private static string ExtractWorkingFileDiff(string diff, string expectedIndex, string filePath)
        {
            var headers = Regex.Matches(diff, @"^Index: (.+)\r?$", RegexOptions.Multiline);
            var selected = new StringBuilder();
            for (int index = 0; index < headers.Count; index++)
            {
                string candidate = headers[index].Groups[1].Value.TrimEnd('\r').Replace('\\', '/');
                if (!string.Equals(candidate, expectedIndex, StringComparison.Ordinal) &&
                    !string.Equals(candidate, filePath.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase)) continue;
                int start = headers[index].Index;
                int end = index + 1 < headers.Count ? headers[index + 1].Index : diff.Length;
                selected.Append(diff.Substring(start, end - start));
            }
            return selected.ToString();
        }

        /// <summary>BOM, UTF-8 또는 CP949로 소스를 읽고 바이너리나 지원하지 않는 인코딩이면 null을 반환한다.</summary>
        private static string TryDecodeReviewSource(byte[] bytes)
        {
            string source;
            if (bytes.Length >= 2 &&
                ((bytes[0] == 0xff && bytes[1] == 0xfe) ||
                 (bytes[0] == 0xfe && bytes[1] == 0xff)))
            {
                using (var stream = new MemoryStream(bytes))
                using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                    source = reader.ReadToEnd();
            }
            else
            {
                int offset = bytes.Length >= 3 && bytes[0] == 0xef &&
                    bytes[1] == 0xbb && bytes[2] == 0xbf ? 3 : 0;
                try
                {
                    source = new UTF8Encoding(false, true)
                        .GetString(bytes, offset, bytes.Length - offset);
                }
                catch (DecoderFallbackException)
                {
                    try
                    {
                        source = Encoding.GetEncoding(949, EncoderFallback.ExceptionFallback,
                            DecoderFallback.ExceptionFallback).GetString(bytes);
                    }
                    catch (DecoderFallbackException) { return null; }
                }
            }
            foreach (char character in source)
                if (char.IsControl(character) && character != '\r' && character != '\n' && character != '\t' && character != '\f')
                    return null;
            return source;
        }

        #endregion
    }
}
