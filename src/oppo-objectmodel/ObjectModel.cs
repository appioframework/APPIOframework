using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using Oppo.Resources.text.help;
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
using Oppo.ObjectModel.CommandStrategies.ReferenceCommands;

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
            var certificateGenerator = new CertificateGenerator(fileSystem);

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

            var newStrategy = CreateNewStrategy(fileSystem, certificateGenerator);
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

            var generateStrategy = CreateGenerateStrategy(fileSystem, certificateGenerator);
            commands.Add(generateStrategy);

			var slnStrategy = CreateSlnStrategy(fileSystem);
			commands.Add(slnStrategy);

			var referenceStrategy = CreateReferenceStrategy(fileSystem);
			commands.Add(referenceStrategy);

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
			var helpOption = new StringBuilder(Constants.CommandName.HelpDash).Append(Constants.HelpOptionSeparator).Append(Constants.CommandName.HelpDashVerbose).ToString();
			var helpStrategyFirstLineText = new MessageLines
            {
                { string.Empty, HelpTextValues.HelpStartWelcome },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.HelpStartDocuLink },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.GeneralUsage },
                { string.Empty, HelpTextValues.HelpStartUsageDescription },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.HelpStartTerminology },
                { string.Empty, HelpTextValues.HelpStartOpcuaapp },
                { string.Empty, HelpTextValues.HelpStartOpcuaappDescription },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.HelpStartOppoOptions },
				{ helpOption, HelpTextValues.HelpCommand },
				{ Constants.CommandName.Version, HelpTextValues.VersionCommand },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.HelpStartCommand },
				{ Constants.CommandName.Build, HelpTextValues.BuildCommand },
				{ Constants.CommandName.Clean, HelpTextValues.CleanCommand },
				{ Constants.CommandName.Deploy, HelpTextValues.DeployCommand },
				{ Constants.CommandName.Generate, HelpTextValues.GenerateCommand },
				{ Constants.CommandName.Help, HelpTextValues.HelpCommand },
				{ Constants.CommandName.Import, HelpTextValues.ImportCommand },
				{ Constants.CommandName.New, HelpTextValues.NewCommand },
				{ Constants.CommandName.Publish, HelpTextValues.PublishCommand },
				{ Constants.CommandName.Reference, HelpTextValues.ReferenceCommand },
				{ Constants.CommandName.Sln, HelpTextValues.SlnCommand }
			};

            var helpStrategyLastLineText = new MessageLines
            {
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.HelpEndCommand },
            };

            return new HelpData
            {
                CommandName = commandName,
                HelpTextFirstLine = helpStrategyFirstLineText,
                HelpTextLastLine = helpStrategyLastLineText,
                LogMessage = LoggingText.OppoHelpCalled,
                HelpText = HelpTextValues.HelpCommand,
            };
        }

		// OPPO Build command
        private static BuildStrategy CreateBuildStrategy(IFileSystem fileSystem)
        {
            // Build command help first lines
            var buildHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, HelpTextValues.BuildFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.GeneralUsage },
                { string.Empty, HelpTextValues.BuildCallDescription }
            };

			// Build command options
			var nameOption = new StringBuilder(Constants.BuildCommandOptions.Name).Append(Constants.HelpOptionSeparator).Append(Constants.BuildCommandOptions.VerboseName).ToString();
			var helpOption = new StringBuilder(Constants.BuildCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.BuildCommandOptions.VerboseHelp).ToString();
			var buildOptions = new MessageLines
			{
				{ nameOption, HelpTextValues.OptionNameDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription }
			};

			// Build help strategy
            var buildHelpStrategyData = new HelpData
            {
                CommandName = Constants.BuildCommandOptions.Help,
                HelpTextFirstLine = buildHelpStrategyHelpText,
				Options = buildOptions,
                LogMessage = LoggingText.OppoHelpForBuildCommandCalled,
                HelpText = HelpTextValues.BuildHelpArgumentCommandDescription,
            };
			
            var buildHelpStrategy = new HelpStrategy<BuildStrategy>(buildHelpStrategyData);

            buildHelpStrategyData.CommandName = Constants.BuildCommandOptions.VerboseHelp;

            var buildHelpVerboseStrategy = new HelpStrategy<BuildStrategy>(buildHelpStrategyData);

            var buildStrategies = new ICommand<BuildStrategy>[]
            {
                new BuildNameStrategy(Constants.BuildCommandOptions.Name, fileSystem),
                new BuildNameStrategy(Constants.BuildCommandOptions.VerboseName, fileSystem),
                buildHelpStrategy,
                buildHelpVerboseStrategy
            };

            var buildStrategyCommandFactory = new CommandFactory<BuildStrategy>(buildStrategies, Constants.BuildCommandOptions.Help);
            buildHelpStrategy.CommandFactory = buildStrategyCommandFactory;
            buildHelpVerboseStrategy.CommandFactory = buildStrategyCommandFactory;

            return new BuildStrategy(buildStrategyCommandFactory);
        }

		// OPPO New command
        private static NewStrategy CreateNewStrategy(IFileSystem fileSystem, AbstractCertificateGenerator certificateGenerator)
        {
            // New command help first lines
            var newHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, HelpTextValues.NewFirstLine },
                { string.Empty, string.Empty},
                { string.Empty, HelpTextValues.GeneralUsage },
                { string.Empty, HelpTextValues.NewCallDescription },
            };

			// New command arguments
			var newArguments = new MessageLines
			{
				{ Constants.NewCommandArguments.OpcuaApp, string.Empty },
				{ Constants.NewCommandArguments.Sln, string.Empty }
			};

			// New command options
			var nameOption = new StringBuilder(Constants.NewCommandOptions.Name).Append(Constants.HelpOptionSeparator).Append(Constants.NewCommandOptions.VerboseName).ToString();
			var helpOption = new StringBuilder(Constants.NewCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.NewCommandOptions.VerboseHelp).ToString();
			var typeOption = new StringBuilder(Constants.NewCommandOptions.Type).Append(Constants.HelpOptionSeparator).Append(Constants.NewCommandOptions.VerboseType).ToString();
			var portOption = new StringBuilder(Constants.NewCommandOptions.Port).Append(Constants.HelpOptionSeparator).Append(Constants.NewCommandOptions.VerbosePort).ToString();
			var urlOption = new StringBuilder(Constants.NewCommandOptions.Url).Append(Constants.HelpOptionSeparator).Append(Constants.NewCommandOptions.VerboseUrl).ToString();

			var newOptions = new MessageLines
			{
				{ nameOption, HelpTextValues.NewCommandOptionNameDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription },
				{ typeOption, HelpTextValues.NewCommandOptionTypeDescription },
				{ portOption, HelpTextValues.NewCommandOptionPortDescription },
				{ urlOption, HelpTextValues.NewCommandOptionUrlDescription },
				{ Constants.NewCommandOptions.VerboseNoCert, HelpTextValues.NewCommandOptionNoCertDescription }
			};

			// New help strategy
			var newHelpStrategyData = new HelpData
			{
				CommandName = Constants.NewCommandOptions.Help,
				HelpTextFirstLine = newHelpStrategyHelpText,
				Arguments = newArguments,
				Options = newOptions,
                LogMessage = LoggingText.OppoHelpForNewCommandCalled,
                HelpText = HelpTextValues.NewHelpArgumentCommandDescription,
            };

            var newHelpStrategy = new HelpStrategy<NewStrategy>(newHelpStrategyData);

            newHelpStrategyData.CommandName = Constants.NewCommandOptions.VerboseHelp;

            var newHelpVerboseStrategy = new HelpStrategy<NewStrategy>(newHelpStrategyData);

            var newStrategies = new ICommand<NewStrategy>[]
            {
                new NewSlnCommandStrategy(fileSystem),
                new NewOpcuaAppCommandStrategy(fileSystem, certificateGenerator),
                newHelpStrategy,
                newHelpVerboseStrategy,
            };

            var newStrategyCommandFactory = new CommandFactory<NewStrategy>(newStrategies, Constants.NewCommandOptions.Help);
            newHelpStrategy.CommandFactory = newStrategyCommandFactory;
            newHelpVerboseStrategy.CommandFactory = newStrategyCommandFactory;

            return new NewStrategy(newStrategyCommandFactory);
        }

		// OPPO Publish command
        private static PublishStrategy CreatePublishStrategy(IFileSystem fileSystem)
        {
            // Publish command help first lines
            var publishHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, HelpTextValues.PublishFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.GeneralUsage },
                { string.Empty, HelpTextValues.PublishCallDescription }
            };

			// Publish command options
			var nameOption = new StringBuilder(Constants.PublishCommandOptions.Name).Append(Constants.HelpOptionSeparator).Append(Constants.PublishCommandOptions.VerboseName).ToString();
			var helpOption = new StringBuilder(Constants.PublishCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.PublishCommandOptions.VerboseHelp).ToString();
			var publishOptions = new MessageLines
			{
				{ nameOption, HelpTextValues.OptionNameDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription }
			};

			// Publish help strategy
			var publishHelpStrategyData = new HelpData
            {
                CommandName = Constants.PublishCommandOptions.Help,
                HelpTextFirstLine = publishHelpStrategyHelpText,
				Options = publishOptions,
                LogMessage = LoggingText.OpcuaappPublishHelpCalled,
                HelpText = HelpTextValues.PublishHelpArgumentCommandDescription,
            };

            var publishHelpStrategy = new HelpStrategy<PublishStrategy>(publishHelpStrategyData);

            publishHelpStrategyData.CommandName = Constants.PublishCommandOptions.VerboseHelp;

            var publishHelpVerboseStrategy = new HelpStrategy<PublishStrategy>(publishHelpStrategyData);

            var publishStrategies = new ICommand<PublishStrategy>[]
            {
                new PublishNameStrategy(Constants.PublishCommandOptions.Name, fileSystem),
                new PublishNameStrategy(Constants.PublishCommandOptions.VerboseName, fileSystem),
                publishHelpStrategy,
                publishHelpVerboseStrategy,
            };

            var publishStrategyCommandFactory = new CommandFactory<PublishStrategy>(publishStrategies, Constants.PublishCommandOptions.Help);
            publishHelpStrategy.CommandFactory = publishStrategyCommandFactory;
            publishHelpVerboseStrategy.CommandFactory = publishStrategyCommandFactory;
            
            return new PublishStrategy(publishStrategyCommandFactory);
        }

		// OPPO Deploy command
        private static DeployStrategy CreateDeployStrategy(IFileSystem fileSystem)
        {
            // Deploy command help first lines
            var deployHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, HelpTextValues.DeployFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.GeneralUsage },
                { string.Empty, HelpTextValues.DeployCallDescription }
            };

			// Deploy command options
			var nameOption = new StringBuilder(Constants.DeployCommandOptions.Name).Append(Constants.HelpOptionSeparator).Append(Constants.DeployCommandOptions.VerboseName).ToString();
			var helpOption = new StringBuilder(Constants.DeployCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.DeployCommandOptions.VerboseHelp).ToString();
			var deployOptions = new MessageLines
			{
				{ nameOption, HelpTextValues.OptionNameDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription }
			};

			var deployHelpStrategyData = new HelpData
			{
				CommandName = Constants.DeployCommandOptions.Help,
				HelpTextFirstLine = deployHelpStrategyHelpText,
				Options = deployOptions,
                LogMessage = LoggingText.OppoHelpForDeployCommandCalled,
                HelpText = HelpTextValues.DeployHelpArgumentCommandDescription,
            };

            var deployHelpStrategy = new HelpStrategy<DeployStrategy>(deployHelpStrategyData);

            deployHelpStrategyData.CommandName = Constants.DeployCommandOptions.VerboseHelp;

            var deployVerboseHelpStrategy = new HelpStrategy<DeployStrategy>(deployHelpStrategyData);

            var deployStrategies = new ICommand<DeployStrategy>[]
            {
                new DeployNameStrategy(Constants.DeployCommandOptions.Name, fileSystem),
                new DeployNameStrategy(Constants.DeployCommandOptions.VerboseName, fileSystem),
                deployHelpStrategy,
                deployVerboseHelpStrategy
            };

            var deployStrategyCommandFactory = new CommandFactory<DeployStrategy>(deployStrategies, Constants.DeployCommandOptions.Help);
            deployHelpStrategy.CommandFactory = deployStrategyCommandFactory;
            deployVerboseHelpStrategy.CommandFactory = deployStrategyCommandFactory;

            return new DeployStrategy(deployStrategyCommandFactory);
        }

		// OPPO Clean command
        private static CleanStrategy CreateCleanStrategy(IFileSystem fileSystem)
        {
            // Clean command help first lines
            var cleanHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, HelpTextValues.CleanFirstLine },
				{ string.Empty, string.Empty },
				{ string.Empty, HelpTextValues.GeneralUsage },
				{ string.Empty, HelpTextValues.CleanCallDescription }
			};
			
			// Clean command options
			var nameOption = new StringBuilder(Constants.CleanCommandOptions.Name).Append(Constants.HelpOptionSeparator).Append(Constants.CleanCommandOptions.VerboseName).ToString();
			var helpOption = new StringBuilder(Constants.CleanCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.CleanCommandOptions.VerboseHelp).ToString();
			var cleanOptions = new MessageLines
			{
				{ nameOption, HelpTextValues.OptionNameDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription }
			};

			// Clean help strategy
			var cleanHelpStrategyData = new HelpData
			{
				CommandName = Constants.CleanCommandOptions.Help,
				HelpTextFirstLine = cleanHelpStrategyHelpText,
				Options = cleanOptions,
                LogMessage = LoggingText.OppoHelpForCleanCommandCalled,
                HelpText = HelpTextValues.CleanHelpArgumentCommandDescription,
            };

            var cleanHelpStrategy = new HelpStrategy<CleanStrategy>(cleanHelpStrategyData);

            cleanHelpStrategyData.CommandName = Constants.CleanCommandOptions.VerboseHelp;

            var cleanHelpVerboseStrategy = new HelpStrategy<CleanStrategy>(cleanHelpStrategyData);

            var cleanStrategies = new ICommand<CleanStrategy>[]
            {
                new CleanNameStrategy(Constants.CleanCommandOptions.Name, fileSystem),
                new CleanNameStrategy(Constants.CleanCommandOptions.VerboseName, fileSystem),
                cleanHelpStrategy,
                cleanHelpVerboseStrategy,
            };
            var cleanStrategyCommandFactory = new CommandFactory<CleanStrategy>(cleanStrategies, Constants.CleanCommandOptions.Help);
            cleanHelpStrategy.CommandFactory = cleanStrategyCommandFactory;
            cleanHelpVerboseStrategy.CommandFactory = cleanStrategyCommandFactory;

            return new CleanStrategy(cleanStrategyCommandFactory);
        }
		
		// OPPO Import Strategy
        private static ImportStrategy CreateImportStrategy(IFileSystem fileSystem)
        {
            // Import command help first lines
            var importHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, HelpTextValues.ImportFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.GeneralUsage },
                { string.Empty, HelpTextValues.ImportCallDescription }
            };

			// Improt command arguments
			var importArguments = new MessageLines
			{
				{ Constants.ImportCommandArguments.InformationModel, string.Empty },
				{ Constants.ImportCommandArguments.Certificate, string.Empty }
			};

			// Import command options
			var nameOption = new StringBuilder(Constants.ImportCommandOptions.Name).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCommandOptions.VerboseName).ToString();
			var pathOption = new StringBuilder(Constants.ImportCommandOptions.Path).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCommandOptions.VerbosePath).ToString();
			var typesOption = new StringBuilder(Constants.ImportCommandOptions.Types).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCommandOptions.VerboseTypes).ToString();
			var helpOption = new StringBuilder(Constants.ImportCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCommandOptions.VerboseHelp).ToString();
			var sampleOption = new StringBuilder(Constants.ImportCommandOptions.Sample).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCommandOptions.VerboseHelp).ToString();
			var keyOption = new StringBuilder(Constants.ImportCertificateCommandArguments.Key).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCertificateCommandArguments.VerboseKey).ToString();
			var projectOption = new StringBuilder(Constants.ImportCertificateCommandArguments.Project).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCertificateCommandArguments.VerboseProject).ToString();
			var certiciateOption = new StringBuilder(Constants.ImportCertificateCommandArguments.Certificate).Append(Constants.HelpOptionSeparator).Append(Constants.ImportCertificateCommandArguments.VerboseCertificate).ToString();
			var clientServerOption = new StringBuilder(Constants.ImportCertificateCommandArguments.VerboseClient).Append(" or ").Append(Constants.ImportCertificateCommandArguments.VerboseServer).ToString();

			var importOptions = new MessageLines
			{
				{ nameOption, HelpTextValues.OptionNameDescription },
				{ pathOption, HelpTextValues.ImportCommandOptionPathDescription },
				{ typesOption, HelpTextValues.ImportCommandOptionTypesDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription },
				{ sampleOption, HelpTextValues.ImportCommandOptionSampleDescription },
				{ keyOption, HelpTextValues.ImportCommandOptionKeyDescription },
				{ projectOption, HelpTextValues.ImportCommandOptionProjectDescription },
				{ certiciateOption, HelpTextValues.ImportCommandOptionCertificateDescription },
				{ clientServerOption, HelpTextValues.ImportCommandOptionClientServerDescription }
			};

			// Import help strategy
			var importHelpStrategyData = new HelpData
			{
				CommandName = Constants.ImportCommandOptions.Help,
				HelpTextFirstLine = importHelpStrategyHelpText,
				Arguments = importArguments,
				Options = importOptions,
                LogMessage = LoggingText.OppoHelpForImportInformationModel,
                HelpText = HelpTextValues.ImportHelpArgumentCommandDescription,
            };

            var importHelpStrategy = new HelpStrategy<ImportStrategy>(importHelpStrategyData);

            importHelpStrategyData.CommandName = Constants.ImportCommandOptions.VerboseHelp;

            var importHelpStrategyVerbose = new HelpStrategy<ImportStrategy>(importHelpStrategyData);
            var importCommands = new ICommand<ImportStrategy>[]
            {
                new ImportInformationModelCommandStrategy(fileSystem, new ModelValidator(fileSystem)),
                new ImportCertificateStrategy(fileSystem), 
                importHelpStrategy,
                importHelpStrategyVerbose
            };

            var importStrategyCommandFactory = new CommandFactory<ImportStrategy>(importCommands, Constants.ImportCommandOptions.Help);
            importHelpStrategy.CommandFactory = importStrategyCommandFactory;
            importHelpStrategyVerbose.CommandFactory = importStrategyCommandFactory;

            return new ImportStrategy(importStrategyCommandFactory);
        }

		// OPPO Generate command
        private static GenerateStrategy CreateGenerateStrategy(IFileSystem fileSystem, AbstractCertificateGenerator certificateGenerator)
        {
            // Generate command help first lines
            var generateHelpStrategyHelpText = new MessageLines
            {
                { string.Empty, HelpTextValues.GenerateFirstLine },
                { string.Empty, string.Empty },
                { string.Empty, HelpTextValues.GeneralUsage },
                { string.Empty, HelpTextValues.GenerateCallDescription }
            };

			// Generate command arguments
			var generateArguments = new MessageLines
			{
				{ Constants.ImportCommandArguments.InformationModel, string.Empty },
				{ Constants.ImportCommandArguments.Certificate, string.Empty }
			};

			// Generate command options
			var nameOption = new StringBuilder(Constants.GenerateCommandOptions.Name).Append(Constants.HelpOptionSeparator).Append(Constants.GenerateCommandOptions.VerboseName).ToString();
			var helpOption = new StringBuilder(Constants.GenerateCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.GenerateCommandOptions.VerboseHelp).ToString();

			var generateOptions = new MessageLines
			{
				{ nameOption, HelpTextValues.OptionNameDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription },
				{ Constants.GenerateCertificateCommandArguments.VerboseKeySize, HelpTextValues.GenerateCommandOptionKeySizeDescription },
				{ Constants.GenerateCertificateCommandArguments.VerboseDays, HelpTextValues.GenerateCommandOptionDaysDescription },
				{ Constants.GenerateCertificateCommandArguments.VerboseOrganization, HelpTextValues.GenerateCommandOptionOrganizationDescription }
			};

			// Generate help strategy
			var generateHelpStrategyData = new HelpData
            {
                CommandName = Constants.GenerateCommandOptions.Help,
                HelpTextFirstLine = generateHelpStrategyHelpText,
				Arguments = generateArguments,
				Options = generateOptions,
                LogMessage = LoggingText.OppoHelpForGenerateCommand,
                HelpText = HelpTextValues.GenerateHelpArgumentCommandDescription,
            };

            var generateHelpStrategy = new HelpStrategy<GenerateStrategy>(generateHelpStrategyData);

            generateHelpStrategyData.CommandName = Constants.GenerateCommandOptions.VerboseHelp;

            var generateHelpStrategyVerbose = new HelpStrategy<GenerateStrategy>(generateHelpStrategyData);

			var modelValidator = new ModelValidator(fileSystem);
			var nodesetGenerator = new NodesetGenerator(fileSystem, modelValidator);

			var generateSubCommands = new ICommand<GenerateStrategy>[]
			{
				new GenerateInformationModelStrategy(Constants.CommandName.GenerateInformationModel, fileSystem, modelValidator, nodesetGenerator),
                new GenerateCertificateStrategy(fileSystem, certificateGenerator),
                generateHelpStrategy,
                generateHelpStrategyVerbose
            };

            var generateStrategyCommandFactory = new CommandFactory<GenerateStrategy>(generateSubCommands, Constants.GenerateCommandOptions.Help);
            generateHelpStrategy.CommandFactory = generateStrategyCommandFactory;
            generateHelpStrategyVerbose.CommandFactory = generateStrategyCommandFactory;

            return new GenerateStrategy(generateStrategyCommandFactory);
        }

		// OPPO Sln command
		private static SlnStrategy CreateSlnStrategy(IFileSystem fileSystem)
		{
			// Sln command help first lines
			var slnHelpStrategyHelpText = new MessageLines
			{
				{ string.Empty, HelpTextValues.SlnFirstLine },
				{ string.Empty, string.Empty },
				{ string.Empty, HelpTextValues.GeneralUsage },
				{ string.Empty, HelpTextValues.SlnCallDescription }
			};

			// Sln command arguments
			var slnArguments = new MessageLines
			{
				{ Constants.SlnCommandArguments.Add, string.Empty },
				{ Constants.SlnCommandArguments.Remove, string.Empty },
				{ Constants.SlnCommandArguments.Build, string.Empty },
				{ Constants.SlnCommandArguments.Publish, string.Empty },
				{ Constants.SlnCommandArguments.Deploy, string.Empty }
			};

			// Sln command options
			var solutionOption = new StringBuilder(Constants.SlnCommandOptions.Solution).Append(Constants.HelpOptionSeparator).Append(Constants.SlnCommandOptions.VerboseSolution).ToString();
			var projectOption = new StringBuilder(Constants.SlnCommandOptions.Project).Append(Constants.HelpOptionSeparator).Append(Constants.SlnCommandOptions.VerboseProject).ToString();
			var helpOption = new StringBuilder(Constants.SlnCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.SlnCommandOptions.VerboseHelp).ToString();

			var slnOptions = new MessageLines
			{
				{ solutionOption, HelpTextValues.SlnCommandOptionSolutionDescription },
				{ projectOption, HelpTextValues.SlnCommandOptionProjectDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription },
			};

			// Sln help strategy
			var slnHelpStrategyData = new HelpData
			{
				CommandName = Constants.SlnCommandOptions.Help,
				HelpTextFirstLine = slnHelpStrategyHelpText,
				Arguments = slnArguments,
				Options = slnOptions,
				LogMessage = LoggingText.OppoHelpForSlnCommand,
				HelpText = HelpTextValues.SlnHelpArgumentCommandDescription,
			};

			var slnHelpStrategy = new HelpStrategy<SlnStrategy>(slnHelpStrategyData);

			slnHelpStrategyData.CommandName = Constants.SlnCommandOptions.VerboseHelp;

			var slnHelpVerboseStrategy = new HelpStrategy<SlnStrategy>(slnHelpStrategyData);

			var SlnBuildCommandData					 = new SlnOperationData();
			SlnBuildCommandData.CommandName			 = Constants.CommandName.Build;
			SlnBuildCommandData.FileSystem			 = fileSystem;
			SlnBuildCommandData.Subcommand			 = new BuildNameStrategy(Constants.CommandName.Build, fileSystem);
			SlnBuildCommandData.SuccessLoggerMessage = LoggingText.SlnBuildSuccess;
			SlnBuildCommandData.SuccessOutputMessage = OutputText.SlnBuildSuccess;
			SlnBuildCommandData.HelpText			 = HelpTextValues.SlnBuildNameArgumentCommandDescription;

			var SlnDeployCommandData				  = new SlnOperationData();
			SlnDeployCommandData.CommandName		  = Constants.CommandName.Deploy;
			SlnDeployCommandData.FileSystem			  = fileSystem;
			SlnDeployCommandData.Subcommand			  = new DeployNameStrategy(Constants.CommandName.Deploy, fileSystem);
			SlnDeployCommandData.SuccessLoggerMessage = LoggingText.SlnDeploySuccess;
			SlnDeployCommandData.SuccessOutputMessage = OutputText.SlnDeploySuccess;
			SlnDeployCommandData.HelpText			  = HelpTextValues.SlnDeployNameArgumentCommandDescription;

			var SlnPublishCommandData				   = new SlnOperationData();
			SlnPublishCommandData.CommandName		   = Constants.CommandName.Publish;
			SlnPublishCommandData.FileSystem		   = fileSystem;
			SlnPublishCommandData.Subcommand		   = new PublishNameStrategy(Constants.CommandName.Publish, fileSystem);
			SlnPublishCommandData.SuccessLoggerMessage = LoggingText.SlnPublishSuccess;
			SlnPublishCommandData.SuccessOutputMessage = OutputText.SlnPublishSuccess;
			SlnPublishCommandData.HelpText             = HelpTextValues.SlnPublishNameArgumentCommandDescription;

			var slnStrategies = new ICommand<SlnStrategy>[]
			{
				new SlnAddCommandStrategy(fileSystem),
				new SlnOperationCommandStrategy(SlnBuildCommandData),
				new SlnOperationCommandStrategy(SlnDeployCommandData),
				new SlnOperationCommandStrategy(SlnPublishCommandData),
				new SlnRemoveCommandStrategy(fileSystem),
				slnHelpStrategy,
				slnHelpVerboseStrategy,
			};

			var slnStrategyCommandFactory = new CommandFactory<SlnStrategy>(slnStrategies, Constants.SlnCommandOptions.Help);
			slnHelpStrategy.CommandFactory = slnStrategyCommandFactory;
			slnHelpVerboseStrategy.CommandFactory = slnStrategyCommandFactory;

			return new SlnStrategy(slnStrategyCommandFactory);
		}
		
		// OPPO Reference command
		private static ReferenceStrategy CreateReferenceStrategy(IFileSystem fileSystem)
		{
			// Reference command help first lines
			var referenceHelpStrategyHelpText = new MessageLines
			{
				{ string.Empty, HelpTextValues.ReferenceFirstLine },
				{ string.Empty, string.Empty },
				{ string.Empty, HelpTextValues.GeneralUsage },
				{ string.Empty, HelpTextValues.ReferenceCallDescription }
			};

			// Reference command arguments
			var referenceArguments = new MessageLines
			{
				{ Constants.ReferenceCommandArguments.Add, string.Empty },
				{ Constants.ReferenceCommandArguments.Remove, string.Empty }
			};

			// Reference command options
			var clientOption = new StringBuilder(Constants.ReferenceCommandOptions.Client).Append(Constants.HelpOptionSeparator).Append(Constants.ReferenceCommandOptions.VerboseClient).ToString();
			var serverOption = new StringBuilder(Constants.ReferenceCommandOptions.Server).Append(Constants.HelpOptionSeparator).Append(Constants.ReferenceCommandOptions.VerboseServer).ToString();
			var helpOption = new StringBuilder(Constants.ReferenceCommandOptions.Help).Append(Constants.HelpOptionSeparator).Append(Constants.ReferenceCommandOptions.VerboseHelp).ToString();

			var referenceOptions = new MessageLines
			{
				{ clientOption, HelpTextValues.ReferenceCommandOptionClientDescription },
				{ serverOption, HelpTextValues.ReferenceCommandOptionServerDescription },
				{ helpOption, HelpTextValues.OptionHelpDescription },
			};

			// Reference help strategy
			var referenceHelpStrategyData = new HelpData
			{
				CommandName = Constants.ReferenceCommandOptions.Help,
				HelpTextFirstLine = referenceHelpStrategyHelpText,
				Arguments = referenceArguments,
				Options = referenceOptions,
				LogMessage = LoggingText.OppoHelpForReferenceCommand,
				HelpText = HelpTextValues.ReferenceHelpArgumentCommandDescription,
			};

			var referenceHelpStrategy = new HelpStrategy<ReferenceStrategy>(referenceHelpStrategyData);

			referenceHelpStrategyData.CommandName = Constants.ReferenceCommandOptions.VerboseHelp;

			var referenceHelpVerboseStrategy = new HelpStrategy<ReferenceStrategy>(referenceHelpStrategyData);
			
			var referenceStrategies = new ICommand<ReferenceStrategy>[]
			{
				new ReferenceAddCommandStrategy(fileSystem),
				new ReferenceRemoveCommandStrategy(fileSystem),
				referenceHelpStrategy,
				referenceHelpVerboseStrategy,
			};

			var referenceStrategyCommandFactory = new CommandFactory<ReferenceStrategy>(referenceStrategies, Constants.ReferenceCommandOptions.Help);
			referenceHelpStrategy.CommandFactory = referenceStrategyCommandFactory;
			referenceHelpVerboseStrategy.CommandFactory = referenceStrategyCommandFactory;

			return new ReferenceStrategy(referenceStrategyCommandFactory);
		}

		public string PrepareCommandFailureOutputText(string[] args)
        {
            return string.Format(OutputText.GeneralCommandExecutionFailure, string.Join(' ', args));
        }
    }
}
