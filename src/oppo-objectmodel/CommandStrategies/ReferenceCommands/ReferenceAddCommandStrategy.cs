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
			var inputParamsArray = inputParams.ToArray();
			var clientNameFlag = inputParamsArray.ElementAtOrDefault(0);
			var clientName = inputParamsArray.ElementAtOrDefault(1);
			var serverNameFlag = inputParamsArray.ElementAtOrDefault(2);
			var serverName = inputParamsArray.ElementAtOrDefault(3);

			var outputMessages = new MessageLines();

			// check if clientNameFlag is valid
			if (clientNameFlag != Constants.ReferenceAddCommandArguments.Client && clientNameFlag != Constants.ReferenceAddCommandArguments.VerboseClient)
			{
				OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
				outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, clientNameFlag), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if serverNameFlag is valid
			if (serverNameFlag != Constants.ReferenceAddCommandArguments.Server && serverNameFlag != Constants.ReferenceAddCommandArguments.VerboseServer)
			{
				OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
				outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, serverNameFlag), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			//Check if client file exists
			var clientFullName = _fileSystem.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject);
			if (string.IsNullOrEmpty(clientName) || !_fileSystem.FileExists(clientFullName))
			{
				OppoLogger.Warn(LoggingText.OppoClientFileNotFound);
				outputMessages.Add(string.Format(OutputText.ClientNotFound, clientFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			//Check if server file exists
			var serverFullName = _fileSystem.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject);
			if (string.IsNullOrEmpty(serverName) || !_fileSystem.FileExists(serverFullName))
			{
				OppoLogger.Warn(LoggingText.OppoServerFileNotFound);
				outputMessages.Add(string.Format(OutputText.ServerNotFound, serverFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialize server file
			OpcuaServerApp oppoServer = SlnUtility.DeserializeFile<OpcuaServerApp>(serverFullName, _fileSystem);
			if (oppoServer == null)
			{
				OppoLogger.Warn(LoggingText.CouldntDeserliazeServer);
				outputMessages.Add(string.Format(OutputText.CouldntDeserliazeServer, serverFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialize client file
			OpcuaClientApp opcuaClient = SlnUtility.DeserializeFile<OpcuaClientApp>(clientFullName, _fileSystem);
			OpcuaClientServerApp opcuaClientServer = null;
			if (opcuaClient != null && opcuaClient.Type == Constants.ApplicationType.ClientServer)
			{
				opcuaClientServer = SlnUtility.DeserializeFile<OpcuaClientServerApp>(clientFullName, _fileSystem);
			}
			else if (opcuaClient == null)
			{
				OppoLogger.Warn(LoggingText.CouldntDeserliazeClient);
				outputMessages.Add(string.Format(OutputText.CouldntDeserliazeClient, clientFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// overwrite client oppoproj file with new server reference
			string clientNewContent = string.Empty;
			if (opcuaClientServer != null)
			{
				opcuaClientServer.ServerReferences.Add(oppoServer);
				clientNewContent = JsonConvert.SerializeObject(opcuaClientServer, Formatting.Indented);
			}
			else
			{
				opcuaClient.ServerReferences.Add(oppoServer);
				clientNewContent = JsonConvert.SerializeObject(opcuaClient, Formatting.Indented);
			}
			_fileSystem.WriteFile(clientFullName, new List<string> { clientNewContent });

			// exit with success
			OppoLogger.Info(LoggingText.ReferenceAddSuccess);
			outputMessages.Add(string.Format(OutputText.RefereneceAddSuccess, serverName, clientName), string.Empty);
			return new CommandResult(true, outputMessages);
		}

		public string GetHelpText()
		{
			return Resources.text.help.HelpTextValues.ReferenceAddNameArgumentCommandDescription;
		}
	}
}