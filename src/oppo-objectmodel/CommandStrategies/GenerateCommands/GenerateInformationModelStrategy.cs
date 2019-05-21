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

			var opcuaappModel = (opcuaappData as IOpcuaServerApp).Models[0];
			
			if (!_nodesetGenerator.GenerateTypesSourceCodeFiles(projectName, opcuaappModel) || !_nodesetGenerator.GenerateNodesetSourceCodeFiles(projectName, opcuaappModel, new List<RequiredModelsData>()))
			{
				outputMessages.Add(_nodesetGenerator.GetOutputMessage(), string.Empty);
				return new CommandResult(false, outputMessages);
			}


			OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
			outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, projectName), string.Empty);
            return new CommandResult(true, outputMessages);           
        }

		public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.GenerateInformationModelCommandDescription;
        }
    }
}