/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

namespace Appio.ObjectModel
{
	public static class RefUtility
	{
		static public void DeserializeClient(ref OpcuaClientApp opcuaClient, ref OpcuaClientServerApp opcuaClientServer, string clientFullName, IFileSystem fileSystem)
		{
			opcuaClient = SlnUtility.DeserializeFile<OpcuaClientApp>(clientFullName, fileSystem);
			if (opcuaClient != null && opcuaClient.Type == Constants.ApplicationType.ClientServer)
			{
				opcuaClientServer = SlnUtility.DeserializeFile<OpcuaClientServerApp>(clientFullName, fileSystem);
			}
		}
	}
}