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
	public class OpcuaClientServerApp : IOpcuaClientServerApp
	{
		public OpcuaClientServerApp()
		{
		}

		public OpcuaClientServerApp(string name, string url, string port)
		{
			Name = name;
			Url = url;
			Port = port;
		}

		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("type")]
		public string Type { get; set; } = Constants.ApplicationType.ClientServer;

		[JsonProperty("url")]
		public string Url { get; set; } = string.Empty;

		[JsonProperty("port")]
		public string Port { get; set; } = string.Empty;

		[JsonProperty("references")]
		[JsonConverter(typeof(OpcuaappConverter<IOpcuaServerApp, OpcuaServerApp>))]
		public List<IOpcuaServerApp> ServerReferences { get; set; } = new List<IOpcuaServerApp>();

		[JsonProperty("models")]
		[JsonConverter(typeof(OpcuaappConverter<IModelData, ModelData>))]
		public List<IModelData> Models { get; set; } = new List<IModelData>();
	}
}