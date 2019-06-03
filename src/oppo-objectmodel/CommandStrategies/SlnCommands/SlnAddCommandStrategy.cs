using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnAddCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;
        private readonly ParameterResolver<ParamId> _resolver;
        
        private enum ParamId {SolutionName, ProjectName}

        public SlnAddCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _resolver = new ParameterResolver<ParamId>(Constants.CommandName.Sln + " " + Name, new []
            {
	            new StringParameterSpecification<ParamId>
	            {
		            Identifier = ParamId.SolutionName,
		            Short = Constants.SlnCommandOptions.Solution,
		            Verbose = Constants.SlnCommandOptions.VerboseSolution
	            },
	            new StringParameterSpecification<ParamId>
	            {
		            Identifier = ParamId.ProjectName,
		            Short = Constants.SlnCommandOptions.Project,
		            Verbose = Constants.SlnCommandOptions.VerboseProject
	            }
            });
        }

        public string Name => Constants.SlnCommandArguments.Add;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();
			var validationMessages = new SlnUtility.ResultMessages();

			var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
			if (error != null)
				return new CommandResult(false, new MessageLines{{error, string.Empty}});

			var solutionName = stringParams[ParamId.SolutionName];
			var projectName = stringParams[ParamId.ProjectName];
			
			// validate solution name
			if(!SlnUtility.ValidateSolution(ref validationMessages, solutionName, _fileSystem))
			{
				OppoLogger.Warn(validationMessages.LoggerMessage);
				outputMessages.Add(validationMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

            // check if *.oppoproj file exists
            var oppoprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject);
            if (string.IsNullOrEmpty(projectName) || !_fileSystem.FileExists(oppoprojFilePath))
            {
                OppoLogger.Warn(LoggingText.SlnAddOppoprojFileNotFound);
                outputMessages.Add(string.Format(OutputText.SlnAddOpcuaappNotFound, oppoprojFilePath), string.Empty);
                return new CommandResult(false, outputMessages);
            }
			
			// deserialize *.opposln file
			var solutionFullName = solutionName + Constants.FileExtension.OppoSln;
			Solution oppoSolution = SlnUtility.DeserializeFile<Solution>(solutionFullName, _fileSystem);
			if (oppoSolution == null)
			{
				OppoLogger.Warn(LoggingText.SlnCouldntDeserliazeSln);
				outputMessages.Add(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialize *.oppoproj file
			OpcuaappReference oppoProj = SlnUtility.DeserializeFile<OpcuaappReference>(oppoprojFilePath, _fileSystem);
            if (oppoProj == null)
            {
                OppoLogger.Warn(LoggingText.SlnAddCouldntDeserliazeOpcuaapp);
                outputMessages.Add(string.Format(OutputText.SlnAddCouldntDeserliazeOpcuaapp, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
			}
			
            // check if sln does not contain opcuaapp yet
            if (!oppoSolution.Projects.Any(x => x.Name == oppoProj.Name))
            {
				// add opcuaapp to sln
				oppoProj.Path = oppoprojFilePath;
				oppoSolution.Projects.Add(oppoProj);

				// serialize and write sln
				var slnNewContent = JsonConvert.SerializeObject(oppoSolution, Formatting.Indented);
				_fileSystem.WriteFile(solutionFullName, new List<string> { slnNewContent });
            }
            else
            {
                OppoLogger.Info(LoggingText.SlnAddContainsOpcuaapp);
                outputMessages.Add(string.Format(OutputText.SlnAddContainsOpcuaapp, solutionName, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }
			
			// exit method with success
            OppoLogger.Info(LoggingText.SlnAddSuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnAddSuccess, projectName, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnAddNameArgumentCommandDescription;
        }
    }
}