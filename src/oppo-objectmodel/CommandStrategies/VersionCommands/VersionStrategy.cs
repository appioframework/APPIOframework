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
            foreach (var assemblyInfo in _reflection.GetOppoAssemblyInfos())
            {
                _writer.WriteLine($"{assemblyInfo.AssemblyName}: {assemblyInfo.AssemblyVersion.Version}, file {assemblyInfo.AssemblyFileVersion.Version}");
            }

            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.VersionCommand;
        }
    }
}
