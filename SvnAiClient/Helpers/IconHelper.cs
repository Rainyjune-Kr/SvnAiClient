using System.Collections.Generic;
using System.Drawing;
using SvnAiClient.Models;

namespace SvnAiClient.Helpers
{
    /// <summary>
    /// SVN 파일 상태에 대응하는 아이콘을 관리한다.
    /// XtraTreeList, XtraGrid에서 상태 아이콘 표시에 사용된다.
    /// </summary>
    public static class IconHelper
    {
        /// <summary>상태별 아이콘 캐시 (중복 로딩 방지)</summary>
        private static readonly Dictionary<SvnStatusKind, Image> IconCache
            = new Dictionary<SvnStatusKind, Image>();

        /// <summary>
        /// SVN 파일 상태에 대응하는 아이콘 이미지를 반환한다.
        /// </summary>
        /// <param name="status">SVN 파일 상태</param>
        /// <returns>상태 아이콘 이미지 (16x16)</returns>
        public static Image GetStatusIcon(SvnStatusKind status)
        {
            // TODO: 10단계에서 리소스 아이콘으로 교체
            // 현재는 색상 블록으로 대체
            if (IconCache.ContainsKey(status)) return IconCache[status];

            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(new SolidBrush(GetStatusColor(status)), 0, 0, 16, 16);
            }

            IconCache[status] = bmp;
            return bmp;
        }

        /// <summary>
        /// SVN 파일 상태에 대응하는 색상을 반환한다.
        /// TortoiseSVN 색상 규칙을 따른다.
        /// </summary>
        /// <param name="status">SVN 파일 상태</param>
        /// <returns>상태 표시 색상</returns>
        public static Color GetStatusColor(SvnStatusKind status)
        {
            switch (status)
            {
                case SvnStatusKind.Modified:   return Color.FromArgb(0, 128, 0);    // 초록
                case SvnStatusKind.Added:      return Color.FromArgb(0, 0, 200);    // 파랑
                case SvnStatusKind.Deleted:    return Color.FromArgb(200, 0, 0);    // 빨강
                case SvnStatusKind.Conflicted: return Color.FromArgb(255, 128, 0);  // 주황
                case SvnStatusKind.Missing:    return Color.FromArgb(128, 0, 128);  // 보라
                case SvnStatusKind.Unversioned:return Color.Gray;
                default:                       return Color.Black;
            }
        }

        /// <summary>SVN 상태에 대응하는 한국어 표시 텍스트를 반환한다.</summary>
        /// <param name="status">SVN 파일 상태</param>
        /// <returns>한국어 상태 텍스트</returns>
        public static string GetStatusDisplayText(SvnStatusKind status)
        {
            switch (status)
            {
                case SvnStatusKind.Modified:    return "수정됨";
                case SvnStatusKind.Added:       return "추가됨";
                case SvnStatusKind.Deleted:     return "삭제됨";
                case SvnStatusKind.Conflicted:  return "충돌";
                case SvnStatusKind.Missing:     return "누락";
                case SvnStatusKind.Unversioned: return "미추적";
                case SvnStatusKind.Normal:      return "정상";
                case SvnStatusKind.Ignored:     return "무시됨";
                default:                        return status.ToString();
            }
        }
    }
}
