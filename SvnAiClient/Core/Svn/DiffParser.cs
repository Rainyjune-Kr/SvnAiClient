using System.Collections.Generic;

namespace SvnAiClient.Core.Svn
{
    /// <summary>Unified Diff를 화면 표시용 라인 목록으로 변환한다.</summary>
    public class DiffParser
    {
        #region Public Methods

        /// <summary>Unified Diff를 파싱하여 원본과 수정본의 라인 번호를 계산한다.</summary>
        public List<DiffLine> Parse(string diffText)
        {
            var result = new List<DiffLine>();

            if (string.IsNullOrEmpty(diffText))
                return result;

            int original = 0;
            int modified = 0;
            bool inHunk = false;

            foreach (var line in diffText
                         .Replace("\r\n", "\n")
                         .Split('\n'))
            {
                if (line.StartsWith("Index:") ||
                    line.StartsWith("===") ||
                    line.StartsWith("Property changes on:"))
                {
                    inHunk = false;

                    result.Add(new DiffLine
                    {
                        LineType = DiffLineType.FileHeader,
                        Content = line
                    });

                    continue;
                }

                if (line.StartsWith("@@"))
                {
                    ParseHunkHeader(line, out original, out modified);
                    inHunk = true;

                    result.Add(new DiffLine
                    {
                        LineType = DiffLineType.HunkHeader,
                        Content = line
                    });

                    continue;
                }

                if (!inHunk)
                {
                    if (line.StartsWith("--- ") ||
                        line.StartsWith("+++ "))
                    {
                        result.Add(new DiffLine
                        {
                            LineType = DiffLineType.FileHeader,
                            Content = line
                        });
                    }

                    continue;
                }

                // 개행 없음 안내와 마지막 빈 문자열은 제외한다.
                if (line.Length == 0 || line.StartsWith("\\ "))
                    continue;

                if (line[0] == '+')
                {
                    result.Add(new DiffLine
                    {
                        LineType = DiffLineType.Added,
                        Content = line,
                        ModifiedLineNumber = modified++
                    });
                }
                else if (line[0] == '-')
                {
                    result.Add(new DiffLine
                    {
                        LineType = DiffLineType.Removed,
                        Content = line,
                        OriginalLineNumber = original++
                    });
                }
                else if (line[0] == ' ')
                {
                    result.Add(new DiffLine
                    {
                        LineType = DiffLineType.Context,
                        Content = line,
                        OriginalLineNumber = original++,
                        ModifiedLineNumber = modified++
                    });
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>헝크 헤더에서 원본과 수정본의 시작 라인 번호를 읽는다.</summary>
        private static void ParseHunkHeader(string header, out int origStart, out int modStart)
        {
            origStart = 0;
            modStart  = 0;

            try
            {
                // 예: @@ -10,6 +10,8 @@
                var atAt = header.IndexOf("@@", 2);
                var inner = atAt > 0
                    ? header.Substring(3, atAt - 3).Trim()
                    : header.Substring(3).Trim();

                var parts = inner.Split(' ');
                foreach (var part in parts)
                {
                    if (part.StartsWith("-"))
                    {
                        var num = part.TrimStart('-').Split(',')[0];
                        int.TryParse(num, out origStart);
                    }
                    else if (part.StartsWith("+"))
                    {
                        var num = part.TrimStart('+').Split(',')[0];
                        int.TryParse(num, out modStart);
                    }
                }
            }
            catch
            {
                // 파싱 실패 시 0으로 유지
            }
        }

        #endregion
    }
}
