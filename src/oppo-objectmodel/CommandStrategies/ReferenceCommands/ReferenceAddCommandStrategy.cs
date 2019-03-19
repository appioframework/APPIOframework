using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.ReferenceCommands
{
	public class ReferenceAddCommandStrategy : ReferenceBase
	{
		public ReferenceAddCommandStrategy(IFileSystem fileSystem) : base(fileSystem) {	}

		public override string Name => Constants.ReferenceCommandName.Add;

		public override CommandResult Execute(IEnumerable<string> inputParams)
		{
			if (!ExecuteCommon(inputParams))
			{
				return new CommandResult(false, _outputMessages);
			}

			// validate server
			var serverFullName = _fileSystem.CombinePaths(_serverName, _serverName + Constants.FileExtension.OppoProject);
			if(!ValidateServer(_serverNameFlag, _serverName, serverFullName))
			{
				return new CommandResult(false, _outputMessages);
			}

			// deserialize server file
			OpcuaServerApp opcuaServer = SlnUtility.DeserializeFile<OpcuaServerApp>(serverFullName, _fileSystem);
			if (opcuaServer == null)
			{
				OppoLogger.Warn(LoggingText.ReferenceAddCouldntDeserliazeServer);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddCouldntDeserliazeServer, serverFullName), string.Empty);
				return new CommandResult(false, _outputMessages);
			}

			// deserialize client file
			OpcuaClientApp opcuaClient = null;
			OpcuaClientServerApp opcuaClientServer = null;
			RefUtility.DeserializeClient(ref opcuaClient, ref opcuaClientServer, _clientFullName, _fileSystem);
			if (opcuaClient == null && opcuaClientServer == null)
			{
				OppoLogger.Warn(LoggingText.ReferenceCouldntDeserliazeClient);
				_outputMessages.Add(string.Format(OutputText.ReferenceCouldntDeserliazeClient, _clientFullName), string.Empty);
				return new CommandResult(false, _outputMessages);
			}

			// check if server is not already a part of client's references
			if(!ServerIsNotYetClientsReference(ref opcuaClient, ref opcuaClientServer, _clientName, opcuaServer.Name))
			{
				return new CommandResult(false, _outputMessages);
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
			_fileSystem.WriteFile(_clientFullName, new List<string> { clientNewContent });

			// exit with success
			OppoLogger.Info(LoggingText.ReferenceAddSuccess);
			_outputMessages.Add(string.Format(OutputText.RefereneceAddSuccess, _serverName, _clientName), string.Empty);
			return new CommandResult(true, _outputMessages);
		}

		private bool ValidateClient(string clientNameFlag, string clientName, string clientFullName)
		{
			// validate client name flag
			if (clientNameFlag != Constants.ReferenceAddCommandArguments.Client && clientNameFlag != Constants.ReferenceAddCommandArguments.VerboseClient)
			{
				OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
				_outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, clientNameFlag), string.Empty);
				return false;
			}

			// check if client oppoproj file exists
			if (string.IsNullOrEmpty(clientName) || !_fileSystem.FileExists(clientFullName))
			{
				OppoLogger.Warn(LoggingText.ReferenceClientOppoprojFileNotFound);
				_outputMessages.Add(string.Format(OutputText.ReferenceClientOppoprojFileNotFound, clientFullName), string.Empty);
				return false;
			}

			return true;
		}

		private bool ValidateServer(string serverNameFlag, string serverName, string serverFullName)
		{
			// validate server name flag
			if (serverNameFlag != Constants.ReferenceAddCommandArguments.Server && serverNameFlag != Constants.ReferenceAddCommandArguments.VerboseServer)
			{
				OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
				_outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, serverNameFlag), string.Empty);
				return false;
			}

			// check if server oppoproj file exists
			if (string.IsNullOrEmpty(serverName) || !_fileSystem.FileExists(serverFullName))
			{
				OppoLogger.Warn(LoggingText.ReferenceAddServerOppoprojFileNotFound);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddServerOppoprojFileNotFound, serverFullName), string.Empty);
				return false;
			}

			return true;
		}

		private bool ServerIsNotYetClientsReference(ref OpcuaClientApp opcuaClient, ref OpcuaClientServerApp opcuaClientServer, string clientName, string serverName)
		{
			if((opcuaClient != null && opcuaClient.ServerReferences.Any(x => x.Name == serverName)) || (opcuaClientServer != null && opcuaClientServer.ServerReferences.Any(x => x.Name == serverName)))
			{
				OppoLogger.Warn(LoggingText.ReferenceAddServerIsPartOfClientReference);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddServerIsPartOfClientReference, serverName, clientName), string.Empty);
				return false;
			}

			return true;
		}

		public override string GetHelpText()
		{
			return Resources.text.help.HelpTextValues.ReferenceAddNameArgumentCommandDescription;
		}
	}
}