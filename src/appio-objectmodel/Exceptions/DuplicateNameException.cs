using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Appio.ObjectModel.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class DuplicateNameException : ArgumentException
    {
        public DuplicateNameException(string duplicatedName)
            : base($"The name '{duplicatedName}' was already provided before.")
        {
        }

        protected DuplicateNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
