/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

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
