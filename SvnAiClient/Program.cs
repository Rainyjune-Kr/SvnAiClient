using System;
using System.Net;
using System.Windows.Forms;
using NLog;
using SvnAiClient.Helpers;
using SvnAiClient.Views.Main;

namespace SvnAiClient
{
    /// <summary>
    /// 애플리케이션 진입점
    /// </summary>
    static class Program
    {
        /// <summary>NLog 로거 인스턴스 (Program 클래스 전용)</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 애플리케이션의 주 진입점
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 처리되지 않은 예외 핸들러 등록
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // HTTPS API 연결에 TLS 1.2를 사용한다.
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Logger.Info("SvnAiClient 시작");

                // 높은 DPI 환경에서 DevExpress 스킨도 Windows 배율에 맞춰 그린다.
                DevExpress.XtraEditors.WindowsFormsSettings.AllowDpiScale = true;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // DevExpress 스킨 초기화는 MainView에서 수행
                using (var fonts = new ApplicationFonts())
                using (var view = new MainView(fonts))
                {
                    InitializeStartupWindow(view);
                    Application.Run(view);
                }

                Logger.Info("SvnAiClient 정상 종료");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "애플리케이션 시작 중 치명적 오류 발생");
                MessageBox.Show(
                    $"애플리케이션을 시작할 수 없습니다.\n\n{ex.Message}",
                    "치명적 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // NLog 리소스 정리
                LogManager.Shutdown();
            }
        }

        /// <summary>주 모니터 중앙에 시작 창을 배치하고 적용된 시스템 DPI를 로그에 기록한다.</summary>
        private static void InitializeStartupWindow(Form view)
        {
            view.StartPosition = FormStartPosition.Manual;

            // 창 핸들을 생성한 뒤 실제 크기를 기준으로 시작 위치를 계산한다.
            using (var graphics = view.CreateGraphics())
            {
                var screen = Screen.PrimaryScreen;
                if (screen == null)
                    return;

                var area = screen.WorkingArea;
                view.Location = new System.Drawing.Point(
                    area.Left + Math.Max(0, (area.Width - view.Width) / 2),
                    area.Top + Math.Max(0, (area.Height - view.Height) / 2));

                Logger.Info("시작 주 모니터: {0}, 시스템 DPI: {1} x {2}",
                    screen.DeviceName, graphics.DpiX, graphics.DpiY);
            }
        }

        /// <summary>
        /// UI 스레드에서 처리되지 않은 예외 핸들러
        /// </summary>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "UI 스레드 미처리 예외");
            MessageBox.Show(
                $"예기치 않은 오류가 발생했습니다.\n\n{e.Exception.Message}",
                "오류",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// 백그라운드 스레드에서 처리되지 않은 예외 핸들러
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Logger.Fatal(ex, "AppDomain 미처리 예외 (IsTerminating: {0})", e.IsTerminating);
        }
    }
}
