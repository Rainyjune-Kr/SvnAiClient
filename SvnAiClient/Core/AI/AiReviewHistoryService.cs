using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>로컬 검수 이력을 저장 및 조회하고 SVN 커밋 리비전을 연결한다.</summary>
    public sealed class AiReviewHistoryService
    {
        #region Fields

        /// <summary>검수 이력 JSON 파일과 쓰기 잠금을 보관할 절대 경로.</summary>
        private readonly string _directory;

        /// <summary>기존 이력 스키마를 유지하는 JSON 직렬화 설정.</summary>
        private static readonly JsonSerializerSettings JsonSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                Converters = { new StringEnumConverter() }
            };

        #endregion

        #region Constructors

        public AiReviewHistoryService() : this(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SvnAiClient", "ReviewHistory"))
        {
        }

        public AiReviewHistoryService(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("이력 저장 폴더가 필요합니다.", nameof(directory));
            _directory = Path.GetFullPath(directory);
        }

        #endregion

        #region Public Methods

        /// <summary>소스와 자격 증명을 제외한 이력을 저장하고 실패는 반환값과 오류 설명으로 전달한다.</summary>
        public bool TrySave(
            AiReviewResult result,
            string workingCopyPath,
            string modelName,
            IEnumerable<string> filePaths,
            string changeContext,
            out Guid historyId,
            out string error)
        {
            historyId = Guid.Empty;
            error = null;
            try
            {
                if (result == null) throw new ArgumentNullException(nameof(result));
                if (filePaths == null) throw new ArgumentNullException(nameof(filePaths));

                var entry = AiReviewHistoryMapper.Create(
                    result, workingCopyPath, modelName, filePaths, changeContext);

                Validate(entry);
                ExecuteWriteWithRetry(() =>
                {
                    Directory.CreateDirectory(_directory);

                    using (AcquireWriteLock())
                        WriteEntry(entry, false);
                });

                historyId = entry.Id; // 디스크 저장에 성공한 경우에만 ID를 반환한다.
                return true;
            }
            catch (Exception ex)
            {
                error = "검수 이력을 저장하지 못했습니다: " + ex.Message;
                return false;
            }
        }

        /// <summary>손상된 파일의 오류를 모으고 읽을 수 있는 이력을 최신순으로 반환한다.</summary>
        public bool TryLoad(
            out List<AiReviewHistoryEntry> entries,
            out List<string> errors)
        {
            entries = new List<AiReviewHistoryEntry>();
            errors = new List<string>();
            try
            {
                if (!Directory.Exists(_directory)) return true;
                foreach (var path in Directory.GetFiles(_directory, "*.json"))
                {
                    try
                    {
                        Guid id;
                        if (!Guid.TryParseExact(Path.GetFileNameWithoutExtension(path), "N", out id))
                            throw new InvalidDataException("이력 파일 이름이 올바르지 않습니다.");
                        entries.Add(ReadEntry(id));
                    }
                    catch (Exception ex)
                    {
                        errors.Add(Path.GetFileName(path) + ": " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add("검수 이력 조회 실패: " + ex.Message);
            }
            entries = entries.OrderByDescending(entry => entry.ReviewedAt).ToList();
            return errors.Count == 0;
        }

        /// <summary>커밋 성공 리비전을 이력에 연결하며 같은 리비전의 재연결은 성공으로 처리한다.</summary>
        public bool TryLinkRevision(Guid historyId, long revision, out string error)
        {
            error = null;

            try
            {
                if (historyId == Guid.Empty || revision < 0)
                {
                    throw new ArgumentException(
                        "이력 ID 또는 SVN 리비전이 올바르지 않습니다.");
                }

                ExecuteWriteWithRetry(() =>
                {
                    using (AcquireWriteLock())
                    {
                        var entry = ReadEntry(historyId);

                        if (entry.SvnRevision.HasValue)
                        {
                            if (entry.SvnRevision.Value == revision)
                                return;

                            throw new InvalidOperationException(
                                "이미 다른 리비전이 연결된 이력입니다.");
                        }

                        entry.SvnRevision = revision;
                        entry.CommittedAt = DateTimeOffset.UtcNow;

                        WriteEntry(entry, true);
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                error = "검수 이력에 SVN 리비전을 연결하지 못했습니다: "
                        + ex.Message;

                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>일시적인 파일 잠금 오류만 대기 후 최대 세 번까지 쓰기를 시도한다.</summary>
        private static void ExecuteWriteWithRetry(Action operation)
        {
            // 최초 시도를 포함해 최대 3회.
            // 실패한 시도의 잠금을 해제한 뒤 150ms, 300ms 대기한다.
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    operation();
                    return;
                }
                catch (IOException ex) when (
                    attempt < 2 && IsTemporaryLockError(ex))
                {
                    System.Threading.Thread.Sleep(150 * (attempt + 1));
                }
            }
        }

        /// <summary>파일 공유 또는 잠금 위반에 해당하는 Windows 오류인지 확인한다.</summary>
        private static bool IsTemporaryLockError(IOException error)
        {
            int windowsError = error.HResult & 0xFFFF;

            // ERROR_SHARING_VIOLATION / ERROR_LOCK_VIOLATION
            return windowsError == 32 || windowsError == 33;
        }

        /// <summary>파일을 배타적으로 열어 여러 애플리케이션의 이력 동시 갱신을 막는다.</summary>
        private FileStream AcquireWriteLock()
        {
            return new FileStream(Path.Combine(_directory, ".write.lock"),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>이력 ID에 해당하는 JSON 파일 경로를 생성한다.</summary>
        private string EntryPath(Guid id)
        {
            return Path.Combine(_directory, id.ToString("N") + ".json");
        }

        /// <summary>이력 파일을 읽고 스키마와 파일 이름의 ID를 검증한다.</summary>
        private AiReviewHistoryEntry ReadEntry(Guid id)
        {
            using (var stream = new FileStream(EntryPath(id), FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var entry = JsonConvert.DeserializeObject<AiReviewHistoryEntry>(
                    reader.ReadToEnd(), JsonSettings);
                Validate(entry);
                if (entry.Id != id)
                    throw new InvalidDataException("이력 ID와 파일 이름이 다릅니다.");
                return entry;
            }
        }

        /// <summary>이력 스키마와 필수 항목 및 상태 값의 유효성을 검증한다.</summary>
        private static void Validate(AiReviewHistoryEntry entry)
        {
            if (entry == null || entry.SchemaVersion != 1 || entry.Id == Guid.Empty ||
                entry.ReviewedAt == default(DateTimeOffset) ||
                entry.Status == AiReviewStatus.NotReviewed ||
                !Enum.IsDefined(typeof(AiReviewStatus), entry.Status) ||
                !Enum.IsDefined(typeof(AiReviewFailureKind), entry.FailureKind) ||
                entry.FilePaths == null || entry.Issues == null || entry.Batches == null)
                throw new InvalidDataException("지원하지 않거나 손상된 검수 이력입니다.");
        }

        /// <summary>이력을 임시 파일에 기록한 뒤 원자적으로 이동하거나 교체한다.</summary>
        private void WriteEntry(AiReviewHistoryEntry entry, bool replace)
        {
            string path = EntryPath(entry.Id);
            string temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                string json = JsonConvert.SerializeObject(entry, Formatting.Indented, JsonSettings);
                byte[] bytes = new UTF8Encoding(false).GetBytes(json);
                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew,
                    FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (replace)
                    File.Replace(temporaryPath, path, null);
                else
                    File.Move(temporaryPath, path);
            }
            finally
            {
                // 임시 파일 정리 실패가 저장/커밋 처리로 전파되지 않도록 한다.
                try { if (File.Exists(temporaryPath)) File.Delete(temporaryPath); }
                catch (Exception) { }
            }
        }

        #endregion
    }
}
