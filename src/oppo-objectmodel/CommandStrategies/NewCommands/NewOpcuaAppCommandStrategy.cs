using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewOpcuaAppCommandStrategy : ICommand<NewStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public NewOpcuaAppCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.NewCommandName.OpcuaApp;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var nameFlag = inputParamsArray.ElementAtOrDefault(0);
            var opcuaAppName = inputParamsArray.ElementAtOrDefault(1);
            var outputMessages = new MessageLines();

            if (nameFlag != Constants.NewOpcuaAppCommandArguments.Name && nameFlag != Constants.NewOpcuaAppCommandArguments.VerboseName)
            {
                OppoLogger.Warn(LoggingText.UnknownNewOpcuaappCommandParam);
                outputMessages.Add(OutputText.NewOpcuaappCommandFailureUnknownParam, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (string.IsNullOrEmpty(opcuaAppName))
            {
                OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
                outputMessages.Add(OutputText.NewOpcuaappCommandFailureUnknownParam, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
                outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailure, opcuaAppName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (_fileSystem.GetInvalidPathChars().Any(opcuaAppName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
                outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailure, opcuaAppName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            DeployTemplateOpcuaApp(opcuaAppName);
            CreateModelsDirectory(opcuaAppName);

            var sourceDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode);
            _fileSystem.CreateDirectory(sourceDirectory);

            DeployTemplateOpcuaClientSourceFiles(sourceDirectory);
            DeployTemplateOpcuaServerSourceFiles(sourceDirectory);

            OppoLogger.Info(string.Format(LoggingText.NewOpcuaappCommandSuccess, opcuaAppName));
            outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandSuccess, opcuaAppName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        private void CreateModelsDirectory(string opcuaAppName)
        {
            var modelsDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);
            _fileSystem.CreateDirectory(modelsDirectory);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }

        private void DeployTemplateOpcuaApp(string opcuaAppName)
        {
            _fileSystem.CreateDirectory(opcuaAppName);
            var projectFilePath = _fileSystem.CombinePaths(opcuaAppName, $"{opcuaAppName}{Constants.FileExtension.OppoProject}");
            _fileSystem.CreateFile(projectFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_oppoproject));
            var mesonFilePath = _fileSystem.CombinePaths(opcuaAppName, Constants.FileName.SourceCode_meson_build);
            _fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_build));
        }

        private void DeployTemplateOpcuaClientSourceFiles(string sourceDirectory)
        {
            var appSourceDirectory = _fileSystem.CombinePaths(sourceDirectory, Constants.DirectoryName.ClientApp);
            _fileSystem.CreateDirectory(appSourceDirectory);
            _fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_client_c));
            _fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_open62541_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_c));
            _fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_open62541_h), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_h));
        }

        private void DeployTemplateOpcuaServerSourceFiles(string sourceDirectory)
        {
            var appSourceDirectory = _fileSystem.CombinePaths(sourceDirectory, Constants.DirectoryName.ServerApp);
            _fileSystem.CreateDirectory(appSourceDirectory);
            _fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_server_c));
            _fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_open62541_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_c));
            _fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_open62541_h), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_h));
        }
    }
}