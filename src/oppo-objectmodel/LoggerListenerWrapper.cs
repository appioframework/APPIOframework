using log4net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class LoggerListenerWrapper : ILoggerListener
    {
        private readonly ILog _log;
        private readonly string _loggerFileName = "oppo.log4net.config";
        private readonly string _loggerRepositoryName = "Oppo";
        private readonly string _loggerName = "log4netFileLogger";

        public LoggerListenerWrapper()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var log4netConfig = _loggerFileName;
            var log4netInfo = new FileInfo(Path.Combine(baseDirectory, log4netConfig));
            var loggerRepository = log4net.LogManager.CreateRepository(_loggerRepositoryName);
            log4net.Config.XmlConfigurator.Configure(loggerRepository, log4netInfo);
            _log = LogManager.GetLogger(_loggerRepositoryName, _loggerName);
        }
        
        public void Error(string message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }
    }
}