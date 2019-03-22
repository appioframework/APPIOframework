namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
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