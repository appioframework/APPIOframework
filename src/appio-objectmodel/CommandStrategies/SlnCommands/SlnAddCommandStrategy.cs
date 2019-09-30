/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Appio.Resources.text.logging;
using Appio.Resources.text.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Appio.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnAddCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;
        private readonly ParameterResolver<ParamId> _resolver;
        
        private enum ParamId {SolutionName, ProjectName}

        public SlnAddCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _resolver = new ParameterResolver<ParamId>(Constants.CommandName.Sln + " " + Name, new []
            {
	            new StringParameterSpecification<ParamId>
	            {
		            Identifier = ParamId.SolutionName,
		            Short = Constants.SlnCommandOptions.Solution,
		            Verbose = Constants.SlnCommandOptions.VerboseSolution
	            },
	            new StringParameterSpecification<ParamId>
	            {
		            Identifier = ParamId.ProjectName,
		            Short = Constants.SlnCommandOptions.Project,
		            Verbose = Constants.SlnCommandOptions.VerboseProject
	            }
            });
        }

        public string Name => Constants.SlnCommandArguments.Add;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();
			var validationMessages = new SlnUtility.ResultMessages();

			var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
			if (error != null)
				return new CommandResult(false, new MessageLines{{error, string.Empty}});

			var solutionName = stringParams[ParamId.SolutionName];
			var projectName = stringParams[ParamId.ProjectName];
			
			// validate solution name
			if(!SlnUtility.ValidateSolution(ref validationMessages, solutionName, _fileSystem))
			{
				AppioLogger.Warn(validationMessages.LoggerMessage);
				outputMessages.Add(validationMessages.OutputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

            // check if *.appioproj file exists
            var appioprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject);
            if (string.IsNullOrEmpty(projectName) || !_fileSystem.FileExists(appioprojFilePath))
            {
                AppioLogger.Warn(LoggingText.SlnAddAppioprojFileNotFound);
                outputMessages.Add(string.Format(OutputText.SlnAddOpcuaappNotFound, appioprojFilePath), string.Empty);
                return new CommandResult(false, outputMessages);
            }
			
			// deserialize *.appiosln file
			var solutionFullName = solutionName + Constants.FileExtension.Appiosln;
			Solution appioSolution = SlnUtility.DeserializeFile<Solution>(solutionFullName, _fileSystem);
			if (appioSolution == null)
			{
				AppioLogger.Warn(LoggingText.SlnCouldntDeserliazeSln);
				outputMessages.Add(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// deserialize *.appioproj file
			OpcuaappReference appioproj = SlnUtility.DeserializeFile<OpcuaappReference>(appioprojFilePath, _fileSystem);
            if (appioproj == null)
            {
                AppioLogger.Warn(LoggingText.SlnAddCouldntDeserliazeOpcuaapp);
                outputMessages.Add(string.Format(OutputText.SlnAddCouldntDeserliazeOpcuaapp, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
			}
			
            // check if sln does not contain opcuaapp yet
            if (!appioSolution.Projects.Any(x => x.Name == appioproj.Name))
            {
				// add opcuaapp to sln
				appioproj.Path = appioprojFilePath;
				appioSolution.Projects.Add(appioproj);

				// serialize and write sln
				var slnNewContent = JsonConvert.SerializeObject(appioSolution, Formatting.Indented);
				_fileSystem.WriteFile(solutionFullName, new List<string> { slnNewContent });
            }
            else
            {
                AppioLogger.Info(LoggingText.SlnAddContainsOpcuaapp);
                outputMessages.Add(string.Format(OutputText.SlnAddContainsOpcuaapp, solutionName, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }
			
			// exit method with success
            AppioLogger.Info(LoggingText.SlnAddSuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnAddSuccess, projectName, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnAddNameArgumentCommandDescription;
        }
    }
}