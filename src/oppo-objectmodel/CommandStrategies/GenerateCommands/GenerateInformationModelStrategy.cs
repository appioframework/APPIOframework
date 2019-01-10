using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System;

namespace Oppo.ObjectModel.CommandStrategies.GenerateCommands
{
    public class GenerateInformationModelStrategy : ICommand<GenerateStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public GenerateInformationModelStrategy(string commandName, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = commandName;
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsList = inputParams.ToList();
            var nameFlag = inputParamsList.ElementAtOrDefault(0);
            var opcuaAppName = inputParamsList.ElementAtOrDefault(1);
            var modelFlag = inputParamsList.ElementAtOrDefault(2);
            var modelFullName = inputParamsList.ElementAtOrDefault(3);

            var outputMessages = new MessageLines();

            //if (string.IsNullOrEmpty(projectName))
            //{
            //    OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
            //    outputMessages.Add(OutputText.OpcuaappBuildFailure, string.Empty);

            //    return new CommandResult(false, outputMessages);
            //}

            //var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            //var mesonResult = _fileSystem.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild);
            //if (!mesonResult)
            //{
            //    OppoLogger.Warn(LoggingText.MesonExecutableFails);
            //    outputMessages.Add(OutputText.OpcuaappBuildFailure, string.Empty);

            //    return new CommandResult(false, outputMessages);
            //}
            //var ninjaResult = _fileSystem.CallExecutable(Constants.ExecutableName.Ninja, buildDirectory, string.Empty);
            //if (!ninjaResult)
            //{
            //    OppoLogger.Warn(LoggingText.NinjaExecutableFails);
            //    outputMessages.Add(OutputText.OpcuaappBuildFailure, string.Empty);
            //    return new CommandResult(false, outputMessages);
            //}

            var srcDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);

            // create inside src/server the information-models directory if it is not already created
            CreateNeededDirectories(srcDirectory);

            var modelName = _fileSystem.GetFileName(modelFullName);
            var args = modelFullName + " " + modelName; 
            var pythonResult = _fileSystem.CallExecutable(Constants.ExecutableName.NodsetCompiler, srcDirectory, args);
            if (!pythonResult)
            {
                OppoLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailure, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            AdjustServerTemplate(srcDirectory, modelName);

            outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, modelFullName), string.Empty);
            OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
            return new CommandResult(true, outputMessages);            
        }

        /// <summary>
        /// Server template should call the (from information-model) generated code. 
        /// </summary>
        /// <param name="srcDirectory"></param>
        private void AdjustServerTemplate(string srcDirectory, string modelName)
        {
            
        }

        private void CreateNeededDirectories(string srcDirectory)
        {
            var pathToCreate = System.IO.Path.Combine(srcDirectory, Constants.DirectoryName.InformationModels);
            if (!_fileSystem.DirectoryExists(pathToCreate))
            {
                _fileSystem.CreateDirectory(pathToCreate);
            }            
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.GenerateInformationModelArgumentCommandDescription;
        }
    }
}