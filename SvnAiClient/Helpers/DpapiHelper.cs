using System;
using System.Security.Cryptography;
using System.Text;
using NLog;

namespace SvnAiClient.Helpers
{
    /// <summary>
    /// Windows DPAPI(Data Protection API)를 사용하여 민감 정보를 암호화/복호화한다.
    /// API Key, SVN 비밀번호 등을 안전하게 저장하기 위해 사용된다.
    /// DPAPI는 현재 사용자 계정에 바인딩되어 타 PC에서는 복호화 불가능하다.
    /// </summary>
    public static class DpapiHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
 
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
 
            try
            {
                var bytes = Encoding.UTF8.GetBytes(plainText);
                var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "DPAPI 암호화 실패.");
                throw;
            }
        }
 
        public static string Decrypt(string encryptedBase64)
        {
            if (string.IsNullOrEmpty(encryptedBase64)) return string.Empty;
 
            try
            {
                var bytes = Convert.FromBase64String(encryptedBase64);
                var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "DPAPI 복호화 실패.");
                throw;
            }
        }
 
        public static string DecryptSafe(string encryptedBase64)
        {
            try
            {
                return Decrypt(encryptedBase64);
            }
            catch
            {
                Logger.Warn("DPAPI 복호화 실패 (손상된 값으로 판단, 빈 문자열 반환).");
                return string.Empty;
            }
        }
    }
}
