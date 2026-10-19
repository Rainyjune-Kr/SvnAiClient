using System.Collections.Generic;
using System.Linq;

namespace SvnAiClient.Models
{
    /// <summary>프로바이더별 모델 프리셋 조회와 토큰 예산 검증을 제공한다.</summary>
    public static class AiModelCatalog
    {
        #region Fields

        /// <summary>프로바이더별 모델 선택 목록과 기본 요청 설정을 제공하는 프리셋.</summary>
        private static readonly List<AiModelPreset> Models = new List<AiModelPreset>
        {
            OpenAI("gpt-6.1-sol", "GPT-6.1 Sol · 균형", true, false),
            OpenAI("gpt-6-luna", "GPT-6 Luna · 저비용", false, true),
            OpenAI("gpt-6-astra", "GPT-6 Astra · 심층", false, false),
            new AiModelPreset
            {
                Provider = "OpenAI", Id = "gpt-4o", Name = "GPT-4o · 기존 모델",
                ContextTokens = 128000, MaxOutputTokens = 16384,
                InputBudget = 64000, OutputBudget = 8192, TimeoutSeconds = 180
            },
            Claude("claude-sonnet-5-5", "Claude Sonnet 5.5 · 균형", true, "medium"),
            Claude("claude-haiku-5-5", "Claude Haiku 5.5 · 저비용", false, "medium"),
            Claude("claude-opus-5-5", "Claude Opus 5.5 · 심층", false, "medium"),
            Claude("claude-fable-5-1", "Claude Fable 5.1 · 고난도", false, "high"),
            Gemini("gemini-3.8-flash", "Gemini 3.8 Flash · 균형", true),
            Gemini("gemini-3.6-flash", "Gemini 3.6 Flash", false),
            Gemini("gemini-3.5-flash-lite", "Gemini 3.5 Flash-Lite · 저비용", false),
            Gemini("gemini-3.1-flash-lite", "Gemini 3.1 Flash-Lite · 저비용", false)
        };

        #endregion

        #region Public Methods

        /// <summary>지정한 프로바이더에 속한 모델 프리셋 목록을 반환한다.</summary>
        public static List<AiModelPreset> ForProvider(string provider)
        {
            return Models.Where(model => model.Provider == provider).ToList();
        }

        /// <summary>프로바이더와 모델 ID가 일치하는 프리셋을 찾는다.</summary>
        public static AiModelPreset Find(string provider, string id)
        {
            return Models.FirstOrDefault(model => model.Provider == provider && model.Id == id);
        }

        /// <summary>알려진 모델의 최대 출력 및 컨텍스트 한도를 확인하고 초과 시 오류 설명을 반환한다.</summary>
        public static string Validate(string provider, string id, int input, int output)
        {
            var preset = Find(provider, id);
            if (preset == null) return null;
            if (output > preset.MaxOutputTokens)
                return preset.Name + "의 최대 생성 토큰은 " + preset.MaxOutputTokens.ToString("N0") + "입니다.";
            if ((long)input + output + 2048 > preset.ContextTokens)
                return preset.Name + "의 컨텍스트 한도 " + preset.ContextTokens.ToString("N0")
                    + "를 입력 예산과 생성 예산의 합이 초과합니다.";
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>OpenAI 모델의 공통 한도와 추론 강도 목록으로 프리셋을 생성한다.</summary>
        private static AiModelPreset OpenAI(string id, string name, bool recommended, bool none)
        {
            return new AiModelPreset
            {
                Provider = "OpenAI", Id = id, Name = name, Recommended = recommended,
                ContextTokens = 1050000, MaxOutputTokens = 128000, Effort = "medium",
                Efforts = none
                    ? new[] { "none", "low", "medium", "high", "xhigh", "max" }
                    : new[] { "low", "medium", "high", "xhigh", "max" }
            };
        }

        /// <summary>Claude 모델의 공통 한도와 기본 추론 강도로 프리셋을 생성한다.</summary>
        private static AiModelPreset Claude(string id, string name, bool recommended, string effort)
        {
            return new AiModelPreset
            {
                Provider = "Claude", Id = id, Name = name, Recommended = recommended,
                ContextTokens = 1000000, MaxOutputTokens = 128000, Effort = effort
            };
        }

        /// <summary>Gemini 모델의 공통 한도로 프리셋을 생성한다.</summary>
        private static AiModelPreset Gemini(string id, string name, bool recommended)
        {
            return new AiModelPreset
            {
                Provider = "Gemini", Id = id, Name = name, Recommended = recommended,
                ContextTokens = 1048576, MaxOutputTokens = 65536
            };
        }

        #endregion
    }
}
