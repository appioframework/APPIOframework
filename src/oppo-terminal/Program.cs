using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.HelloCommands;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.ObjectModel.CommandStrategies.VersionCommands;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            var commandFactory = CreateCommandFactory();
            var objectModel = new ObjectModel.ObjectModel(commandFactory);
            var result = objectModel.ExecuteCommand(args);
            return result == Constants.CommandResults.Success ? 0 : 1;
        }

        private static ICommandFactory<ObjectModel.ObjectModel> CreateCommandFactory()
        {
            var reflection = new ReflectionWrapper();
            var writer = new ConsoleWriter();
            var fileSystem = new FileSystemWrapper();

            var commands = new List<ICommand<ObjectModel.ObjectModel>>();

            var buildHelpStrategyHelpText = new[]
            {
                new KeyValuePair<string, string>("Arguments:", ""),
                new KeyValuePair<string, string>("<Project>", "The project directory to use"),
                new KeyValuePair<string, string>(string.Empty, string.Empty),
                new KeyValuePair<string, string>("Options:", "")       
            };

            var buildHelpStrategy = new BuildHelpStrategy(Constants.BuildCommandArguments.Help, writer, buildHelpStrategyHelpText);
            var buildHelpVerboseStrategy = new BuildHelpStrategy(Constants.BuildCommandArguments.VerboseHelp, writer, buildHelpStrategyHelpText);

            var buildStrategies = new ICommand<BuildStrategy>[]
            {
                new BuildNameStrategy(Constants.BuildCommandArguments.Name, fileSystem),
                new BuildNameStrategy(Constants.BuildCommandArguments.VerboseName, fileSystem),
                buildHelpStrategy,
                buildHelpVerboseStrategy
            };

            var buildStrategyCommandFactory = new CommandFactory<BuildStrategy>(buildStrategies, Constants.BuildCommandArguments.Help);
            buildHelpStrategy.CommandFactory = buildStrategyCommandFactory;
            buildHelpVerboseStrategy.CommandFactory = buildStrategyCommandFactory;

            commands.Add(new BuildStrategy(buildStrategyCommandFactory));
            commands.Add(new HelloStrategy(writer));

            var helpStrategy = new HelpStrategy(Constants.CommandName.Help, writer);
            commands.Add(helpStrategy);

            var shortHelpStrategy = new HelpStrategy(Constants.CommandName.ShortHelp, writer);
            commands.Add(shortHelpStrategy);

            var newStrategies = new ICommand<NewStrategy>[] 
            {
                new NewSlnCommandStrategy(fileSystem),
                new NewOpcuaAppCommandStrategy(fileSystem),
                new NewHelpCommandStrategy(Constants.NewCommandName.Help, writer),
                new NewHelpCommandStrategy(Constants.NewCommandName.VerboseHelp,writer)
            };

            var newStrategyCommandFactory = new CommandFactory<NewStrategy>(newStrategies, Constants.NewCommandName.Help);
            commands.Add(new NewStrategy(newStrategyCommandFactory));

            var publishStrategies = new ICommand<PublishStrategy>[] 
            {
                new PublishNameStrategy(Constants.PublishCommandArguments.Name, fileSystem),
                new PublishNameStrategy(Constants.PublishCommandArguments.VerboseName, fileSystem),
                new PublishHelpStrategy(Constants.PublishCommandArguments.Help, writer),
                new PublishHelpStrategy(Constants.PublishCommandArguments.VerboseHelp, writer)
            };

            var publishStrategyCommandFactory = new CommandFactory<PublishStrategy>(publishStrategies, Constants.PublishCommandArguments.Help);
            commands.Add(new PublishStrategy(publishStrategyCommandFactory));

            commands.Add(new VersionStrategy(reflection, writer));            

            var factory = new CommandFactory<ObjectModel.ObjectModel>(commands, Constants.CommandName.Help);

            helpStrategy.CommandFactory = factory;
            shortHelpStrategy.CommandFactory = factory;            

            return factory;
        }
    }
}