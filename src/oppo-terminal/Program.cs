using System.Diagnostics.CodeAnalysis;
using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            args = new string[] { "new","opcuaapp","-n","MyProj" };
            SetupOppoLogger();


            var objectModel = new ObjectModel.ObjectModel(ObjectModel.ObjectModel.CreateCommandFactory(new ReflectionWrapper()));

            Constants.CommandResults.Failure = objectModel.PrepareCommandFailureOutputText(args);

            var writer = new ConsoleWriter();

            var commandResult = objectModel.ExecuteCommand(args);

            if (commandResult.OutputMessages != null)
            {
                writer.Write(commandResult.OutputMessages);
            }
            System.Console.ReadKey();
            var commandResultSucces = commandResult.Sucsess;
            return commandResultSucces ? 0 : 1;
        }

        private static void SetupOppoLogger()
        {
            // setups the logger
            OppoLogger.RegisterListener(new LoggerListenerWrapper());
        }
    }
}