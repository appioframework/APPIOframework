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

namespace Appio.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnRemoveCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;
        private readonly ParameterResolver<ParamId> _resolver;
        
        private enum ParamId {SolutionName, ProjectName}

        public SlnRemoveCommandStrategy(IFileSystem fileSystem)
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

        public string Name => Constants.SlnCommandArguments.Remove;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();

            var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var solutionName = stringParams[ParamId.SolutionName];
            var projectName = stringParams[ParamId.ProjectName];
            
            // check if solution file is existing
            var solutionFullName = _fileSystem.CombinePaths(solutionName + Constants.FileExtension.Appiosln);
            if (string.IsNullOrEmpty(solutionName) || !_fileSystem.FileExists(solutionFullName))
            {
                AppioLogger.Warn(LoggingText.SlnAppioslnFileNotFound);
                outputMessages.Add(string.Format(OutputText.SlnAppioslnNotFound, solutionFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

			// deserialise solution file
			Solution appioSolution = SlnUtility.DeserializeFile<Solution>(solutionFullName, _fileSystem);
			if (appioSolution == null)
			{
				AppioLogger.Warn(LoggingText.SlnCouldntDeserliazeSln);
				outputMessages.Add(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if the project to remove is part of the solution
			var appioproj = appioSolution.Projects.SingleOrDefault(x => x.Name == projectName);
            if (appioproj != null)
            {
                // remove opcuaapp from sln
                appioSolution.Projects.Remove(appioproj);

                // serialize and write sln
                var slnNewContent = JsonConvert.SerializeObject(appioSolution, Formatting.Indented);
                _fileSystem.WriteFile(solutionFullName, new List<string> { slnNewContent });
            }
            else
            {
                AppioLogger.Warn(LoggingText.SlnRemoveOpcuaappIsNotInSln);
                outputMessages.Add(string.Format(OutputText.SlnRemoveOpcuaappIsNotInSln, projectName, solutionName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

   

            // exit method with success
            AppioLogger.Info(LoggingText.SlnRemoveSuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnRemoveSuccess, projectName, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnRemoveNameArgumentCommandDescription;
        }
        
    }
}