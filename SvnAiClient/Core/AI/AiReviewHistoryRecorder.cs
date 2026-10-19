using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>검수 요청의 성공, 실패 및 취소 결과를 로컬 이력으로 기록하며 저장 실패를 격리한다.</summary>
    internal sealed class AiReviewHistoryRecorder
    {
        #region Fields

        /// <summary>이력 저장 실패를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>이력에 기록할 작업 복사본 루트 경로.</summary>
        private readonly string _workingCopyPath;

        #endregion

        #region Properties

        /// <summary>이 기록기에서 이력 저장 실패가 발생했는지 여부.</summary>
        public bool HasSaveFailure { get; private set; }

        #endregion

        #region Constructors

        public AiReviewHistoryRecorder(string workingCopyPath)
        {
            _workingCopyPath = workingCopyPath;
        }

        #endregion

        #region Public Methods

        /// <summary>검수 결과를 저장하며 요청이 예외나 취소로 끝나도 해당 상태의 이력을 남긴다.</summary>
        public async Task<AiReviewExecutionResult> ReviewAsync(
            AiReviewService service,
            List<AiReviewFile> reviewFiles,
            List<string> paths,
            string providerName,
            string modelName,
            string changeContext,
            CancellationToken token,
            int excludedFileCount = 0)
        {
            var timer = Stopwatch.StartNew();
            AiReviewResult result;

            try
            {
                result = await service.ReviewAsync(
                    reviewFiles, token, changeContext);
            }
            catch (OperationCanceledException)
            {
                await SaveInterruptedReviewAsync(
                    paths, providerName, modelName,
                    changeContext, timer.ElapsedMilliseconds,
                    AiReviewStatus.Cancelled,
                    AiReviewFailureKind.Cancelled,
                    "검수 요청이 취소되었습니다.");

                throw;
            }
            catch (Exception ex)
            {
                await SaveInterruptedReviewAsync(
                    paths, providerName, modelName,
                    changeContext, timer.ElapsedMilliseconds,
                    AiReviewStatus.Failed,
                    ex is TimeoutException
                        ? AiReviewFailureKind.Timeout
                        : AiReviewFailureKind.ProviderError,
                    "검수 요청 중 오류가 발생했습니다.");

                throw;
            }

            if (excludedFileCount > 0)
                result.Summary = (result.Summary ?? string.Empty) + "\n\n검수 범위: 텍스트 "
                    + reviewFiles.Count + "개. 바이너리 또는 지원하지 않는 인코딩 " + excludedFileCount
                    + "개는 AI 검수에서 제외하며, 커밋 승인 시 해시와 SVN 상태를 확인합니다.";

            // 화면이 닫혔거나 현재 내용이 바뀌었어도 반환된 결과는 보존한다.
            Guid id = await SaveReviewHistoryAsync(
                result, paths, modelName, changeContext);

            return new AiReviewExecutionResult
            {
                Result = result,
                HistoryId = id
            };
        }

        #endregion

        #region Private Methods

        /// <summary>예외 또는 취소 상태의 조회용 결과를 구성하고 이력에 저장한다.</summary>
        private Task<Guid> SaveInterruptedReviewAsync(
            List<string> paths,
            string providerName,
            string modelName,
            string changeContext,
            long elapsedMs,
            AiReviewStatus status,
            AiReviewFailureKind failureKind,
            string summary)
        {
            var result = new AiReviewResult
            {
                ProviderName = providerName,
                ReviewedAt = DateTime.Now,
                ElapsedMs = elapsedMs,
                Status = status,
                FailureKind = failureKind,
                Summary = summary
            };

            result.Batches.Add(new AiReviewBatch
            {
                FilePaths = new List<string>(paths),
                Status = status,
                FailureKind = failureKind,
                ScopeDescription = "선택 파일 전체 소스 통합 검수"
            });

            return SaveReviewHistoryAsync(
                result, paths, modelName, changeContext);
        }

        /// <summary>취소 토큰 없이 이력을 저장하고 저장 실패는 결과 대신 별도 상태와 로그로 기록한다.</summary>
        private async Task<Guid> SaveReviewHistoryAsync(
            AiReviewResult result,
            List<string> paths,
            string modelName,
            string changeContext)
        {
            string error = null;

            Guid id = await Task.Run(() =>
            {
                try
                {
                    Guid savedId;
                    bool saved = new AiReviewHistoryService().TrySave(
                        result,
                        _workingCopyPath,
                        modelName,
                        paths,
                        changeContext,
                        out savedId,
                        out error);

                    return saved ? savedId : Guid.Empty;
                }
                catch (Exception ex)
                {
                    error = "검수 이력 저장 서비스 오류: " + ex.Message;
                    return Guid.Empty;
                }
            }); // 검수 취소 토큰을 전달하지 않아 취소 이력도 저장한다.

            if (error != null)
            {
                HasSaveFailure = true;
                Logger.Warn(error);
            }

            return id;
        }

        #endregion
    }
}
