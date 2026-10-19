using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Svn
{
    /// <summary>화면에 SVN 작업의 공개 진입점을 제공하고 기능별 서비스에 실행을 위임한다.</summary>
    public class SvnService : IDisposable
    {
        #region Fields

        /// <summary>SVN 작업의 진행 및 오류 정보를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>작업 복사본 전체 및 부분 상태 조회를 담당한다.</summary>
        private readonly SvnStatusService _status;

        /// <summary>작업 복사본 변경 작업과 연결 정보 조회를 담당한다.</summary>
        private readonly SvnWorkingCopyService _workingCopy;

        /// <summary>작업 복사본 Diff와 검수용 소스 조회를 담당한다.</summary>
        private readonly SvnDiffService _diff;

        /// <summary>승인한 파일의 해시와 SVN 상태를 수집한다.</summary>
        private readonly SvnSnapshotService _snapshots;

        /// <summary>로그와 리비전 간 비교를 담당한다.</summary>
        private readonly SvnHistoryService _history;

        /// <summary>서비스 해제 처리의 중복 실행 여부.</summary>
        private bool _disposed;

        #endregion

        #region Constructors

        public SvnService(SvnSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var clients = new SvnClientFactory(settings);
            _status = new SvnStatusService(clients);
            _workingCopy = new SvnWorkingCopyService(clients);
            _diff = new SvnDiffService(clients);
            _snapshots = new SvnSnapshotService(clients, _diff);
            _history = new SvnHistoryService(clients);
        }

        #endregion

        #region Public Methods

        /// <summary>작업 복사본 전체의 SVN 상태를 조회한다.</summary>
        public Task<List<SvnFileStatus>> GetStatusAsync(
            string workingCopyPath,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _status.GetStatusAsync(workingCopyPath, cancellationToken);
        }

        /// <summary>지정한 경로만 조회하여 부분 갱신용 상태를 반환한다.</summary>
        public Task<List<SvnFileStatus>> GetStatusForPathsAsync(
            IEnumerable<string> paths,
            CancellationToken token = default(CancellationToken))
        {
            return _status.GetStatusForPathsAsync(paths, token);
        }

        /// <summary>작업 복사본 파일의 Diff를 조회한다.</summary>
        public Task<string> GetDiffAsync(
            string filePath,
            CancellationToken cancellationToken = default(CancellationToken),
            bool fullContext = false)
        {
            return _diff.GetDiffAsync(filePath, cancellationToken, fullContext);
        }

        /// <summary>저장소 최신 HEAD와 현재 작업 파일을 전체 문맥으로 비교한다.</summary>
        public Task<string> GetHeadWorkingDiffAsync(string filePath,
            CancellationToken token = default(CancellationToken))
        {
            return _diff.GetHeadWorkingDiffAsync(filePath, token);
        }

        /// <summary>여러 파일의 Diff를 파일 경로별로 조회한다.</summary>
        public Task<Dictionary<string, string>> GetDiffsAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken),
            bool fullContext = false)
        {
            return _diff.GetDiffsAsync(filePaths, cancellationToken, fullContext);
        }

        /// <summary>커밋 대상의 내용 해시, 존재 여부, SVN 상태 및 속성 Diff를 수집한다.</summary>
        public Task<Dictionary<string, SvnFileSnapshot>> GetFileSnapshotsAsync(
            IEnumerable<string> filePaths, CancellationToken token = default(CancellationToken))
        {
            return _snapshots.CaptureAsync(filePaths, token);
        }

        /// <summary>검수 가능한 텍스트 파일의 변경 전후 전체 소스와 Diff를 조회한다.</summary>
        public Task<List<AiReviewFile>> GetReviewFilesAsync(
            IEnumerable<string> filePaths,
            CancellationToken token = default(CancellationToken))
        {
            return _diff.GetReviewFilesAsync(filePaths, token);
        }

        /// <summary>선택 항목을 커밋하고 성공 리비전이 확인되면 이후 취소나 오류에도 해당 리비전을 반환한다.</summary>
        public Task<long> CommitAsync(
            IEnumerable<string> filePaths,
            string message,
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _workingCopy.CommitAsync(filePaths, message, progressCallback, cancellationToken);
        }

        /// <summary>작업 복사본을 최신 리비전으로 갱신하고 진행 상황을 전달한다.</summary>
        public Task UpdateAsync(
            string workingCopyPath,
            Action<string> progressCallback = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _workingCopy.UpdateAsync(workingCopyPath, progressCallback, cancellationToken);
        }

        /// <summary>선택 경로 자체의 로컬 변경을 되돌린다.</summary>
        public Task RevertAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _workingCopy.RevertAsync(filePaths, cancellationToken);
        }

        /// <summary>검증된 미등록 파일의 SVN 추가를 예약한다.</summary>
        public Task AddAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _workingCopy.AddAsync(filePaths, cancellationToken);
        }

        /// <summary>로컬 파일을 유지하면서 검증된 파일의 SVN 삭제를 예약한다.</summary>
        public Task DeleteAsync(
            IEnumerable<string> filePaths,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _workingCopy.DeleteAsync(filePaths, cancellationToken);
        }

        /// <summary>지정한 폴더가 유효한 SVN 작업 복사본인지 확인한다.</summary>
        public bool IsWorkingCopy(string path)
        {
            return _workingCopy.IsWorkingCopy(path);
        }

        /// <summary>작업 복사본 경로에 연결된 저장소 URL을 조회한다.</summary>
        public string GetRepositoryUrl(string workingCopyPath)
        {
            return _workingCopy.GetRepositoryUrl(workingCopyPath);
        }

        /// <summary>파일 또는 폴더 범위의 SVN 로그를 조회한다.</summary>
        public Task<List<SvnLogEntry>> GetLogAsync(
            string workingCopyPath,
            int maxCount = 100,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _history.GetLogAsync(workingCopyPath, maxCount, cancellationToken);
        }

        /// <summary>조회 범위 내 두 리비전 사이의 변경 경로를 반환한다.</summary>
        public Task<List<SvnChangedPath>> GetRevisionChangesAsync(
            string localPath,
            long fromRevision,
            long toRevision,
            CancellationToken token = default(CancellationToken))
        {
            return _history.GetRevisionChangesAsync(localPath, fromRevision, toRevision, token);
        }

        /// <summary>선택 리비전과 직전 리비전 사이의 파일 Diff를 조회한다.</summary>
        public Task<string> GetRevisionDiffAsync(
            string workingCopyPath,
            SvnChangedPath change,
            long revision,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _history.GetRevisionDiffAsync(workingCopyPath, change, revision, cancellationToken);
        }

        /// <summary>두 리비전 사이의 선택 파일 Diff를 조회한다.</summary>
        public Task<string> GetRevisionDiffAsync(
            string workingCopyPath,
            SvnChangedPath change,
            long fromRevision,
            long toRevision,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _history.GetRevisionDiffAsync(workingCopyPath, change, fromRevision, toRevision, cancellationToken);
        }

        #endregion

        #region Disposal

        /// <summary>서비스 해제 상태를 기록하며 실제 SVN 클라이언트는 각 작업에서 해제한다.</summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Logger.Debug("SvnService 리소스 해제");
        }

        #endregion
    }
}
