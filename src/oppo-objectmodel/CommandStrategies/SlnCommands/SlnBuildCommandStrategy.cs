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
			var deserializationMessages = new SlnUtility.ResultMessages();

			// validate solution name and deserialize *.opposln file
			Solution oppoSolution = null;
			if (!SlnUtility.DeserializeSolution(ref deserializationMessages, ref oppoSolution, solutionNameFlag, solutionName, _fileSystem))
			{
				OppoLogger.Warn(deserializationMessages.LoggerMessage);
				outputMessages.Add(deserializationMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// build projects that are part of solution
			var buildCommandStrategy = new BuildCommands.BuildNameStrategy(Constants.CommandName.Build, _fileSystem);
			foreach (var project in oppoSolution.Projects)
			{
				var commandResult = buildCommandStrategy.Execute(new string[] { project.Name });
				if(!commandResult.Sucsess)
				{
					return commandResult;
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