namespace SvnAiClient.Models
{
    /// <summary>리비전에서 변경된 저장소 경로와 작업 종류 및 폴더 여부를 보관한다.</summary>
    public class SvnChangedPath
    {
        #region Properties

        /// <summary>저장소 루트 기준 변경 경로.</summary>
        public string Path { get; set; }

        /// <summary>SVN 변경 작업 종류를 나타내는 문자열.</summary>
        public string Action { get; set; }

        /// <summary>폴더 항목인지 여부.</summary>
        public bool IsDirectory { get; set; }

        #endregion

        #region Public Methods

        /// <summary>변경 작업 종류와 저장소 경로를 조합한 표시 문자열을 구성한다.</summary>
        public override string ToString()
        {
            return Action + "  " + Path;
        }

        #endregion
    }
}
