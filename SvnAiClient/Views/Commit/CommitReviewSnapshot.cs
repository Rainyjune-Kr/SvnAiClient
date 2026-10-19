using System;
using System.Collections.Generic;
using SvnAiClient.Models;

namespace SvnAiClient.Views.Commit
{
    /// <summary>검수 당시 대상과 내용 및 결과를 함께 보관하고 비교 연산을 제공한다.</summary>
    internal sealed class CommitReviewSnapshot
    {
        #region Properties

        /// <summary>검수 당시 대상 파일의 비교용 Diff 스냅샷.</summary>
        public Dictionary<string, string> Diffs { get; private set; }

        /// <summary>검수 당시 전체 커밋 대상의 해시와 SVN 상태.</summary>
        public Dictionary<string, SvnFileSnapshot> Files { get; private set; }

        /// <summary>이슈 위치 이동에 사용할 검수 당시 전체 문맥 Diff 스냅샷.</summary>
        public Dictionary<string, string> ViewerDiffs { get; private set; }

        /// <summary>Diff가 없는 파일도 포함한 검수 당시 선택 경로 목록.</summary>
        public List<string> Paths { get; private set; }

        /// <summary>검수를 시작할 때 선택한 프로바이더 식별자.</summary>
        public string ProviderName { get; private set; }

        /// <summary>검수를 시작할 때 입력한 변경 배경과 요청 내용.</summary>
        public string ChangeContext { get; private set; }

        /// <summary>해당 검수 요청에서 반환된 결과.</summary>
        public AiReviewResult Result { get; private set; }

        /// <summary>저장된 검수 이력 ID이며 저장 실패 시 Guid.Empty.</summary>
        public Guid HistoryId { get; private set; }

        #endregion

        #region Constructors

        public CommitReviewSnapshot(
            Dictionary<string, string> diffs, Dictionary<string, string> viewerDiffs,
            List<string> paths, string providerName, string changeContext,
            AiReviewResult result, Guid historyId, Dictionary<string, SvnFileSnapshot> files)
        {
            Diffs = new Dictionary<string, string>(diffs, StringComparer.OrdinalIgnoreCase);
            ViewerDiffs = new Dictionary<string, string>(viewerDiffs, viewerDiffs.Comparer);
            Paths = new List<string>(paths);
            ProviderName = providerName;
            ChangeContext = changeContext;
            Result = result;
            HistoryId = historyId;
            Files = new Dictionary<string, SvnFileSnapshot>(files, StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Methods

        /// <summary>파일 집합과 Diff 문자열이 정확히 일치하는지 확인한다.</summary>
        public static bool SameDiffs(
            Dictionary<string, string> left,
            Dictionary<string, string> right)
        {
            if (left.Count != right.Count)
                return false;

            foreach (var item in left)
            {
                string value;

                if (!right.TryGetValue(item.Key, out value) ||
                    !string.Equals(
                        item.Value,
                        value,
                        StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>현재 선택 경로와 기대 경로를 대소문자 구분 없이 집합으로 비교한다.</summary>
        public static bool HaveSameTargets(
            IEnumerable<string> currentPaths, IEnumerable<string> expectedPaths)
        {
            var paths = new HashSet<string>(
                currentPaths,
                StringComparer.OrdinalIgnoreCase);

            return paths.SetEquals(expectedPaths);
        }

        #endregion
    }
}
