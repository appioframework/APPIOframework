using System;

namespace Oppo.ObjectModel
{
    public interface ILoggerListener
    {
        void Error(string message, Exception exception);

        void Warn(string message);

        void Info(string message);
    }
}