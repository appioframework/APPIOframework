/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel.CommandStrategies.DeployCommands
{
    public class DeployStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<DeployStrategy> _factory;

        public DeployStrategy(ICommandFactory<DeployStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Deploy;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.DeployCommand;
        }
    }
}