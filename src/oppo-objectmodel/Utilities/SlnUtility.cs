using System;
using System.IO;
using Newtonsoft.Json;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel
{
	public static class SlnUtility
	{
		public struct ResultMessages
		{
			public string LoggerMessage { get; set; }
			public string OutputMessage { get; set; }
		};

		static public TDependance DeserializeFile<TDependance>(string jsonFileFullName, IFileSystem fileSystem) where TDependance : class
		{
			TDependance deserializedData;

			using (var memoryStream = fileSystem.ReadFile(jsonFileFullName))
			{
				StreamReader reader = new StreamReader(memoryStream);
				var jsonFileContent = reader.ReadToEnd();

				try
				{
					deserializedData = JsonConvert.DeserializeObject<TDependance>(jsonFileContent);
					if (deserializedData == null)
					{
						throw null;
					}
				}
				catch (Exception)
				{
					return null;
				}
			}

			return deserializedData;
		}

		static public bool DeserializeSolution(ref ResultMessages messages, ref Solution solution, string solutionNameFlag, string solutionName, IFileSystem fileSystem)
		{
			// validate solution name
			if (!ValidateSolution(ref messages, solutionNameFlag, solutionName, fileSystem))
			{
				return false;
			}

			// deserialize *.opposln file
			var solutionFullName = solutionName + Constants.FileExtension.OppoSln;
			solution = DeserializeFile<Solution>(solutionFullName, fileSystem);
			if (solution == null)
			{
				messages.LoggerMessage = LoggingText.SlnCouldntDeserliazeSln;
				messages.OutputMessage = string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName);
				return false;
			}

			return true;
		}

		static public bool ValidateSolution(ref ResultMessages messages, string solutionNameFlag, string solutionName, IFileSystem fileSystem)
		{
			// check if solutionNameFlag is valid
			if (solutionNameFlag != Constants.SlnAddCommandArguments.Solution && solutionNameFlag != Constants.SlnAddCommandArguments.VerboseSolution)
			{
				messages.LoggerMessage = LoggingText.SlnUnknownCommandParam;
				messages.OutputMessage = string.Format(OutputText.SlnUnknownParameter, solutionNameFlag);
				return false;
			}

			// check if *.opposln file exists
			var solutionFullName = solutionName + Constants.FileExtension.OppoSln;
			if (string.IsNullOrEmpty(solutionName) || !fileSystem.FileExists(solutionFullName))
			{
				messages.LoggerMessage = LoggingText.SlnOpposlnFileNotFound;
				messages.OutputMessage = string.Format(OutputText.SlnOpposlnNotFound, solutionFullName);
				return false;
			}

			return true;
		}
	}
}