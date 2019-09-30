/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

namespace Appio.ObjectModel.CommandStrategies.SlnCommands
{
	public struct SlnOperationData
	{
		public string CommandName { get; set; }
		public IFileSystem FileSystem { get; set; }
		public ICommand Subcommand { get; set; }
		public string SuccessLoggerMessage { get; set; }
		public string SuccessOutputMessage { get; set; }
		public string HelpText { get; set; }
	}
}