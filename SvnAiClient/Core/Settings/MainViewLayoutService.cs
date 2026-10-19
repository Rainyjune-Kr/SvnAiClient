using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NLog;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Settings
{
    /// <summary>업무 설정과 분리하여 메인 화면 배치를 저장하고, 실패는 로그로만 남긴다.</summary>
    public sealed class MainViewLayoutService
    {
        #region Fields

        /// <summary>배치 파일 읽기와 저장 오류를 기록하는 로거.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>메인 화면 배치 JSON의 저장 경로.</summary>
        private readonly string _filePath;

        #endregion

        #region Constructors

        public MainViewLayoutService()
        {
            _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SvnAiClient", "main-view-layout.json");
        }

        #endregion

        #region Public Methods

        /// <summary>저장된 배치를 반환하며, 파일이 없거나 읽을 수 없으면 기본 배치를 사용한다.</summary>
        public MainViewLayout Load()
        {
            try
            {
                return File.Exists(_filePath)
                    ? JsonConvert.DeserializeObject<MainViewLayout>(File.ReadAllText(_filePath, Encoding.UTF8))
                    : null;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "메인 화면 배치 불러오기 실패");
                return null;
            }
        }

        /// <summary>임시 파일을 거쳐 배치를 저장하며, 실패해도 화면 종료를 방해하지 않는다.</summary>
        public void Save(MainViewLayout layout)
        {
            if (layout == null) return;
            var temporaryPath = _filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                File.WriteAllText(temporaryPath, JsonConvert.SerializeObject(layout, Formatting.Indented),
                    new UTF8Encoding(false));
                if (File.Exists(_filePath))
                    File.Replace(temporaryPath, _filePath, null);
                else
                    File.Move(temporaryPath, _filePath);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "메인 화면 배치 저장 실패");
            }
            finally
            {
                try
                {
                    if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "메인 화면 배치 임시 파일 정리 실패");
                }
            }
        }

        #endregion
    }
}
