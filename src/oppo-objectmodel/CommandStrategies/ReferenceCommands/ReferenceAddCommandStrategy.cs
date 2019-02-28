using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.ReferenceCommands
{
    public class ReferenceAddCommandStrategy : ICommand<ReferenceStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public ReferenceAddCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.ReferenceCommandName.Add;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray    = inputParams.ToArray();
            var serverNameFlag      = inputParamsArray.ElementAtOrDefault(0);
            var serverName          = inputParamsArray.ElementAtOrDefault(1);
            var clientNameFlag      = inputParamsArray.ElementAtOrDefault(2);
            var clientName          = inputParamsArray.ElementAtOrDefault(3);

            var outputMessages = new MessageLines();


            // check if serverNameFlag is valid
            if (serverNameFlag != Constants.ReferenceAddCommandArguments.Server && serverNameFlag != Constants.ReferenceAddCommandArguments.VerboseServer)
            {
                OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
                outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, serverNameFlag), string.Empty);
                return new CommandResult(false, outputMessages);  
            }

            // check if clientNameFlag is valid
            if (clientNameFlag != Constants.ReferenceAddCommandArguments.Client && clientNameFlag != Constants.ReferenceAddCommandArguments.VerboseClient)
            {
                OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
                outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, clientNameFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }

           //Check if server file exists
            var ServerFullName = _fileSystem.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject);
            if (string.IsNullOrEmpty(serverName) || !_fileSystem.FileExists(ServerFullName))
            {
                OppoLogger.Warn(LoggingText.OppoServerFileNotFound);
                outputMessages.Add(string.Format(OutputText.ServerNotFound, ServerFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            //Check if client file exists
            var ClientFullName = _fileSystem.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject);
            if (string.IsNullOrEmpty(clientName) || !_fileSystem.FileExists(ClientFullName))
            {
                OppoLogger.Warn(LoggingText.OppoClientFileNotFound);
                outputMessages.Add(string.Format(OutputText.ClientNotFound, ClientFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }
            return new CommandResult(false, outputMessages);
        }
            
        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.ReferenceAddNameArgumentCommandDescription;
        }
    }
}