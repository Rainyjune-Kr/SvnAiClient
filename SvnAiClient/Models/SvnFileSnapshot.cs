using System;
using System.Collections.Generic;

namespace SvnAiClient.Models
{
    /// <summary>승인한 파일의 내용, SVN 상태 및 속성 Diff를 메모리에 보관한다.</summary>
    public sealed class SvnFileSnapshot
    {
        #region Properties

        /// <summary>파일이 디스크에 존재하는지 여부.</summary>
        public bool Exists { get; private set; }
        /// <summary>삭제 예약 상태이며 로컬 잔여 파일의 내용은 비교하지 않는다.</summary>
        public bool IsDeleted { get; private set; }
        /// <summary>파일 내용의 SHA-256 해시. 삭제 예약 파일은 null이다.</summary>
        public string ContentHash { get; private set; }
        /// <summary>노드, 본문 및 속성 상태를 합친 SVN 상태 식별값.</summary>
        public string State { get; private set; }
        /// <summary>SVN BASE 리비전.</summary>
        public long BaseRevision { get; private set; }
        /// <summary>속성 변경을 포함한 SVN Diff.</summary>
        public string Diff { get; private set; }

        #endregion

        #region Constructors

        internal SvnFileSnapshot(bool exists, bool isDeleted, string contentHash,
            string state, long baseRevision, string diff)
        {
            Exists = exists;
            IsDeleted = isDeleted;
            ContentHash = contentHash;
            State = state;
            BaseRevision = baseRevision;
            Diff = diff ?? string.Empty;
        }

        #endregion

        #region Operations

        /// <summary>대상 집합과 파일별 SVN 상태, Diff 및 내용을 비교한다.</summary>
        public static bool AreEqual(Dictionary<string, SvnFileSnapshot> left,
            Dictionary<string, SvnFileSnapshot> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            foreach (var pair in left)
            {
                SvnFileSnapshot other;
                var value = pair.Value;
                if (value == null || !right.TryGetValue(pair.Key, out other) || other == null ||
                    value.IsDeleted != other.IsDeleted || value.State != other.State ||
                    value.BaseRevision != other.BaseRevision ||
                    !string.Equals(value.Diff, other.Diff, StringComparison.Ordinal)) return false;
                if (!value.IsDeleted && (value.Exists != other.Exists ||
                    value.ContentHash != other.ContentHash)) return false;
            }
            return true;
        }

        #endregion
    }
}
