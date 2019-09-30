/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Appio.ObjectModel
{
    public class OpcuaappReference : IOpcuaappReference
    {
        public OpcuaappReference()
        {
        }

        public OpcuaappReference(string name,  string path)
        {
            Name = name;
            Path = path;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("path")]
        public string Path { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string Type { get; }
    }
}