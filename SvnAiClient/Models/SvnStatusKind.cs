namespace SvnAiClient.Models
{
    /// <summary>작업 복사본 항목의 SVN 상태를 구분한다.</summary>
    public enum SvnStatusKind
    {
        #region Values

        /// <summary>상태 없음 또는 알 수 없는 상태.</summary>
        None,

        /// <summary>수정된 항목.</summary>
        Modified,

        /// <summary>추가 예약된 항목.</summary>
        Added,

        /// <summary>삭제 예약된 항목.</summary>
        Deleted,

        /// <summary>충돌이 발생한 항목.</summary>
        Conflicted,

        /// <summary>SVN 삭제 예약 없이 로컬에서 사라진 항목.</summary>
        Missing,

        /// <summary>SVN 추적 대상이 아닌 항목.</summary>
        Unversioned,

        /// <summary>변경 없는 정상 항목.</summary>
        Normal,

        /// <summary>SVN 무시 규칙에 해당하는 항목.</summary>
        Ignored,

        /// <summary>외부 참조 항목.</summary>
        External,

        /// <summary>불완전하게 체크아웃된 항목.</summary>
        Incomplete

        #endregion
    }
}
