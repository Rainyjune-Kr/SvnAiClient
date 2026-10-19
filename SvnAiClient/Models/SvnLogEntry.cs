using System;
using System.Collections.Generic;

namespace SvnAiClient.Models
{
    /// <summary>SVN 커밋 로그와 변경 경로 및 로그 화면 표시 정보를 보관한다.</summary>
    public class SvnLogEntry
    {
        #region Properties

        /// <summary>커밋 리비전 번호.</summary>
        public long Revision { get; set; }

        /// <summary>커밋 작성자.</summary>
        public string Author { get; set; }

        /// <summary>로컬 시간으로 변환한 커밋 시각.</summary>
        public DateTime Date { get; set; }

        /// <summary>SVN 커밋 메시지.</summary>
        public string Message { get; set; }

        /// <summary>로그 화면에 표시할 작업 종류와 변경 경로 문자열 목록.</summary>
        public List<string> ChangedPaths { get; set; } = new List<string>();

        /// <summary>로그 그리드에 표시할 커밋 날짜와 시각 문자열.</summary>
        public string DateText => Date.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>r 접두어를 포함한 리비전 표시 문자열.</summary>
        public string RevisionText => $"r{Revision}";

        /// <summary>리비전의 변경 경로와 작업 종류를 포함한 구조화된 목록.</summary>
        public List<SvnChangedPath> Changes { get; set; } = new List<SvnChangedPath>();

        #endregion
    }
}
