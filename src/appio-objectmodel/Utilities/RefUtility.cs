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