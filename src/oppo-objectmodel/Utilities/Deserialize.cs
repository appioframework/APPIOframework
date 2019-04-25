using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Oppo.ObjectModel
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

					if (deserializedData == null)
					{
						throw null;
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