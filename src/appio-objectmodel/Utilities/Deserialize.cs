/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Appio.ObjectModel
{
	public static class Deserialize
	{
		static public IOpcuaapp Opcuaapp(string jsonFileFullName, IFileSystem fileSystem)
		{
			IOpcuaapp deserializedData = null;

			using (var memoryStream = fileSystem.ReadFile(jsonFileFullName))
			{
				StreamReader reader = new StreamReader(memoryStream);
				var jsonFileContent = reader.ReadToEnd();

				try
				{
					var opcuaappType = JObject.Parse(jsonFileContent)["type"].ToString();

					if (opcuaappType == Constants.ApplicationType.Client)
					{
						deserializedData = JsonConvert.DeserializeObject<OpcuaClientApp>(jsonFileContent);
					}
					else if (opcuaappType == Constants.ApplicationType.Server)
					{
						deserializedData = JsonConvert.DeserializeObject<OpcuaServerApp>(jsonFileContent);
					}
					else if (opcuaappType == Constants.ApplicationType.ClientServer)
					{
						deserializedData = JsonConvert.DeserializeObject<OpcuaClientServerApp>(jsonFileContent);
					}
				}
				catch (Exception)
				{
					return null;
				}
			}

			return deserializedData;
		}
	}
}