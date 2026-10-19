using System.Collections.Generic;

namespace SvnAiClient.Models
{
    /// <summary>설정 JSON에 저장할 애플리케이션 전체 설정을 보관한다.</summary>
    public class AppSettings
    {
        #region Properties

        /// <summary>AI 프로바이더별 연결 및 검수 요청 설정.</summary>
        public AiProviderSettings AiProviders { get; set; } = new AiProviderSettings();

        /// <summary>현재 선택한 AI 프로바이더 식별자.</summary>
        public string ActiveAiProvider { get; set; } = "OpenAI";

        /// <summary>커밋 전에 AI 검수를 자동으로 실행할지 여부.</summary>
        public bool AutoReviewBeforeCommit { get; set; } = true;

        /// <summary>검수 항목명과 해당 항목의 활성화 여부.</summary>
        public Dictionary<string, bool> ReviewItems { get; set; } = new Dictionary<string, bool>
        {
            { "잠재적 버그 및 논리 오류", true },
            { "보안 취약점", true },
            { "코드 스타일 및 가독성", true },
            { "불필요한 디버그 코드", true },
            { "변경 범위 요약", true }
        };

        /// <summary>화면에 적용할 DevExpress 스킨 이름.</summary>
        public string UiSkin { get; set; } = "Office 2016 Colorful";

        /// <summary>애플리케이션 언어 코드.</summary>
        public string Language { get; set; } = "ko-KR";

        /// <summary>등록된 작업 복사본 목록.</summary>
        public List<WorkingCopyEntry> WorkingCopies { get; set; } = new List<WorkingCopyEntry>();

        /// <summary>다음 실행 시 복원할 마지막 작업 복사본 경로.</summary>
        public string LastSelectedWorkingCopy { get; set; } = string.Empty;

        #endregion
    }
}
