using Newtonsoft.Json;

namespace SvnAiClient.Models
{
    /// <summary>AI 검수에 전송할 파일 경로와 변경 전후 전체 소스 및 Diff를 보관한다.</summary>
    public class AiReviewFile
    {
        #region Properties

        /// <summary>AI 검수에 전송하는 대상 파일 경로.</summary>
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        /// <summary>SVN BASE의 변경 전 전체 소스.</summary>
        [JsonProperty("originalSource")]
        public string OriginalSource { get; set; }

        /// <summary>작업 복사본의 변경 후 전체 소스.</summary>
        [JsonProperty("modifiedSource")]
        public string ModifiedSource { get; set; }

        /// <summary>변경 전후 소스의 Unified Diff.</summary>
        [JsonProperty("diff")]
        public string Diff { get; set; }

        #endregion
    }
}
