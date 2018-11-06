namespace Oppo.ObjectModel
{
    public static class Constants
    {
        public const string HelloString = "Hello from OPPO";

        public static class FileExtension
        {
            public const string OppoSln = ".opposln";
        }

        public static class CommandName
        {
            public const string New = "new";
            public const string Sln = "sln";
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

        public static class SlnCommandName
        {
            public const string New = "new";            
        }

        public static class SlnNewCommandArguments
        {
            public const string Name = "-n";
            public const string VerboseName = "--name";
        }

        public static class NewCommandName
        {
            public const string Sln = "sln";
        }

        public static class NewSlnCommandArguments
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