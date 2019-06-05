using System.Diagnostics.CodeAnalysis;
using Appio.ObjectModel;

namespace Appio.Terminal
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            SetupAppioLogger();
			
            var objectModel = new ObjectModel.ObjectModel(ObjectModel.ObjectModel.CreateCommandFactory(new ReflectionWrapper()));

            Constants.CommandResults.Failure = objectModel.PrepareCommandFailureOutputText(args);

            var writer = new ConsoleWriter();

            var commandResult = objectModel.ExecuteCommand(args);

            if (commandResult.OutputMessages != null)
            {
                writer.Write(commandResult.OutputMessages);
            }

            var commandResultSucces = commandResult.Success;
            return commandResultSucces ? 0 : 1;
        }

        private static void SetupAppioLogger()
        {
            // setups the logger
            AppioLogger.RegisterListener(new LoggerListenerWrapper());
        }
    }
}