using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.VersionCommands
{
    public class VersionStrategy : ICommandStrategy
    {
        private readonly IReflection _reflection;
        private readonly IWriter _writer;

        public VersionStrategy(IReflection reflection, IWriter writer)
        {
            _reflection = reflection;
            _writer = writer;
        }

        public string Name => Constants.CommandName.Version;

        public string Execute(IEnumerable<string> inputsParams)
        {
            var printableInfos = new Dictionary<string, string>();
            foreach (var assemblyInfo in _reflection.GetOppoAssemblyInfos())
            {
                var versionString = $"{assemblyInfo.AssemblyVersion.Major}.{assemblyInfo.AssemblyVersion.Minor}.{assemblyInfo.AssemblyVersion.Revision}";
                printableInfos.Add(assemblyInfo.AssemblyName, string.Format(Resources.text.version.VersionText.AssemblyVersionInfo, versionString));
            }

            _writer.WriteLines(printableInfos);

            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.VersionCommand;
        }
    }
}
