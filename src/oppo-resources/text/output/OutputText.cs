using System;

namespace Oppo.Resources.text.output
{
    public static class OutputText
    {
        // general
        public const string GeneralCommandExecutionFailure = "Command \"oppo {0}\" failed!";
		public const string NodesetValidationFailure = "It was not possible to validate the model '{0}' against known opc-ua 'UANodeSet.xsd'!";

		// publish <command>
		public const string OpcuaappPublishFailure = "Publish failure!";
		public const string OpcuaappPublishFailureMissingExecutables = "Publish '{0}' failure! Missing client and server executable files";
		public const string OpcuaappPublishSuccess = "Publish '{0}' success!";

        // build <command>
        public const string OpcuaappBuildFailure = "Build failure!";
		public const string OpcuaappBuildFailureProjectDoesNotExist = "Build failed! Project '{0}' does not exist!";
		public const string OpcuaappBuildSuccess = "Build '{0}' success!";

        // new <command>
        public const string NewOpcuaappCommandSuccess = "An opcuaapp with name '{0}' was successfully created!";
        public const string NewOpcuaappCommandFailure = "Creating opcuaapp with name '{0}' failed!";
        public const string NewOpcuaappCommandFailureUnknownParam = "Unknown command parameter '{0}'!";
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
        public const string ImportInformationModelCommandUnknownParamFailure = "Import information model failure!";
        public const string ImportInformationModelCommandInvalidOpcuaappName = "Import information model for opcuaapp '{0}' failure!";
        public const string ImportInformationModelCommandInvalidModelPath = "Import information model failure, invalid character in model path '{0}'!";
        public const string ImportInformationModelCommandInvalidModelExtension = "Import information model failure, invalid model extension '{0}'!";
        public const string ImportInformationModelCommandNotExistingModelPath = "Import information model failure, can't find model '{0}'!";
        public const string ImportInformationModelCommandMissingModelPath = "Import information model failure, missing model name!";
		public const string ImportInformationModelCommandOpcuaappIsAClient = "Import information model failure, can not import models to client!";
		public const string ImportInforamtionModelCommandFailureCannotReadOppoprojFile = "Import information model failure, can not read oppoproj file!";
		public const string ImportInforamtionModelCommandFailureModelDuplication = "Import information model failure, opcuaapp '{0}' already has a model with name '{1}'!";
		public const string ImportInforamtionModelCommandFailureModelMissingUri = "Import information model failure, model '{0}' does not contain namespace uri!";

		// import sample <command>
		public const string ImportSampleInformationModelSuccess = "Sample information model '{0}' import success!";

        // generate information model <command>
        public const string GenerateInformationModelSuccess = "The opcuaapp with the name '{0}' was succesfully updated with the Information model '{1}'.";
        public const string GenerateInformationModelFailure = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'.";
        public const string GenerateInformationModelGenerateTypesFailure = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Can't generate datatypes '{2}'!";
        public const string GenerateInformationModelFailureMissingModel = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Can't find the model '{2}'!";
        public const string GenerateInformationModelFailureMissingFile = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Can't find the file '{2}'!";
        public const string GenerateInformationModelFailureInvalidModel = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Invalid model extension '{2}'!";
        public const string GenerateInformationModelFailureInvalidFile = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Invalid file extension '{2}'!";
        public const string GenerateInformationModelFailureUnknownParam = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Unknown command parameter '{2}'!";
        public const string GenerateInformationModelFailureEmptyOpcuaAppName = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Empty opcuaapp name!";
		public const string GenerateInformationModelRequiredModelFailure = "It was not possible to update opcuaapp with the name '{0}' with the Information model '{1}'! Can't use required model '{2}'!";

        // sln common <command>
        public const string SlnOpposlnNotFound		= "The solution file '{0}' not found!";
        public const string SlnUnknownParameter		= "Unknown command parameter '{0}'!";
		public const string SlnCouldntDeserliazeSln = "The solution file '{0}' has invalid content!";
		// sln add <command>
		public const string SlnAddSuccess					= "The opcuaapp with the name '{0}' was successfully added to solution '{1}'.";
		public const string SlnAddOpcuaappAlreadyExist		= "The opcuaapp with the name '{0}' is already a part of the solution '{1}'.";
		public const string SlnAddOpcuaappNotFound			= "The opcuaapp project file '{0}' not found!";
		public const string SlnAddCouldntDeserliazeOpcuaapp	= "The opcuaapp project file '{0}' has invalid content!";
		public const string SlnAddContainsOpcuaapp			= "The solution '{0}' already contains the opcuaapp '{1}'";
		// sln remove <command>
		public const string SlnRemoveSuccess				= "The opcuaapp with the name '{0}' was successfully removed from the solution '{1}'.";
		public const string SlnRemoveOpcuaappNameEmpty		= "The opcuaapp project name empty!";
		public const string SlnRemoveOpcuaappIsNotInSln		= "The opcuaapp project '{0}' is not a part of the solution '{1}'!";
		// sln build <command>
		public const string SlnBuildSuccess = "Projects that are part of solution '{0}' were successfully built.";
		public const string SlnPublishSuccess = "Projects that are part of solution '{0}' were successfully published.";
		public const string SlnDeploySuccess = "Projects that are part of solution '{0}' were successfully deployed.";


		// reference common
		public const string ReferenceUnknownParameter = "Unknown reference command parameter '{0}'!";
		public const string ReferenceClientOppoprojFileNotFound = "The client's oppoproj file '{0}' not found!";
		public const string ReferenceCouldntDeserliazeClient = "The client file '{0}' has invalid content!";

		// reference add <command>
		public const string ReferenceAddServerIsPartOfClientReference = "Server '{0}' is already the reference of client '{1}'!";
		public const string ReferenceAddServerOppoprojFileNotFound = "The server's oppoproj file '{0}' not found!";
		public const string ReferenceAddCouldntDeserliazeServer = "The server file '{0}' has invalid content!";
		public const string RefereneceAddSuccess = "The reference of the server with the name '{0}' was successfully added to the client '{1}'.";
		public const string ReferenceAddClientCannotBeReferred = "The opcuaapp '{0}' is a client. Client cannot refer to another client!";
		public const string ReferenceAddClientIsAServer = "The opcuaapp '{0}' is a server. Cannot add reference to server!";

		// reference remove <command>
		public const string ReferenceRemoveSuccess = "The opcuaapp with the name '{0}' was successfully removed from the server '{1}'.";
		public const string ReferenceRemoveServerNameEmpty = "The server name empty!";
		public const string ReferenceRemoveServerIsNotInClient = "The reference server '{0}' is not a part of the client '{1}'!";
	}
}
