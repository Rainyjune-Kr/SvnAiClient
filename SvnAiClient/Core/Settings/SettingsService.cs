using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NLog;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Settings
{
    /// <summary>애플리케이션 설정의 로컬 저장, 불러오기 및 편집용 복사를 담당한다.</summary>
    public class SettingsService
    {
        #region Fields

        /// <summary>설정 파일을 읽거나 정리하는 중 발생한 오류를 기록한다.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>설정을 읽고 저장할 JSON 파일의 절대 경로.</summary>
        private readonly string _settingsFilePath;

        /// <summary>최근에 불러왔거나 저장에 성공한 설정.</summary>
        private AppSettings _currentSettings;

        #endregion

        #region Properties

        /// <summary>현재 설정을 반환하며, 처음 접근할 때 파일에서 불러온다.</summary>
        public AppSettings Current
        {
            get { return _currentSettings ?? Load(); }
        }

        #endregion

        #region Constructors

        public SettingsService()
            : this(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SvnAiClient", "settings.json"))
        {
        }

        public SettingsService(string settingsFilePath)
        {
            if (string.IsNullOrWhiteSpace(settingsFilePath))
                throw new ArgumentException("설정 파일 경로가 필요합니다.", nameof(settingsFilePath));

            _settingsFilePath = Path.GetFullPath(settingsFilePath);
        }

        #endregion

        #region Public Methods

        /// <summary>설정을 불러와 누락된 항목을 보완하고, 손상된 JSON은 백업 후 오류를 전달한다.</summary>
        public AppSettings Load()
        {
            if (!File.Exists(_settingsFilePath))
            {
                _currentSettings = new AppSettings();
                return _currentSettings;
            }

            try
            {
                var json = File.ReadAllText(_settingsFilePath, Encoding.UTF8);

                var settings = JsonConvert.DeserializeObject<AppSettings>(json);

                if (settings == null)
                    throw new JsonSerializationException("설정 파일 내용이 비어 있습니다.");

                _currentSettings = NormalizeSettings(settings);
                return _currentSettings;
            }
            catch (JsonException ex)
            {
                // 손상된 파일을 보존한 뒤 오류를 호출 화면에 전달한다.
                var backupPath = _settingsFilePath + ".invalid-"
                    + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N");

                File.Copy(_settingsFilePath, backupPath);

                Logger.Error(ex, "설정 파일 해석 실패");

                throw new InvalidDataException(
                    "설정 파일이 손상되었습니다. 기존 파일의 백업을 생성했습니다.", ex);
            }
        }

        /// <summary>임시 파일을 통해 설정을 저장하고, 저장에 성공한 설정만 현재 설정으로 반영한다.</summary>
        public void Save(AppSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var directory = Path.GetDirectoryName(_settingsFilePath);
            Directory.CreateDirectory(directory);

            // 같은 폴더에 임시 파일을 생성한다.
            var temporaryPath = _settingsFilePath + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);

                File.WriteAllText(temporaryPath, json, new UTF8Encoding(false));

                if (File.Exists(_settingsFilePath))
                {
                    // 기존 파일을 백업하고 임시 파일로 교체한다.
                    File.Replace(temporaryPath, _settingsFilePath, _settingsFilePath + ".bak");
                }
                else
                {
                    File.Move(temporaryPath, _settingsFilePath);
                }

                // 파일 저장이 성공한 뒤 현재 설정을 갱신한다.
                _currentSettings = settings;
            }
            finally
            {
                DeleteTemporaryFile(temporaryPath);
            }
        }

        /// <summary>원본과 분리된 편집용 복사본을 만들고 누락된 설정을 기본값으로 보완한다.</summary>
        public static AppSettings CreateEditableCopy(AppSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var json = JsonConvert.SerializeObject(settings);

            var copy = JsonConvert.DeserializeObject<AppSettings>(json);

            return NormalizeSettings(copy);
        }

        #endregion

        #region Private Methods

        /// <summary>임시 파일을 정리하고, 파일 접근 오류는 로그에만 기록한다.</summary>
        private static void DeleteTemporaryFile(string temporaryPath)
        {
            try
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
            catch (IOException ex)
            {
                Logger.Warn(ex, "설정 임시 파일 정리 실패");
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Warn(ex, "설정 임시 파일 정리 권한 부족");
            }
        }

        /// <summary>사용자가 지정한 값을 유지하면서 누락되거나 유효하지 않은 설정을 보완한다.</summary>
        private static AppSettings NormalizeSettings(AppSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var defaults = new AppSettings();

            if (settings.AiProviders == null)
                settings.AiProviders = defaults.AiProviders;

            var providers = settings.AiProviders;
            var defaultProviders = defaults.AiProviders;

            if (providers.Claude == null)
                providers.Claude = defaultProviders.Claude;

            if (providers.OpenAi == null)
                providers.OpenAi = defaultProviders.OpenAi;

            if (providers.Gemini == null)
                providers.Gemini = defaultProviders.Gemini;

            if (providers.LocalLlm == null)
                providers.LocalLlm = defaultProviders.LocalLlm;

            if (settings.WorkingCopies == null)
                settings.WorkingCopies = defaults.WorkingCopies;

            // 손상된 목록의 빈 항목은 제거한다.
            settings.WorkingCopies.RemoveAll(entry => entry == null);

            if (settings.ReviewItems == null)
            {
                settings.ReviewItems = defaults.ReviewItems;
            }
            else
            {
                // 기존 사용자가 선택한 값은 유지한다.
                foreach (var item in defaults.ReviewItems)
                {
                    if (!settings.ReviewItems.ContainsKey(item.Key))
                        settings.ReviewItems.Add(item.Key, item.Value);
                }
            }

            switch (settings.ActiveAiProvider)
            {
                case "Claude":
                case "OpenAI":
                case "Gemini":
                case "Local":
                    break;

                default:
                    settings.ActiveAiProvider = defaults.ActiveAiProvider;
                    break;
            }

            if (settings.LastSelectedWorkingCopy == null)
                settings.LastSelectedWorkingCopy = string.Empty;

            return settings;
        }

        #endregion
    }
}
