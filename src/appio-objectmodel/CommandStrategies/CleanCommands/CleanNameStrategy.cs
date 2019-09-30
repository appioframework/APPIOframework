/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanNameStrategy : ICommand<CleanStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public CleanNameStrategy(string name, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = name;
        }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var projectName = inputParamsArray.ElementAtOrDefault(0);
            var outputMessages = new MessageLines();

            if (string.IsNullOrEmpty(projectName) || !_fileSystem.DirectoryExists(projectName))
            {
                AppioLogger.Info(Resources.text.logging.LoggingText.CleanFailure);
                outputMessages.Add(Resources.text.output.OutputText.OpcuaappCleanFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            _fileSystem.DeleteDirectory(buildDirectory);
            AppioLogger.Info(Resources.text.logging.LoggingText.CleanSuccess);                        
            outputMessages.Add(string.Format(Resources.text.output.OutputText.OpcuaappCleanSuccess, projectName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanNameArgumentCommandDescription;
        }
    }
}