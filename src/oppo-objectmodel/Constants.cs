namespace Oppo.ObjectModel
{
    public static class Constants
    {
        public const string HelloString = "Hello from OPPO";

        public static class FileExtension
        {
            public const string OppoSln = ".opposln";
            public const string OppoProject = ".oppoproj";
        }

        public static class CommandName
        {
            public const string New = "new";
            public const string Hello = "hello";
            public const string Build = "build";
            public const string Publish = "publish";
            public const string Help = "help";
            public const string ShortHelp= "?";
        }

        public static class CommandResults
        {
            public const string Success = "success";
            public const string Failure = "failure";
        }

        public static class NewCommandName
        {
            public const string Sln = "sln";
            public const string OpcuaApp = "opcuaapp";
        }

        public static class DirectoryName
        {
            public const string SourceCode = "src";
        }

        public static class FileName
        {
            public const string SourceCode_main_c = "main.c";
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
            public const string ModeAll = "-all";
            public const string ModelRebuild= "-r";
        }

        public static class PublishCommandArguments
        {
            public const string ModeAll = "-all";
        }
    }
}