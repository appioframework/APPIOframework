namespace Oppo.Resources.text.logging
{
    public static class LoggingText
    {
        public const string NullInputParams_Msg = "Null input parameters passed. Specify any valid parameters.";
        public const string InvalidSolutionName = "Invalid solution name!";
        public const string EmptySolutionName = "Empty solution name!";
        public const string UnknownNewSlnCommandParam = "Unknown new sln <command> parameter!";
        public const string NewSlnCommandSuccess = "Solution with name {0} was successfully created";
        public const string UnknownNewOpcuaappCommandParam = "Unknown new opcuaapp <command> parameter!";
        public const string NewOpcuaappCommandSuccess = "opcuaapp with name {0} was successfully created";
        public const string ImportInforamtionModelCommandSuccess = "Information model '{0}' imported successfully";
        public const string EmptyOpcuaappName = "Empty opcuaapp name!";
        public const string InvalidOpcuaappName = "Invalid opcuaapp name!";
        public const string MesonExecutableFails = "Meson Failed!";
        public const string NinjaExecutableFails = "Ninja Failed!";
        public const string NodesetCompilerExecutableFails = "Nodeset compiler Failed!";
        public const string GeneratedTypesExecutableFails = "Generate datatypes for NodeSet information-model Failed!";
        public const string BuildSuccess = "Build Success!";
        public const string CleanSuccess = "Clean Success!";
        public const string CleanFailure = "Clean Failure!";
        public const string OpcuaappPublishedSuccess = "Publish Success!";
        public const string OpcuaappDeploySuccess = "Deploy Success!";
        public const string OpcuaappPublishHelpCalled = "Publish <command> --help called";
        public const string OppoHelpCalled = "Help <command> called";
        public const string OppoHelpForNewCommandCalled = "Help for new <command> called";
        public const string OppoHelpForBuildCommandCalled = "Help for build <command> called";
        public const string OppoHelpForCleanCommandCalled = "Help for clean <command> called";
        public const string UnknownCommandCalled = "Unknown <command> called";
        public const string VersionCommandCalled = "version <command> called";
        public const string OppoHelpForDeployCommandCalled = "Help for deploy <command> called";
        public const string GenerateInformationModelSuccess = "Generate information-model success!";
        public const string NodesetCompilerExecutableFailsMissingModelFile = "Generate information-model failure! Can't find model with path '{0}'!";
        public const string NodesetCompilerExecutableFailsMissingFile = "Generate information-model failure! Can't find file with path '{0}'!";
        public const string NodesetCompilerExecutableFailsInvalidModelFile = "Generate information-model failure! Invalid model file extension '{0}'!";
        public const string NodesetCompilerExecutableFailsInvalidFile = "Generate information-model failure! Invalid file extension '{0}'!";
        public const string NodesetCompilerExecutableFailsRequiredModel = "Generate information-model failure! Can't use required model '{0}'!";
        // generate command
        public const string GenerateInformationModelFailureUnknownParam = "Generate information-model failure! Unknown command parameter '{0}'!";
        public const string GenerateInformationModelFailureEmptyOpcuaAppName = "Generate information-model failure! Empty opcuaapp name!";
        public const string OppoHelpForGenerateCommand = "Help for generate <command> called";
        public const string GenerateInformationModelFailureValidatingModel = "Model '{0}' validation failure!";

        public const string ValidatingModel = "Validating model '{0}' against '{1}'.";
        public const string ValidationError = "Validation error \n {0}";


        // sln add command
        public const string SlnAddSuccess					= "Opcuaapp project file successfullt added to sln.";
        public const string SlnAddUknownCommandParam		= "Unknown sln add command parameter!";
        public const string SlnAddOpposlnFileNotFound		= "Missing solution file!";
        public const string SlnAddOppoprojFileNotFound		= "Missing project file!";
        public const string SlnAddCouldntDeserliazeSln		= "Couldn't deserialize sln file!";
        public const string SlnAddCouldntDeserliazeOpcuaapp	= "Couldn't deserialize oppoproj file!";
        public const string SlnAddContainsOpcuaapp			= "The solution already contains the opcuaapp project.";
		public const string OppoHelpForSlnCommand			= "Help for sln <command> called";


		public const string DirectoryIOException = "Directory IOException detected!";
        public const string DirectoryNotFoundException = "Directory DirectoryNotFoundException detected!";
        public const string PathTooLongException = "Directory PathTooLongException detected!";

        public const string ExceptionOccured = "FileWrapper ExceptionOccured, Program termination!";
        public const string CreateDebianInstallerFails = "Create Debian installer failed!";
        public const string MissingBuiltOpcuaAppFiles = "Missing built opcuaapp files!";
        public const string MissingPublishedOpcuaAppFiles = "Missing published opcuaapp files!";
        public const string ImportInforamtionModelCommandFailure = "Information model import failure, invalid character in name '{0}'";
        public const string UnknownImportInfomrationModelCommandParam = "Unknown import information-model <command> parameter!";
        public const string InvalidInformationModelPath = "Information model import failure, invalid character in model path '{0}'";
        public const string InvalidInformationModelExtension = "Information model import failure, invalid model extension '{0}'";
        public const string InvalidInformationModelNotExistingPath = "Information model import failure, can't find model '{0}'!";
        public const string InvalidInformationModelMissingModelFile = "Information model import failure, missing model file!";
        public const string OppoHelpForImportInformationModel = "Help for import <command> called";
    }
}