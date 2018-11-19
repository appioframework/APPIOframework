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
           
            if (commandResult.OutputMessages != null)
            {
                writer.Write(commandResult.OutputMessages);
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

            var buildHelpStrategyHelpText = new MessageLines
            {
                { "Arguments:", "" },
                { "<Project>", "The project directory to use" },
                { string.Empty, string.Empty },
                { "Options:", "" }       
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

            var newHelpStrategyHelpText = new MessageLines
            {                
                {"Arguments:", string.Empty },
                {"<Object>", "The object to create, can either be:" },
                {string.Empty, "sln" },
                {string.Empty, "opcuaapp" },
                {string.Empty, "" },
                {"Options:", "" },
                { "-n", "Name of the object to create"},
                {"--name", "Name of the object to create" },
                {"-h", "New help" },
                { "--help", "New help" }
            };

            var newStrategies = new ICommand<NewStrategy>[] 
            {
                new NewSlnCommandStrategy(fileSystem),
                new NewOpcuaAppCommandStrategy(fileSystem),
                new NewHelpCommandStrategy(Constants.NewCommandName.Help, newHelpStrategyHelpText),
                new NewHelpCommandStrategy(Constants.NewCommandName.VerboseHelp, newHelpStrategyHelpText)
            };

            var newStrategyCommandFactory = new CommandFactory<NewStrategy>(newStrategies, Constants.NewCommandName.Help);
            commands.Add(new NewStrategy(newStrategyCommandFactory));

            var publishHelpStrategyHelpText = new MessageLines
            {
                {"Arguments:", string.Empty },
                {"<Project>:", "The project directory to use" },
                {string.Empty, string.Empty },
                {"Options:", string.Empty },
                {"-n:", "Project name" },
                {"--name:", "Project name" },
                {"-h:", "Publish help" },
                { "--help:", "Publish help" }
            };

            var publishStrategies = new ICommand<PublishStrategy>[] 
            {
                new PublishNameStrategy(Constants.PublishCommandArguments.Name, fileSystem),
                new PublishNameStrategy(Constants.PublishCommandArguments.VerboseName, fileSystem),
                new PublishHelpStrategy(Constants.PublishCommandArguments.Help, publishHelpStrategyHelpText),
                new PublishHelpStrategy(Constants.PublishCommandArguments.VerboseHelp, publishHelpStrategyHelpText)
            };

            var publishStrategyCommandFactory = new CommandFactory<PublishStrategy>(publishStrategies, Constants.PublishCommandArguments.Help);
            commands.Add(new PublishStrategy(publishStrategyCommandFactory));

            commands.Add(new VersionStrategy(reflection));

            var cleanHelpStrategyHelpText = new MessageLines
            {
                {"Arguments:", "" },
                {"<Project>", "The project directory to use" },
                {string.Empty, string.Empty },
                { "Options:", "" }
            };

            var cleanHelpStrategy = new CleanHelpStrategy(Constants.CleanCommandArguments.Help, cleanHelpStrategyHelpText);
            var cleanHelpVerboseStrategy = new CleanHelpStrategy(Constants.CleanCommandArguments.VerboseHelp, cleanHelpStrategyHelpText);

            var cleanStrategies = new ICommand<CleanStrategy>[]
            {
                new CleanNameStrategy(Constants.CleanCommandArguments.Name, fileSystem),
                new CleanNameStrategy(Constants.CleanCommandArguments.VerboseName, fileSystem),
                cleanHelpStrategy,
                cleanHelpVerboseStrategy,
            };
            var cleanStrategyCommandFactory = new CommandFactory<CleanStrategy>(cleanStrategies, Constants.CleanCommandArguments.Help);
            cleanHelpStrategy.CommandFactory = cleanStrategyCommandFactory;
            cleanHelpVerboseStrategy.CommandFactory = cleanStrategyCommandFactory;

            commands.Add(new CleanStrategy(cleanStrategyCommandFactory));

            var factory = new CommandFactory<ObjectModel.ObjectModel>(commands, Constants.CommandName.Help);

            helpStrategy.CommandFactory = factory;
            shortHelpStrategy.CommandFactory = factory;            

            return factory;
        }
    }
}