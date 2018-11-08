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
            var reflection = new ReflectionWrapper();
            var writer = new ConsoleWriter();
            var commandFactory = new CommandStrategyFactory(reflection, writer);
            var objectModel = new ObjectModel.ObjectModel(commandFactory);
            var result = objectModel.ExecuteCommand(args);
            return result == Constants.CommandResults.Success ? 0 : 1;
        }
    }
}