using System;
using System.Text;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel
{
    public class CertificateGenerator : AbstractCertificateGenerator
    {
        private readonly IFileSystem _fileSystem;

        public CertificateGenerator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public override void Generate(string appName, string filePrefix, uint keySize, uint days, string organization)
        {
            var openSSLConfigBuilder =
                new StringBuilder(_fileSystem.LoadTemplateFile(Resources.Resources.OpenSSLConfigTemplateFileName));
            openSSLConfigBuilder.Replace("$KEY_SIZE", keySize.ToString());
            openSSLConfigBuilder.Replace("$ORG_NAME", organization);
            openSSLConfigBuilder.Replace("$COMMON_NAME", appName);

            var certificateConfigPath = _fileSystem.CombinePaths(appName, Constants.FileName.CertificateConfig);
            try
            {
                _fileSystem.CreateFile(certificateConfigPath, openSSLConfigBuilder.ToString());
            }
            catch (Exception)
            {
                OppoLogger.Warn(string.Format(LoggingText.CertificateGeneratorFailureNonexistentDirectory, appName));
                return;
            }

            var separator = filePrefix == string.Empty ? "" : "_";
            
            _fileSystem.CallExecutable(Constants.ExecutableName.OpenSSL, appName, string.Format(Constants.ExternalExecutableArguments.OpenSSL, filePrefix + separator, days));
            _fileSystem.CallExecutable(Constants.ExecutableName.OpenSSL, appName, string.Format(Constants.ExternalExecutableArguments.OpenSSLConvertKeyFromPEM, filePrefix + separator + Constants.FileName.PrivateKeyPEM, filePrefix + separator + Constants.FileName.PrivateKeyDER));
            OppoLogger.Info(string.Format(LoggingText.CertificateGeneratorSuccess, filePrefix == string.Empty ? appName : appName + "/" + filePrefix));
        }
    }
}