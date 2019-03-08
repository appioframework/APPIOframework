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
			private string loggerMessage;
			public string setLoggerMessage { set { getLoggerMessage = value; } }
			public string getLoggerMessage { get; private set; }
		
			private string outputMessage;
			public string setOutputMessage { set { getOutputMessage = value; } }
			public string getOutputMessage { get; private set; }
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

		static public bool ValidateSolution(ref ResultMessages messages, string solutionNameFlag, string solutionName, IFileSystem fileSystem)
		{
			// check if solutionNameFlag is valid
			if (solutionNameFlag != Constants.SlnAddCommandArguments.Solution && solutionNameFlag != Constants.SlnAddCommandArguments.VerboseSolution)
			{
				messages.setLoggerMessage = LoggingText.SlnUnknownCommandParam;
				messages.setOutputMessage = string.Format(OutputText.SlnUnknownParameter, solutionNameFlag);
				return false;
			}

			// check if *.opposln file exists
			var solutionFullName = solutionName + Constants.FileExtension.OppoSln;
			if (string.IsNullOrEmpty(solutionName) || !fileSystem.FileExists(solutionFullName))
			{
				messages.setLoggerMessage = LoggingText.SlnOpposlnFileNotFound;
				messages.setOutputMessage = string.Format(OutputText.SlnOpposlnNotFound, solutionFullName);
				return false;
			}

			return true;
		}
	}
}