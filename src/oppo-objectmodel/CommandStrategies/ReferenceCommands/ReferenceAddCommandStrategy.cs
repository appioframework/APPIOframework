using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
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

		private struct ResultMessages
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
			if(!ValidateClient(ref resultMessages, clientNameFlag, clientName, clientFullName))
			{
				OppoLogger.Warn(resultMessages.LoggerMessage);
				outputMessages.Add(resultMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// validate server
			var serverFullName = _fileSystem.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject);
			if(!ValidateServer(ref resultMessages, serverNameFlag, serverName, serverFullName))
			{
				OppoLogger.Warn(resultMessages.LoggerMessage);
				outputMessages.Add(resultMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialize server file
			OpcuaServerApp opcuaServer = SlnUtility.DeserializeFile<OpcuaServerApp>(serverFullName, _fileSystem);
			if (opcuaServer == null)
			{
				OppoLogger.Warn(LoggingText.ReferenceAddCouldntDeserliazeServer);
				outputMessages.Add(string.Format(OutputText.ReferenceAddCouldntDeserliazeServer, serverFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialize client file
			OpcuaClientApp opcuaClient = null;
			OpcuaClientServerApp opcuaClientServer = null;
			RefUtility.DeserializeClient(ref opcuaClient, ref opcuaClientServer, clientFullName, _fileSystem);
			if (opcuaClient == null && opcuaClientServer == null)
			{
				OppoLogger.Warn(LoggingText.ReferenceCouldntDeserliazeClient);
				outputMessages.Add(string.Format(OutputText.ReferenceCouldntDeserliazeClient, clientFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if server is not already a part of client's references
			if(!ServerIsNotYetClientsReference(ref resultMessages, ref opcuaClient, ref opcuaClientServer, opcuaServer, clientName, serverName))
			{
				OppoLogger.Warn(resultMessages.LoggerMessage);
				outputMessages.Add(resultMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// overwrite client oppoproj file with new server reference
			string clientNewContent = string.Empty;
			if (opcuaClientServer != null)
			{
				opcuaClientServer.ServerReferences.Add(opcuaServer);
				clientNewContent = JsonConvert.SerializeObject(opcuaClientServer, Formatting.Indented);
			}
			else
			{
				opcuaClient.ServerReferences.Add(opcuaServer);
				clientNewContent = JsonConvert.SerializeObject(opcuaClient, Formatting.Indented);
			}
			_fileSystem.WriteFile(clientFullName, new List<string> { clientNewContent });

			// exit with success
			OppoLogger.Info(LoggingText.ReferenceAddSuccess);
			outputMessages.Add(string.Format(OutputText.RefereneceAddSuccess, serverName, clientName), string.Empty);
			return new CommandResult(true, outputMessages);
		}

		private bool ValidateClient(ref ResultMessages resultMessages, string clientNameFlag, string clientName, string clientFullName)
		{
			// validate client name flag
			if (clientNameFlag != Constants.ReferenceAddCommandArguments.Client && clientNameFlag != Constants.ReferenceAddCommandArguments.VerboseClient)
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceUnknownCommandParam;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceUnknownParameter, clientNameFlag);
				return false;
			}

			// check if client oppoproj file exists
			if (string.IsNullOrEmpty(clientName) || !_fileSystem.FileExists(clientFullName))
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceClientOppoprojFileNotFound;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceClientOppoprojFileNotFound, clientFullName);
				return false;
			}

			return true;
		}

		private bool ValidateServer(ref ResultMessages resultMessages, string serverNameFlag, string serverName, string serverFullName)
		{
			// validate server name flag
			if (serverNameFlag != Constants.ReferenceAddCommandArguments.Server && serverNameFlag != Constants.ReferenceAddCommandArguments.VerboseServer)
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceUnknownCommandParam;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceUnknownParameter, serverNameFlag);
				return false;
			}

			// check if server oppoproj file exists
			if (string.IsNullOrEmpty(serverName) || !_fileSystem.FileExists(serverFullName))
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceAddServerOppoprojFileNotFound;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceAddServerOppoprojFileNotFound, serverFullName);
				return false;
			}

			return true;
		}

		private bool ServerIsNotYetClientsReference(ref ResultMessages resultMessages, ref OpcuaClientApp opcuaClient, ref OpcuaClientServerApp opcuaClientServer, OpcuaServerApp opcuaServer, string clientName, string serverName)
		{
			if((opcuaClient != null && opcuaClient.ServerReferences.Any(x => x.Name == opcuaServer.Name)) || (opcuaClientServer != null && opcuaClientServer.ServerReferences.Any(x => x.Name == opcuaServer.Name)))
			{
				resultMessages.LoggerMessage = LoggingText.ReferenceAddServerIsPartOfClientReference;
				resultMessages.OutputMessage = string.Format(OutputText.ReferenceAddServerIsPartOfClientReference, serverName, clientName);
				return false;
			}

			return true;
		}

		public string GetHelpText()
		{
			return Resources.text.help.HelpTextValues.ReferenceAddNameArgumentCommandDescription;
		}
	}
}