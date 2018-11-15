using System;

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
        public const string EmptyOpcuaappName = "Empty opcuaapp name!";
        public const string InvalidOpcuaappName = "Invalid opcuaapp name!";
        public const string MesonExecutableFails = "Meson Failed!";
        public const string NinjaExecutableFails = "Ninja Failed!";
        public const string BuildSuccess = "Build Success!";
        public const string OpcuaappPublishedSuccess = "Publish Success!";
        public const string OpcuaappPublishHelpCalled = "Publish <command> --help called";        
    }
}