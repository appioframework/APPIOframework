/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

namespace Appio.ObjectModel.CommandStrategies.HelpCommands
{
	public class HelpData
	{
		public string CommandName { get; set; }
		public MessageLines HelpTextFirstLine { get; set; }
		public MessageLines Arguments { get; set; }
		public MessageLines Options { get; set; }
		public MessageLines HelpTextLastLine { get; set; }
		public string LogMessage { get; set; }
		public string HelpText { get; set; }

        public HelpData()
        {
            HelpTextFirstLine = new MessageLines();
			Arguments = new MessageLines();
			Options = new MessageLines();
            HelpTextLastLine  = new MessageLines();
        }

        public HelpData Clone()
        {
			return new HelpData
			{
				CommandName       = CommandName,
				HelpTextFirstLine = new MessageLines(HelpTextFirstLine),
				Arguments         = new MessageLines(Arguments),
				Options			  = new MessageLines(Options),
                HelpTextLastLine  = new MessageLines(HelpTextLastLine),
                LogMessage        = LogMessage,
                HelpText          = HelpText,
            };
        }
    }
}
