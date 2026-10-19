using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SvnAiClient.Core.AI;
using SvnAiClient.Models;

namespace SvnAiClient.Views.AiReviewHistory
{
    /// <summary>검수 이력 조회, 작업 복사본 필터 및 읽기 전용 상세 표시를 담당한다.</summary>
    public sealed class AiReviewHistoryPresenter
    {
        #region Fields

        /// <summary>이력 선택 상태를 읽고 결과 표시를 요청하는 화면 계약.</summary>
        private readonly AiReviewHistoryInterface _view;

        /// <summary>검수 이력 JSON 파일의 읽기를 담당한다.</summary>
        private readonly AiReviewHistoryService _service;

        /// <summary>현재 작업 복사본 필터에 사용할 루트 경로. 전체 조회에서는 null일 수 있다.</summary>
        private readonly string _workingCopyPath;

        /// <summary>마지막 조회에서 읽은 전체 검수 이력 스냅샷.</summary>
        private List<AiReviewHistoryEntry> _entries = new List<AiReviewHistoryEntry>();

        /// <summary>마지막 조회에서 발생한 개별 파일 및 전체 조회 오류.</summary>
        private List<string> _loadErrors = new List<string>();

        /// <summary>이력 파일을 읽는 중에 중복 조회가 실행되지 않도록 한다.</summary>
        private bool _loading;

        /// <summary>화면이 닫힌 후 조회 결과를 컨트롤에 반영하지 않도록 한다.</summary>
        private bool _closing;

        #endregion

        #region Properties

        /// <summary>종료 처리가 시작되지 않았고 화면에 결과를 표시할 수 있는지 여부.</summary>
        private bool CanUpdateView { get { return !_closing && _view.IsAvailable; } }

        /// <summary>현재 작업 복사본 경로가 있어 해당 범위의 필터를 사용할 수 있는지 여부.</summary>
        private bool CanFilterCurrent { get { return !string.IsNullOrWhiteSpace(_workingCopyPath); } }

        #endregion

        #region Initialization

        public AiReviewHistoryPresenter(AiReviewHistoryInterface view,
            string workingCopyPath, AiReviewHistoryService service)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (service == null) throw new ArgumentNullException(nameof(service));
            _view = view;
            _workingCopyPath = workingCopyPath;
            _service = service;
        }

        public void Initialize()
        {
            _view.ConfigureCurrentOnlyFilter(CanFilterCurrent);
        }

        #endregion

        #region View Requests

        public async Task ReloadAsync()
        {
            if (_loading || !CanUpdateView) return;
            _loading = true;
            _view.SetLoading(true, CanFilterCurrent);
            _view.ShowStatus("검수 이력 읽는 중...");

            try
            {
                var loaded = await Task.Run(() =>
                {
                    List<AiReviewHistoryEntry> entries;
                    List<string> errors;
                    _service.TryLoad(out entries, out errors);
                    return Tuple.Create(entries, errors);
                });

                if (!CanUpdateView) return;
                _entries = loaded.Item1;
                _loadErrors = loaded.Item2;
                ApplyFilter();
            }
            catch (Exception ex)
            {
                if (!CanUpdateView) return;
                _entries = new List<AiReviewHistoryEntry>();
                _loadErrors = new List<string> { "검수 이력 조회 실패: " + ex.Message };
                ApplyFilter();
            }
            finally
            {
                _loading = false;
                if (CanUpdateView)
                    _view.SetLoading(false, CanFilterCurrent);
            }
        }

        public void ShowSelectedEntry()
        {
            if (!CanUpdateView) return;
            var entry = _view.SelectedEntry;
            _view.ShowDetail(entry, entry == null ? "이력을 선택하세요." : BuildDetail(entry));
        }

        public void Close()
        {
            _closing = true;
        }

        #endregion

        #region Operations

        /// <summary>현재 작업 복사본 필터를 적용하고 표시 건수, 조회 오류 및 선택 상세를 갱신한다.</summary>
        public void ApplyFilter()
        {
            if (!CanUpdateView) return;
            var visible = _entries.Where(entry => !_view.CurrentOnly ||
                string.Equals((entry.WorkingCopyPath ?? string.Empty).TrimEnd('\\', '/'),
                    (_workingCopyPath ?? string.Empty).TrimEnd('\\', '/'),
                    StringComparison.OrdinalIgnoreCase)).ToList();

            _view.ShowEntries(visible);
            _view.ShowErrors(_loadErrors.Count == 0
                ? "조회 오류가 없습니다." : string.Join(Environment.NewLine, _loadErrors), _loadErrors.Count);
            _view.ShowStatus("표시 " + visible.Count + "건 / 전체 " + _entries.Count
                + "건 / 조회 오류 " + _loadErrors.Count + "건 / 과거 이력은 조회 전용입니다.");
            ShowSelectedEntry();
        }

        /// <summary>목록의 날짜, 검수 상태 및 SVN 리비전을 화면 표시용 문자열로 변환한다.</summary>
        public static string FormatColumnText(string fieldName, object value)
        {
            if (fieldName == "ReviewedAt" && value is DateTimeOffset)
                return ((DateTimeOffset)value).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            if (fieldName == "Status" && value is AiReviewStatus)
                return StatusText((AiReviewStatus)value);
            if (fieldName == "SvnRevision")
                return value == null || value == DBNull.Value ? "미연결" : "r" + value;
            return null;
        }

        /// <summary>검수 상태, 대상 파일, 이슈, 배치, 토큰 및 리비전을 읽기 전용 상세 문구로 구성한다.</summary>
        private static string BuildDetail(AiReviewHistoryEntry entry)
        {
            var text = new StringBuilder();

            text.AppendLine("이력 ID: " + entry.Id);
            text.AppendLine("검수 시각: "
                + entry.ReviewedAt.ToLocalTime()
                    .ToString("yyyy-MM-dd HH:mm:ss zzz"));
            text.AppendLine("작업 폴더: " + entry.WorkingCopyPath);
            text.AppendLine("프로바이더 / 모델: "
                + entry.ProviderName + " / " + entry.ModelName);
            text.AppendLine("검수 상태: " + StatusText(entry.Status));
            text.AppendLine("실패 유형: " + FailureText(entry.FailureKind));

            text.AppendLine("SVN 리비전: "
                + (entry.SvnRevision.HasValue
                    ? "r" + entry.SvnRevision.Value
                    : "미연결"));

            text.AppendLine("커밋 시각: "
                + (entry.CommittedAt.HasValue
                    ? entry.CommittedAt.Value.ToLocalTime()
                        .ToString("yyyy-MM-dd HH:mm:ss zzz")
                    : "미연결"));

            text.AppendLine("추정 입력 토큰: "
                + entry.EstimatedInputTokens.ToString("N0"));
            text.AppendLine("실제 입력 / 출력 토큰: "
                + TokenText(entry.ActualInputTokens)
                + " / " + TokenText(entry.ActualOutputTokens));
            text.AppendLine("검수 소요 시간: "
                + entry.ElapsedMs.ToString("N0") + " ms");

            AppendSection(
                text, "변경 배경 / 검수 요청", entry.ChangeContext);
            AppendSection(text, "검수 요약", entry.Summary);
            AppendSection(
                text, "제안 커밋 메시지", entry.SuggestedCommitMessage);
            AppendSection(
                text,
                "대상 파일",
                string.Join(Environment.NewLine, entry.FilePaths));

            text.AppendLine().AppendLine("[이슈]");

            var issues = entry.Issues
                .Where(issue => issue != null)
                .ToList();

            if (issues.Count == 0)
                text.AppendLine("기록된 이슈 없음");

            for (int i = 0; i < issues.Count; i++)
            {
                var issue = issues[i];

                text.AppendLine(
                    (i + 1) + ". [" + issue.Severity + "] "
                    + issue.Category + " / " + issue.Title);

                text.AppendLine(
                    "위치: " + (issue.FilePath ?? "미지정")
                    + (issue.LineNumber.HasValue
                        ? " : " + issue.LineNumber.Value + "행"
                        : ""));

                text.AppendLine(issue.Description).AppendLine();
            }

            text.AppendLine("[배치별 검수 상태]");

            var batches = entry.Batches
                .Where(batch => batch != null)
                .ToList();

            if (batches.Count == 0)
                text.AppendLine("기록된 배치 없음");

            for (int i = 0; i < batches.Count; i++)
            {
                var batch = batches[i];

                text.AppendLine(
                    "배치 " + (i + 1) + ": " + StatusText(batch.Status)
                    + " / " + FailureText(batch.FailureKind));

                text.AppendLine(string.Join(
                    Environment.NewLine,
                    batch.FilePaths ?? new List<string>()));

                text.AppendLine(
                    "제안 메시지: " + batch.SuggestedCommitMessage)
                    .AppendLine();
            }

            return text.ToString();
        }

        /// <summary>상세 문구에 구역 제목과 본문을 추가하고 빈 내용은 기록 없음으로 표시한다.</summary>
        private static void AppendSection(
            StringBuilder text,
            string title,
            string value)
        {
            text.AppendLine().AppendLine("[" + title + "]");
            text.AppendLine(
                string.IsNullOrWhiteSpace(value) ? "기록 없음" : value);
        }

        /// <summary>실제 토큰 수를 숫자로 표시하고 제공되지 않은 값은 미제공으로 구분한다.</summary>
        private static string TokenText(int? value)
        {
            return value.HasValue
                ? value.Value.ToString("N0")
                : "미제공";
        }

        /// <summary>검수 상태를 이력 조회용 한국어 표시명으로 변환한다.</summary>
        private static string StatusText(AiReviewStatus status)
        {
            switch (status)
            {
                case AiReviewStatus.Succeeded: return "성공";
                case AiReviewStatus.Partial: return "부분 성공";
                case AiReviewStatus.Failed: return "실패";
                case AiReviewStatus.Cancelled: return "취소";
                default: return "미검수";
            }
        }

        /// <summary>검수 실패 유형을 이력 조회용 한국어 설명으로 변환한다.</summary>
        private static string FailureText(AiReviewFailureKind kind)
        {
            switch (kind)
            {
                case AiReviewFailureKind.None:
                    return "없음";
                case AiReviewFailureKind.InvalidResponse:
                    return "응답 형식 오류";
                case AiReviewFailureKind.ProviderError:
                    return "프로바이더 오류";
                case AiReviewFailureKind.Timeout:
                    return "시간 초과";
                case AiReviewFailureKind.Cancelled:
                    return "취소";
                case AiReviewFailureKind.InputTooLarge:
                    return "입력 한도 / 설정 오류";
                case AiReviewFailureKind.InvalidDiff:
                    return "Diff 오류";
                default:
                    return kind.ToString();
            }
        }

        #endregion
    }
}
