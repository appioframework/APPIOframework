using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.ReferenceCommands
{
	public class ReferenceRemoveCommandStrategy : ICommand<ReferenceStrategy>
	{
		private readonly IFileSystem _fileSystem;

		public ReferenceRemoveCommandStrategy(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public string Name => Constants.ReferenceCommandName.Remove;

		public CommandResult Execute(IEnumerable<string> inputParams)
		{
			var inputParamsArray = inputParams.ToArray();
			var serverNameFlag = inputParamsArray.ElementAtOrDefault(0);
			var serverName = inputParamsArray.ElementAtOrDefault(1);
			var clientNameFlag = inputParamsArray.ElementAtOrDefault(2);
			var clientName = inputParamsArray.ElementAtOrDefault(3);

			var outputMessages = new MessageLines();

			// check if serverNameFlag is valid
			if (serverNameFlag != Constants.ReferenceRemoveCommandArguments.Server && serverNameFlag != Constants.ReferenceRemoveCommandArguments.VerboseServer)
			{
				OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
				outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, serverNameFlag), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// validate client name flag
			if (clientNameFlag != Constants.ReferenceRemoveCommandArguments.Client && clientNameFlag != Constants.ReferenceRemoveCommandArguments.VerboseClient)
			{
				OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
				outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, clientNameFlag), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if client name is empty 
			var clientFullName = _fileSystem.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject);
			if (string.IsNullOrEmpty(clientName) || !_fileSystem.FileExists(clientFullName))
			{
				OppoLogger.Warn(LoggingText.ReferenceClientOppoprojFileNotFound);
				outputMessages.Add(string.Format(OutputText.ReferenceClientOppoprojFileNotFound, clientFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if server name is empty
			if (string.IsNullOrEmpty(serverName))
			{
				OppoLogger.Warn(LoggingText.ReferenceRemoveServerNameEmpty);
				outputMessages.Add(OutputText.ReferenceRemoveServerNameEmpty, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialise client file
			OpcuaClientApp oppoClient = SlnUtility.DeserializeFile<OpcuaClientApp>(clientFullName, _fileSystem);
			if (oppoClient == null)
			{
				OppoLogger.Warn(LoggingText.ReferenceCouldntDeserliazeClient);
				outputMessages.Add(string.Format(OutputText.ReferenceCouldntDeserliazeClient, clientFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if server is a part of the client
			var oppoProject = oppoClient.ServerReferences.SingleOrDefault(x => x.Name == serverName);
			if (oppoProject != null)
			{
				// remove server from client
				oppoClient.ServerReferences.Remove(oppoProject);

				// serialize and write client
				var clientNewContent = JsonConvert.SerializeObject(oppoClient, Formatting.Indented);
				_fileSystem.WriteFile(clientFullName, new List<string> { clientNewContent });
			}
			else
			{
				OppoLogger.Warn(LoggingText.ReferenceRemoveServerIsNotInClient);
				outputMessages.Add(string.Format(OutputText.ReferenceRemoveServerIsNotInClient, serverName, clientName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// exit method with success
			OppoLogger.Info(LoggingText.ReferenceRemoveSuccess);
			outputMessages.Add(string.Format(OutputText.ReferenceRemoveSuccess, clientName, serverName), string.Empty);
			return new CommandResult(true, outputMessages);
		}

		public string GetHelpText()
		{
			return Resources.text.help.HelpTextValues.ReferenceRemoveNameArgumentCommandDescription;
		}

	}
}