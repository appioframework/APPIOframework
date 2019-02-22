using System;
using System.IO;
using Newtonsoft.Json;

namespace Oppo.ObjectModel
{
    public static class SlnUtility
    {
		static public TDependance DeserializeFile<TDependance>(string jsonFileFullName, IFileSystem fileSystem) where TDependance : class
		{
			TDependance deserializedData;

			using (var memoryStream = fileSystem.ReadFile(jsonFileFullName))
			{
				StreamReader reader = new StreamReader(memoryStream);
				var jsonFileContent = reader.ReadToEnd();

				try
				{
					deserializedData = JsonConvert.DeserializeObject<TDependance>(jsonFileContent);
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