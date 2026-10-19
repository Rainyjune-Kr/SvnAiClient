using System;
using SharpSvn;
using SvnAiClient.Models;

namespace SvnAiClient.Core.Svn
{
    /// <summary>인증 정보와 SSL 설정을 적용한 SVN 클라이언트를 생성한다.</summary>
    internal sealed class SvnClientFactory
    {
        #region Fields

        /// <summary>클라이언트에 적용할 SVN 인증 및 SSL 설정.</summary>
        private readonly SvnSettings _settings;

        #endregion

        #region Constructors

        public SvnClientFactory(SvnSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
        }

        #endregion

        #region Public Methods

        /// <summary>호출자가 사용 후 해제할 인증 설정이 적용된 클라이언트를 생성한다.</summary>
        public SvnClient CreateClient()
        {
            var client = new SvnClient();

            if (!string.IsNullOrEmpty(_settings.Username))
            {
                client.Authentication.ForceCredentials(
                    _settings.Username,
                    DpapiDecryptPassword());
            }

            // SSL 인증서 오류 무시 설정
            if (_settings.IgnoreSslErrors)
            {
                client.Authentication.SslServerTrustHandlers += (sender, e) =>
                {
                    e.AcceptedFailures = e.Failures;
                    e.Save = true;
                };
            }

            return client;
        }

        #endregion

        #region Private Methods

        /// <summary>현재 사용자 DPAPI로 저장된 SVN 비밀번호를 복호화한다.</summary>
        private string DpapiDecryptPassword()
        {
            return Helpers.DpapiHelper.DecryptSafe(_settings.EncryptedPassword);
        }

        #endregion
    }
}
