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
		
		public struct ResultMessages
		{
			public string LoggerMessage { get; set; }
			public string OutputMessage { get; set; }
		};

		public CommandResult Execute(IEnumerable<string> inputParams)
		{
			var inputParamsArray = inputParams.ToArray();
			var clientNameFlag = inputParamsArray.ElementAtOrDefault(0);
			var clientName = inputParamsArray.ElementAtOrDefault(1);
			var serverNameFlag = inputParamsArray.ElementAtOrDefault(2);
			var serverName = inputParamsArray.ElementAtOrDefault(3);

			var outputMessages = new MessageLines();
			var resultMessages = new ResultMessages();

			// validate client
			var clientFullName = _fileSystem.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject);
			if (!ValidateClient(ref resultMessages, clientNameFlag, clientName, clientFullName))
			{
				OppoLogger.Warn(resultMessages.LoggerMessage);
				outputMessages.Add(resultMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// validate server
			if (!ValidateServer(ref resultMessages, serverNameFlag, serverName))
			{
				OppoLogger.Warn(resultMessages.LoggerMessage);
				outputMessages.Add(resultMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialise client file
			OpcuaClientApp opcuaClient = null;
			OpcuaClientServerApp opcuaClientServer = null;
			RefUtility.DeserializeClient(ref opcuaClient, ref opcuaClientServer, clientFullName, _fileSystem);
			if (opcuaClient == null && opcuaClientServer == null)
			{
				OppoLogger.Warn(LoggingText.ReferenceCouldntDeserliazeClient);
				outputMessages.Add(string.Format(OutputText.ReferenceCouldntDeserliazeClient, clientFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if server is part of client's reference and remove it
			string clientNewContent = string.Empty;
			IOpcuaServerApp serverReference = null;
			if (opcuaClientServer != null && (serverReference = opcuaClientServer.ServerReferences.SingleOrDefault(x => x.Name == serverName)) != null)
			{
				opcuaClientServer.ServerReferences.Remove(serverReference);
				clientNewContent = JsonConvert.SerializeObject(opcuaClientServer, Formatting.Indented);
			}
			else if (opcuaClient != null && (serverReference = opcuaClient.ServerReferences.SingleOrDefault(x => x.Name == serverName)) != null)
			{
				opcuaClient.ServerReferences.Remove(serverReference);
				clientNewContent = JsonConvert.SerializeObject(opcuaClient, Formatting.Indented);
			}
			else
			{
				OppoLogger.Warn(LoggingText.ReferenceRemoveServerIsNotInClient);
				outputMessages.Add(string.Format(OutputText.ReferenceRemoveServerIsNotInClient, serverName, clientName), string.Empty);
				return new CommandResult(false, outputMessages);
			}
			_fileSystem.WriteFile(clientFullName, new List<string> { clientNewContent });

			// exit method with success
			OppoLogger.Info(LoggingText.ReferenceRemoveSuccess);
			outputMessages.Add(string.Format(OutputText.ReferenceRemoveSuccess, clientName, serverName), string.Empty);
			return new CommandResult(true, outputMessages);
		}

		private bool ValidateClient(ref ResultMessages resultMessages, string clientNameFlag, string clientName, string clientFullName)
		{
			// validate client name flag
			if (clientNameFlag != Constants.ReferenceRemoveCommandArguments.Client && clientNameFlag != Constants.ReferenceRemoveCommandArguments.VerboseClient)
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceUnknownCommandParam;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceUnknownParameter, clientNameFlag);
				return false;
			}

			// check if client name is empty 
			if (string.IsNullOrEmpty(clientName) || !_fileSystem.FileExists(clientFullName))
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceClientOppoprojFileNotFound;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceClientOppoprojFileNotFound, clientFullName);
				return false;
			}

			return true;
		}

		private bool ValidateServer(ref ResultMessages resultMessages, string serverNameFlag, string serverName)
		{
			// check if serverNameFlag is valid
			if (serverNameFlag != Constants.ReferenceRemoveCommandArguments.Server && serverNameFlag != Constants.ReferenceRemoveCommandArguments.VerboseServer)
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceUnknownCommandParam;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceUnknownParameter, serverNameFlag);
				return false;
			}

			// check if server name is empty
			if (string.IsNullOrEmpty(serverName))
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceRemoveServerNameEmpty;
				resultMessages.OutputMessage = OutputText.ReferenceRemoveServerNameEmpty;
				return false;
			}

			return true;
		}

		public string GetHelpText()
		{
			return Resources.text.help.HelpTextValues.ReferenceRemoveNameArgumentCommandDescription;
		}

	}
}