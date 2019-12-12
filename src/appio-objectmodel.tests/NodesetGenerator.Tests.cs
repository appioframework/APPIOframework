/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using NUnit.Framework;
using Moq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Appio.Resources.text.output;
using Appio.Resources.text.logging;

namespace Appio.ObjectModel.Tests
{
	public class NodesetGeneratorShould
	{
		NodesetGenerator _objectUnderTest;

		private readonly string _projectName = "testApp";
		private readonly string _modelFullName = "model.xml";
		private readonly string _namespaceVariable = "ns_model";
		private readonly List<RequiredModelsData> _requiredModelData = new List<RequiredModelsData>{ new RequiredModelsData("requiredModel.xml") };

		protected static string[] InvalidTypesFullNames()
		{
			return new string[] { "types", "types.xml", "types.txt", "types.typ" };
		}
		protected static string[] InvalidModelFullNames()
		{
			return new string[] { "model", "model.bsd", "model.txt", "model.mod" };
		}

		private IModelData _defaultModelData;

		private Mock<IFileSystem> _fileSystemMock;
		private Mock<IModelValidator> _modelValidator;
		private Mock<ILoggerListener> _loggerListenerMock;
		private bool _loggerWroteOut;

		private readonly string _defaultServerMesonBuild = "server_app_sources += [\n]";
		private readonly string _defaultLoadInformationModelsC = "UA_StatusCode loadInformationModels(UA_Server* server)\n{\n\treturn UA_STATUSCODE_GOOD;\n}";

		private readonly string _emptyNodesetContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><UANodeSet/>";
		private readonly string _sampleNodesetContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
														"<UANodeSet xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" LastModified=\"2012-12-31T00:00:00Z\" xmlns=\"http://opcfoundation.org/UA/2011/03/UANodeSet.xsd\">" +
															"<NamespaceUris>" +
																"<Uri>sample_namespace</Uri>" +
																"<Uri>required_namespace</Uri>" +
															"</NamespaceUris>" +
															"<Models>" +
																"<Model ModelUri = \"sample_namespace\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\">" +
																	"<RequiredModel ModelUri = \"http://opcfoundation.org/UA/\" Version=\"1.04\" PublicationDate=\"2016-12-31T00:00:00Z\" />" +
																	"<RequiredModel ModelUri = \"required_namespace\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\" />" +
																"</Model>" +
															"</Models>" +
															"<UAMethod NodeId=\"ns=1;i=3000\" BrowseName=\"startPump\" ParentNodeId=\"ns=1;i=1000\">" +
																"<DisplayName>StartPump</DisplayName>" +
																"<References/>" +
															"</UAMethod>" +
															"<UAMethod NodeId = \"ns=1;i=3001\" BrowseName=\"stopPump\" ParentNodeId=\"ns=1;i=1000\">" +
																"<DisplayName>StopPump</DisplayName>" +
																"<References/>" +
															"</UAMethod>" +
														"</UANodeSet>";
		private readonly string _defaultMainCallbakcsCFileContent = "UA_StatusCode addCallbacks(UA_Server *server)\n{\n\treturn UA_STATUSCODE_GOOD;\n}";

		[SetUp]
		public void SetupTest()
		{
			_defaultModelData = new ModelData(_modelFullName, null, _namespaceVariable, null);
			_fileSystemMock = new Mock<IFileSystem>();
			_modelValidator = new Mock<IModelValidator>();
			_loggerListenerMock = new Mock<ILoggerListener>();
			_loggerWroteOut = false;
			_objectUnderTest = new NodesetGenerator(_fileSystemMock.Object, _modelValidator.Object);
			AppioLogger.RegisterListener(_loggerListenerMock.Object);
		}

		[TearDown]
		public void CleanUpTest()
		{
			AppioLogger.RemoveListener(_loggerListenerMock.Object);
		}

		[Test]
		public void Fail_OnGeneratingModelWhenModelNameEmpty()
		{
			// Arrange
			_loggerListenerMock.Setup(listener => listener.Warn(LoggingText.GenerateInformationModelFailureEmptyModelName)).Callback(delegate { _loggerWroteOut = true; });

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles(_projectName, new ModelData(), new List<RequiredModelsData>());

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWroteOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureEmptyModelName, _projectName), _objectUnderTest.GetOutputMessage());
		}

		[Test]
		public void Fail_OnGeneratingNodesetFileWithInvalidExntension([ValueSource(nameof(InvalidModelFullNames))] string invalidModelFullName)
		{
			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, invalidModelFullName))).Callback(delegate { _loggerWroteOut = true; });

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles(_projectName, new ModelData(invalidModelFullName, null, null, null), new List<RequiredModelsData>());

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWroteOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidModel, _projectName, invalidModelFullName), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.GetExtension(invalidModelFullName), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingNotExistingNodesetFile()
		{
			// Arrange file system
			var modelPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _modelFullName);

			_fileSystemMock.Setup(x => x.GetExtension(_modelFullName)).Returns(Constants.FileExtension.InformationModel);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName)).Returns(modelPath);
			_fileSystemMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, _modelFullName))).Callback(delegate { _loggerWroteOut = true; });

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles(_projectName, _defaultModelData, new List<RequiredModelsData>());

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWroteOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingModel, _projectName, _modelFullName, modelPath), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.FileExists(modelPath), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingInvalidModel()
		{
			// Arrange file system
			var modelPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _modelFullName);

			_fileSystemMock.Setup(x => x.GetExtension(_modelFullName)).Returns(Constants.FileExtension.InformationModel);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName)).Returns(modelPath);
			_fileSystemMock.Setup(x => x.FileExists(modelPath)).Returns(true);
			_modelValidator.Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetValidationFailure, _modelFullName))).Callback(delegate { _loggerWroteOut = true; });

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles(_projectName, _defaultModelData, new List<RequiredModelsData>());

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWroteOut);
			Assert.AreEqual(string.Format(OutputText.NodesetValidationFailure, _modelFullName), _objectUnderTest.GetOutputMessage());
			_modelValidator.Verify(x => x.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingNodesetWithFailingPythonScript()
		{
			// Arrange file system
			var modelPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _modelFullName);
			var srcDirectory = Path.Combine(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			var modelName = Path.GetFileNameWithoutExtension(_modelFullName);
			var modelSourcePath = Path.Combine(Constants.DirectoryName.Models, _modelFullName);
			var modelSourceRelativePath = @"../../" + modelSourcePath;
			var modelTargetFullPath = Path.Combine(Constants.DirectoryName.InformationModels, modelName.ToLower());

			_fileSystemMock.Setup(x => x.GetExtension(_modelFullName)).Returns(Constants.FileExtension.InformationModel);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName)).Returns(modelPath);
			_fileSystemMock.Setup(x => x.FileExists(modelPath)).Returns(true);
			_modelValidator.Setup(x => x.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(srcDirectory);
			_fileSystemMock.Setup(x => x.GetFileNameWithoutExtension(_modelFullName)).Returns(modelName);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower())).Returns(modelTargetFullPath);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, _modelFullName)).Returns(modelSourcePath);
			_fileSystemMock.Setup(x => x.CallExecutable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(LoggingText.NodesetCompilerExecutableFails)).Callback(delegate { _loggerWroteOut = true; });
			AppioLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles(_projectName, _defaultModelData, new List<RequiredModelsData>());

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWroteOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailure, _projectName, _modelFullName), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Success_OnGeneratingNodeset()
		{
			// Arrange file system
			var modelPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _modelFullName);
			var srcDirectory = Path.Combine(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			var modelName = Path.GetFileNameWithoutExtension(_modelFullName);
			var modelSourcePath = Path.Combine(Constants.DirectoryName.Models, _modelFullName);
			var modelSourceRelativePath = @"../../" + modelSourcePath;
			var modelTargetRelativePath = Path.Combine(Constants.DirectoryName.InformationModels, modelName.ToLower());
			var requiredModelName = Path.GetFileNameWithoutExtension(_requiredModelData[0].ModelName);
			var requiredModelRelativePath = Path.Combine(Constants.DirectoryName.Models, _requiredModelData[0].ModelName);
			var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath +
										Constants.ExecutableName.NodesetCompilerInternalHeaders +
										string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) +
										string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) +
										string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) +
										string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) +
										string.Format(Constants.ExecutableName.NodesetCompilerExisting, @"../../" + requiredModelRelativePath) +
										string.Format(Constants.ExecutableName.NodesetCompilerXml, modelSourceRelativePath, modelTargetRelativePath);
			var serverMesonBuildFilePath = Path.Combine(srcDirectory, Constants.FileName.SourceCode_meson_build);
			var loadInformationModelsFilePath = Path.Combine(srcDirectory, Constants.FileName.SourceCode_loadInformationModels_c);
			var mainCallbacksFilePath = Path.Combine(srcDirectory, Constants.FileName.SourceCode_mainCallbacks_c);

			_fileSystemMock.Setup(x => x.GetExtension(_modelFullName)).Returns(Constants.FileExtension.InformationModel);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName)).Returns(modelPath);
			_fileSystemMock.Setup(x => x.FileExists(modelPath)).Returns(true);
			_modelValidator.Setup(x => x.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(srcDirectory);
			_fileSystemMock.Setup(x => x.GetFileNameWithoutExtension(_modelFullName)).Returns(modelName);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower())).Returns(modelTargetRelativePath);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, _modelFullName)).Returns(modelSourcePath);
			_fileSystemMock.Setup(x => x.GetFileNameWithoutExtension(_requiredModelData[0].ModelName)).Returns(requiredModelName);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, _requiredModelData[0].ModelName)).Returns(requiredModelRelativePath);
			_fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(srcDirectory, Constants.FileName.SourceCode_meson_build)).Returns(serverMesonBuildFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(srcDirectory, Constants.FileName.SourceCode_loadInformationModels_c)).Returns(loadInformationModelsFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(srcDirectory, Constants.FileName.SourceCode_mainCallbacks_c)).Returns(mainCallbacksFilePath);

			using (var serverMesonBuildFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultServerMesonBuild)))
			using (var loadInformationModelsFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultLoadInformationModelsC)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			using (var mainCallbacksFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultMainCallbakcsCFileContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(serverMesonBuildFilePath)).Returns(serverMesonBuildFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(loadInformationModelsFilePath)).Returns(loadInformationModelsFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelPath)).Returns(nodesetFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(mainCallbacksFilePath)).Returns(mainCallbacksFileStream);

				// Act
				var result = _objectUnderTest.GenerateNodesetSourceCodeFiles(_projectName, new ModelData(_modelFullName, null, _namespaceVariable, null), _requiredModelData);

				// Assert
				Assert.IsTrue(result);
				Assert.AreEqual(string.Empty, _objectUnderTest.GetOutputMessage());
				_fileSystemMock.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs), Times.Once);
				_fileSystemMock.Verify(x => x.ReadFile(serverMesonBuildFilePath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(serverMesonBuildFilePath, It.IsAny<IEnumerable<string>>()), Times.Once);
				_fileSystemMock.Verify(x => x.ReadFile(loadInformationModelsFilePath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(loadInformationModelsFilePath, It.IsAny<IEnumerable<string>>()), Times.Once);
				_fileSystemMock.Verify(x => x.ReadFile(modelPath), Times.Once);
				_fileSystemMock.Verify(x => x.ReadFile(mainCallbacksFilePath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(mainCallbacksFilePath, It.IsAny<IEnumerable<string>>()), Times.Once);
			}
		}
	}
}