using System;

namespace Oppo.ObjectModel
{
    public interface ILogger
    {
        void Error(string message, Exception exception);
    }
}