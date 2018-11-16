using System.Collections.Generic;
using Oppo.ObjectModel.Extensions;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.CommandStrategies.VersionCommands
{
    public class VersionStrategy : ICommand<ObjectModel>
    {
        private readonly IReflection _reflection;

        public VersionStrategy(IReflection reflection)
        {
            _reflection = reflection;
        }

        public string Name => Constants.CommandName.Version;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var printableInfos = new Dictionary<string, string>();
            foreach (var info in _reflection.GetOppoAssemblyInfos())
            {
                printableInfos.Add(info.AssemblyName, string.Format(Resources.text.version.VersionText.AssemblyVersionInfo, info.AssemblyVersion.ToPrintableString()));
            }

            OppoLogger.Info(LoggingText.VersionCommandCalled);
            return new CommandResult(true, string.Empty, printableInfos);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.VersionCommand;
        }
    }
}
