using System;

namespace Appio.Resources.text.output
{
    public static class OutputText
    {
        // general
        public const string GeneralCommandExecutionFailure = "Command \"appio {0}\" failed!";
		public const string NodesetValidationFailure = "It was not possible to validate the model '{0}' against known opc-ua 'UANodeSet.xsd'!";

		// parameter resolver
	    public const string UnknownParameterProvided = "Unrecognized parameter '{0}'. Expected parameter {1} instead";
	    public const string MissingRequiredParameter = "Missing required parameter {0}";
	    public const string ParameterValueMissing = "Parameter value for '{0}' empty or not provided";
	    public const string DuplicateParameterProvided = "Duplicate parameter '{0}'";

	    //publish <command>
		public const string OpcuaappPublishFailure = "Publish failure!";
		public const string OpcuaappPublishFailureMissingExecutables = "Publish '{0}' failure! Missing client and server executable files";
		public const string OpcuaappPublishSuccess = "Publish '{0}' success!";

        // build <command>
        public const string OpcuaappBuildFailure = "Build failure!";
		public const string OpcuaappBuildFailureProjectDoesNotExist = "Build failed! Project '{0}' does not exist!";
		public const string OpcuaappBuildSuccess = "Build '{0}' success!";

        // new <command>
        public const string NewOpcuaappCommandSuccess = "An opcuaapp with name '{0}' was successfully created!";
		public const string NewOpcuaappCommandFailureInvalidProjectName = "Invalid opcuaapp name '{0}'!";
		public const string NewOpcuaappCommandFailureUnknownProjectType = "Unknown opcua application type '{0}'!";
		public const string NewOpcuaappCommandFailureInvalidServerUrl = "Invalid server url '{0}'!";
		public const string NewOpcuaappCommandFailureInvalidServerPort = "Invalid server port '{0}'!";
		public const string NewSlnCommandSuccess = "A solution with name '{0}' was successfully created!";
        public const string NewSlnCommandFailure = "Creating solution with name '{0}' failed!";
        public const string NewSlnCommandFailureUnknownParam = "Creating solution failed!";

        // clean <command>
        public const string OpcuaappCleanFailure = "Clean failure!";
        public const string OpcuaappCleanSuccess = "Clean '{0}' success!";

        // deploy <command>
        public const string OpcuaappDeploySuccess = "Deploy '{0}' success!";
        public const string OpcuaappDeployFailure = "Deploy failure!";
        public const string OpcuaappDeployWithNameFailure = "Deploy '{0}' failure!";

        // import <command>
        public const string ImportInformationModelCommandSuccess = "Import information model '{0}' success!";
        public const string ImportInformationModelCommandInvalidOpcuaappName = "Import information model for opcuaapp '{0}' failure!";
        public const string ImportInformationModelCommandInvalidModelPath = "Import information model failure, invalid character in model path '{0}'!";
        public const string ImportInformationModelCommandInvalidModelExtension = "Import information model failure, invalid model extension '{0}'!";
        public const string ImportInformationModelCommandNotExistingModelPath = "Import information model failure, can't find model '{0}'!";
        public const string ImportInformationModelCommandMissingModelPath = "Import information model failure, missing model name!";
		public const string ImportInformationModelCommandOpcuaappIsAClient = "Import information model failure, can not import models to client!";
		public const string ImportInforamtionModelCommandFailureCannotReadAppioprojFile = "Import information model failure, can not read appioproj file!";
		public const string ImportInforamtionModelCommandFailureModelNameDuplication = "Import information model failure, opcuaapp '{0}' already has a model with name '{1}'!";
		public const string ImportInforamtionModelCommandFailureModelUriDuplication = "Import information model failure, opcuaapp '{0}' already has a model with uri '{1}'!";
		public const string ImportInforamtionModelCommandFailureModelMissingUri = "Import information model failure, model '{0}' does not contain namespace uri!";
		public const string ImportInformationModelCommandFailureTypesFileDoesNotExist = "Import information model failute, types file '{0}' does not exist!";
		public const string ImportInformationModelCommandFailureTypesHasInvalidExtension = "Import information model failute, types file has invalid extension '{0}'!";

		// import sample <command>
		public const string ImportCertificateCommandSuccess = "Imported key & certificate successfully!";
        public const string ImportCertificateCommandWrongServerClient = "Specified --server/--client incorrectly!";// import sample <command>
		public const string ImportSampleInformationModelSuccess = "Sample information model '{0}' import success!";

        // generate certificate <command>
        public const string GenerateCertificateCommandSuccess = "Generated new certificate(s) for opcuaapp '{0}'";
        public const string GenerateCertificateCommandFailureNotParsable = "Certificate generation error, arguments could not be parsed!";
        public const string GenerateCertificateCommandFailureNotFound = "Certificate generation error, project '{0}' could not be found!";
        
        // generate information model <command>
		public const string GenerateInformationModelSuccess = "All information model were successfully generated got opcuaapp '{0}'.";
        public const string GenerateInformationModelFailure = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'.";
        public const string GenerateInformationModelGenerateTypesFailure = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Can't generate datatypes '{2}'!";
        public const string GenerateInformationModelFailureMissingModel = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Can't find the model '{2}'!";
        public const string GenerateInformationModelFailureMissingFile = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Can't find the file '{2}'!";
        public const string GenerateInformationModelFailureInvalidModel = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Invalid model extension '{1}'!";
        public const string GenerateInformationModelFailureInvalidFile = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Invalid file extension '{2}'!";
        public const string GenerateInformationModelFailureEmptyModelName = "It was not possible to update opcuaapp with the name '{0}'! Empty model name!";
        public const string GenerateInformationModelFailureCouldntDeserliazeOpcuaapp = "It was not possible to update opcuaapp with the name '{0}'! Couldn't read project file '{1}'!";
        public const string GenerateInformationModelFailuteOpcuaappIsAClient = "It was not possible to update opcuaapp with the name '{0}'! Can not generate models for client!";
        public const string GenerateInformationModelInvalidModelsList = "It was not possible to update opcuaapp with the name '{0}'! Provided model list contains duplications or misses some models!";
        public const string GenerateInformationModelCircularDependency = "It was not possible to update opcuaapp with the name '{0}'! Models refer to each other in endless loop!!";


        // sln common <command>
        public const string SlnAppioslnNotFound		= "The solution file '{0}' not found!";
		public const string SlnCouldntDeserliazeSln = "The solution file '{0}' has invalid content!";
		// sln add <command>
		public const string SlnAddSuccess					= "The opcuaapp with the name '{0}' was successfully added to solution '{1}'.";
		public const string SlnAddOpcuaappNotFound			= "The opcuaapp project file '{0}' not found!";
		public const string SlnAddCouldntDeserliazeOpcuaapp	= "The opcuaapp project file '{0}' has invalid content!";
		public const string SlnAddContainsOpcuaapp			= "The solution '{0}' already contains the opcuaapp '{1}'";
		// sln remove <command>
		public const string SlnRemoveSuccess				= "The opcuaapp with the name '{0}' was successfully removed from the solution '{1}'.";
		public const string SlnRemoveOpcuaappIsNotInSln		= "The opcuaapp project '{0}' is not a part of the solution '{1}'!";
		// sln build <command>
		public const string SlnBuildSuccess = "Projects that are part of solution '{0}' were successfully built.";
		public const string SlnPublishSuccess = "Projects that are part of solution '{0}' were successfully published.";
		public const string SlnDeploySuccess = "Projects that are part of solution '{0}' were successfully deployed.";


		// reference common
		public const string ReferenceClientAppioprojFileNotFound = "The client's appioproj file '{0}' not found!";
		public const string ReferenceCouldntDeserliazeClient = "The client file '{0}' has invalid content!";

		// reference add <command>
		public const string ReferenceAddServerIsPartOfClientReference = "Server '{0}' is already the reference of client '{1}'!";
		public const string ReferenceAddServerAppioprojFileNotFound = "The server's appioproj file '{0}' not found!";
		public const string ReferenceAddCouldntDeserliazeServer = "The server file '{0}' has invalid content!";
		public const string RefereneceAddSuccess = "The reference of the server with the name '{0}' was successfully added to the client '{1}'.";
		public const string ReferenceAddClientCannotBeReferred = "The opcuaapp '{0}' is a client. Client cannot refer to another client!";
		public const string ReferenceAddClientIsAServer = "The opcuaapp '{0}' is a server. Cannot add reference to server!";

		// reference remove <command>
		public const string ReferenceRemoveSuccess = "The opcuaapp with the name '{0}' was successfully removed from the server '{1}'.";
		public const string ReferenceRemoveServerIsNotInClient = "The reference server '{0}' is not a part of the client '{1}'!";
	}
}
