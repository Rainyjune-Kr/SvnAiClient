using System;
using System.Drawing;

namespace SvnAiClient.Helpers
{
    /// <summary>공통 테마 글꼴을 화면에 전달하며 호출자의 사용 수명을 관리한다.</summary>
    public sealed class ApplicationFonts : IDisposable
    {
        #region Fields

        /// <summary>해제한 글꼴 제공 객체의 재사용을 방지한다.</summary>
        private bool _disposed;

        #endregion

        #region Operations

        /// <summary>디자이너와 실행 화면이 동일한 SUIT 패밀리와 크기를 공유한다.</summary>
        public Font GetFont(float size, FontStyle style = FontStyle.Regular)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
            return ViewTheme.GetFont(size, style);
        }

        /// <summary>코드 영역에서 사용할 공통 D2Coding 글꼴을 반환한다.</summary>
        public Font GetCodeFont(float size)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
            return ViewTheme.GetCodeFont(size);
        }

        /// <summary>제공 객체를 해제한다. 공유 글꼴은 테마가 프로세스·디자인 도메인 종료 시 해제한다.</summary>
        public void Dispose()
        {
            _disposed = true;
        }

        #endregion
    }
}

