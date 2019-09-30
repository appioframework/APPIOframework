/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Appio.ObjectModel
{
    public class OpcuaClientApp : IOpcuaClientApp
    {
        public OpcuaClientApp()
        {
        }

        public OpcuaClientApp(string name)
        {
            Name = name;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = Constants.ApplicationType.Client;
		
		[JsonProperty("references")]
		[JsonConverter(typeof(OpcuaappConverter<IOpcuaServerApp, OpcuaServerApp>))]
		public List<IOpcuaServerApp> ServerReferences { get; set; } = new List<IOpcuaServerApp>();
    }
}