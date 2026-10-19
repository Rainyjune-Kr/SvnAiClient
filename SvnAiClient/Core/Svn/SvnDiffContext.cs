using System;
using System.IO;
using System.Text;
using System.Threading;

namespace SvnAiClient.Core.Svn
{
    /// <summary>작업 복사본과 리비전 Diff에서 사용할 전체 문맥 크기를 계산한다.</summary>
    internal static class SvnDiffContext
    {
        #region Public Methods

        /// <summary>소스 줄 수와 삭제된 줄 수를 합산해 전체 소스를 포함할 문맥 크기를 구한다.</summary>
        public static int GetFullContextSize(
            string diff,
            Stream source,
            CancellationToken token)
        {
            long count = 1;

            using (var reader = new StreamReader(
                       source, Encoding.UTF8, true, 4096, true))
            {
                while (reader.ReadLine() != null)
                {
                    token.ThrowIfCancellationRequested();
                    count++;
                }
            }

            // 수정본에서 사라진 줄도 포함하여 원본 전체를 덮는다.
            foreach (var line in new DiffParser().Parse(diff))
            {
                token.ThrowIfCancellationRequested();

                if (line.LineType == DiffLineType.Removed)
                    count++;
            }

            if (count > int.MaxValue)
            {
                throw new InvalidOperationException(
                    "표시할 소스의 줄 수가 너무 많습니다.");
            }

            return (int)count;
        }

        #endregion
    }
}
