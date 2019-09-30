/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Collections.Generic;
using Appio.ObjectModel.Extensions;
using Appio.Resources.text.logging;

namespace Appio.ObjectModel.CommandStrategies.VersionCommands
{
    public class VersionStrategy : ICommand<ObjectModel>
    {
        private readonly IReflection _reflection;

        public VersionStrategy(IReflection reflection)
        {
            _reflection = reflection;
        }

        public string Name => Constants.CommandName.Version;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();

            foreach (var info in _reflection.GetAppioAssemblyInfos())
            {
                outputMessages.Add(info.AssemblyName, string.Format(Resources.text.version.VersionText.AssemblyVersionInfo, info.AssemblyVersion.ToPrintableString()));
            }

            AppioLogger.Info(LoggingText.VersionCommandCalled);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.VersionCommand;
        }
    }
}