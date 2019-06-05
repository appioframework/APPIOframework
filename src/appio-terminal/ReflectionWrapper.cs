using System;
using System.Diagnostics;
using System.Reflection;
using Appio.ObjectModel;

namespace Appio.Terminal
{
    public class ReflectionWrapper : IReflection
    {
        public AssemblyInfo[] GetAppioAssemblyInfos()
        {
            var appioTerminalAssembly = typeof(global::Appio.Terminal.Program).Assembly;
            var appioObjectModelAssembly = typeof(global::Appio.ObjectModel.ObjectModel).Assembly;
            var appioResourcesAssembly = typeof(global::Appio.Resources.Resources).Assembly;

            return new[]
            {
                GetAssemblyInfo(appioTerminalAssembly),
                GetAssemblyInfo(appioObjectModelAssembly),
                GetAssemblyInfo(appioResourcesAssembly),
            };
        }

        private static AssemblyInfo GetAssemblyInfo(Assembly assembly)
        {
            var fileVersionString = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            return new AssemblyInfo(assembly.ManifestModule.Name, assembly.GetName().Version, new Version(fileVersionString));
        }
    }
}
