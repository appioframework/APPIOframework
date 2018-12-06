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

            var buildHelpStrategyData = new HelpData
            {
                CommandName = Constants.BuildCommandArguments.Help,
                HelpTextFirstLine = buildHelpStrategyHelpText,
                LogMessage = LoggingText.OppoHelpForBuildCommandCalled,
                HelpText = Resources.text.help.HelpTextValues.BuildHelpArgumentCommandDescription,
            };

            var buildHelpStrategy = new HelpStrategy<BuildStrategy>(buildHelpStrategyData);

            buildHelpStrategyData.CommandName = Constants.BuildCommandArguments.VerboseHelp;

            var buildHelpVerboseStrategy = new HelpStrategy<BuildStrategy>(buildHelpStrategyData);

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

            var helpStrategyData = new HelpData
            {
                CommandName       = Constants.CommandName.Help,
                HelpTextFirstLine = { { Resources.text.help.HelpTextValues.HelpStartCommand, "" } },
                HelpTextLastLine  = { { "", Resources.text.help.HelpTextValues.HelpEndCommand } },
                LogMessage        = LoggingText.OppoHelpCalled,
                HelpText          = Resources.text.help.HelpTextValues.HelpCommand,
            };

            var helpStrategy = new HelpStrategy<ObjectModel>(helpStrategyData);
            commands.Add(helpStrategy);

            helpStrategyData.CommandName = Constants.CommandName.ShortHelp;

            var shortHelpStrategy = new HelpStrategy<ObjectModel>(helpStrategyData);
            commands.Add(shortHelpStrategy);

            helpStrategyData.CommandName = Constants.CommandName.HelpDash;

            var helpDashStrategy = new HelpStrategy<ObjectModel>(helpStrategyData);
            commands.Add(helpDashStrategy);

            helpStrategyData.CommandName = Constants.CommandName.HelpDashVerbose;

            var helpDashVerboseStrategy = new HelpStrategy<ObjectModel>(helpStrategyData);
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

            var newHelpStrategyData = new HelpData
            {
                CommandName = Constants.NewCommandName.Help,
                HelpTextFirstLine = newHelpStrategyHelpText,
                LogMessage = LoggingText.OppoHelpForNewCommandCalled,
                HelpText = Resources.text.help.HelpTextValues.NewHelpArgumentCommandDescription,
            };

            var newHelpStrategy = new HelpStrategy<NewStrategy>(newHelpStrategyData);

            newHelpStrategyData.CommandName = Constants.NewCommandName.VerboseHelp;

            var newHelpVerboseStrategy = new HelpStrategy<NewStrategy>(newHelpStrategyData);

            var newStrategies = new ICommand<NewStrategy>[]
            {
                new NewSlnCommandStrategy(fileSystem),
                new NewOpcuaAppCommandStrategy(fileSystem),
                newHelpStrategy,
                newHelpVerboseStrategy,
            };

            var newStrategyCommandFactory = new CommandFactory<NewStrategy>(newStrategies, Constants.NewCommandName.Help);
            newHelpStrategy.CommandFactory = newStrategyCommandFactory;
            newHelpVerboseStrategy.CommandFactory = newStrategyCommandFactory;

            commands.Add(new NewStrategy(newStrategyCommandFactory));

            var publishHelpStrategyHelpText = new MessageLines
            {
                {"Arguments:", string.Empty },
                {"<Project>:", "The project directory to use" },
                {string.Empty, string.Empty },
                {"Options:", string.Empty },
            };

            var publishHelpStrategyData = new HelpData
            {
                CommandName = Constants.PublishCommandArguments.Help,
                HelpTextFirstLine = publishHelpStrategyHelpText,
                LogMessage = LoggingText.OpcuaappPublishHelpCalled,
                HelpText = Resources.text.help.HelpTextValues.PublishHelpArgumentCommandDescription,
            };

            var publishHelpStrategy = new HelpStrategy<PublishStrategy>(publishHelpStrategyData);

            publishHelpStrategyData.CommandName = Constants.PublishCommandArguments.VerboseHelp;

            var publishHelpVerboseStrategy = new HelpStrategy<PublishStrategy>(publishHelpStrategyData);

            var publishStrategies = new ICommand<PublishStrategy>[]
            {
                new PublishNameStrategy(Constants.PublishCommandArguments.Name, fileSystem),
                new PublishNameStrategy(Constants.PublishCommandArguments.VerboseName, fileSystem),
                publishHelpStrategy,
                publishHelpVerboseStrategy,
            };

            var publishStrategyCommandFactory = new CommandFactory<PublishStrategy>(publishStrategies, Constants.PublishCommandArguments.Help);
            publishHelpStrategy.CommandFactory = publishStrategyCommandFactory;
            publishHelpVerboseStrategy.CommandFactory = publishStrategyCommandFactory;

            commands.Add(new PublishStrategy(publishStrategyCommandFactory));

            var deployHelpStrategyHelpText = new MessageLines
            {
                {"Arguments:", string.Empty },
                {"<Project>:", "The project directory to deploy" },
                {string.Empty, string.Empty },
                {"Options:", string.Empty },
            };

            var deployHelpStrategyData = new HelpData
            {
                CommandName = Constants.DeployCommandArguments.Help,
                HelpTextFirstLine = deployHelpStrategyHelpText,
                LogMessage = LoggingText.OppoHelpForDeployCommandCalled,
                HelpText = Resources.text.help.HelpTextValues.DeployHelpArgumentCommandDescription,
            };

            var deployHelpStrategy = new HelpStrategy<DeployStrategy>(deployHelpStrategyData);

            deployHelpStrategyData.CommandName = Constants.CleanCommandArguments.VerboseHelp;

            var deployVerboseHelpStrategy = new HelpStrategy<DeployStrategy>(deployHelpStrategyData);

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

            var cleanHelpStrategyData = new HelpData
            {
                CommandName = Constants.CleanCommandArguments.Help,
                HelpTextFirstLine = cleanHelpStrategyHelpText,
                LogMessage = LoggingText.OppoHelpForCleanCommandCalled,
                HelpText = Resources.text.help.HelpTextValues.CleanHelpArgumentCommandDescription,
            };

            var cleanHelpStrategy = new HelpStrategy<CleanStrategy>(cleanHelpStrategyData);

            cleanHelpStrategyData.CommandName = Constants.CleanCommandArguments.VerboseHelp;

            var cleanHelpVerboseStrategy = new HelpStrategy<CleanStrategy>(cleanHelpStrategyData);

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

            var importHelpStrategyData = new HelpData
            {
                CommandName = Constants.ImportInformationModelCommandArguments.Help,
                HelpTextFirstLine = importHelpStrategyHelpText,
                LogMessage = LoggingText.OppoHelpForImportInformationModel,
                HelpText = Resources.text.help.HelpTextValues.ImportHelpArgumentCommandDescription,
            };

            var importHelpStrategy = new HelpStrategy<ImportStrategy>(importHelpStrategyData);

            importHelpStrategyData.CommandName = Constants.ImportInformationModelCommandArguments.VerboseHelp;

            var importHelpStrategyVerbose = new HelpStrategy<ImportStrategy>(importHelpStrategyData);
            var importCommands = new ICommand<ImportStrategy>[]
            {
                new ImportInformationModelCommandStrategy(fileSystem),
                new ImportInformationModelSamplesStrategy(fileSystem, Constants.ImportInformationModelCommandArguments.Sample),
                new ImportInformationModelSamplesStrategy(fileSystem, Constants.ImportInformationModelCommandArguments.VerboseSample),
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