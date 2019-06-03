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

		public override string Name => Constants.ReferenceCommandArguments.Add;

		public override CommandResult Execute(IEnumerable<string> inputParams)
		{
			if (!ExecuteCommon(inputParams))
			{
				return new CommandResult(false, _outputMessages);
			}

			// validate server
			var serverFullName = _fileSystem.CombinePaths(_serverName, _serverName + Constants.FileExtension.OppoProject);
			if(!ValidateServer(_serverName, serverFullName))
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

			// check if deserialized server is not a client
			if(opcuaServer.Type == Constants.ApplicationType.Client)
			{
				OppoLogger.Warn(LoggingText.ReferenceAddClientCannotBeReferred);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddClientCannotBeReferred, _serverName), string.Empty);
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

			// check if deserialized client is not a server
			if(!ClientIsNotAServer(ref opcuaClient, ref opcuaClientServer, _clientName))
			{
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

		private bool ValidateServer(string serverName, string serverFullName)
		{
			// check if server oppoproj file exists
			if (string.IsNullOrEmpty(serverName) || !_fileSystem.FileExists(serverFullName))
			{
				OppoLogger.Warn(LoggingText.ReferenceAddServerOppoprojFileNotFound);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddServerOppoprojFileNotFound, serverFullName), string.Empty);
				return false;
			}

			return true;
		}

		private bool ClientIsNotAServer(ref OpcuaClientApp opcuaClient, ref OpcuaClientServerApp opcuaClientServer, string clientName)
		{
			if ((opcuaClient != null && opcuaClient.Type == Constants.ApplicationType.Server) || (opcuaClientServer != null && opcuaClientServer.Type == Constants.ApplicationType.Server))
			{
				OppoLogger.Warn(LoggingText.ReferenceAddClientIsAServer);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddClientIsAServer, clientName), string.Empty);
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