using System;
using System.Collections.Generic;
using SvnAiClient.Core.Svn;

namespace SvnAiClient.Controls
{
    /// <summary>UI 컨트롤에 의존하지 않고 Unified Diff의 좌우 표시 행과 이동 위치를 계산한다.</summary>
    internal static class DiffDisplayBuilder
    {
        #region Public Methods

        /// <summary>삭제와 추가를 정렬하고 실제 줄번호 및 변경 구간의 표시 위치를 계산한다.</summary>
        public static DiffDisplayData Build(List<DiffLine> lines)
        {
            var display = new DiffDisplayData();
            var rows = display.Rows;

            int i = 0;

            while (i < lines.Count)
            {
                var line = lines[i];

                if (line.LineType == DiffLineType.FileHeader)
                {
                    i++;
                    continue;
                }

                if (line.LineType == DiffLineType.HunkHeader)
                {
                    rows.Add(new DiffDisplayRow
                    {
                        LeftText = line.Content,
                        RightText = line.Content,
                        LeftMarker = DiffDisplayMarker.Header,
                        RightMarker = DiffDisplayMarker.Header
                    });

                    i++;
                    continue;
                }

                if (line.LineType == DiffLineType.Context)
                {
                    rows.Add(new DiffDisplayRow
                    {
                        LeftText = CodeText(line),
                        RightText = CodeText(line),
                        LeftNumber = line.OriginalLineNumber,
                        RightNumber = line.ModifiedLineNumber
                    });

                    i++;
                    continue;
                }

                if (line.LineType == DiffLineType.Removed ||
                    line.LineType == DiffLineType.Added)
                {
                    // 연속된 삭제·추가 묶음을 하나의 변경 구간으로 기록한다.
                    display.ChangeRows.Add(rows.Count);

                    var removed = new List<DiffLine>();
                    var added = new List<DiffLine>();

                    while (i < lines.Count &&
                           lines[i].LineType == DiffLineType.Removed)
                    {
                        removed.Add(lines[i++]);
                    }

                    while (i < lines.Count &&
                           lines[i].LineType == DiffLineType.Added)
                    {
                        added.Add(lines[i++]);
                    }

                    int count = Math.Max(removed.Count, added.Count);

                    for (int j = 0; j < count; j++)
                    {
                        bool hasLeft = j < removed.Count;
                        bool hasRight = j < added.Count;

                        rows.Add(new DiffDisplayRow
                        {
                            LeftText = hasLeft
                                ? CodeText(removed[j])
                                : string.Empty,

                            RightText = hasRight
                                ? CodeText(added[j])
                                : string.Empty,

                            LeftNumber = hasLeft
                                ? removed[j].OriginalLineNumber
                                : null,

                            RightNumber = hasRight
                                ? added[j].ModifiedLineNumber
                                : null,

                            LeftMarker = !hasLeft
                                ? DiffDisplayMarker.Empty
                                : hasRight
                                    ? DiffDisplayMarker.Modified
                                    : DiffDisplayMarker.Removed,

                            RightMarker = !hasRight
                                ? DiffDisplayMarker.Empty
                                : hasLeft
                                    ? DiffDisplayMarker.Modified
                                    : DiffDisplayMarker.Added
                        });
                    }

                    continue;
                }

                i++;
            }

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                int? number = rows[rowIndex].RightNumber;

                if (number.HasValue && number.Value > 0)
                    display.ModifiedLineRows[number.Value] = rowIndex;
            }

            return display;
        }

        #endregion

        #region Private Methods

        /// <summary>Diff 행에서 추가, 삭제 또는 문맥 접두사 한 글자를 제외한 소스 텍스트를 반환한다.</summary>
        private static string CodeText(DiffLine line)
        {
            return string.IsNullOrEmpty(line.Content)
                ? string.Empty
                : line.Content.Substring(1);
        }

        #endregion
    }
}
