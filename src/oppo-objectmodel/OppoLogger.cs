using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Oppo.ObjectModel
{
    public static class OppoLogger
    {
        /// <summary>
        ///  As listeners.
        /// </summary>
        private static readonly ConcurrentBag<ILoggerListener> _listeners = new ConcurrentBag<ILoggerListener>();
        
        /// <summary>
        /// Gets all LoggerListeners.
        /// </summary>
        /// <value>
        /// LoggerListener
        /// </value>
        public static IEnumerable<ILoggerListener> LoggerListeners
        {
            get { return _listeners; }
        }

        /// <summary>
        /// Registers the listener.
        /// </summary>
        /// <param name="loggerListener">The logger listener.</param>
        public static void RegisterListener(ILoggerListener loggerListener)
        {
            _listeners.Add(loggerListener);
        }

        /// <summary>
        /// Registers the listener.
        /// </summary>
        /// <param name="loggerListener">The logger listener.</param>
        public static void RemoveListener(ILoggerListener loggerListener)
        {
            _listeners.TryTake(out loggerListener);
        }

        /// <summary>
        /// Logs info message using LoggerListener.
        /// </summary>
        /// <param name="message">Info message</param>
        public static void Info(string message)
        {
            foreach (var loggerListener in _listeners)
            {
                loggerListener.Info(message);
            }
        }

        /// <summary>
        /// Logs warn message using LoggerListener.
        /// </summary>
        /// <param name="message">Error message.</param>
        public static void Warn(string message)
        {
            foreach (var loggerListener in _listeners)
            {
                loggerListener.Warn(message);
            }
        }

        /// <summary>
        /// Removes all listeners.
        /// </summary>
        public static void RemoveAllListeners()
        {
            _listeners.Clear();
        }

        /// <summary>
        /// Logs error message using LoggerListener.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="exception">Error exception</param>
        public static void Error(string message, Exception exception)
        {
            foreach (var loggerListener in _listeners)
            {
                loggerListener.Error(message, exception);
            }
        }       
    }
}