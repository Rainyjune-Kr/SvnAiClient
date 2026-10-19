using System;

namespace SvnAiClient.Models
{
    /// <summary>작업 복사본 파일 또는 폴더의 상태와 화면 표시 및 선택 정보를 보관한다.</summary>
    public class SvnFileStatus
    {
        #region Properties

        /// <summary>파일 또는 폴더의 로컬 절대 경로.</summary>
        public string FullPath { get; set; }

        /// <summary>SVN 상태 조회에서 받은 항목 경로.</summary>
        public string RelativePath { get; set; }

        /// <summary>파일 또는 폴더 이름.</summary>
        public string FileName { get; set; }

        /// <summary>SVN에서 조회한 파일 또는 폴더의 상태.</summary>
        public SvnStatusKind Status { get; set; }

        /// <summary>원래 SVN 상태에서 관리 중이거나 추가 예약된 항목인지 여부.</summary>
        public bool IsVersioned { get; set; }

        /// <summary>속성 변경과 하위 변경 여부를 포함한 상태 표시 문자열.</summary>
        public string StatusText
        {
            get
            {
                var text = Status == SvnStatusKind.None
                    ? "미확인"
                    : Status.ToString();

                if (HasPropertyChanges)
                    text += " (속성 변경)";

                if (IsDirectory && HasDescendantChanges)
                    text += " (하위 변경)";

                return text;
            }
        }

        /// <summary>로컬 파일의 마지막 수정 시각.</summary>
        public DateTime LastModified { get; set; }

        /// <summary>커밋 대상으로 선택했는지 여부.</summary>
        public bool IsChecked { get; set; } = false;

        /// <summary>폴더 항목인지 여부.</summary>
        public bool IsDirectory { get; set; }

        /// <summary>항목 자체의 SVN 속성 변경 여부.</summary>
        public bool HasPropertyChanges { get; set; }

        /// <summary>폴더 하위에 변경된 항목이 존재하는지 여부.</summary>
        public bool HasDescendantChanges { get; set; }

        /// <summary>폴더 표시 또는 파일 확장자로 구성한 화면 표시용 유형.</summary>
        public string FileType
        {
            get
            {
                if (IsDirectory) return "[폴더]";
                var ext = System.IO.Path.GetExtension(FileName);
                return string.IsNullOrEmpty(ext) ? "(없음)" : ext.ToLower();
            }
        }

        #endregion
    }
}
