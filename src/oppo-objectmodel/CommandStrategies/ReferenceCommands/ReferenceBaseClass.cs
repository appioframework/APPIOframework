using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.ReferenceCommands
{
	public abstract class ReferenceBase : ICommand<ReferenceStrategy>
	{
		protected readonly IFileSystem _fileSystem;
		
		private enum ParamId {ClientName, ServerName}
		
		protected ReferenceBase(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public virtual string Name => string.Empty;

		protected string _clientName;
		protected string _clientFullName;
		protected string _serverName;
		protected MessageLines _outputMessages;

		protected bool ExecuteCommon(IEnumerable<string> inputParams)
		{
			var resolver = new ParameterResolver<ParamId>(Constants.CommandName.Reference + " " + Name, new []
			{
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.ClientName,
					Short = Constants.ReferenceAddCommandArguments.Client,
					Verbose = Constants.ReferenceAddCommandArguments.VerboseClient
				},
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.ServerName,
					Short = Constants.ReferenceAddCommandArguments.Server,
					Verbose = Constants.ReferenceAddCommandArguments.VerboseServer
				}
			});

			var (error, stringParams, _) = resolver.ResolveParams(inputParams);

			if (error != null)
			{
				_outputMessages = new MessageLines {{error, string.Empty}};
				return false;
			}

			_serverName = stringParams[ParamId.ServerName];
			_clientName = stringParams[ParamId.ClientName];
			
			_outputMessages = new MessageLines();

			// check if client oppoproj file exists
			_clientFullName = _fileSystem.CombinePaths(_clientName, _clientName + Constants.FileExtension.OppoProject);
			if (!_fileSystem.FileExists(_clientFullName))
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