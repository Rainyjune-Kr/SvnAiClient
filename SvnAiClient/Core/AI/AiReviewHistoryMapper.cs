using System;
using System.Collections.Generic;
using System.Linq;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>검수 결과에서 로컬 이력에 저장할 항목만 복사한다.</summary>
    internal static class AiReviewHistoryMapper
    {
        #region Public Methods

        /// <summary>검수 시작 시점의 정보와 결과를 독립된 이력 스냅샷으로 변환한다.</summary>
        public static AiReviewHistoryEntry Create(
            AiReviewResult result,
            string workingCopyPath,
            string modelName,
            IEnumerable<string> filePaths,
            string changeContext)
        {
            var entry = new AiReviewHistoryEntry
            {
                Id = Guid.NewGuid(),
                ReviewedAt = new DateTimeOffset(result.ReviewedAt),
                WorkingCopyPath = workingCopyPath,
                ProviderName = result.ProviderName,
                ModelName = modelName,
                FilePaths = filePaths.ToList(),
                ChangeContext = changeContext,
                Status = result.Status,
                FailureKind = result.FailureKind,
                Summary = result.Summary,
                SuggestedCommitMessage = result.SuggestedCommitMessage,
                EstimatedInputTokens = result.EstimatedInputTokens,
                ActualInputTokens = result.ActualInputTokens,
                ActualOutputTokens = result.ActualOutputTokens,
                ElapsedMs = result.ElapsedMs
            };

            foreach (var issue in result.Issues ?? new List<AiReviewIssue>())
            {
                if (issue == null) continue;
                entry.Issues.Add(new AiReviewHistoryIssue
                {
                    Severity = issue.Severity,
                    Category = issue.Category,
                    Title = issue.Title,
                    Description = issue.Description,
                    FilePath = issue.FilePath,
                    LineNumber = issue.LineNumber
                });
            }

            foreach (var batch in result.Batches ?? new List<AiReviewBatch>())
            {
                if (batch == null) continue;
                entry.Batches.Add(new AiReviewHistoryBatch
                {
                    FilePaths = batch.FilePaths == null
                        ? new List<string>() : new List<string>(batch.FilePaths),
                    Status = batch.Status,
                    FailureKind = batch.FailureKind,
                    SuggestedCommitMessage = batch.SuggestedCommitMessage
                });
            }

            return entry;
        }

        #endregion
    }
}
