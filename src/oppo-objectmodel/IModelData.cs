using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oppo.ObjectModel
{
    public interface IModelData
    {
		string Name { get; set; }
		string Uri { get; set; }
		string Types { get; set; }
		//List<string> requiredModels { get; set; }
		string NamespaceVariable { get; set; }
    }
}