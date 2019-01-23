namespace Oppo.ObjectModel
{
    public static class Constants
    {
        public const string HelloString = "Hello from OPPO";
        public const string IncludeSnippet = "#include";
        public const string server = "server";

        public static class FileExtension
        {
            public const string OppoSln = ".opposln";
            public const string OppoProject = ".oppoproj";
            public const string DebianInstaller = ".deb";
            public const string InformationModel = ".xml";
            public const string CFile = ".c";
        }

        public static class CommandName
        {
            public const string New = "new";
            public const string Hello = "hello";
            public const string Build = "build";
            public const string Publish = "publish";
            public const string Help = "help";
            public const string HelpDash = "-h";
            public const string HelpDashVerbose = "--help";
            public const string ShortHelp = "?";
            public const string Version = "version";
            public const string Clean = "clean";
            public const string Deploy = "deploy";
            public const string Import = "import";
            public const string Generate = "generate";
            public const string GenerateInformationModel = "information-model";
        }

        public static class CommandResults
        {
            public const string Success = "success";
            public static string Failure { get; set; } = "failure";
        }

        public static class NewCommandName
        {
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Sln = "sln";
            public const string OpcuaApp = "opcuaapp";
        }

        public static class DirectoryName
        {
            public const string SourceCode = "src";
            public const string ClientApp = "client";
            public const string ServerApp = "server";
            public const string MesonBuild = "build";
            public const string Publish = "publish";
            public const string Deploy = "deploy";
            public const string Temp = "temp";
            public const string OpcuaappInstaller = "oppo-opcuaapp";
            public const string Usr = "usr";
            public const string Bin = "bin";
            public const string Models = "models";
            public const string InformationModels = "information-models";
        }

        public static class ExecutableName
        {
            public const string Meson = "meson";
            public const string Ninja = "ninja";
            public const string AppClient = "client-app";
            public const string AppServer = "server-app";
            public const string CreateDebianInstaller = "dpkg";
            public static readonly string CreateDebianInstallerArguments = "--build " + DirectoryName.OpcuaappInstaller;
            public const string OppoResourcesDll = "oppo-resources.dll";

            public const string NodsetCompiler = @"python3";
            // 0 xml model source path
            // 1 output directory with name for generated files and method
            public const string NodsetCompilerArguments = @"/etc/oppo/tools/open62541/v0.3.0/nodeset-compiler/nodeset_compiler.py --types-array=UA_TYPES --existing /etc/oppo/tools/open62541/v0.3.0/existing-nodes/Opc.Ua.NodeSet2.xml --xml {0} {1}";
        }

        public static class FileName
        {
            public const string SourceCode_main_c               = "main.c";
            public const string SourceCode_models_c             = "models.c";
            public const string SourceCode_nodeSetFunctions_c   = "nodeSetFunctions.c";
            public const string SourceCode_meson_build          = "meson.build";
            public const string SampleInformationModelFile      = "OpcUaDiModel.xml";
        }

        public static class NewSlnCommandArguments
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class NewOpcuaAppCommandArguments
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }


        public static class BuildCommandArguments
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
            public const string VerboseHelp = "--help";
            public const string Help = "-h";
        }

        public static class PublishCommandArguments
        {
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class CleanCommandArguments
        {
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class DeployCommandArguments
        {
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class GenerateInformationModeCommandArguments
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
            public const string Model = "-m";
            public const string VerboseModel = "--model";
            public const string VerboseHelp = "--help";
            public const string Help = "-h";
        }

        public static class GenerateCommandArguments
        {
            public const string InformationModel = "information-model";
            public const string VerboseHelp = "--help";
            public const string Help = "-h";
        }

        public static class ImportInformationModelCommandName
        {
            public const string InformationModel = "information-model";
        }

        public static class GenerateInformationModelCommandName
        {
            public const string InformationModel = "information-model";
        }

        public static class ImportInformationModelCommandArguments
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
            public const string Path = "-p";
            public const string VerbosePath = "--path";
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
            public const string Sample = "-s";
            public const string VerboseSample = "--sample";

        }

        public static class NodeSetFunctioncContent
        {
            public const string ReturnLine = "return UA_STATUSCODE_GOOD;";
            public const string FunctionSnippetPart1 = "\tif ({0}(server) != UA_STATUSCODE_GOOD)";
            public const string FunctionSnippetPart2 = "\t{";
            public const string FunctionSnippetPart3 = "\t\tUA_LOG_ERROR(UA_Log_Stdout, UA_LOGCATEGORY_SERVER, \"Could not add the {0} nodeset. Check previous output for any error.\");";
            public const string FunctionSnippetPart4 = "\t\treturn UA_STATUSCODE_BADUNEXPECTEDERROR;";
            public const string FunctionSnippetPart5 = "\t}";
                

        }
    }
}