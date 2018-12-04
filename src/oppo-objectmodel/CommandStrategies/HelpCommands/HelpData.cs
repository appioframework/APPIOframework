namespace Oppo.ObjectModel.CommandStrategies.HelpCommands
{
    public class HelpData
    {
        public string CommandName { get; set; }
        public MessageLines HelpTextFirstLine { get; set; }
        public MessageLines HelpTextLastLine { get; set; }
        public string LogMessage { get; set; }
        public string HelpText { get; set; }

        public HelpData()
        {
            HelpTextFirstLine = new MessageLines();
            HelpTextLastLine  = new MessageLines();
        }

        public HelpData Clone()
        {
            return new HelpData
            {
                CommandName       = CommandName,
                HelpTextFirstLine = new MessageLines(HelpTextFirstLine),
                HelpTextLastLine  = new MessageLines(HelpTextLastLine),
                LogMessage        = LogMessage,
                HelpText          = HelpText,
            };
        }
    }
}
