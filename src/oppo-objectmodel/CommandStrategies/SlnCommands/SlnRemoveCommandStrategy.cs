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
        private string oppoprojFilePath;

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
            var slnMemoryStream = _fileSystem.ReadFile(solutionFullName);
            StreamReader readerSln = new StreamReader(slnMemoryStream);
            var slnContent = readerSln.ReadToEnd();
            Solution oppoSolution;
            try
            {
                oppoSolution = JsonConvert.DeserializeObject<Solution>(slnContent);
                if (oppoSolution == null)
                {
                    throw null;
                }
            }
            catch (Exception)
            {
                OppoLogger.Warn(LoggingText.SlnCouldntDeserliazeSln);
                outputMessages.Add(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), string.Empty);
                return new CommandResult(false, outputMessages);
            }
            slnMemoryStream.Close();
            slnMemoryStream.Dispose();

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
            return Resources.text.help.HelpTextValues.CleanNameArgumentCommandDescription;
        }
        
    }
}