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
using Oppo.ObjectModel.CommandStrategies.GenerateCommands;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;

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
            
            var buildStrategy = CreateBuildStrategy(fileSystem);
            commands.Add(buildStrategy);
            commands.Add(new HelloStrategy());

            var helpStrategy = new HelpStrategy<ObjectModel>(CreateHelpData(Constants.CommandName.Help));
            commands.Add(helpStrategy);

            var shortHelpStrategy = new HelpStrategy<ObjectModel>(CreateHelpData(Constants.CommandName.ShortHelp));
            commands.Add(shortHelpStrategy);

            var helpDashStrategy = new HelpStrategy<ObjectModel>(CreateHelpData(Constants.CommandName.HelpDash));
            commands.Add(helpDashStrategy);

            var helpDashVerboseStrategy = new HelpStrategy<ObjectModel>(CreateHelpData(Constants.CommandName.HelpDashVerbose));
            commands.Add(helpDashVerboseStrategy);

            var newStrategy = CreateNewStrategy(fileSystem);
            commands.Add(newStrategy);

            var publishStrategy = CreatePublishStrategy(fileSystem);
            commands.Add(publishStrategy);

            var deployStrategy = CreateDeployStrategy(fileSystem);
            commands.Add(deployStrategy);

            commands.Add(new VersionStrategy(reflection));

            var cleanStrategy = CreateCleanStrategy(fileSystem);
            commands.Add(cleanStrategy);

            var importStrategy = CreateImportStrategy(fileSystem);
            commands.Add(importStrategy);

            var generateStrategy = CreateGenerateStrategy(fileSystem);
            commands.Add(generateStrategy);

			var slnStrategy = CreateSlnStrategy(fileSystem);
			commands.Add(slnStrategy);

            var factory = new CommandFactory<ObjectModel>(commands, Constants.CommandName.Help);

            helpStrategy.CommandFactory = factory;
            shortHelpStrategy.CommandFactory = factory;
            helpDashStrategy.CommandFactory = factory;
            helpDashVerboseStrategy.CommandFactory = factory;

            return factory;
        }

        private static HelpData CreateHelpData(string commandName)
        {
            // generic help data
            var helpStrategyFirstLineText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartWelcome },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartDocuLink },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartUsageDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartTerminology },
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartOpcuaapp },
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartOpcuaappDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartOppoOptions },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.HelpStartCommand },
            };

            var helpStrategyLastLineText = new MessageLines
            {
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.HelpEndCommand },
            };

            return new HelpData
            {
                CommandName = commandName,
                HelpTextFirstLine = helpStrategyFirstLineText,
                HelpTextLastLine = helpStrategyLastLineText,
                LogMessage = LoggingText.OppoHelpCalled,
                HelpText = Resources.text.help.HelpTextValues.HelpCommand,
            };
        }

        private static BuildStrategy CreateBuildStrategy(IFileSystem fileSystem)
        {
            // oppo build <command>
            var buildHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.BuildFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.BuildCallDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
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

            return new BuildStrategy(buildStrategyCommandFactory);
        }

        private static NewStrategy CreateNewStrategy(IFileSystem fileSystem)
        {
            // oppo new <command>
            var newHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.NewFirstLine },
                { string.Empty, string.Empty},
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.NewCallDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralArguments },
                { string.Empty, Resources.text.help.HelpTextValues.NewArgumentsObject },
                { string.Empty, Resources.text.help.HelpTextValues.NewArgumentsSln },
                { string.Empty, Resources.text.help.HelpTextValues.NewArgumentsOpcuaapp },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
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

            return new NewStrategy(newStrategyCommandFactory);
        }

        private static PublishStrategy CreatePublishStrategy(IFileSystem fileSystem)
        {
            // oppo publish <command>
            var publishHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.PublishFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.PublishCallDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
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
            
            return new PublishStrategy(publishStrategyCommandFactory);
        }

        private static DeployStrategy CreateDeployStrategy(IFileSystem fileSystem)
        {
            // oppo deploy <command>
            var deployHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.DeployFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.DeployCallDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
            };

            var deployHelpStrategyData = new HelpData
            {
                CommandName = Constants.DeployCommandArguments.Help,
                HelpTextFirstLine = deployHelpStrategyHelpText,
                LogMessage = LoggingText.OppoHelpForDeployCommandCalled,
                HelpText = Resources.text.help.HelpTextValues.DeployHelpArgumentCommandDescription,
            };

            var deployHelpStrategy = new HelpStrategy<DeployStrategy>(deployHelpStrategyData);

            deployHelpStrategyData.CommandName = Constants.DeployCommandArguments.VerboseHelp;

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

            return new DeployStrategy(deployStrategyCommandFactory);
        }

        private static CleanStrategy CreateCleanStrategy(IFileSystem fileSystem)
        {
            // oppo clean <command>
            var cleanHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.CleanFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.CleanCallDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
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

            return new CleanStrategy(cleanStrategyCommandFactory);
        }

        private static ImportStrategy CreateImportStrategy(IFileSystem fileSystem)
        {
            // oppo import <command>
            var importHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.ImportFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.ImportCallDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralArguments },
                { string.Empty, Resources.text.help.HelpTextValues.ImportArguments },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
                { "-n", "The name to opcuapp to use" },
                { "--name", "The name to opcuapp to use" },
                { "-p", "Information models path to use" },
                { "--path", "Information models path to use" },
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
                importHelpStrategy,
                importHelpStrategyVerbose
            };

            var importStrategyCommandFactory = new CommandFactory<ImportStrategy>(importCommands, Constants.ImportInformationModelCommandArguments.Help);
            importHelpStrategy.CommandFactory = importStrategyCommandFactory;
            importHelpStrategyVerbose.CommandFactory = importStrategyCommandFactory;

            return new ImportStrategy(importStrategyCommandFactory);
        }

        private static GenerateStrategy CreateGenerateStrategy(IFileSystem fileSystem)
        {
            // oppo generate <command>
            var generateHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, Resources.text.help.HelpTextValues.GenerateFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
                { string.Empty, Resources.text.help.HelpTextValues.GenerateCallDescription },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralArguments },
                { string.Empty, Resources.text.help.HelpTextValues.GenerateArguments },
                { string.Empty, string.Empty },
                { string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
                { "-n", "The name to opcuapp to use" },
                { "--name", "The name to opcuapp to use" },
                { "-m", "Information model to use" },
                { "--model", "Information model to use" },
            };

            var generateHelpStrategyData = new HelpData
            {
                CommandName = Constants.GenerateCommandArguments.Help,
                HelpTextFirstLine = generateHelpStrategyHelpText,
                LogMessage = LoggingText.OppoHelpForGenerateCommand,
                HelpText = Resources.text.help.HelpTextValues.GenerateHelpArgumentCommandDescription,
            };

            var generateHelpStrategy = new HelpStrategy<GenerateStrategy>(generateHelpStrategyData);

            generateHelpStrategyData.CommandName = Constants.GenerateCommandArguments.VerboseHelp;

            var generateHelpStrategyVerbose = new HelpStrategy<GenerateStrategy>(generateHelpStrategyData);
            
            var generateSubCommands = new ICommand<GenerateStrategy>[]
            {
                new GenerateInformationModelStrategy(Constants.CommandName.GenerateInformationModel, fileSystem, new ModelValidator(fileSystem)),
                generateHelpStrategy,
                generateHelpStrategyVerbose
            };

            var generateStrategyCommandFactory = new CommandFactory<GenerateStrategy>(generateSubCommands, Constants.GenerateCommandArguments.Help);
            generateHelpStrategy.CommandFactory = generateStrategyCommandFactory;
            generateHelpStrategyVerbose.CommandFactory = generateStrategyCommandFactory;

            return new GenerateStrategy(generateStrategyCommandFactory);
        }

		private static SlnStrategy CreateSlnStrategy(IFileSystem fileSystem)
		{
			var slnHelpStrategyHelpText = new MessageLines
			{
				{ string.Empty, Resources.text.help.HelpTextValues.SlnFirstLine },
				{ string.Empty, string.Empty },
				{ string.Empty, Resources.text.help.HelpTextValues.GeneralUsage },
				{ string.Empty, Resources.text.help.HelpTextValues.SlnCallDescription },
				{ string.Empty, string.Empty },
				{ string.Empty, Resources.text.help.HelpTextValues.GeneralArguments },
				{ string.Empty, Resources.text.help.HelpTextValues.SlnArgumentAdd },
				{ string.Empty, Resources.text.help.HelpTextValues.SlnArgumentBuild },
				{ string.Empty, Resources.text.help.HelpTextValues.SlnArgumentRemove },
				{ string.Empty, string.Empty },
				{ string.Empty, Resources.text.help.HelpTextValues.GeneralOptions },
			};

			var slnHelpStrategyData = new HelpData
			{
				CommandName = Constants.SlnCommandName.Help,
				HelpTextFirstLine = slnHelpStrategyHelpText,
				LogMessage = LoggingText.OppoHelpForSlnCommand,
				HelpText = Resources.text.help.HelpTextValues.SlnHelpArgumentCommandDescription,
			};

			var slnHelpStrategy = new HelpStrategy<SlnStrategy>(slnHelpStrategyData);

			slnHelpStrategyData.CommandName = Constants.SlnCommandName.VerboseHelp;

			var slnHelpVerboseStrategy = new HelpStrategy<SlnStrategy>(slnHelpStrategyData);

			var slnStrategies = new ICommand<SlnStrategy>[]
			{
				new SlnAddCommandStrategy(fileSystem),
				new SlnBuildCommandStrategy(fileSystem),
				new SlnRemoveCommandStrategy(fileSystem),
				slnHelpStrategy,
				slnHelpVerboseStrategy,
			};

			var slnStrategyCommandFactory = new CommandFactory<SlnStrategy>(slnStrategies, Constants.NewCommandName.Help);
			slnHelpStrategy.CommandFactory = slnStrategyCommandFactory;
			slnHelpVerboseStrategy.CommandFactory = slnStrategyCommandFactory;

			return new SlnStrategy(slnStrategyCommandFactory);
		}

        public string PrepareCommandFailureOutputText(string[] args)
        {
            return string.Format(Resources.text.output.OutputText.GeneralCommandExecutionFailure, string.Join(' ', args));
        }
    }
}
