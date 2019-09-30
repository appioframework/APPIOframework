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

namespace Appio.ObjectModel.CommandStrategies.NewCommands
{
    public class NewSlnCommandStrategy : ICommand<NewStrategy>
    {
        private readonly IFileSystem _fileSystem;
        private readonly ParameterResolver<ParamId> _resolver;
        
        private enum ParamId {SolutionName}

        public NewSlnCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _resolver = new ParameterResolver<ParamId>(Constants.CommandName.New + " " + Name, new []
            {
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.SolutionName,
                    Short = Constants.NewCommandOptions.Name,
                    Verbose = Constants.NewCommandOptions.VerboseName
                }
            });
        }

        public string Name => Constants.NewCommandArguments.Sln;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();

            var (error, stringParams, _) = _resolver.ResolveParams(inputParams);

            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var solutionName = stringParams[ParamId.SolutionName];
            
            if (_fileSystem.GetInvalidFileNameChars().Any(solutionName.Contains))
            {
                AppioLogger.Warn(LoggingText.InvalidSolutionName);
                outputMessages.Add(string.Format(OutputText.NewSlnCommandFailure, solutionName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var solutionFilePath = $"{solutionName}{Constants.FileExtension.Appiosln}";
            _fileSystem.CreateFile(solutionFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.AppioSlnTemplateFileName));
            AppioLogger.Info(string.Format(LoggingText.NewSlnCommandSuccess, solutionFilePath));
            outputMessages.Add(string.Format(OutputText.NewSlnCommandSuccess, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}