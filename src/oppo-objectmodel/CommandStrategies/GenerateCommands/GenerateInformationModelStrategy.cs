using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.GenerateCommands
{
    public class GenerateInformationModelStrategy : ICommand<GenerateStrategy>
	{
		private readonly IFileSystem _fileSystem;
		private readonly INodesetGenerator _nodesetGenerator;

		private enum ParamId {AppName, ModelFullName, TypesFullName, RequiredModelFullName}

        private readonly ParameterResolver<ParamId> _resolver;

        public GenerateInformationModelStrategy(string commandName, IFileSystem fileSystem, IModelValidator modelValidator, INodesetGenerator nodesetGenerator)
        {
            Name = commandName;
			_fileSystem = fileSystem;
			_nodesetGenerator = nodesetGenerator;

			_resolver = new ParameterResolver<ParamId>(Constants.CommandName.Generate + " " + Name, new []
            {

                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.AppName,
                    Short = Constants.GenerateInformationModeCommandArguments.Name,
                    Verbose = Constants.GenerateInformationModeCommandArguments.VerboseName
                }
            });
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var projectName = stringParams[ParamId.AppName];

            var outputMessages = new MessageLines();

			// deserialize oppoproj file
			var oppoprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject);
			var opcuaappData = Deserialize.Opcuaapp(oppoprojFilePath, _fileSystem);
			if (opcuaappData == null)
			{
				OppoLogger.Warn(LoggingText.GenerateInformationModelFailureCouldntDeserliazeOpcuaapp);
				outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureCouldntDeserliazeOpcuaapp, projectName, oppoprojFilePath), string.Empty);
				return new CommandResult(false, outputMessages);
			}
			if (opcuaappData.Type == Constants.ApplicationType.Client)
			{
				OppoLogger.Warn(LoggingText.GenerateInformationModelFailuteOpcuaappIsAClient);
				outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailuteOpcuaappIsAClient, projectName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			var opcuaappModels = (opcuaappData as IOpcuaServerApp).Models;

			// check if models are valid
			var modelsValid = ValidateModels(opcuaappModels);
			if(!modelsValid)
			{
				OppoLogger.Warn(LoggingText.GenerateInformationModelInvalidModelsList);
				outputMessages.Add(string.Format(OutputText.GenerateInformationModelInvalidModelsList, projectName), string.Empty);
				return new CommandResult(false, outputMessages);
			}
			
			// TODO: check if there is a circular dependency between models

			// generate models
			foreach(var model in opcuaappModels)
			{
				if (!_nodesetGenerator.GenerateTypesSourceCodeFiles(projectName, model) || !_nodesetGenerator.GenerateNodesetSourceCodeFiles(projectName, model, new List<RequiredModelsData>()))
				{
					outputMessages.Add(_nodesetGenerator.GetOutputMessage(), string.Empty);
					return new CommandResult(false, outputMessages);
				}
			}

			// exit method with positive result
			OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
			outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, projectName), string.Empty);
            return new CommandResult(true, outputMessages);           
        }

		private bool ValidateModels(List<IModelData> models)
		{
			// validate each and every model
			for (int modelIndex = 0; modelIndex < models.Count; modelIndex++)
			{
				// check for model duplications
				if (models.Where(x => x.Name == models[modelIndex].Name || x.Uri == models[modelIndex].Uri).Count() > 1)
				{
					return false;
				}

				// check if all required models exists
				for (int requiredModelIndex = 0; requiredModelIndex < models[modelIndex].RequiredModelUris.Count; requiredModelIndex++)
				{
					if (models.Where(x => x.Name != models[modelIndex].Name).SingleOrDefault(x => x.Uri == models[modelIndex].RequiredModelUris[requiredModelIndex]) == null)
					{
						return false;
					}
				}
			}

			return true;
		}

		public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.GenerateInformationModelCommandDescription;
        }
    }
}