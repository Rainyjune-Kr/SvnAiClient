using System.Threading;
using System.Threading.Tasks;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>프로바이더별 AI 검수 요청과 구성 상태를 제공하는 계약.</summary>
    public interface IAiProvider
    {
        #region Properties

        /// <summary>검수 결과에 기록할 프로바이더 표시 이름.</summary>
        string ProviderName { get; }

        /// <summary>API 키 또는 엔드포인트 등 필수 설정의 구성 여부.</summary>
        bool IsConfigured { get; }

        #endregion

        #region Methods

        /// <summary>검수 지시와 전체 소스 및 Diff를 포함한 입력으로 AI 검수를 수행한다.</summary>
        Task<AiReviewResult> ReviewCodeAsync(
            string prompt,
            string diffContent,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}
