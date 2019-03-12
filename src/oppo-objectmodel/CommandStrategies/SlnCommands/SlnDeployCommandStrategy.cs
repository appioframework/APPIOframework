using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnDeployCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public SlnDeployCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.SlnCommandName.Deploy;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray    = inputParams.ToArray();
            var solutionNameFlag    = inputParamsArray.ElementAtOrDefault(0);
            var solutionName        = inputParamsArray.ElementAtOrDefault(1);

            var outputMessages = new MessageLines();
			var deserializationMessages = new SlnUtility.ResultMessages();
			
			// validate solution name and deserialize *.opposln file
			Solution oppoSolution = null;
			if(!SlnUtility.DeserializeSolution(ref deserializationMessages, ref oppoSolution, solutionNameFlag, solutionName, _fileSystem))
			{
				OppoLogger.Warn(deserializationMessages.LoggerMessage);
				outputMessages.Add(deserializationMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deploy projects that are part of solution
			var deployCommandStrategy = new DeployCommands.DeployNameStrategy(Constants.CommandName.Deploy, _fileSystem);
			foreach (var project in oppoSolution.Projects)
			{
				var commandResult = deployCommandStrategy.Execute(new string[] { project.Name });
				if(!commandResult.Sucsess)
				{
					return commandResult;
				}
			}


			// exit method with success
            OppoLogger.Info(LoggingText.SlnDeploySuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnDeploySuccess, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }
        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnDeployNameArgumentCommandDescription;
        }
    }
}