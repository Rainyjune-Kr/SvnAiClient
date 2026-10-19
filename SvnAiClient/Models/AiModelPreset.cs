namespace SvnAiClient.Models
{
    /// <summary>모델 선택 화면에 표시할 정보와 권장 요청 설정 및 한도를 보관한다.</summary>
    public class AiModelPreset
    {
        #region Properties

        /// <summary>모델 프리셋이 속한 프로바이더 식별자.</summary>
        public string Provider { get; set; }

        /// <summary>모델 ID.</summary>
        public string Id { get; set; }

        /// <summary>모델 선택 목록에 표시할 이름.</summary>
        public string Name { get; set; }

        /// <summary>권장 모델로 표시할지 여부.</summary>
        public bool Recommended { get; set; }

        /// <summary>모델의 입력과 출력에 사용할 컨텍스트 토큰 한도.</summary>
        public int ContextTokens { get; set; }

        /// <summary>모델이 지원하는 최대 출력 토큰 수.</summary>
        public int MaxOutputTokens { get; set; }

        /// <summary>모델 선택 시 적용할 권장 입력 토큰 예산.</summary>
        public int InputBudget { get; set; } = 240000;

        /// <summary>모델 선택 시 적용할 권장 출력 토큰 예산.</summary>
        public int OutputBudget { get; set; } = 32768;

        /// <summary>모델 선택 시 적용할 권장 요청 제한 시간(초).</summary>
        public int TimeoutSeconds { get; set; } = 300;

        /// <summary>모델 프리셋의 기본 추론 강도.</summary>
        public string Effort { get; set; }

        /// <summary>모델 선택 화면에서 제공할 추론 강도 목록.</summary>
        public string[] Efforts { get; set; } = new string[0];

        #endregion

        #region Public Methods

        /// <summary>권장 여부와 모델 ID를 포함한 모델 선택 목록의 표시 문자열을 구성한다.</summary>
        public override string ToString()
        {
            return Name + (Recommended ? " (권장)" : "") + " · " + Id;
        }

        #endregion
    }
}
