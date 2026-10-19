using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using SharpSvn;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Svn
{
    /// <summary>커밋 승인과 검수 유효성 확인에 사용할 파일 스냅샷을 수집한다.</summary>
    internal sealed class SvnSnapshotService
    {
        #region Fields

        /// <summary>SVN 상태를 읽는 클라이언트 팩터리.</summary>
        private readonly SvnClientFactory _clients;
        /// <summary>본문과 속성 변경을 읽는 Diff 서비스.</summary>
        private readonly SvnDiffService _diff;

        #endregion

        #region Constructors

        public SvnSnapshotService(SvnClientFactory clients, SvnDiffService diff)
        {
            if (clients == null) throw new ArgumentNullException(nameof(clients));
            if (diff == null) throw new ArgumentNullException(nameof(diff));
            _clients = clients;
            _diff = diff;
        }

        #endregion

        #region Operations

        /// <summary>모든 대상의 SVN 상태와 Diff를 읽고 삭제 예약 외의 파일 내용을 해시한다.</summary>
        public async Task<Dictionary<string, SvnFileSnapshot>> CaptureAsync(
            IEnumerable<string> filePaths, CancellationToken token)
        {
            if (filePaths == null) throw new ArgumentNullException(nameof(filePaths));
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in filePaths) paths.Add(Path.GetFullPath(path));
            var diffs = await _diff.GetDiffsAsync(paths, token);
            return await Task.Run(() =>
            {
                try
                {
                    var result = new Dictionary<string, SvnFileSnapshot>(StringComparer.OrdinalIgnoreCase);
                    using (var client = _clients.CreateClient())
                    {
                        foreach (var path in paths)
                        {
                            token.ThrowIfCancellationRequested();
                            var args = new SvnStatusArgs { Depth = SvnDepth.Empty, RetrieveAllEntries = true };
                            args.Cancel += (sender, e) => e.Cancel = token.IsCancellationRequested;
                            Collection<SvnStatusEventArgs> statuses;
                            if (!client.GetStatus(path, args, out statuses))
                            {
                                token.ThrowIfCancellationRequested();
                                throw new InvalidOperationException("SVN 상태 확인 실패: " + path);
                            }
                            SvnStatusEventArgs status = null;
                            foreach (var item in statuses)
                                if (string.Equals(Path.GetFullPath(item.FullPath), path, StringComparison.OrdinalIgnoreCase)) status = item;
                            if (status == null || status.NodeKind == SvnNodeKind.Directory ||
                                status.LocalPropertyStatus == SvnStatus.Conflicted ||
                                (status.LocalNodeStatus != SvnStatus.Normal && status.LocalNodeStatus != SvnStatus.Modified &&
                                 status.LocalNodeStatus != SvnStatus.Added && status.LocalNodeStatus != SvnStatus.Deleted))
                                throw new InvalidOperationException("커밋할 수 없는 파일 상태입니다: " + path);
                            bool deleted = status.LocalNodeStatus == SvnStatus.Deleted;
                            bool exists = File.Exists(path);
                            if (!deleted && !exists) throw new FileNotFoundException("커밋 대상 파일이 없습니다.", path);
                            string hash = deleted ? null : HashFile(path, token);
                            string diff;
                            diffs.TryGetValue(path, out diff);
                            result.Add(path, new SvnFileSnapshot(exists, deleted, hash,
                                status.LocalNodeStatus + "/" + status.LocalContentStatus + "/" + status.LocalPropertyStatus,
                                status.Revision, diff));
                        }
                    }
                    token.ThrowIfCancellationRequested();
                    return result;
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
            }, token);
        }

        /// <summary>전체 파일을 메모리에 올리지 않고 쓰기가 차단된 스트림에서 SHA-256을 계산한다.</summary>
        private static string HashFile(string path, CancellationToken token)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var algorithm = SHA256.Create())
            {
                var buffer = new byte[65536];
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    token.ThrowIfCancellationRequested();
                    algorithm.TransformBlock(buffer, 0, count, buffer, 0);
                }
                token.ThrowIfCancellationRequested();
                algorithm.TransformFinalBlock(new byte[0], 0, 0);
                return BitConverter.ToString(algorithm.Hash).Replace("-", string.Empty);
            }
        }

        #endregion
    }
}
