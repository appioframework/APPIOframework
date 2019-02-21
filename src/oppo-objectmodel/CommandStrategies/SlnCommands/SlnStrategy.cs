using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<SlnStrategy> _factory;

        public SlnStrategy(ICommandFactory<SlnStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Sln;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var commandName = inputParamsArray.ElementAtOrDefault(0);
            var commandParams = inputParamsArray.Skip(1).ToArray();
            var command = _factory.GetCommand(commandName);
            return command.Execute(commandParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnCommand;
        }

		static public Solution DeserializeSolutionFile(string solutionFullName, IFileSystem fileSystem)
		{
			var slnMemoryStream = fileSystem.ReadFile(solutionFullName);
			StreamReader readerSln = new StreamReader(slnMemoryStream);
			var slnContent = readerSln.ReadToEnd();

			Solution oppoSolution;
			try
			{
				oppoSolution = JsonConvert.DeserializeObject<Solution>(slnContent);
				if (oppoSolution == null)
				{
					throw null;
				}
			}
			catch (Exception)
			{
				return null;
			}
			slnMemoryStream.Close();
			slnMemoryStream.Dispose();

			return oppoSolution;
		}
    }
}
