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
			var validationMessages = new SlnUtility.ResultMessages();
			
			// validate solution name
			if (!SlnUtility.ValidateSolution(ref validationMessages, solutionNameFlag, solutionName, _fileSystem))
			{
				OppoLogger.Warn(validationMessages.getLoggerMessage);
				outputMessages.Add(validationMessages.getOutputMessage, string.Empty);
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
			// build projects that are part of solution
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