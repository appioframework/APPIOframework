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

            if (nameFlag != Constants.NewOpcuaAppCommandArguments.Name && nameFlag != Constants.NewOpcuaAppCommandArguments.VerboseName)
            {
                OppoLogger.Warn(LoggingText.UnknownNewOpcuaappCommandParam);
                return new CommandResult(false, OutputText.NewOpcuaappCommandFailureUnknownParam);
            }

            if (string.IsNullOrEmpty(opcuaAppName))
            {
                OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
                return new CommandResult(false, OutputText.NewOpcuaappCommandFailureUnknownParam);
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
                return new CommandResult(false, string.Format(OutputText.NewOpcuaappCommandFailure, opcuaAppName));
            }

            if (_fileSystem.GetInvalidPathChars().Any(opcuaAppName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
                return new CommandResult(false, string.Format(OutputText.NewOpcuaappCommandFailure, opcuaAppName));
            }

            DeployTemplateOpcuaApp(opcuaAppName);
            DeployTemplateOpcuaSourceFiles(opcuaAppName);

            OppoLogger.Info(string.Format(LoggingText.NewOpcuaappCommandSuccess, opcuaAppName));
            return new CommandResult(true, string.Format(OutputText.NewOpcuaappCommandSuccess, opcuaAppName));
        }

        public string GetHelpText()
        {
            return string.Empty;
        }

        private void DeployTemplateOpcuaApp(string opcuaAppName)
        {
            _fileSystem.CreateDirectory(opcuaAppName);
            var projectFilePath = _fileSystem.CombinePaths(opcuaAppName, $"{opcuaAppName}{Constants.FileExtension.OppoProject}");
            _fileSystem.CreateFile(projectFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName));
            var mesonFilePath = _fileSystem.CombinePaths(opcuaAppName, Constants.FileName.SourceCode_meson_build);
            _fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_build));
        }

        private void DeployTemplateOpcuaSourceFiles(string opcuaAppDirectoryName)
        {
            var sourceCodeFilePath = _fileSystem.CombinePaths(opcuaAppDirectoryName, Constants.DirectoryName.SourceCode);
            _fileSystem.CreateDirectory(sourceCodeFilePath);
            _fileSystem.CreateFile(_fileSystem.CombinePaths(sourceCodeFilePath, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_c));
            _fileSystem.CreateFile(_fileSystem.CombinePaths(sourceCodeFilePath, Constants.FileName.SourceCode_open62541_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_c));
            _fileSystem.CreateFile(_fileSystem.CombinePaths(sourceCodeFilePath, Constants.FileName.SourceCode_open62541_h), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_h));
        }
    }
}