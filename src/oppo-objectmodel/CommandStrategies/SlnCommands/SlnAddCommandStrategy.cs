using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnAddCommandStrategy : ICommand<SlnStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public SlnAddCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.SlnCommandName.Add;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray    = inputParams.ToArray();
            var solutionNameFlag    = inputParamsArray.ElementAtOrDefault(0);
            var solutionName        = inputParamsArray.ElementAtOrDefault(1);
            var projectNameFlag     = inputParamsArray.ElementAtOrDefault(2);
            var projectName         = inputParamsArray.ElementAtOrDefault(3);

            var outputMessages = new MessageLines();



            // check if solutionNameFlag is valid
            if(solutionNameFlag != Constants.SlnAddCommandArguments.Solution && solutionNameFlag != Constants.SlnAddCommandArguments.VerboseSolution)
			{
				OppoLogger.Warn(LoggingText.SlnAddUknownCommandParam);
				outputMessages.Add(string.Format(OutputText.SlnAddUnknownParameter, solutionNameFlag), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if *.opposln file exists
			var solutionFullName = _fileSystem.CombinePaths(solutionName + Constants.FileExtension.OppoSln);
			if (string.IsNullOrEmpty(solutionName) || !_fileSystem.FileExists(solutionFullName))
			{
				OppoLogger.Warn(LoggingText.SlnAddOpposlnFileNotFound);
				outputMessages.Add(string.Format(OutputText.SlnAddSlnNotFound, solutionFullName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// check if projectNameFlag is valid
			if (projectNameFlag != Constants.SlnAddCommandArguments.Project && projectNameFlag != Constants.SlnAddCommandArguments.VerboseProject)
            {
                OppoLogger.Warn(LoggingText.SlnAddUknownCommandParam);
                outputMessages.Add(string.Format(OutputText.SlnAddUnknownParameter, projectNameFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // check if *.oppoproj file exists
            var oppoprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject);
            if (string.IsNullOrEmpty(projectName) || !_fileSystem.FileExists(oppoprojFilePath))
            {
                OppoLogger.Warn(LoggingText.SlnAddOppoprojFileNotFound);
                outputMessages.Add(string.Format(OutputText.SlnAddOpcuaappNotFound, oppoprojFilePath), string.Empty);
                return new CommandResult(false, outputMessages);
            }



            // deserialize *.opposln file
            var slnMemoryStream = _fileSystem.ReadFile(solutionFullName);
            StreamReader readerSln = new StreamReader(slnMemoryStream);
            var slnContent = readerSln.ReadToEnd();
            Solution oppoSolution;
            try
            {
                oppoSolution = JsonConvert.DeserializeObject<Solution>(slnContent);
				if (oppoSolution == null)
				{
					throw new System.ArgumentNullException();
				}
            }
            catch(Exception)
            {
                OppoLogger.Warn(LoggingText.SlnAddCouldntDeserliazeSln);
                outputMessages.Add(string.Format(OutputText.SlnAddCouldntDeserliazeSln, solutionName), string.Empty);
                return new CommandResult(false, outputMessages);
            }
			slnMemoryStream.Close();
			slnMemoryStream.Dispose();

            // deserialize *.oppoproj file
            var opcuaappMemoryStream = _fileSystem.ReadFile(oppoprojFilePath);
            StreamReader readerOpcuaapp = new StreamReader(opcuaappMemoryStream);
            var opcuaappContent = readerOpcuaapp.ReadToEnd();
            Opcuaapp oppoProj;
            try
            {
                oppoProj = JsonConvert.DeserializeObject<Opcuaapp>(opcuaappContent);
            }
            catch(Exception)
            {
                OppoLogger.Warn(LoggingText.SlnAddCouldntDeserliazeOpcuaapp);
                outputMessages.Add(string.Format(OutputText.SlnAddCouldntDeserliazeOpcuaapp, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
			}
			opcuaappMemoryStream.Close();
			opcuaappMemoryStream.Dispose();



            // check if sln does not contain opcuaapp yet
            if (!oppoSolution.Projects.Any(x => x.Name == oppoProj.Name))
            {
                // add opcuaapp to sln
                oppoSolution.Projects.Add(oppoProj);

				// serialize and write sln
				var slnNewContent = JsonConvert.SerializeObject(oppoSolution, Formatting.Indented);
				_fileSystem.WriteFile(solutionFullName, new List<string> { slnNewContent });
            }
            else
            {
                OppoLogger.Info(LoggingText.SlnAddContainsOpcuaapp);
                outputMessages.Add(string.Format(OutputText.SlnAddContainsOpcuaapp, solutionName, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }



			// exit method with success
            OppoLogger.Info(LoggingText.SlnAddSuccess);                        
            outputMessages.Add(string.Format(OutputText.SlnAddSuccess, projectName, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanNameArgumentCommandDescription;
        }
    }
}