using System.Collections.Generic;
using SvnAiClient.Models;

namespace SvnAiClient.Views.Settings
{
    /// <summary>설정 화면 입출력 전용 데이터. 평문 API 키는 파일에 저장하지 않는다.</summary>
    public sealed class SettingsViewData
    {
        #region Properties

        /// <summary>기본으로 사용할 AI 프로바이더 이름.</summary>
        public string ActiveProvider { get; set; }
        /// <summary>Claude 모델과 검수 요청에 사용할 입력값.</summary>
        public SettingsProviderData Claude { get; set; } = new SettingsProviderData();
        /// <summary>OpenAI 모델과 검수 요청에 사용할 입력값.</summary>
        public SettingsProviderData OpenAi { get; set; } = new SettingsProviderData();
        /// <summary>Gemini 모델과 검수 요청에 사용할 입력값.</summary>
        public SettingsProviderData Gemini { get; set; } = new SettingsProviderData();
        /// <summary>로컬 AI 서버와 모델에 대한 입력값.</summary>
        public SettingsProviderData LocalLlm { get; set; } = new SettingsProviderData();
        /// <summary>커밋 전에 AI 검수를 자동으로 실행할지 여부.</summary>
        public bool AutoReviewBeforeCommit { get; set; }
        /// <summary>검수 항목 이름별 활성화 여부.</summary>
        public Dictionary<string, bool> ReviewItems { get; set; }
            = new Dictionary<string, bool>();

        #endregion
    }

    public sealed class SettingsProviderData
    {
        #region Properties

        /// <summary>화면 표시·입력용 평문 API 키. 저장 시 Presenter가 암호화한다.</summary>
        public string ApiKey { get; set; }
        /// <summary>선택하거나 직접 입력한 모델 ID.</summary>
        public string Model { get; set; }
        /// <summary>로컬 AI 서버의 접속 주소.</summary>
        public string EndpointUrl { get; set; }
        /// <summary>검수 요청에 허용할 추정 입력 토큰 상한.</summary>
        public int InputTokenLimit { get; set; }
        /// <summary>AI 응답에 허용할 출력 토큰 상한.</summary>
        public int MaxTokens { get; set; }
        /// <summary>화면에서 입력하는 OpenAI 요청 제한 시간(초).</summary>
        public int TimeoutSeconds { get; set; }
        /// <summary>OpenAI 추론 강도. 입력 컨트롤이 비활성이면 null로 기존 값을 유지한다.</summary>
        public string ReasoningEffort { get; set; }
        /// <summary>모델 선택 목록에 표시할 프로바이더별 프리셋.</summary>
        public List<AiModelPreset> Models { get; set; } = new List<AiModelPreset>();

        #endregion
    }
}
