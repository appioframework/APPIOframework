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
        private enum ParamId {AppName, ModelFullName, TypesFullName, RequiredModelFullName}

        private readonly ParameterResolver<ParamId> _resolver;
        private readonly IFileSystem _fileSystem;
        private readonly IModelValidator _modelValidator;

        public GenerateInformationModelStrategy(string commandName, IFileSystem fileSystem, IModelValidator modelValidator)
        {
            _fileSystem = fileSystem;
            _modelValidator = modelValidator;
            Name = commandName;
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