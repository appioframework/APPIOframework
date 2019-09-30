/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Collections.Generic;

namespace Appio.ObjectModel.CommandStrategies.HelloCommands
{
    public class HelloStrategy : ICommand<ObjectModel>
    {
        public string Name => Constants.CommandName.Hello;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();
            outputMessages.Add(Constants.HelloString, string.Empty);            
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.HelloCommand;
        }
    }
}