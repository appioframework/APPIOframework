namespace Oppo.ObjectModel
{
	public static class RefUtility
	{
		public struct ResultMessages
		{
			public string LoggerMessage { get; set; }
			public string OutputMessage { get; set; }
		};

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