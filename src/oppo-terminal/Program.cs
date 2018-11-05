using System.Diagnostics.CodeAnalysis;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    static class Program
    {
        static int Main(string[] args)
        {
            var commandFactory = new CommandStrategyFactory();
            var objectModel = new ObjectModel.ObjectModel(commandFactory);
            var writer = new ConsoleWriter();
            var terminal = new OppoTerminal(objectModel, writer);

            var result = Constants.CommandResults.Success;
            if (args?.Length > 0)
            {
                result = terminal.Execute(args);
            }

            return result == Constants.CommandResults.Success ? 0 : 1;
        }
    }
}