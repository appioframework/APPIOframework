using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnBuildCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public SlnBuildCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.SlnCommandName.Build;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray    = inputParams.ToArray();
            var solutionNameFlag    = inputParamsArray.ElementAtOrDefault(0);
            var solutionName        = inputParamsArray.ElementAtOrDefault(1);

            var outputMessages = new MessageLines();
			

            // check if solutionNameFlag is valid
            if(solutionNameFlag != Constants.SlnAddCommandArguments.Solution && solutionNameFlag != Constants.SlnAddCommandArguments.VerboseSolution)
			{
				OppoLogger.Warn(LoggingText.SlnUnknownCommandParam);
				outputMessages.Add(string.Format(OutputText.SlnUnknownParameter, solutionNameFlag), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if *.opposln file exists
			var solutionFullName = _fileSystem.CombinePaths(solutionName + Constants.FileExtension.OppoSln);
			if (string.IsNullOrEmpty(solutionName) || !_fileSystem.FileExists(solutionFullName))
			{
				OppoLogger.Warn(LoggingText.SlnOpposlnFileNotFound);
				outputMessages.Add(string.Format(OutputText.SlnOpposlnNotFound, solutionFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialize *.opposln file
			Solution oppoSolution = SlnUtility.DeserializeFile<Solution>(solutionFullName, _fileSystem);
			if (oppoSolution == null)
			{
				OppoLogger.Warn(LoggingText.SlnCouldntDeserliazeSln);
				outputMessages.Add(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), string.Empty);
				return new CommandResult(false, outputMessages);
			}
			else
			{
				var buildCommandStrategy = new BuildCommands.BuildNameStrategy(Constants.CommandName.Build, _fileSystem);
				foreach (var project in oppoSolution.Projects)
				{
					var commandResult = buildCommandStrategy.Execute(new string[] { project.Name });
					if(!commandResult.Sucsess)
					{
						return commandResult;
					}
				}
			}


			// exit method with success
            OppoLogger.Info(LoggingText.SlnBuildSuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnBuildSuccess, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }
        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnBuildNameArgumentCommandDescription;
        }
    }
}