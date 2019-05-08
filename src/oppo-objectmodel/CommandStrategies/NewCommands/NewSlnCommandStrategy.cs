using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewSlnCommandStrategy : ICommand<NewStrategy>
    {
        private readonly IFileSystem _fileSystem;
        private readonly ParameterResolver<ParamId> _resolver;
        
        private enum ParamId {SolutionName}

        public NewSlnCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _resolver = new ParameterResolver<ParamId>(Constants.CommandName.New + " " + Name, new []
            {
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.SolutionName,
                    Short = Constants.NewSlnCommandArguments.Name,
                    Verbose = Constants.NewSlnCommandArguments.VerboseName
                }
            });
        }

        public string Name => Constants.NewCommandName.Sln;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();

            var (error, stringParams, _) = _resolver.ResolveParams(inputParams);

            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var solutionName = stringParams[ParamId.SolutionName];
            
            if (_fileSystem.GetInvalidFileNameChars().Any(solutionName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidSolutionName);
                outputMessages.Add(string.Format(OutputText.NewSlnCommandFailure, solutionName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var solutionFilePath = $"{solutionName}{Constants.FileExtension.OppoSln}";
            _fileSystem.CreateFile(solutionFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName));
            OppoLogger.Info(string.Format(LoggingText.NewSlnCommandSuccess, solutionFilePath));
            outputMessages.Add(string.Format(OutputText.NewSlnCommandSuccess, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}