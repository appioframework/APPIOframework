using System;
using System.Diagnostics.CodeAnalysis;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class LoggerWrapper : ILogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Error(string message, Exception exception)
        {
            log.Error(message, exception);
        }
    }
}