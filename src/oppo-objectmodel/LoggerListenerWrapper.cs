using System;
using System.Diagnostics.CodeAnalysis;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "/usr/bin/oppo.log4net.config")]

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class LoggerListenerWrapper : ILoggerListener
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Error(string message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public void Warn(string message)
        {
            log.Warn(message);
        }
    }
}