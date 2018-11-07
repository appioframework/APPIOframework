using System.Diagnostics.CodeAnalysis;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            var writer = new ConsoleWriter();
            var commandFactory = new CommandStrategyFactory(writer);
            var objectModel = new ObjectModel.ObjectModel(commandFactory);
            var terminal = new OppoTerminal(objectModel);

            var result = terminal.Execute(args);
            return result == Constants.CommandResults.Success ? 0 : 1;
        }
    }
}