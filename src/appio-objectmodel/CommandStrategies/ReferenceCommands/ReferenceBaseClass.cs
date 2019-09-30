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

namespace Appio.ObjectModel.CommandStrategies.ReferenceCommands
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
					Short = Constants.ReferenceCommandOptions.Client,
					Verbose = Constants.ReferenceCommandOptions.VerboseClient
				},
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.ServerName,
					Short = Constants.ReferenceCommandOptions.Server,
					Verbose = Constants.ReferenceCommandOptions.VerboseServer
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

			// check if client appioproj file exists
			_clientFullName = _fileSystem.CombinePaths(_clientName, _clientName + Constants.FileExtension.Appioproject);
			if (!_fileSystem.FileExists(_clientFullName))
			{
				AppioLogger.Warn(LoggingText.ReferenceClientAppioprojFileNotFound);
				_outputMessages.Add(string.Format(OutputText.ReferenceClientAppioprojFileNotFound, _clientFullName), string.Empty);
				return false;
			}

			// exit with success
			return true;
		}

		public virtual CommandResult Execute(IEnumerable<string> inputParams) => null;

		public virtual string GetHelpText() => string.Empty;
	}
}