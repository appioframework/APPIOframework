namespace Oppo.ObjectModel
{
    public static class Constants
    {
        public const string HelloString = "Hello from OPPO";

        public static class FileExtension
        {
            public const string OppoSln = ".opposln";
            public const string OppoProject = ".oppoproj";
            public const string DebianInstaller = ".deb";
            public const string InformationModel = ".xml";
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
            public const string ShortHelp= "?";
            public const string Version= "version";
            public const string Clean = "clean";
            public const string Deploy = "deploy";
            public const string Import = "import";
        }

        public static class CommandResults
        {
            public const string Success = "success";
            public const string Failure = "failure";
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
        }

        public static class FileName
        {
            public const string SourceCode_main_c = "main.c";
            public const string SourceCode_meson_build = "meson.build";
            public const string SourceCode_open62541_c = "open62541.c";
            public const string SourceCode_open62541_h = "open62541.h";
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

        public static class ImportInformationModelCommandName
        {
            public const string InformationModel = "information-model";
        }

        public static class ImportInformationModelCommandArguments
        {
            public const string Path = "-p";
            public const string VerbosePath = "--path";
            public const string Help = "-h";
            public const string VerboseHelp = "--help";
        }
    }
}