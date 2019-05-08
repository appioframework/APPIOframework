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
		private readonly ParameterResolver<ParamId> _resolver;
        
		private enum ParamId {SolutionName}
		
		private readonly ICommand _subcommand;
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
			
			_resolver = new ParameterResolver<ParamId>(Constants.CommandName.Sln + " " + Name, new []
			{
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.SolutionName,
					Short = Constants.SlnAddCommandArguments.Solution,
					Verbose = Constants.SlnAddCommandArguments.VerboseSolution
				}
			});
        }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();
			var validationMessages = new SlnUtility.ResultMessages();
			
			var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
			if (error != null)
				return new CommandResult(false, new MessageLines{{error, string.Empty}});
			
			var solutionName = stringParams[ParamId.SolutionName];
			
			// validate solution name
			if (!SlnUtility.ValidateSolution(ref validationMessages, solutionName, _fileSystem))
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
				if(!commandResult.Success)
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