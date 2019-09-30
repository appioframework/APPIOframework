/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

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
