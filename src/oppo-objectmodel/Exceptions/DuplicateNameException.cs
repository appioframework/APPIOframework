using System;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.ObjectModel.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DuplicateNameException : ArgumentException
    {
        public DuplicateNameException(string duplicatedName)
            : base($"The name '{duplicatedName}' was already provided before.")
        {
        }
    }
}
