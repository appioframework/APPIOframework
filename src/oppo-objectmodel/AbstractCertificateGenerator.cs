namespace Oppo.ObjectModel
{
    public abstract class AbstractCertificateGenerator
    {
        public abstract void Generate(string appName,
            string filePrefix,
            uint keySize,
            uint days,
            string organization);

        public virtual void Generate(string appName, string filePrefix = "")
        {
            Generate(appName,
                filePrefix ?? appName,
                Constants.ExternalExecutableArguments.OpenSSLDefaultKeySize,
                Constants.ExternalExecutableArguments.OpenSSLDefaultDays, Constants.ExternalExecutableArguments.OpenSSLDefaultOrganization);
        }
    }
}