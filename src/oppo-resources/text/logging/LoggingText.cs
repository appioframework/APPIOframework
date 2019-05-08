using System;

namespace Oppo.Resources.text.logging
{
    public static class LoggingText
    {
		// common
		public const string NodesetValidationFailure = "Nodeset '{0}' validation failure!";

	    public const string MissingRequiredParameter = "Missing required parameter in {0} <command>";
	    public const string DuplicateParameterProvided = "Duplicate parameter in {0} <command>";
	    public const string ParameterValueMissing = "Parameter value for {0} <command> missing";
	    public const string UnknownParameterProvided = "Unknown {0} <command> parameter";
	    
	    public const string CertificateGeneratorSuccess = "Generated new private key and certificate for instance '{0}'!";
	    public const string CertificateGeneratorFailureNonexistentDirectory = "Cannot generate private key and certificate! Directory '{0}' does not exist!";
	    public const string NullInputParams_Msg = "Null input parameters passed. Specify any valid parameters.";
        public const string InvalidSolutionName = "Invalid solution name!";
        public const string NewSlnCommandSuccess = "Solution with name {0} was successfully created";
        public const string UnknownNewOpcuaappCommandParam = "Unknown new opcuaapp <command> parameter!";
		public const string InvalidOpcuaappType = "Unknown opcua application type!";
		public const string InvalidServerUrl = "Invalid opcua server app url!";
		public const string InvalidServerPort = "Invalid opcua server app port!";
		public const string NewOpcuaappCommandSuccess = "opcuaapp with name {0} was successfully created";
        public const string ImportInforamtionModelCommandSuccess = "Information model '{0}' imported successfully";
        public const string ImportCertificateSuccess = "Certificate '{0}' and private key '{1}' imported successfully";
        public const string ImportCertificateFailureWrongFormat = "Certificate import failure, invalid format '{0}'";
        public const string ImportCertificateFailureWrongClientServer = "Certificate import failure, invalid use of '--server' or '--client'";
        public const string ImportCertificateFailureMissingClientServer = "Certificate import failure, specify '--server' or '--client'";
        public const string EmptyOpcuaappName = "Empty opcuaapp name!";
        public const string InvalidOpcuaappName = "Invalid opcuaapp name!";
        public const string MesonExecutableFails = "Meson Failed!";
        public const string NinjaExecutableFails = "Ninja Failed!";
        public const string NodesetCompilerExecutableFails = "Nodeset compiler Failed!";
        public const string GeneratedTypesExecutableFails = "Generate datatypes for NodeSet information-model Failed!";
        public const string BuildSuccess = "Build Success!";
		public const string BuildProjectDoesNotExist = "Build failed! Project does not exist!";
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

        public const string ValidatingModel = "Validating model '{0}' against '{1}'.";
        public const string ValidationError = "Validation error \n {0}";


		// sln common
		public const string SlnUnknownCommandParam	= "Unknown sln add command parameter!";
		public const string SlnOpposlnFileNotFound	= "Missing solution file!";
		public const string SlnCouldntDeserliazeSln	= "Couldn't deserialize sln file!";
		public const string OppoHelpForSlnCommand	= "Help for sln <command> called";
		// sln add command
		public const string SlnAddSuccess				    = "Opcuaapp project file successfully added to sln.";
        public const string SlnAddOppoprojFileNotFound		= "Missing project file!";
        public const string SlnAddCouldntDeserliazeOpcuaapp	= "Couldn't deserialize oppoproj file!";
        public const string SlnAddContainsOpcuaapp			= "The solution already contains the opcuaapp project.";
		// sln remove command
		public const string SlnRemoveSuccess			= "Opcuaapp project was successfully removed from sln.";
		public const string SlnRemoveOppoprojNameEmpty	= "Empty project name!";
		public const string SlnRemoveOpcuaappIsNotInSln = "Opcuaapp is not a part of the soluton!";
		//sln build command
		public const string SlnBuildSuccess = "Projects that are part of solution were successfully built.";
		public const string SlnPublishSuccess = "Projects that are part of solution were successfully published.";
		public const string SlnDeploySuccess = "Projects that are part of solution were successfully deployed.";


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
		public const string ImportInformationModelCommandOpcuaappIsAClient = "Import information model failure, can not import models to client!";
		public const string ImportInforamtionModelCommandFailureCannotReadOppoprojFile = "Import information model failure, can not read oppoproj file!";
		public const string ImportInforamtionModelCommandFailureModelDuplication = "Import information model failure, model is already part of opcuaapp!";
		public const string ImportInforamtionModelCommandFailureModelMissingUri = "Import information model failure, model does not contain namespace uri!";
		public const string ImportInformationModelCommandFailureInvalidTypesFlag = "Import information model failure, invalid types flag!";
		public const string ImportInformationModelCommandFailureMissingTypesName = "Import information model failute, missing types name!";
		public const string ImportInformationModelCommandFailureTypesFileDoesNotExist = "Import information model failute, types file does not exist!";
		public const string ImportInformationModelCommandFailureTypesHasInvalidExtension = "Import information model failute, types file has invalid extension!";
		public const string OppoHelpForImportInformationModel = "Help for import <command> called";

		// reference common
		public const string ReferenceUnknownCommandParam = "Unknown reference command parameter!";
		public const string ReferenceClientOppoprojFileNotFound = "Client's oppoproj file not found!";
		public const string ReferenceCouldntDeserliazeClient = "Couldn't deserialize client file!";
		public const string OppoHelpForReferenceCommand = "Help for reference <command> called";

		// reference add command
		public const string ReferenceAddSuccess = "Server reference successfully added to client project file.";
		public const string ReferenceAddServerOppoprojFileNotFound = "Server's oppoproj file not found!";
		public const string ReferenceAddCouldntDeserliazeServer = "Couldn't deserialize server file!";
		public const string ReferenceAddServerIsPartOfClientReference = "Server is already client's reference!";
		public const string ReferenceAddClientCannotBeReferred = "Client cannot refer to another Client!";
		public const string ReferenceAddClientIsAServer = "Client cannot be a server type!";
		
		// reference remove command
		public const string ReferenceRemoveSuccess = "Opcuaapp reference was successfully removed from server.";
		public const string ReferenceRemoveServerNameEmpty = "Empty server name!";
		public const string ReferenceRemoveServerIsNotInClient = "Server is not a part of the client reference list!";
	}
}