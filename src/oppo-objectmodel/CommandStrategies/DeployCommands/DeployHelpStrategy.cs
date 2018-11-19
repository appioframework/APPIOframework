using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.DeployCommands
{
    public class DeployHelpStrategy : ICommand<DeployStrategy>
    {
        private readonly MessageLines _helpText;

        public DeployHelpStrategy(string deployHelpCommandName, MessageLines helpText)
        {
            _helpText = helpText;
            Name = deployHelpCommandName;
        }

        public string Name { get; private set; }

        public ICommandFactory<DeployStrategy> CommandFactory { get; set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines(_helpText);
           
            foreach (var command in CommandFactory.Commands)
            {
                outputMessages.Add(command.Name, command.GetHelpText());
            }

            OppoLogger.Info(LoggingText.OppoHelpForDeployCommandCalled);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.DeployHelpArgumentCommandDescription;
        }
    }
}