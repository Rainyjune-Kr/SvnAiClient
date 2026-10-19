using System.Collections.Generic;
using SvnAiClient.Models;

namespace SvnAiClient.Views.AiReviewHistory
{
    /// <summary>로컬 검수 이력 조회 화면의 선택과 표시 기능에 대한 계약.</summary>
    public interface AiReviewHistoryInterface
    {
        #region Properties

        /// <summary>화면이 폐기되지 않아 조회 결과를 표시할 수 있는지 여부.</summary>
        bool IsAvailable { get; }

        /// <summary>현재 작업 복사본의 이력만 표시할지 여부.</summary>
        bool CurrentOnly { get; }

        /// <summary>이력 목록에서 선택한 항목. 항목이 없으면 null이다.</summary>
        AiReviewHistoryEntry SelectedEntry { get; }

        #endregion

        #region View Contract

        void ConfigureCurrentOnlyFilter(bool available);
        void SetLoading(bool loading, bool canFilter);
        void ShowEntries(List<AiReviewHistoryEntry> entries);
        void ShowErrors(string text, int count);
        void ShowStatus(string text);
        void ShowDetail(AiReviewHistoryEntry entry, string text);

        #endregion
    }
}
