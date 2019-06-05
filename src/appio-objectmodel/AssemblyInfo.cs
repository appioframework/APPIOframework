using System;
using System.Diagnostics.CodeAnalysis;

namespace Appio.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class AssemblyInfo
    {
        public AssemblyInfo(string name, Version version, Version fileVersion)
        {
            AssemblyName = name;
            AssemblyVersion = version;
            AssemblyFileVersion = fileVersion;
        }

        public string AssemblyName { get; }
        public Version AssemblyVersion { get; }
        public Version AssemblyFileVersion { get; }
    }
}
