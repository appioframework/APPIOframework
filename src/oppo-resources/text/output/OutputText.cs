using System;

namespace Oppo.Resources.text.output
{
    public static class OutputText
    {
        // general
        public const string GeneralCommandExecutionFailure = "Command \"oppo {0}\" failed!";

        // publish <command>
        public const string OpcuaappPublishFailure = "Publish failure!";
        public const string OpcuaappPublishSuccess = "Publish '{0}' success!";

        // build <command>
        public const string OpcuaappBuildFailure = "Build failure!";
        public const string OpcuaappBuildSuccess = "Build '{0}' success!";

        // new <command>
        public const string NewOpcuaappCommandSuccess = "An opcuaapp with name '{0}' was successfully created!";
        public const string NewOpcuaappCommandFailure = "Creating opcuaapp with name '{0}' failed!";
        public const string NewOpcuaappCommandFailureUnknownParam = "Creating opcuaapp failed!";
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

        // import sample <command>
        public const string ImportSampleInformationModelSuccess = "Sample information model '{0}' import success!";
    }
}
