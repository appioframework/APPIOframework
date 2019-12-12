/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;
using Appio.Resources.text.output;
using Appio.Resources.text.logging;

namespace Appio.ObjectModel
{
	public struct RequiredModelsData
	{
		public string ModelName { get; set; }
		public bool RequiredTypes { get; set; }
		public RequiredModelsData(string modelName, bool requiredTypes)
		{
			ModelName = modelName;
			RequiredTypes = requiredTypes;
		}
	}

	public class NodesetGenerator : INodesetGenerator
    {
		private readonly IFileSystem _fileSystem;
		private readonly IModelValidator _modelValidator;

		private string _outputMessage;
		
		public NodesetGenerator(IFileSystem fileSystem, IModelValidator modelValidator)
        {
			_fileSystem = fileSystem;
			_modelValidator = modelValidator;
			_outputMessage = string.Empty;
		}

		public string GetOutputMessage()
		{
			return _outputMessage;
		}
        		
		private void CreateDirectoryForGeneratedModelsSourceCode(string srcDirectory)
		{
			var pathToCreate = _fileSystem.CombinePaths(srcDirectory, Constants.DirectoryName.InformationModels);
			if (!_fileSystem.DirectoryExists(pathToCreate))
			{
				_fileSystem.CreateDirectory(pathToCreate);
			}
		}

		public bool GenerateNodesetSourceCodeFiles(string projectName, IModelData modelData, List<RequiredModelsData> requiredModelsData)
		{
			// Verify if nodeset file name is not empty
			if (string.IsNullOrEmpty(modelData.Name))
			{
				AppioLogger.Warn(LoggingText.GenerateInformationModelFailureEmptyModelName);
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureEmptyModelName, projectName);
				return false;
			}

			// Verify nodeset extension
			if (_fileSystem.GetExtension(modelData.Name) != Constants.FileExtension.InformationModel)
			{
				AppioLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, modelData.Name));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidModel, projectName, modelData.Name);
				return false;
			}

			// Verify if nodeset file exists
			var modelPath = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Models, modelData.Name);
			if (!_fileSystem.FileExists(modelPath))
			{
				AppioLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, modelData.Name));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingModel, projectName, modelData.Name, modelPath);
				return false;
			}

			// Validate nodeset
			if (!_modelValidator.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName))
			{
				AppioLogger.Warn(string.Format(LoggingText.NodesetValidationFailure, modelData.Name));
				_outputMessage = string.Format(OutputText.NodesetValidationFailure, modelData.Name);
				return false;
			}
			
			// Create a directory for generated C code
			var srcDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			CreateDirectoryForGeneratedModelsSourceCode(srcDirectory);
			
			// Build nodeset compiler script arguments
			var modelName = _fileSystem.GetFileNameWithoutExtension(modelData.Name);
			var nodesetCompilerArgs = BuildNodesetCompilerArgs(modelName, modelData, requiredModelsData);
			
			// Execute nodeset compiler call
			var nodesetResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs);
			if (!nodesetResult)
			{
				AppioLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailure, projectName, modelData.Name);
				return false;
			}

			// Add nodeset header file to server's meson build
			AdjustServerMesonBuildCFile(srcDirectory, modelName);

			// Add nodeset function call to server code
			AdjustLoadInformationModelsCFile(srcDirectory, modelName);

			// Add method callback functioncs to server code
			AdjustMainCallbacksCFile(srcDirectory, modelPath, modelData);
			
			return true;
		}

		private string BuildNodesetCompilerArgs(string modelName, IModelData modelData, List<RequiredModelsData> requiredModelsData)
		{
			// Build model source and target paths
			var modelSourceRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, modelData.Name);
			var modelTargetRelativePath = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName);

			// Build nodeset compiler script arguments:
			// add compiler path, internal headers flag and basic nodeset types
			StringBuilder compilerArgs = new StringBuilder(Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes));
			// add types for each required nodeset
			foreach(var nodeset in requiredModelsData)
			{
				var requiredModelTypes = nodeset.RequiredTypes ? (_fileSystem.GetFileNameWithoutExtension(nodeset.ModelName) + Constants.InformationModelsName.Types).ToUpper() : Constants.ExecutableName.NodesetCompilerBasicTypes;
				compilerArgs.Append(string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, requiredModelTypes));
			}
			// add currently compiled nodeset types and basic nodeset path
			var typesNameForScriptCall = string.IsNullOrEmpty(modelData.Types) ? Constants.ExecutableName.NodesetCompilerBasicTypes : (modelName + Constants.InformationModelsName.Types).ToUpper();
			compilerArgs.Append(string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, typesNameForScriptCall) + string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset));
			// add nodeset path for each required nodeset
			foreach(var nodeset in requiredModelsData)
			{
				var requiredModelRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, nodeset.ModelName);
				compilerArgs.Append(string.Format(Constants.ExecutableName.NodesetCompilerExisting, requiredModelRelativePath));
			}
			// add currently compiled nodeset path
			compilerArgs.Append(string.Format(Constants.ExecutableName.NodesetCompilerXml, modelSourceRelativePath, modelTargetRelativePath));

			return compilerArgs.ToString();
		}

		// Adding header file include to server's meson build
		private void AdjustServerMesonBuildCFile(string srcDirectory, string fileNameToInclude)
		{
			var sourceFileSnippet = string.Format(Constants.InformationModelsName.FileSnippet, fileNameToInclude);

			using (var modelsFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_meson_build)))
			{
				var currentFileContentLineByLine = ParseStreamToListOfString(modelsFileStream);

				if (!currentFileContentLineByLine.Any(x => x.Contains(sourceFileSnippet)))
				{
					var lastFunctionLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains("]"));
					if (lastFunctionLinePosition != -1)
					{
						currentFileContentLineByLine.Insert(lastFunctionLinePosition, sourceFileSnippet);
					}
					_fileSystem.WriteFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_meson_build), currentFileContentLineByLine);
				}
			}
		}

		// Adding nodeset function call to server code
		private void AdjustLoadInformationModelsCFile(string srcDirectory, string functionName)
        {
            var functionSnippet = string.Format(Constants.LoadInformationModelsContent.FunctionSnippetPart1,functionName);

			List<string> currentFileContentLineByLine;
			using (var loadInformationModelsFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_loadInformationModels_c)))
			{
				currentFileContentLineByLine = ParseStreamToListOfString(loadInformationModelsFileStream);
			}
			
			if (!currentFileContentLineByLine.Any(x => x.Contains(functionSnippet.Substring(1))))
			{
				var lastFunctionLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains(Constants.LoadInformationModelsContent.ReturnLine));
				if (lastFunctionLinePosition != Constants.NumericValues.TextNotFound)
				{
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Empty);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.LoadInformationModelsContent.FunctionSnippetPart5);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.LoadInformationModelsContent.FunctionSnippetPart4);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Format(Constants.LoadInformationModelsContent.FunctionSnippetPart3, functionName));
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.LoadInformationModelsContent.FunctionSnippetPart2);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Format(Constants.LoadInformationModelsContent.FunctionSnippetPart1, functionName));
				}

				_fileSystem.WriteFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_loadInformationModels_c), currentFileContentLineByLine);
			}
		}

		// Adding UAMethod callback functions to server source code
		private void AdjustMainCallbacksCFile(string srcDirectory, string nodesetPath, IModelData modelData)
		{
			// read XML file
			XmlDocument nodesetXml = new XmlDocument();
			using (var nodesetStream = _fileSystem.ReadFile(nodesetPath))
			{
				StreamReader reader = new StreamReader(nodesetStream);
				var xmlFileContent = reader.ReadToEnd();
				nodesetXml.LoadXml(xmlFileContent);
			}

			// extract UAMethods
			var nsmgr = new XmlNamespaceManager(nodesetXml.NameTable);
			nsmgr.AddNamespace(Constants.NodesetXml.UANodeSetNamespaceShortcut, new UriBuilder(Constants.NodesetXml.UANodeSetNamespaceScheme, Constants.NodesetXml.UANodeSetNamespaceHost, -1, Constants.NodesetXml.UANodeSetNamespaceValuePath).ToString());
			var methodNodes = nodesetXml.SelectNodes(string.Format(Constants.NodesetXml.UANodeSetUAMethod, Constants.NodesetXml.UANodeSetNamespaceShortcut), nsmgr);

			// skip the rest of method if there are no UAMethod in nodeset file
			if(methodNodes.Count == 0)
			{
				return;
			}

			// get content of mainCallbacks.c file
			List<string> currentFileContentLineByLine = null;
			var mainCallbacksCPath = _fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_mainCallbacks_c);
			using (var mainCallbacksFileStream = _fileSystem.ReadFile(mainCallbacksCPath))
			{
				currentFileContentLineByLine = ParseStreamToListOfString(mainCallbacksFileStream).ToList();
			}

			// generate method callbacks for each method
			GenerateCallbackFunctions(methodNodes, modelData, ref currentFileContentLineByLine);

			// write new content to mainCallbacks.c file
			_fileSystem.WriteFile(mainCallbacksCPath, currentFileContentLineByLine);
		}

		private void GenerateCallbackFunctions(XmlNodeList methodNodes, IModelData modelData, ref List<string> currentFileContentLineByLine)
		{
			// for each method found in nodeset generate callback functions
			foreach (XmlNode node in methodNodes)
			{
				var methodBrowseName = node.Attributes[Constants.NodesetXml.UANodeSetUAMethodBrowseName].Value;
				var methodNodeId = uint.Parse(Regex.Split(node.Attributes[Constants.NodesetXml.UANodeSetUaMethodNodeId].Value, @"\D+").Last());

				var lastUAMethodLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains(string.Format(Constants.UAMethodCallback.FunctionName, modelData.NamespaceVariable, methodNodeId)));
				if (lastUAMethodLinePosition == Constants.NumericValues.TextNotFound)
				{
					// add callback function
					var addCallbacksFunctionLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains(Constants.UAMethodCallback.AddCallbacks));
					if (addCallbacksFunctionLinePosition != Constants.NumericValues.TextNotFound)
					{
						currentFileContentLineByLine.Insert(addCallbacksFunctionLinePosition, string.Format(Constants.UAMethodCallback.FunctionBody, methodBrowseName, modelData.Name, modelData.NamespaceVariable, methodNodeId));
					}

					// call callback function in addCallbacks function
					var addCallbacksReturnLinePosition = currentFileContentLineByLine.FindLastIndex(x => x.Contains(Constants.UAMethodCallback.ReturnLine));
					if (addCallbacksReturnLinePosition != Constants.NumericValues.TextNotFound)
					{
						currentFileContentLineByLine.Insert(addCallbacksReturnLinePosition, string.Format(Constants.UAMethodCallback.FunctionCall, methodBrowseName, modelData.Name, modelData.NamespaceVariable, methodNodeId));
					}
				}
			}
		}

		// Conversion of stream to List of strings
		private List<string> ParseStreamToListOfString(Stream stream)
		{
			var fileContent = new List<string>();
			using (var sr = new StreamReader(stream))
			{
				string lineOfText;
				while ((lineOfText = sr.ReadLine()) != null)
				{
					fileContent.Add(lineOfText);
				}
				stream.Position = 0;
			}
			return fileContent.ToList();
		}
	}
}