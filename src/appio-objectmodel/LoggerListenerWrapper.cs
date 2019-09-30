/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using log4net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Appio.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class LoggerListenerWrapper : ILoggerListener
    {
        private readonly ILog _log;
        private readonly string _loggerFileName = "appio.log4net.config";
        private readonly string _loggerRepositoryName = "appio";
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