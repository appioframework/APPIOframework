using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Oppo.ObjectModel
{
    public static class OppoLogger
    {
        /// <summary>
        ///  As listeners.
        /// </summary>
        private static readonly Collection<ILoggerListener> _listeners = new Collection<ILoggerListener>();

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
            _listeners.Remove(loggerListener);
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