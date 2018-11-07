using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishStrategy : ICommandStrategy
    {
        public string Execute(IEnumerable<string> inputsParams)
        {
            var firstInputParam = inputsParams.FirstOrDefault();

            if (firstInputParam == Constants.PublishCommandArguments.ModeAll)
            {
                // command logic here..

                return Constants.CommandResults.Success;
            }

            return Constants.CommandResults.Failure;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpText.PublishCommand;
        }
    }
}
