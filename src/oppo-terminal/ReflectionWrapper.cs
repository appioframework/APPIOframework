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
            var oppoTerminalAssembly = typeof(global::Oppo.Terminal.Program).Assembly;
            var oppoObjectModelAssembly = typeof(global::Oppo.ObjectModel.ObjectModel).Assembly;
            var oppoResourcesAssembly = typeof(global::Oppo.Resources.Resources).Assembly;

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
