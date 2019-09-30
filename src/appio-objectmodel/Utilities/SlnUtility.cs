/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using System.IO;
using Newtonsoft.Json;
using Appio.Resources.text.logging;
using Appio.Resources.text.output;

namespace Appio.ObjectModel
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

		static public bool ValidateSolution(ref ResultMessages messages, string solutionName, IFileSystem fileSystem)
		{
			// check if *.appiosln file exists
			var solutionFullName = solutionName + Constants.FileExtension.Appiosln;
			if (string.IsNullOrEmpty(solutionName) || !fileSystem.FileExists(solutionFullName))
			{
				messages.LoggerMessage = LoggingText.SlnAppioslnFileNotFound;
				messages.OutputMessage = string.Format(OutputText.SlnAppioslnNotFound, solutionFullName);
				return false;
			}

			return true;
		}
	}
}