namespace Appio.Resources.text.help
{
    public static class HelpTextValues
    {
        //General texts
        public const string GeneralUsage = "Usage:";
        public const string GeneralOptions = "Options:";
        public const string GeneralArguments = "Arguments:";

        //Help command
        public const string HelpStartWelcome = "Welcome to the Adaptable Plug & Produce Industrial Orchestration Framework!";
        public const string HelpStartDocuLink = "For a more detailed documentation visit https://appioframework.readthedocs.io/";
        public const string HelpStartUsageDescription = "appio [command] <Argument 1> <Argument n> <Options>";
        public const string HelpStartAppioOptions = "APPIO options:";
        public const string HelpStartCommand = "APPIO commands:";

        public const string BuildCommand = "Build an appio project.";
        public const string PublishCommand = "Publish an appio project for deployment.";
        public const string NewCommand = "Create a new appio project.";
        public const string HelpCommand = "Show command line Help.";
        public const string HelloCommand = "---";
        public const string VersionCommand = "Display the installed appio version.";
        public const string CleanCommand = "Clean an appio project.";
        public const string DeployCommand = "Generate an installer containing the opcua-app.";
        public const string ImportCommand = "Import an external ressources.";
		public const string SlnCommand = "Manage group of opcuaapp projects organised into solution.";
        public const string ReferenceCommand = "Manage references between server and clients.";

		public const string HelpEndCommand = "Run \"appio [command] --help\" for more information about a command.";

        //Build command
        public const string BuildFirstLine = "Build an opcua-app from an APPIO project.";
        public const string BuildCallDescription = "appio build <Options>";

        //Clean command
        public const string CleanFirstLine = "Clean an APPIO project. Removes temporary files and folders.";
        public const string CleanCallDescription = "appio clean <Options>";

        //Publish command
        public const string PublishFirstLine = "Publish an APPIO project for deployment or usage. It copies the binaries into the publish folder.";
        public const string PublishCallDescription = "appio publish <Options>";

        //Deploy command
        public const string DeployFirstLine = "Build an installer package(.deb) containing the opcua-app ready for deployment.";
        public const string DeployCallDescription = "appio deploy <Options>";

		//Import command
		public const string ImportFirstLine = "Import files to APPIO project. Depends on used argument server certificates or information models will be imported to the projct.";
        public const string ImportCallDescription = "appio import <Argument> <Options>";
        public const string ImportArguments = "information-model";
		public const string ImportCommandOptionNameDescription = "appio project name";
		public const string ImportCommandOptionPathDescription = "the path of the information model (arg: information-model)";
		public const string ImportCommandOptionTypesDescription = "(optional) necessary data types for imported information model. Has to be bsd file (arg: information-model)";
		public const string ImportCommandOptionSampleDescription = "imports a sample information model to project (arg: information-model)";
		public const string ImportCommandOptionKeyDescription = "the path to key file (arg: certificate)";
		public const string ImportCommandOptionCertificateDescription = "the path to certificate file (arg: certificate)";
		public const string ImportCommandOptionClientServerDescription = "(optional) specifies to which application (server or client) file will be imported in case of opc ua ClientServer application (arg: certificate)";

		//New command
		public const string NewFirstLine = "Create a new APPIO project or solution.";
        public const string NewCallDescription = "appio new <Argument> <Options>";
        public const string NewArgumentsObject = "<Object> The object to create, can either be:";
        public const string NewArgumentsSln = "sln";
        public const string NewArgumentsOpcuaapp = "opcuapp";
		public const string NewCommandOptionNameDescription = "appio project or solution name";
		public const string NewCommandOptionTypeDescription = "appio project type: Client, Server, ClientServer (arg: opcuaapp)";
		public const string NewCommandOptionPortDescription = "server port number (arg: opcuaapp)";
		public const string NewCommandOptionUrlDescription = "server url (arg: opcuaapp)";
		public const string NewCommandOptionNoCertDescription = "when set then no security certificates created (arg: opcuaapp)";

		//Generate command
		public const string GenerateCommand = "Generate artifacts from the imported ressources.";
		public const string GenerateFirstLine = "Generate files in the APPIO project. Depends on used argument server certificates or information model files will be generated in the project.";
        public const string GenerateCallDescription = "appio generate <Argument> <Options>";
        public const string GenerateArguments = "information-model";
        public const string GenerateInformationModelCommandDescription = "Generates information-model code from ...";
		public const string GenerateCommandOptionKeySizeDescription = "(optional, default: 1024) key length used for certificat generation. Required by OpenSSL library (arg: certificate)";
		public const string GenerateCommandOptionDaysDescription = "(optional, default: 365) days valid of generated certificate. Required by OpenSSL library (arg: certificate)";
		public const string GenerateCommandOptionOrganizationDescription = "(optional, default: MyOrg) organization name used for certificate generation. Required by OpenSSL library (arg: certificate)";

		//Version command

		//Sln command
		public const string SlnFirstLine = "Manage projects in solution file. Projects organised into one solution can be built, published and deployed at once with a single command.";
		public const string SlnCallDescription = "appio sln <Argument> <Options>";
		public const string SlnCommandOptionSolutionDescription = "solution name";
		public const string SlnCommandOptionProjectDescription = "project name (arg: add, remove)";
		public const string SlnCommandArgumentAddDescription = "Add an APPIO project to the solution";
		public const string SlnCommandArtgumentBuildDescription = "Executes appio build for each APPIO project contained in the solution";
		public const string SlnCommandArtgumentPublishDescription = "Executes appio publish for each APPIO project contained in the solution";
		public const string SlnCommandArtgumentDeployDescription = "Executes appio deploy for each APPIO project contained in the solution";
		public const string SlnCommandArtgumentRemoveDescription = "Remove and APPIO project from the solution";

		//Reference command
		public const string ReferenceFirstLine = "Add and remove references between servers and clients. Clients use server references to automatically connect when started.";
		public const string ReferenceCallDescription = "appio referance <Argument> <Options>";
		public const string ReferenceCommandOptionClientDescription = "client application name";
		public const string ReferenceCommandOptionServerDescription = "server application name";


		//Commands description
		public const string BuildHelpArgumentCommandDescription = "Build help";
        public const string BuildNameArgumentCommandDescription = "Project name";
        public const string CleanHelpArgumentCommandDescription = "Clean help";
        public const string CleanNameArgumentCommandDescription = "Project name";
        public const string DeployHelpArgumentCommandDescription = "Deploy help";
        public const string DeployNameArgumentCommandDescription = "Deploy name";
        public const string ImportHelpArgumentCommandDescription = "Import help";
        public const string NewHelpArgumentCommandDescription = "New help";
        public const string PublishHelpArgumentCommandDescription = "Publish help";
        public const string PublishNameArgumentCommandDescription = "Project name";
        public const string ImportSamplesArgumentCommandDescription = "Import Samples";
        public const string GenerateHelpArgumentCommandDescription = "Generate help";
        public const string SlnHelpArgumentCommandDescription = "Sln help";
        public const string SlnAddNameArgumentCommandDescription = "Add project to solution";
		public const string SlnRemoveNameArgumentCommandDescription = "Remove project from solution";
		public const string SlnBuildNameArgumentCommandDescription = "Build all solution's projects";
		public const string SlnPublishNameArgumentCommandDescription = "Publish all solution's projects";
		public const string SlnDeployNameArgumentCommandDescription = "Deploy all solution's projects";
		public const string ReferenceAddNameArgumentCommandDescription = "Add reference server to client";
		public const string ReferenceRemoveNameArgumentCommandDescription = "Remove reference server from client";
		public const string ReferenceHelpArgumentCommandDescription = "Reference help";

		// Help option descriptions
		public const string OptionHelpDescription = "prints this help";
		public const string OptionNameDescription = "appio project name";
	}
}