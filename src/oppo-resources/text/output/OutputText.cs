using System;

namespace Oppo.Resources.text.output
{
    public static class OutputText
    {
        // publish <command>
        public const string OpcuaappPublishFailure = "Publish failure!";
        public const string OpcuaappPublishSuccess = "Publish '{0}' Success!";

        // build <command>
        public const string OpcuaappBuildFailure = "Build failure!";
        public const string OpcuaappBuildSuccess = "Build '{0}' Success!";

        // new <command>
        public const string NewOpcuaappCommandSuccess = "opcuaapp with name '{0}' was successfully created";
        public const string NewOpcuaappCommandFailure = "create opcuaapp with name '{0}' failed";
        public const string NewOpcuaappCommandFailureUnknownParam = "create opcuaapp <command> failed";
        public const string NewSlnCommandSuccess = "solution with name '{0}' was successfully created";
        public const string NewSlnCommandFailure = "create solution with name '{0}' failed";
        public const string NewSlnCommandFailureUnknownParam = "create solution <command> failed";
    }
}
