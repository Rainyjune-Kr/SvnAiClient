using System;
using SvnAiClient.Models;
using System.Collections.Generic;

namespace SvnAiClient.Views.Main
{
    /// <summary>커밋 화면에서 최종 승인한 변경 정보를 메인 Presenter로 전달한다.</summary>
    public sealed class MainCommitRequest
    {
        #region Properties

        /// <summary>사용자가 최종 승인한 커밋 대상 파일 경로.</summary>
        public List<string> FilePaths { get; set; }

        /// <summary>최종 입력한 커밋 메시지.</summary>
        public string Message { get; set; }

        /// <summary>실제 SVN 커밋 직전에 재검증할 승인 시점의 파일 스냅샷.</summary>
        public Dictionary<string, SvnFileSnapshot> ApprovedFiles { get; set; }

        /// <summary>성공 리비전을 연결할 검수 이력 ID. 연결 대상이 없으면 Guid.Empty이다.</summary>
        public Guid ReviewHistoryId { get; set; }

        #endregion
    }
}
