using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.CleanCommands;
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
            SetupOppoLogger();
            var objectModel = new ObjectModel.ObjectModel(commandFactory);

            var writer = new ConsoleWriter();

            var commandResult = objectModel.ExecuteCommand(args);
            writer.WriteLine(commandResult.Message);

            if (commandResult.OutputText != null)
            {
                writer.WriteLines(commandResult.OutputText);
            }

            var commandResultSucces = commandResult.Sucsess;
            return commandResultSucces ? 0 : 1;
        }

        private static void SetupOppoLogger()
        {
            // setups the logger
            OppoLogger.RegisterListener(new LoggerListenerWrapper());
        }

        private static ICommandFactory<ObjectModel.ObjectModel> CreateCommandFactory()
        {
            var reflection = new ReflectionWrapper();
            var fileSystem = new FileSystemWrapper();

            var commands = new List<ICommand<ObjectModel.ObjectModel>>();

            var buildHelpStrategyHelpText = new[]
            {
                new KeyValuePair<string, string>("Arguments:", ""),
                new KeyValuePair<string, string>("<Project>", "The project directory to use"),
                new KeyValuePair<string, string>(string.Empty, string.Empty),
                new KeyValuePair<string, string>("Options:", "")       
            };

            var buildHelpStrategy = new BuildHelpStrategy(Constants.BuildCommandArguments.Help, buildHelpStrategyHelpText);
            var buildHelpVerboseStrategy = new BuildHelpStrategy(Constants.BuildCommandArguments.VerboseHelp, buildHelpStrategyHelpText);

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
            commands.Add(new HelloStrategy());

            var helpStrategy = new HelpStrategy(Constants.CommandName.Help);
            commands.Add(helpStrategy);

            var shortHelpStrategy = new HelpStrategy(Constants.CommandName.ShortHelp);
            commands.Add(shortHelpStrategy);

            var newStrategies = new ICommand<NewStrategy>[] 
            {
                new NewSlnCommandStrategy(fileSystem),
                new NewOpcuaAppCommandStrategy(fileSystem),
                new NewHelpCommandStrategy(Constants.NewCommandName.Help),
                new NewHelpCommandStrategy(Constants.NewCommandName.VerboseHelp)
            };

            var newStrategyCommandFactory = new CommandFactory<NewStrategy>(newStrategies, Constants.NewCommandName.Help);
            commands.Add(new NewStrategy(newStrategyCommandFactory));

            var publishStrategies = new ICommand<PublishStrategy>[] 
            {
                new PublishNameStrategy(Constants.PublishCommandArguments.Name, fileSystem),
                new PublishNameStrategy(Constants.PublishCommandArguments.VerboseName, fileSystem),
                new PublishHelpStrategy(Constants.PublishCommandArguments.Help),
                new PublishHelpStrategy(Constants.PublishCommandArguments.VerboseHelp)
            };

            var publishStrategyCommandFactory = new CommandFactory<PublishStrategy>(publishStrategies, Constants.PublishCommandArguments.Help);
            commands.Add(new PublishStrategy(publishStrategyCommandFactory));

            commands.Add(new VersionStrategy(reflection));            

            commands.Add(new CleanStrategy(fileSystem));

            var factory = new CommandFactory<ObjectModel.ObjectModel>(commands, Constants.CommandName.Help);

            helpStrategy.CommandFactory = factory;
            shortHelpStrategy.CommandFactory = factory;            

            return factory;
        }
    }
}