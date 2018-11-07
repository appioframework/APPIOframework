using System;

namespace Oppo.ObjectModel.Extensions
{
    internal static class VersionExtensions
    {
        internal static string ToPrintableString(this Version instance)
        {
            return $"{instance.Major}.{instance.Minor}.{instance.Revision}";
        }
    }
}
