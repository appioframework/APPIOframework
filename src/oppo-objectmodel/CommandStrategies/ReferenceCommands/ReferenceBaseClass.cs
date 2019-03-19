using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.ReferenceCommands
{
	public abstract class ReferenceBase : ICommand<ReferenceStrategy>
	{
		protected readonly IFileSystem _fileSystem;
		
		public ReferenceBase(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public virtual string Name => string.Empty;

		protected string _clientNameFlag;
		protected string _clientName;
		protected string _clientFullName;
		protected string _serverNameFlag;
		protected string _serverName;
		protected MessageLines _outputMessages;

		protected bool ExecuteCommon(IEnumerable<string> inputParams)
		{
			var inputParamsArray = inputParams.ToArray();
			_clientNameFlag = inputParamsArray.ElementAtOrDefault(0);
			_clientName = inputParamsArray.ElementAtOrDefault(1);
			_serverNameFlag = inputParamsArray.ElementAtOrDefault(2);
			_serverName = inputParamsArray.ElementAtOrDefault(3);

			_outputMessages = new MessageLines();
			
			// validate client name flag
			if (_clientNameFlag != Constants.ReferenceAddCommandArguments.Client && _clientNameFlag != Constants.ReferenceAddCommandArguments.VerboseClient)
			{
				OppoLogger.Warn(LoggingText.ReferenceUnknownCommandParam);
				_outputMessages.Add(string.Format(OutputText.ReferenceUnknownParameter, _clientNameFlag), string.Empty);
				return false;
			}

			// check if client oppoproj file exists
			_clientFullName = _fileSystem.CombinePaths(_clientName, _clientName + Constants.FileExtension.OppoProject);
			if (string.IsNullOrEmpty(_clientName) || !_fileSystem.FileExists(_clientFullName))
			{
				OppoLogger.Warn(LoggingText.ReferenceClientOppoprojFileNotFound);
				_outputMessages.Add(string.Format(OutputText.ReferenceClientOppoprojFileNotFound, _clientFullName), string.Empty);
				return false;
			}

			// exit with success
			return true;
		}

		public virtual CommandResult Execute(IEnumerable<string> inputParams) => null;

		public virtual string GetHelpText() => string.Empty;
	}
}