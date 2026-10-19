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
    /// <summary>작업 복사본의 변경 작업과 저장소 연결 정보 조회를 담당한다.</summary>
    internal sealed class SvnWorkingCopyService
    {
        #region Fields

        /// <summary>SVN 작업의 진행 및 오류 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>작업마다 인증 설정을 적용한 SVN 클라이언트를 생성한다.</summary>
        private readonly SvnClientFactory _clients;

        #endregion

        #region Constructors

        public SvnWorkingCopyService(SvnClientFactory clients)
        {
            if (clients == null) throw new ArgumentNullException(nameof(clients));
            _clients = clients;
        }

        #endregion

        #region Public Methods

        /// <summary>선택 항목을 커밋하고 성공 리비전이 확인되면 이후 취소나 오류에도 해당 리비전을 반환한다.</summary>
        public Task<long> CommitAsync(
            IEnumerable<string> filePaths,
            string message,
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                SvnCommitResult commitResult = null;

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var client = _clients.CreateClient())
                    {
                        var args = new SvnCommitArgs
                        {
                            LogMessage = message
                        };

                        args.Cancel += (sender, e) =>
                            e.Cancel = cancellationToken.IsCancellationRequested;

                        args.Committing += (sender, e) => { progressCallback?.Invoke("Commit 중..."); };

                        var targets = new Collection<string>(
                            new List<string>(filePaths));

                        client.Commit(targets, args, out commitResult);

                        if (commitResult == null || commitResult.Revision < 0)
                        {
                            throw new InvalidOperationException(
                                "커밋 완료 리비전을 확인할 수 없습니다. "
                                + "SVN 로그에서 완료 여부를 확인한 후 다시 시도하세요.");
                        }

                        // 완료 리비전을 받은 뒤에는 취소 요청으로 성공을 바꾸지 않는다.
                        Logger.Info("Commit 완료: r{0}", commitResult.Revision);
                        return commitResult.Revision;
                    }
                }
                catch (Exception ex)
                {
                    if (commitResult != null && commitResult.Revision >= 0)
                    {
                        Logger.Warn(ex,
                            "Commit 완료 리비전 확인 후 오류 발생: r{0}",
                            commitResult.Revision);

                        return commitResult.Revision;
                    }

                    if (ex is OperationCanceledException)
                        throw;

                    if (ex is SvnException &&
                        cancellationToken.IsCancellationRequested)
                    {
                        Logger.Info("Commit 취소 요청: 완료 리비전 미확인");

                        throw new OperationCanceledException(
                            "커밋 취소가 요청되었습니다. SVN 로그에서 완료 여부를 확인하세요.",
                            ex,
                            cancellationToken);
                    }

                    Logger.Error(ex, "Commit 오류");
                    throw;
                }
            }, cancellationToken);
        }

        /// <summary>작업 복사본을 최신 리비전으로 갱신하고 진행 상황을 전달한다.</summary>
        public Task UpdateAsync(
            string workingCopyPath,
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var client = _clients.CreateClient())
                    {
                        var args = new SvnUpdateArgs();

                        args.Cancel += (sender, e) =>
                            e.Cancel = cancellationToken.IsCancellationRequested;

                        args.Notify += (sender, e) =>
                        {
                            progressCallback?.Invoke($"{e.Action}: {e.Path}");
                        };

                        SvnUpdateResult updateResult;
                        client.Update(workingCopyPath, args, out updateResult);

                        Logger.Info($"Update 완료: {workingCopyPath}");
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SvnException) when (cancellationToken.IsCancellationRequested)
                {
                    Logger.Info("Update 취소: {0}", workingCopyPath);
                    throw new OperationCanceledException(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Update 오류: {workingCopyPath}");
                    throw;
                }
            }, cancellationToken);
        }

        /// <summary>선택 경로 자체의 로컬 변경을 되돌린다.</summary>
        public Task RevertAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (filePaths == null)
                throw new ArgumentNullException(nameof(filePaths));

            var paths = new List<string>(filePaths);

            return Task.Run(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var client = _clients.CreateClient())
                    {
                        var args = new SvnRevertArgs
                        {
                            Depth = SvnDepth.Empty
                        };

                        args.Cancel += (sender, e) =>
                            e.Cancel = cancellationToken.IsCancellationRequested;

                        foreach (var path in paths)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            if (!client.Revert(path, args))
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                throw new InvalidOperationException(
                                    "Revert가 완료되지 않았습니다: " + path);
                            }

                            Logger.Debug("Revert: {0}", path);
                        }
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
                    Logger.Error(ex, "Revert 오류");
                    throw;
                }
            }, cancellationToken);
        }

        /// <summary>검증된 미등록 파일의 SVN 추가를 예약한다.</summary>
        public Task AddAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ScheduleFilesAsync(
                filePaths, true, cancellationToken);
        }

        /// <summary>로컬 파일을 유지하면서 검증된 파일의 SVN 삭제를 예약한다.</summary>
        public Task DeleteAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ScheduleFilesAsync(
                filePaths, false, cancellationToken);
        }

        /// <summary>지정한 폴더가 유효한 SVN 작업 복사본인지 확인한다.</summary>
        public bool IsWorkingCopy(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return false;

            try
            {
                using (var client = _clients.CreateClient())
                {
                    SvnInfoEventArgs info;
                    return client.GetInfo(path, out info);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>작업 복사본 경로에 연결된 저장소 URL을 조회한다.</summary>
        public string GetRepositoryUrl(string workingCopyPath)
        {
            try
            {
                using (var client = _clients.CreateClient())
                {
                    SvnInfoEventArgs info;
                    if (client.GetInfo(workingCopyPath, out info))
                        return info.Uri?.ToString() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"GetRepositoryUrl 오류: {workingCopyPath}");
            }

            return string.Empty;
        }

        #endregion

        #region Private Methods

        /// <summary>전체 대상의 상태를 먼저 검증한 뒤 파일별 추가 또는 삭제 예약을 실행한다.</summary>
        private Task ScheduleFilesAsync(
            IEnumerable<string> filePaths,
            bool isAdd,
            CancellationToken cancellationToken)
        {
            if (filePaths == null)
                throw new ArgumentNullException(nameof(filePaths));

            // 작업 시작 전에 대상 경로를 고정한다.
            var paths = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var path in filePaths)
                paths.Add(Path.GetFullPath(path));

            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var client = _clients.CreateClient())
                {
                    // 변경 전에 모든 대상의 실제 상태를 검증한다.
                    foreach (var path in paths)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var statusArgs = new SvnStatusArgs
                        {
                            Depth = SvnDepth.Empty,
                            RetrieveAllEntries = true,
                            RetrieveIgnoredEntries = true
                        };

                        statusArgs.Cancel += (sender, e) =>
                            e.Cancel = cancellationToken.IsCancellationRequested;

                        Collection<SvnStatusEventArgs> statuses;
                        bool statusRead;

                        try
                        {
                            statusRead = client.GetStatus(
                                path, statusArgs, out statuses);
                        }
                        catch (SvnException) when (
                            cancellationToken.IsCancellationRequested)
                        {
                            throw new OperationCanceledException(cancellationToken);
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        if (!statusRead || statuses == null)
                        {
                            throw new InvalidOperationException(
                                "SVN 상태를 확인할 수 없습니다: " + path);
                        }

                        SvnStatusEventArgs target = null;

                        foreach (var status in statuses)
                        {
                            if (string.Equals(
                                    Path.GetFullPath(status.FullPath),
                                    path,
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                target = status;
                                break;
                            }
                        }

                        if (target == null ||
                            target.NodeKind == SvnNodeKind.Directory ||
                            Directory.Exists(path))
                        {
                            throw new InvalidOperationException(
                                "파일만 처리할 수 있습니다: " + path);
                        }

                        var state = SvnStatusService.MapStatus(target.LocalNodeStatus);

                        if (isAdd)
                        {
                            if (state != SvnStatusKind.Unversioned ||
                                !File.Exists(path))
                            {
                                throw new InvalidOperationException(
                                    "존재하는 미등록 파일만 추가할 수 있습니다: "
                                    + path);
                            }
                        }
                        else
                        {
                            if (state != SvnStatusKind.Normal &&
                                state != SvnStatusKind.Modified &&
                                state != SvnStatusKind.Missing)
                            {
                                throw new InvalidOperationException(
                                    "삭제 예약할 수 없는 상태입니다: " + path);
                            }
                        }
                    }

                    // SVN 로컬 작업은 파일별로 실행된다.
                    // 중간 실패 시 앞서 성공한 파일의 변경은 남을 수 있다.
                    foreach (var path in paths)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        try
                        {
                            bool succeeded;

                            if (isAdd)
                            {
                                var args = new SvnAddArgs
                                {
                                    Depth = SvnDepth.Empty,
                                    AddParents = false,
                                    Force = false
                                };

                                args.Cancel += (sender, e) =>
                                    e.Cancel = cancellationToken.IsCancellationRequested;

                                succeeded = client.Add(path, args);
                            }
                            else
                            {
                                var args = new SvnDeleteArgs
                                {
                                    KeepLocal = true,
                                    Force = false
                                };

                                args.Cancel += (sender, e) =>
                                    e.Cancel = cancellationToken.IsCancellationRequested;

                                succeeded = client.Delete(path, args);
                            }

                            if (!succeeded)
                            {
                                throw new InvalidOperationException(
                                    "SVN 작업이 완료되지 않았습니다: " + path);
                            }
                        }
                        catch (SvnException)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            throw;
                        }
                    }
                }
            }, cancellationToken);
        }

        #endregion
    }
}
