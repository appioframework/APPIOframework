using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface INodesetGenerator
    {
		string GetOutputMessage();
		bool GenerateTypesSourceCodeFiles(string projectName, IModelData modelData);
		bool GenerateNodesetSourceCodeFiles(string projectName, IModelData modelData, List<RequiredModelsData> requiredModelsData);
	}
}