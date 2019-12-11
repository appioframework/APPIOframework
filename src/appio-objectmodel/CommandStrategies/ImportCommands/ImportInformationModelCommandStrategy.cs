/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Appio.Resources.text.logging;
using Appio.Resources.text.output;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace Appio.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportInformationModelCommandStrategy : ICommand<ImportStrategy>
    {
	    enum ParamId {AppName, ModelPath, TypesPath, Sample}

	    private readonly ParameterResolver<ParamId> _resolver;
        private readonly IFileSystem _fileSystem;
		private readonly IModelValidator _modelValidator;
		private readonly MessageLines _outputMessages;

		public ImportInformationModelCommandStrategy(IFileSystem fileSystem, IModelValidator modelValidator)
        {
            _fileSystem = fileSystem;
			_modelValidator = modelValidator;
			_outputMessages = new MessageLines();
			
			_resolver = new ParameterResolver<ParamId>(Constants.CommandName.Import + " " + Name, new []
			{
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.AppName,
					Short = Constants.ImportCommandOptions.Name,
					Verbose = Constants.ImportCommandOptions.VerboseName
				},
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.ModelPath,
					Short = Constants.ImportCommandOptions.Path,
					Verbose = Constants.ImportCommandOptions.VerbosePath,
					Default = string.Empty
				},
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.TypesPath,
					Short = Constants.ImportCommandOptions.Types,
					Verbose = Constants.ImportCommandOptions.VerboseTypes,
					Default = string.Empty
				}
			}, new []
			{
				new BoolParameterSpecification<ParamId>
				{
					Identifier = ParamId.Sample,
					Short = Constants.ImportCommandOptions.Sample,
					Verbose = Constants.ImportCommandOptions.VerboseSample
				}, 
			});
		}

        public string Name => Constants.ImportCommandArguments.InformationModel;

		public CommandResult Execute(IEnumerable<string> inputParams)
		{
			var (error, parameters, options) = _resolver.ResolveParams(inputParams);
			
			if (error != null)
				return new CommandResult(false, new MessageLines {{error, string.Empty}});
			
			var opcuaAppName = parameters[ParamId.AppName];
			var modelPath = parameters[ParamId.ModelPath];
			var typesPath = parameters[ParamId.TypesPath];
			
			// opcuaapp name validation
			if (!ValidateOpcuaAppName(opcuaAppName))
			{
				return new CommandResult(false, _outputMessages);
			}


			var appioprojFilePath = _fileSystem.CombinePaths(opcuaAppName, opcuaAppName + Constants.FileExtension.Appioproject);
			var modelData = new ModelData();

			if (options[ParamId.Sample])
			{
				var modelsDir = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);

				var nodesetContent = _fileSystem.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName);
				var nodesetFilePath = _fileSystem.CombinePaths(modelsDir, Constants.FileName.SampleInformationModelFile);
				_fileSystem.CreateFile(nodesetFilePath, nodesetContent);

				var typesContent = _fileSystem.LoadTemplateFile(Resources.Resources.SampleInformationModelTypesFileName);
				var typesFilePath = _fileSystem.CombinePaths(modelsDir, Constants.FileName.SampleInformationModelTypesFile);
				_fileSystem.CreateFile(typesFilePath, typesContent);

				if (!UpdateAppioProjFile(appioprojFilePath, modelData, opcuaAppName, Constants.FileName.SampleInformationModelFile, nodesetFilePath, Constants.FileName.SampleInformationModelTypesFile))
					return new CommandResult(false, _outputMessages);

				_outputMessages.Add(string.Format(OutputText.ImportSampleInformationModelSuccess, Constants.FileName.SampleInformationModelFile), string.Empty);
				AppioLogger.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, Constants.FileName.SampleInformationModelFile));
				return new CommandResult(true, _outputMessages);
			}

			// nodeset validation
			if (!ValidateModel(modelPath))
			{
				return new CommandResult(false, _outputMessages);
			}
			var modelFileName = _fileSystem.GetFileName(modelPath);

			// types validation
			var typesFileName = string.Empty;
			if (typesPath != string.Empty && !ValidateTypes(out typesFileName, typesPath))
			{
				return new CommandResult(false, _outputMessages);
			}

			if (!UpdateAppioProjFile(appioprojFilePath, modelData, opcuaAppName, modelFileName, modelPath, typesFileName))
				return new CommandResult(false, _outputMessages);

			// copy model file
			var modelsDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);            
            var targetModelFilePath = _fileSystem.CombinePaths(modelsDirectory, modelFileName);
            _fileSystem.CopyFile(modelPath, targetModelFilePath);

			// copy types file
			if (typesPath != string.Empty)
			{
				var targetTypesFilePath = _fileSystem.CombinePaths(modelsDirectory, typesFileName);
				_fileSystem.CopyFile(typesPath, targetTypesFilePath);
			}

			// exit with success
            AppioLogger.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelPath));
			_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandSuccess, modelPath), string.Empty);
            return new CommandResult(true, _outputMessages);
        }

		private bool UpdateAppioProjFile(string appioprojFilePath, ModelData modelData,
			string opcuaAppName, string modelFileName, string modelPath, string typesFileName)
		{
			// deserialize appioproj file
			if (!DeserializeAppioprojFile(appioprojFilePath, out var opcuaappData))
			{
				return false;
			}

			// build model data
			var opcuaappDataAsServer = opcuaappData as IOpcuaServerApp;
			if (!BuildModelData(ref modelData, opcuaappDataAsServer, opcuaAppName, modelFileName, modelPath, typesFileName))
			{
				return false;
			}

			// add model to project structure, serialize structure and write to appioproj file
			opcuaappDataAsServer.Models.Add(modelData);
			var appioprojNewContent = JsonConvert.SerializeObject(opcuaappData, Newtonsoft.Json.Formatting.Indented);
			_fileSystem.WriteFile(appioprojFilePath, new List<string> {appioprojNewContent});
			return true;
		}

		private bool ValidateOpcuaAppName(string opcuaAppName)
		{
			if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains) || !_fileSystem.DirectoryExists(opcuaAppName))
			{
				AppioLogger.Warn(LoggingText.InvalidOpcuaappName);
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), string.Empty);
				return false;
			} 

			return true;
		}

		private bool ValidateModel(string modelPath)
		{
			// model path validation
			if (modelPath == string.Empty)
			{
				AppioLogger.Warn(LoggingText.InvalidInformationModelMissingModelFile);
				_outputMessages.Add(OutputText.ImportInformationModelCommandMissingModelPath, string.Empty);
				return false;
			}

			if (_fileSystem.GetInvalidPathChars().Any(modelPath.Contains))
			{
				AppioLogger.Warn(string.Format(LoggingText.InvalidInformationModelPath, modelPath));
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidModelPath, modelPath), string.Empty);
				return false;
			}

			if (!_fileSystem.FileExists(modelPath))
			{
				AppioLogger.Warn(string.Format(LoggingText.InvalidInformationModelNotExistingPath, modelPath));
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandNotExistingModelPath, modelPath), string.Empty);
				return false;
			}

			// model file name/extension validation
			var modelFileName = _fileSystem.GetFileName(modelPath);
			if (_fileSystem.GetExtension(modelPath) != Constants.FileExtension.InformationModel)
			{
				AppioLogger.Warn(string.Format(LoggingText.InvalidInformationModelExtension, modelFileName));
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidModelExtension, modelFileName), string.Empty);
				return false;
			}

			// validate model against UANodeSet xsd file
			if (!_modelValidator.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName))
			{
				AppioLogger.Warn(string.Format(LoggingText.NodesetValidationFailure, modelPath));
				_outputMessages.Add(string.Format(OutputText.NodesetValidationFailure, modelPath), string.Empty);
				return false;
			}

			return true;
		}

		private bool ValidateTypes(out string typesFileName, string typesPath)
		{
			typesFileName = _fileSystem.GetFileName(typesPath);
			if (_fileSystem.GetExtension(typesPath) != Constants.FileExtension.ModelTypes)
			{
				AppioLogger.Warn(LoggingText.ImportInformationModelCommandFailureTypesHasInvalidExtension);
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandFailureTypesHasInvalidExtension, typesFileName), string.Empty);
				return false;
			}

			if (!_fileSystem.FileExists(typesPath))
			{
				AppioLogger.Warn(LoggingText.ImportInformationModelCommandFailureTypesFileDoesNotExist);
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandFailureTypesFileDoesNotExist, typesPath), string.Empty);
				return false;
			}

			return true;
		}

		private bool ExtractNodesetUris(ref ModelData modelData, string nodesetPath)
		{
			// read XML file
			XmlDocument nodesetXml = new XmlDocument();
			using (var nodesetStream = _fileSystem.ReadFile(nodesetPath))
			{
				StreamReader reader = new StreamReader(nodesetStream);
				var xmlFileContent = reader.ReadToEnd();
				nodesetXml.LoadXml(xmlFileContent);
			}
			
			// extract namespace uri
			var nsmgr = new XmlNamespaceManager(nodesetXml.NameTable);
			nsmgr.AddNamespace(Constants.NodesetXml.UANodeSetNamespaceShortcut, new UriBuilder(Constants.NodesetXml.UANodeSetNamespaceScheme, Constants.NodesetXml.UANodeSetNamespaceHost, Constants.NumericValues.PortNumberNotSpecified, Constants.NodesetXml.UANodeSetNamespaceValuePath).ToString());
			var modelNode = nodesetXml.SelectSingleNode(string.Format(Constants.NodesetXml.UANodeSetNamespaceFullPath, Constants.NodesetXml.UANodeSetNamespaceShortcut), nsmgr);

			// validate namespace uri
			if(modelNode == null || modelNode.Attributes == null || modelNode.Attributes[Constants.NodesetXml.UANodeSetNamespaceModelUri] == null)
			{
				AppioLogger.Warn(LoggingText.ImportInforamtionModelCommandFailureModelMissingUri);
				_outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandFailureModelMissingUri, nodesetPath), string.Empty);
				return false;
			}

			// write namespace uri to model data
			modelData.Uri = modelNode.Attributes[Constants.NodesetXml.UANodeSetNamespaceModelUri].Value;

			// find required model uris and write them to model data
			if(modelNode.ChildNodes.Count > 0)
			{
				for(int index = 0; index < modelNode.ChildNodes.Count; index++)
				{
					var requiredModelUri = modelNode.ChildNodes[index].Attributes[Constants.NodesetXml.UANodeSetNamespaceModelUri].Value;
					if (requiredModelUri != new UriBuilder(Constants.NodesetXml.UANodeSetNamespaceScheme, Constants.NodesetXml.UANodeSetNamespaceHost, Constants.NumericValues.PortNumberNotSpecified, Constants.NodesetXml.UANodeSetNamespaceBasicValuePath).ToString())
					{
						modelData.RequiredModelUris.Add(requiredModelUri);
					}
				}
			}

			return true;
		}

		private bool DeserializeAppioprojFile(string appioprojFilePath, out IOpcuaapp opcuaappData)
		{
			// deserialize appioproj file
			opcuaappData = Deserialize.Opcuaapp(appioprojFilePath, _fileSystem);
			if (opcuaappData == null)
			{
				AppioLogger.Warn(LoggingText.ImportInforamtionModelCommandFailureCannotReadAppioprojFile);
				_outputMessages.Add(OutputText.ImportInforamtionModelCommandFailureCannotReadAppioprojFile, string.Empty);
				return false;
			}
			if (opcuaappData.Type == Constants.ApplicationType.Client)
			{
				AppioLogger.Warn(LoggingText.ImportInformationModelCommandOpcuaappIsAClient);
				_outputMessages.Add(OutputText.ImportInformationModelCommandOpcuaappIsAClient, string.Empty);
				return false;
			}

			return true;
		}

		private bool BuildModelData(ref ModelData modelData, IOpcuaServerApp opcuaServerData, string opcuaAppName, string modelFileName, string modelPath, string typesFileName)
		{
			// build model data
			modelData.Name = modelFileName;
			if (!ExtractNodesetUris(ref modelData, modelPath))
			{
				return false;
			}
			modelData.Types = typesFileName;
			modelData.NamespaceVariable = Constants.NodesetXml.NamespaceVariablePrefix + _fileSystem.GetFileNameWithoutExtension(modelFileName);

			// check if appioproj file already contains model with imported model name
			var modelName = modelData.Name;
			if (opcuaServerData.Models.Any(x => x.Name == modelName))
			{
				AppioLogger.Warn(LoggingText.ImportInforamtionModelCommandFailureModelDuplication);
				_outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandFailureModelNameDuplication, opcuaAppName, modelFileName), string.Empty);
				return false;
			}
			// check if appioproj file already contains model with imported model namespace uri
			var modelUri = modelData.Uri;
			if (opcuaServerData.Models.Any(x => x.Uri == modelUri))
			{
				AppioLogger.Warn(LoggingText.ImportInforamtionModelCommandFailureModelDuplication);
				_outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandFailureModelUriDuplication, opcuaAppName, modelData.Uri), string.Empty);
				return false;
			}

			return true;
		}

		public string GetHelpText()
        {
            return string.Empty;
        }
    }
}