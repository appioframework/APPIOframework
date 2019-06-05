using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnRemoveCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;
        private readonly ParameterResolver<ParamId> _resolver;
        
        private enum ParamId {SolutionName, ProjectName}

        public SlnRemoveCommandStrategy(IFileSystem fileSystem)
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

        public string Name => Constants.SlnCommandArguments.Remove;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();

            var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var solutionName = stringParams[ParamId.SolutionName];
            var projectName = stringParams[ParamId.ProjectName];
            
            // check if solution file is existing
            var solutionFullName = _fileSystem.CombinePaths(solutionName + Constants.FileExtension.OppoSln);
            if (string.IsNullOrEmpty(solutionName) || !_fileSystem.FileExists(solutionFullName))
            {
                OppoLogger.Warn(LoggingText.SlnOpposlnFileNotFound);
                outputMessages.Add(string.Format(OutputText.SlnOpposlnNotFound, solutionFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

			// deserialise solution file
			Solution oppoSolution = SlnUtility.DeserializeFile<Solution>(solutionFullName, _fileSystem);
			if (oppoSolution == null)
			{
				OppoLogger.Warn(LoggingText.SlnCouldntDeserliazeSln);
				outputMessages.Add(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if the project to remove is part of the solution
			var oppoProj = oppoSolution.Projects.SingleOrDefault(x => x.Name == projectName);
            if (oppoProj != null)
            {
                // remove opcuaapp from sln
                oppoSolution.Projects.Remove(oppoProj);

                // serialize and write sln
                var slnNewContent = JsonConvert.SerializeObject(oppoSolution, Formatting.Indented);
                _fileSystem.WriteFile(solutionFullName, new List<string> { slnNewContent });
            }
            else
            {
                OppoLogger.Warn(LoggingText.SlnRemoveOpcuaappIsNotInSln);
                outputMessages.Add(string.Format(OutputText.SlnRemoveOpcuaappIsNotInSln, projectName, solutionName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

   

            // exit method with success
            OppoLogger.Info(LoggingText.SlnRemoveSuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnRemoveSuccess, projectName, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnRemoveNameArgumentCommandDescription;
        }
        
    }
}