using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SharpSvn;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Svn
{
    /// <summary>작업 복사본 전체 또는 지정한 경로의 SVN 상태를 조회한다.</summary>
    internal sealed class SvnStatusService
    {
        #region Fields

        /// <summary>SVN 작업의 진행 및 오류 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>작업마다 인증 설정을 적용한 SVN 클라이언트를 생성한다.</summary>
        private readonly SvnClientFactory _clients;

        #endregion

        #region Constructors

        public SvnStatusService(SvnClientFactory clients)
        {
            if (clients == null) throw new ArgumentNullException(nameof(clients));
            _clients = clients;
        }

        #endregion

        #region Public Methods

        /// <summary>하위 항목을 포함한 작업 복사본 전체의 SVN 상태를 조회한다.</summary>
        public Task<List<SvnFileStatus>> GetStatusAsync(
            string workingCopyPath,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                var result = new List<SvnFileStatus>();

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var client = _clients.CreateClient())
                    {
                        var args = new SvnStatusArgs
                        {
                            Depth = SvnDepth.Infinity,

                            // 정상 항목도 포함하여 파일 상태를 정확히 구분한다.
                            RetrieveAllEntries = true,

                            // 무시된 항목을 미등록 항목과 구분한다.
                            RetrieveIgnoredEntries = true
                        };

                        args.Cancel += (sender, e) =>
                            e.Cancel = cancellationToken.IsCancellationRequested;

                        Collection<SvnStatusEventArgs> statuses;
                        client.GetStatus(workingCopyPath, args, out statuses);

                        cancellationToken.ThrowIfCancellationRequested();

                        if (statuses == null)
                            return result;

                        foreach (var s in statuses)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var status = MapStatus(s.LocalNodeStatus);

                            result.Add(new SvnFileStatus
                            {
                                FullPath = s.FullPath,
                                RelativePath = s.Path,
                                FileName = Path.GetFileName(s.FullPath),
                                Status = status,
                                IsVersioned = IsVersionedStatus(s.LocalNodeStatus),
                                LastModified = File.Exists(s.FullPath)
                                    ? File.GetLastWriteTime(s.FullPath)
                                    : DateTime.MinValue,
                                IsChecked = true,
                                IsDirectory = s.NodeKind == SvnNodeKind.Directory,
                                HasPropertyChanges =
                                    s.LocalPropertyStatus != SvnStatus.None
                                    && s.LocalPropertyStatus != SvnStatus.Normal
                            });
                        }

                        Logger.Debug(
                            $"Status 조회 완료: {result.Count}개 파일 ({workingCopyPath})");
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SvnException) when (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"GetStatus 오류: {workingCopyPath}");
                    throw;
                }

                cancellationToken.ThrowIfCancellationRequested();
                return result;
            }, cancellationToken);
        }

        /// <summary>지정한 경로 자체의 상태만 조회하여 부분 갱신에 사용할 결과를 반환한다.</summary>
        public Task<List<SvnFileStatus>> GetStatusForPathsAsync(
            IEnumerable<string> paths,
            CancellationToken token = default(CancellationToken))
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            var targets = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var path in paths)
                targets.Add(Path.GetFullPath(path));

            return Task.Run(() =>
            {
                var result = new List<SvnFileStatus>();

                try
                {
                    using (var client = _clients.CreateClient())
                    {
                        foreach (var path in targets)
                        {
                            token.ThrowIfCancellationRequested();

                            var args = new SvnStatusArgs
                            {
                                Depth = SvnDepth.Empty,
                                RetrieveAllEntries = true,
                                RetrieveIgnoredEntries = true
                            };

                            args.Cancel += (sender, e) =>
                                e.Cancel = token.IsCancellationRequested;

                            Collection<SvnStatusEventArgs> statuses;

                            if (!client.GetStatus(path, args, out statuses))
                                throw new InvalidOperationException(
                                    "경로 상태 조회 실패: " + path);

                            token.ThrowIfCancellationRequested();

                            if (statuses == null)
                                continue;

                            foreach (var status in statuses)
                            {
                                if (!string.Equals(
                                    Path.GetFullPath(status.FullPath),
                                    path,
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                result.Add(new SvnFileStatus
                                {
                                    FullPath = path,
                                    RelativePath = status.Path,
                                    FileName = Path.GetFileName(path),
                                    Status = MapStatus(status.LocalNodeStatus),
                                    IsVersioned = IsVersionedStatus(status.LocalNodeStatus),
                                    LastModified = File.Exists(path)
                                        ? File.GetLastWriteTime(path)
                                        : DateTime.MinValue,
                                    IsDirectory =
                                        status.NodeKind == SvnNodeKind.Directory,
                                    HasPropertyChanges =
                                        status.LocalPropertyStatus != SvnStatus.None
                                        && status.LocalPropertyStatus != SvnStatus.Normal,
                                    IsChecked = false
                                });
                            }
                        }
                    }

                    return result;
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
            }, token);
        }

        #endregion

        #region Internal Methods

        /// <summary>표시용 상태로 변환해도 교체·방해 상태 등의 SVN 관리 여부가 유실되지 않도록 한다.</summary>
        private static bool IsVersionedStatus(SvnStatus status)
        {
            return status != SvnStatus.None && status != SvnStatus.NotVersioned && status != SvnStatus.Ignored;
        }

        /// <summary>SharpSvn 상태 값을 애플리케이션의 파일 상태로 변환한다.</summary>
        internal static SvnStatusKind MapStatus(SvnStatus status)
        {
            switch (status)
            {
                case SvnStatus.Modified: return SvnStatusKind.Modified;
                case SvnStatus.Added: return SvnStatusKind.Added;
                case SvnStatus.Deleted: return SvnStatusKind.Deleted;
                case SvnStatus.Conflicted: return SvnStatusKind.Conflicted;
                case SvnStatus.Missing: return SvnStatusKind.Missing;
                case SvnStatus.NotVersioned: return SvnStatusKind.Unversioned;
                case SvnStatus.Normal: return SvnStatusKind.Normal;
                case SvnStatus.Ignored: return SvnStatusKind.Ignored;
                case SvnStatus.External: return SvnStatusKind.External;
                case SvnStatus.Incomplete: return SvnStatusKind.Incomplete;
                default: return SvnStatusKind.None;
            }
        }

        #endregion
    }
}
