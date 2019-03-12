using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnOperationCommandStrategy : ICommand<SlnStrategy>
	{
		public string Name { get; }
		private readonly IFileSystem _fileSystem;
		private ICommand _subcommand;
		public string SuccessLoggerMessage { get; }
		public string SuccessOutputMessage { get; }
		public string HelpText { get; }

		public SlnOperationCommandStrategy(SlnOperationData operationData)
        {
			Name = operationData.CommandName;
            _fileSystem = operationData.FileSystem;
			_subcommand = operationData.Subcommand;
			SuccessLoggerMessage = operationData.SuccessLoggerMessage;
			SuccessOutputMessage = operationData.SuccessOutputMessage;
			HelpText = operationData.HelpText;
        }

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
				OppoLogger.Warn(validationMessages.LoggerMessage);
				outputMessages.Add(validationMessages.OutputMessage, string.Empty);
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
			foreach (var project in oppoSolution.Projects)
			{
				var commandResult = _subcommand.Execute(new string[] { project.Name });
				if(!commandResult.Sucsess)
				{
					return commandResult;
				}
			}
			
			// exit method with success
            OppoLogger.Info(SuccessLoggerMessage);                        
            outputMessages.Add(string.Format(SuccessOutputMessage, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }
        public string GetHelpText()
        {
			return HelpText;
        }
    }
}