using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScintillaNET;

namespace SvnAiClient.Controls
{
    /// <summary>기존 가로 휠 처리와 스크롤 동기화 알림을 제공하는 Diff 편집기.</summary>
    [ToolboxItem(false)]
    public class DiffScintilla : Scintilla
    {
        #region Fields

        /// <summary>일반 마우스 휠 메시지 번호.</summary>
        private const int WmMouseWheel = 0x020A;

        /// <summary>가로 마우스 휠 메시지 번호.</summary>
        private const int WmMouseHWheel = 0x020E;

        /// <summary>휠 메시지에서 Shift 키 상태를 구분하는 마스크.</summary>
        private const int MkShift = 0x0004;

        /// <summary>휠 메시지에서 Ctrl 키 상태를 구분하는 마스크.</summary>
        private const int MkControl = 0x0008;

        /// <summary>정밀 휠 입력에서 다음 이동에 누적할 소수 픽셀 값.</summary>
        private double _wheelRemainder;

        #endregion

        #region Events

        /// <summary>가로 휠 입력으로 스크롤 위치를 처리한 뒤 좌우 동기화를 알린다.</summary>
        public event EventHandler HorizontalWheelScrolled;

        #endregion

        #region Control Methods

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WmMouseWheel ||
                message.Msg == WmMouseHWheel)
            {
                long parameter = message.WParam.ToInt64();

                // 휠 회전량은 부호가 있는 상위 16비트이다.
                int delta = unchecked(
                    (short)((parameter >> 16) & 0xFFFF));

                bool shift = (parameter & MkShift) != 0;
                bool control = (parameter & MkControl) != 0;

                if (message.Msg == WmMouseHWheel ||
                    (shift && !control))
                {
                    // 좌우 휠: 양수가 오른쪽
                    // Shift+일반 휠: 아래로 돌리면 오른쪽
                    int horizontalDelta =
                        message.Msg == WmMouseHWheel
                            ? delta
                            : -delta;

                    ScrollHorizontal(horizontalDelta);

                    message.Result = IntPtr.Zero;
                    return;
                }
            }

            // 일반 휠과 Ctrl+휠은 기존 동작을 유지한다.
            base.WndProc(ref message);
        }

        #endregion

        #region Operations

        /// <summary>휠 회전량을 글꼴 배율에 맞는 가로 이동량으로 환산해 스크롤 범위 안에 적용한다.</summary>
        private void ScrollHorizontal(int delta)
        {
            // 현재 글꼴·배율 기준 약 8글자씩 이동한다.
            int step = Math.Max(
                16,
                TextWidth(Style.Default, "        "));

            // 작은 회전량도 누적하여 정밀 휠 입력을 지원한다.
            double pixels =
                delta * (step / 120.0) + _wheelRemainder;

            int movement = (int)Math.Truncate(pixels);
            _wheelRemainder = pixels - movement;

            if (movement == 0)
                return;

            int visibleWidth = Math.Max(
                1,
                ClientSize.Width
                - Margins[0].Width
                - Margins[1].Width);

            int maximum = Math.Max(
                0,
                ScrollWidth - visibleWidth);

            XOffset = Math.Max(
                0,
                Math.Min(maximum, XOffset + movement));

            Invalidate();

            var handler = HorizontalWheelScrolled;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
