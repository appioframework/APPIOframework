using Newtonsoft.Json;

namespace Oppo.ObjectModel
{
	public class ModelData : IModelData
	{
		public ModelData()
		{
		}

		public ModelData(string name, string uri, string types, string namespaceVariable)
		{
			Name = name;
			Uri = uri;
			Types = types;
			NamespaceVariable = namespaceVariable;
		}

		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("uri")]
		public string Uri { get; set; } = string.Empty;

		[JsonProperty("types")]
		public string Types { get; set; } = string.Empty;

		[JsonProperty("namespaceVariable")]
		public string NamespaceVariable { get; set; } = string.Empty;
	}
}