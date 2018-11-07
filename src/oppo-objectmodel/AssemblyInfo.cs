using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class AssemblyInfo
    {
        public AssemblyInfo(string name, AssemblyVersionAttribute version, AssemblyFileVersionAttribute fileVersion)
        {
            AssemblyName = name;
            AssemblyVersion = version;
            AssemblyFileVersion = fileVersion;
        }

        public string AssemblyName { get; }
        public AssemblyVersionAttribute AssemblyVersion { get; }
        public AssemblyFileVersionAttribute AssemblyFileVersion { get; }
    }
}
