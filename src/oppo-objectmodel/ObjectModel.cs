using System.Collections.Generic;
using System;
using System.Linq;
using Oppo.Resources.text.logging;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.CleanCommands;
using Oppo.ObjectModel.CommandStrategies.DeployCommands;
using Oppo.ObjectModel.CommandStrategies.HelloCommands;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.ObjectModel.CommandStrategies.VersionCommands;

namespace Oppo.ObjectModel
{
    public class ObjectModel : IObjectModel
    {      
        private readonly ICommandFactory<ObjectModel> _commandStrategyFactory;        

        public ObjectModel(ICommandFactory<ObjectModel> commandFactory)
        {
            _commandStrategyFactory = commandFactory;
        }
        
        public CommandResult ExecuteCommand(IEnumerable<string> inputParams)
        {
            if (inputParams == null)
            {
                try
                {
                    throw new ArgumentNullException(nameof(inputParams));                    
                }
                catch (Exception ex)
                {
                    OppoLogger.Error(LoggingText.NullInputParams_Msg, ex);
                    throw;
                }                
            }

            var inputParamsArray = inputParams.ToArray();
            var strategy = _commandStrategyFactory.GetCommand(inputParamsArray.FirstOrDefault());
            return strategy.Execute(inputParamsArray.Skip(1));
        }

        public static ICommandFactory<ObjectModel> CreateCommandFactory(IReflection reflectionWrapper)
        {
            var reflection = reflectionWrapper;
            var fileSystem = new FileSystemWrapper();

            var commands = new List<ICommand<ObjectModel>>();

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

            var helpDashStrategy = new HelpStrategy(Constants.CommandName.HelpDash);
            commands.Add(helpDashStrategy);

            var helpDashVerboseStrategy = new HelpStrategy(Constants.CommandName.HelpDashVerbose);
            commands.Add(helpDashVerboseStrategy);

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

            var deployHelpStrategyHelpText = new MessageLines
            {
                {"Arguments:", string.Empty },
                {"<Project>:", "The project directory to deploy" },
                {string.Empty, string.Empty },
                {"Options:", string.Empty },
            };

            var deployHelpStrategy = new DeployHelpStrategy(Constants.DeployCommandArguments.Help, deployHelpStrategyHelpText);
            var deployVerboseHelpStrategy = new DeployHelpStrategy(Constants.DeployCommandArguments.VerboseHelp, deployHelpStrategyHelpText);
            var deployStrategies = new ICommand<DeployStrategy>[]
            {
                new DeployNameStrategy(Constants.DeployCommandArguments.Name, fileSystem),
                new DeployNameStrategy(Constants.DeployCommandArguments.VerboseName, fileSystem),
                deployHelpStrategy,
                deployVerboseHelpStrategy
            };

            var deployStrategyCommandFactory = new CommandFactory<DeployStrategy>(deployStrategies, Constants.DeployCommandArguments.Help);
            deployHelpStrategy.CommandFactory = deployStrategyCommandFactory;
            deployVerboseHelpStrategy.CommandFactory = deployStrategyCommandFactory;
            commands.Add(new DeployStrategy(deployStrategyCommandFactory));

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

            var importHelpStrategyHelpText = new MessageLines
            {
                {"Arguments:", "" },
                {"information-model", "OPC UA conform information model" },
                {"<opcuaapp name>", "The name to opcuapp to use" },
                {string.Empty, string.Empty },
                { "Options:", "" },
                { "-p","Information models path to use"},
                { "--path","Information models path to use"}
            };

            var importHelpStrategy = new ImportHelpStrategy(Constants.ImportInformationModelCommandArguments.Help, importHelpStrategyHelpText);
            var importHelpStrategyVerbose = new ImportHelpStrategy(Constants.ImportInformationModelCommandArguments.VerboseHelp, importHelpStrategyHelpText);
            var importCommands = new ICommand<ImportStrategy>[]
            {
                new ImportInformationModelCommandStrategy(fileSystem),
                importHelpStrategy,
                importHelpStrategyVerbose
            };

            var importStrategyCommandFactory = new CommandFactory<ImportStrategy>(importCommands, Constants.ImportInformationModelCommandArguments.Help);
            importHelpStrategy.CommandFactory = importStrategyCommandFactory;
            importHelpStrategyVerbose.CommandFactory = importStrategyCommandFactory;
            commands.Add(new ImportStrategy(importStrategyCommandFactory));

            var factory = new CommandFactory<ObjectModel>(commands, Constants.CommandName.Help);

            helpStrategy.CommandFactory = factory;
            shortHelpStrategy.CommandFactory = factory;
            helpDashStrategy.CommandFactory = factory;
            helpDashVerboseStrategy.CommandFactory = factory;

            return factory;
        }
    }
}