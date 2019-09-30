/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Appio.Resources.text.logging;
using Appio.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Appio.ObjectModel.CommandStrategies.ReferenceCommands
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
			var serverFullName = _fileSystem.CombinePaths(_serverName, _serverName + Constants.FileExtension.Appioproject);
			if(!ValidateServer(_serverName, serverFullName))
			{
				return new CommandResult(false, _outputMessages);
			}

			// deserialize server file
			OpcuaServerApp opcuaServer = SlnUtility.DeserializeFile<OpcuaServerApp>(serverFullName, _fileSystem);
			if (opcuaServer == null)
			{
				AppioLogger.Warn(LoggingText.ReferenceAddCouldntDeserliazeServer);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddCouldntDeserliazeServer, serverFullName), string.Empty);
				return new CommandResult(false, _outputMessages);
			}

			// check if deserialized server is not a client
			if(opcuaServer.Type == Constants.ApplicationType.Client)
			{
				AppioLogger.Warn(LoggingText.ReferenceAddClientCannotBeReferred);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddClientCannotBeReferred, _serverName), string.Empty);
				return new CommandResult(false, _outputMessages);
			}

			// deserialize client file
			OpcuaClientApp opcuaClient = null;
			OpcuaClientServerApp opcuaClientServer = null;
			RefUtility.DeserializeClient(ref opcuaClient, ref opcuaClientServer, _clientFullName, _fileSystem);
			if (opcuaClient == null && opcuaClientServer == null)
			{
				AppioLogger.Warn(LoggingText.ReferenceCouldntDeserliazeClient);
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

			// overwrite client appioproj file with new server reference
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
			AppioLogger.Info(LoggingText.ReferenceAddSuccess);
			_outputMessages.Add(string.Format(OutputText.RefereneceAddSuccess, _serverName, _clientName), string.Empty);
			return new CommandResult(true, _outputMessages);
		}

		private bool ValidateServer(string serverName, string serverFullName)
		{
			// check if server appioproj file exists
			if (string.IsNullOrEmpty(serverName) || !_fileSystem.FileExists(serverFullName))
			{
				AppioLogger.Warn(LoggingText.ReferenceAddServerAppioprojFileNotFound);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddServerAppioprojFileNotFound, serverFullName), string.Empty);
				return false;
			}

			return true;
		}

		private bool ClientIsNotAServer(ref OpcuaClientApp opcuaClient, ref OpcuaClientServerApp opcuaClientServer, string clientName)
		{
			if ((opcuaClient != null && opcuaClient.Type == Constants.ApplicationType.Server) || (opcuaClientServer != null && opcuaClientServer.Type == Constants.ApplicationType.Server))
			{
				AppioLogger.Warn(LoggingText.ReferenceAddClientIsAServer);
				_outputMessages.Add(string.Format(OutputText.ReferenceAddClientIsAServer, clientName), string.Empty);
				return false;
			}

			return true;
		}

		private bool ServerIsNotYetClientsReference(ref OpcuaClientApp opcuaClient, ref OpcuaClientServerApp opcuaClientServer, string clientName, string serverName)
		{
			if((opcuaClient != null && opcuaClient.ServerReferences.Any(x => x.Name == serverName)) || (opcuaClientServer != null && opcuaClientServer.ServerReferences.Any(x => x.Name == serverName)))
			{
				AppioLogger.Warn(LoggingText.ReferenceAddServerIsPartOfClientReference);
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