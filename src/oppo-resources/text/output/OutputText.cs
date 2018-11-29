namespace Oppo.Resources.text.output
{
    public static class OutputText
    {
        // publish <command>
        public const string OpcuaappPublishFailure = "Publish failure!";
        public const string OpcuaappPublishSuccess = "Publish '{0}' success!";

        // build <command>
        public const string OpcuaappBuildFailure = "Build failure!";
        public const string OpcuaappBuildSuccess = "Build '{0}' success!";

        // new <command>
        public const string NewOpcuaappCommandSuccess = "opcuaapp with name '{0}' was successfully created";
        public const string NewOpcuaappCommandFailure = "create opcuaapp with name '{0}' failed";
        public const string NewOpcuaappCommandFailureUnknownParam = "create opcuaapp <command> failed";
        public const string NewSlnCommandSuccess = "solution with name '{0}' was successfully created";
        public const string NewSlnCommandFailure = "create solution with name '{0}' failed";
        public const string NewSlnCommandFailureUnknownParam = "create solution <command> failed";

        // clean <command>
        public const string OpcuaappCleanFailure = "Clean failure!";
        public const string OpcuaappCleanSuccess = "Clean '{0}' success!";

        // deploy <command>
        public const string OpcuaappDeploySuccess = "Deploy '{0}' success!";
        public const string OpcuaappDeployFailure = "Deploy failure!";
        public const string OpcuaappDeployWithNameFailure = "Deploy '{0}' failure!";

        // import <command>
        public const string ImportInforamtionModelCommandSuccess = "Import information model '{0}' success!";
        public const string ImportInforamtionModelCommandFailure = "Import information model '{0}' failure!";
        public const string ImportInforamtionModelCommandUnknownParamFailure = "Import information model failure!";
        public const string ImportInforamtionModelCommandInvalidOpcuaappName = "Import information model for opcuaapp '{0}' failure!";
        public const string ImportInforamtionModelCommandInvalidModelPath = "Information model import failure, invalid character in model path '{0}'";
        public const string ImportInforamtionModelCommandInvalidModelExtension = "Information model import failure, invalid model extension '{0}'";
        public const string ImportInforamtionModelCommandNotExistingModelPath = "Information model import failure, can't find model '{0}'!";
        public const string ImportInforamtionModelCommandMissingModelPath = "Information model import failure, missing model name!";
    }
}
