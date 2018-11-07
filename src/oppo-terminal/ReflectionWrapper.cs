using System;
using System.Diagnostics;
using System.Reflection;
using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    public class ReflectionWrapper : IReflection
    {
        public AssemblyInfo[] GetOppoAssemblyInfos()
        {
            var oppoTerminalAssembly = typeof(OppoTerminal).Assembly;
            var oppoObjectModelAssembly = typeof(ObjectModel.ObjectModel).Assembly;
            var oppoResourcesAssembly = typeof(Resources.Resources).Assembly;

            return new[]
            {
                GetAssemblyInfo(oppoTerminalAssembly),
                GetAssemblyInfo(oppoObjectModelAssembly),
                GetAssemblyInfo(oppoResourcesAssembly),
            };
        }

        private static AssemblyInfo GetAssemblyInfo(Assembly assembly)
        {
            var fileVersionString = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            return new AssemblyInfo(assembly.ManifestModule.Name, assembly.GetName().Version, new Version(fileVersionString));
        }
    }
}
