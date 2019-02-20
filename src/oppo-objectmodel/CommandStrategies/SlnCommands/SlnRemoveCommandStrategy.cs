using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnRemoveCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public SlnRemoveCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.SlnCommandName.Remove;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray    = inputParams.ToArray();
            var solutionNameFlag    = inputParamsArray.ElementAtOrDefault(0);
            var solutionName        = inputParamsArray.ElementAtOrDefault(1);
            var projectNameFlag     = inputParamsArray.ElementAtOrDefault(2);
            var projectName         = inputParamsArray.ElementAtOrDefault(3);

            var outputMessages = new MessageLines();

            // validate flags
            if (solutionNameFlag != Constants.SlnRemoveCommandArguments.Solution && solutionNameFlag != Constants.SlnRemoveCommandArguments.VerboseSolution)
            {
                OppoLogger.Warn(LoggingText.SlnUnknownCommandParam);
                outputMessages.Add(string.Format(OutputText.SlnUnknownParameter, solutionNameFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }
            if (projectNameFlag != Constants.SlnRemoveCommandArguments.Project && projectNameFlag != Constants.SlnRemoveCommandArguments.VerboseProject)
            {
                OppoLogger.Warn(LoggingText.SlnUnknownCommandParam);
                outputMessages.Add(string.Format(OutputText.SlnUnknownParameter, projectNameFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }


            // check if solution file is existing
            var solutionFullName = _fileSystem.CombinePaths(solutionName + Constants.FileExtension.OppoSln);
            if (string.IsNullOrEmpty(solutionName) || !_fileSystem.FileExists(solutionFullName))
            {
                OppoLogger.Warn(LoggingText.SlnOpposlnFileNotFound);
                outputMessages.Add(string.Format(OutputText.SlnOpposlnNotFound, solutionFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // check if project name is empty 
            
            if (string.IsNullOrEmpty(projectName))
            {
                OppoLogger.Warn(LoggingText.SlnRemoveOppoprojNameEmpty);
                outputMessages.Add (OutputText.SlnRemoveOpcuaappNameEmpty,string.Empty);
                return new CommandResult(false, outputMessages);
            }


            // deserialise solution file

            // check if the project to remove is part of the solution

            // remove the project

            // exit method with success

            OppoLogger.Info(LoggingText.SlnAddSuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnAddSuccess, projectName, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanNameArgumentCommandDescription;
        }
        
    }
}