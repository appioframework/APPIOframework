using System;

namespace Oppo.ObjectModel
{
    public interface ILoggerListener
    {
        void Error(string message, Exception exception);
    }
}