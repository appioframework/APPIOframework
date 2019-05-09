using NUnit.Framework;
using Moq;
using System.IO;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests
{
    public class NodesetGeneratorShould
    {
		NodesetGenerator _objectUnderTest;

		private readonly string _projectName = "testApp";
		private readonly string _modelFullName = "model.xml";
		private readonly string _typesFullName = "types.bsd";

		protected static string[] InvalidTypesFullNames()
		{
			return new string[] { "types", "types.xml", "types.txt", "types.typ" };
		}
		protected static string[] InvalidModelFullNames()
		{
			return new string[] { "model", "model.bsd", "model.txt", "model.mod"};
		}

		private Mock<IFileSystem> _fileSystemMock;
		private Mock<IModelValidator> _modelValidator;
		private Mock<ILoggerListener> _loggerListenerMock;
		private bool _loggerWrittenOut;

		[SetUp]
        public void SetupTest()
        {
			_fileSystemMock = new Mock<IFileSystem>();
			_modelValidator = new Mock<IModelValidator>();
			_loggerListenerMock = new Mock<ILoggerListener>();
			_loggerWrittenOut = false;
		}

        [TearDown]
        public void CleanUpTest()
		{
			OppoLogger.RemoveListener(_loggerListenerMock.Object);
		}
		
		[Test]
		public void Fail_OnGeneratingTypesFileWithInvalidExtension([ValueSource(nameof(InvalidTypesFullNames))] string invalidTypesFullName)
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, invalidTypesFullName, _fileSystemMock.Object, null);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, invalidTypesFullName))).Callback(delegate { _loggerWrittenOut = true; });
			OppoLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateTypesSourceCodeFiles();

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWrittenOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidFile, _projectName, _modelFullName, invalidTypesFullName), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.GetExtension(invalidTypesFullName), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingNotExistingTypesFile()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, _typesFullName, _fileSystemMock.Object, null);

			// Arrange file system
			var typesPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _typesFullName);

			_fileSystemMock.Setup(x => x.GetExtension(_typesFullName)).Returns(Constants.FileExtension.ModelTypes);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _typesFullName)).Returns(typesPath);
			_fileSystemMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, typesPath))).Callback(delegate { _loggerWrittenOut = true; });
			OppoLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateTypesSourceCodeFiles();

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWrittenOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingFile, _projectName, _modelFullName, typesPath), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.FileExists(typesPath), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingTypesWithFailingPythonScript()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, _typesFullName, _fileSystemMock.Object, null);

			// Arrange file system
			var typesPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _typesFullName);
			var srcDirectory = Path.Combine(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			var modelName = Path.GetFileNameWithoutExtension(_modelFullName);

			_fileSystemMock.Setup(x => x.GetExtension(_typesFullName)).Returns(Constants.FileExtension.ModelTypes);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _typesFullName)).Returns(typesPath);
			_fileSystemMock.Setup(x => x.FileExists(typesPath)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(srcDirectory);
			_fileSystemMock.Setup(x => x.GetFileNameWithoutExtension(_modelFullName)).Returns(modelName);
			_fileSystemMock.Setup(x => x.CallExecutable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(LoggingText.GeneratedTypesExecutableFails)).Callback(delegate { _loggerWrittenOut = true; });
			OppoLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateTypesSourceCodeFiles();

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWrittenOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, _projectName, _modelFullName, _typesFullName), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Success_OnGeneratingModelTypes()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, _typesFullName, _fileSystemMock.Object, null);

			// Arrange file system
			var typesPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _typesFullName);
			var srcDirectory = Path.Combine(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			var modelName = Path.GetFileNameWithoutExtension(_modelFullName);
			var typesSourcePath = Path.Combine(Constants.DirectoryName.Models, _typesFullName);
			var typesSourceFullPath = @"../../" + typesSourcePath;
			var typesTargetFullPath = Path.Combine(Constants.DirectoryName.InformationModels, modelName.ToLower());
			var generatedTypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath + string.Format(Constants.ExecutableName.GenerateDatatypesTypeBsd, typesSourceFullPath) + " " + typesTargetFullPath + Constants.InformationModelsName.Types;

			_fileSystemMock.Setup(x => x.GetExtension(_typesFullName)).Returns(Constants.FileExtension.ModelTypes);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _typesFullName)).Returns(typesPath);
			_fileSystemMock.Setup(x => x.FileExists(typesPath)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(srcDirectory);
			_fileSystemMock.Setup(x => x.GetFileNameWithoutExtension(_modelFullName)).Returns(modelName);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower())).Returns(typesTargetFullPath);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, _typesFullName)).Returns(typesSourcePath);
			_fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, generatedTypesArgs)).Returns(true);
			
			// Act
			var result = _objectUnderTest.GenerateTypesSourceCodeFiles();

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(string.Empty, _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, generatedTypesArgs), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingNodesetFileWithInvalidExntension([ValueSource(nameof(InvalidModelFullNames))] string invalidModelFullName)
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, invalidModelFullName, null, _fileSystemMock.Object, null);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, invalidModelFullName))).Callback(delegate { _loggerWrittenOut = true; });
			OppoLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles();

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWrittenOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidModel, _projectName, invalidModelFullName), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.GetExtension(invalidModelFullName), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingNotExistingNodesetFile()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, null, _fileSystemMock.Object, null);

			// Arrange file system
			var modelPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _modelFullName);

			_fileSystemMock.Setup(x => x.GetExtension(_modelFullName)).Returns(Constants.FileExtension.InformationModel);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName)).Returns(modelPath);
			_fileSystemMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, _modelFullName))).Callback(delegate { _loggerWrittenOut = true; });
			OppoLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles();

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWrittenOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingModel, _projectName, _modelFullName, modelPath), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.FileExists(modelPath), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingInvalidModel()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, null, _fileSystemMock.Object, _modelValidator.Object);

			// Arrange file system
			var modelPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _modelFullName);

			_fileSystemMock.Setup(x => x.GetExtension(_modelFullName)).Returns(Constants.FileExtension.InformationModel);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName)).Returns(modelPath);
			_fileSystemMock.Setup(x => x.FileExists(modelPath)).Returns(true);
			_modelValidator.Setup(x => x.Validate(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			// Arrange logger listener
			_loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.NodesetValidationFailure, _modelFullName))).Callback(delegate { _loggerWrittenOut = true; });
			OppoLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles();

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWrittenOut);
			Assert.AreEqual(string.Format(OutputText.NodesetValidationFailure, _modelFullName), _objectUnderTest.GetOutputMessage());
			_modelValidator.Verify(x => x.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName), Times.Once);
		}

		[Test]
		public void Fail_OnGeneratingNodesetWithFailingPythonScript()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, null, _fileSystemMock.Object, _modelValidator.Object);

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
			_loggerListenerMock.Setup(listener => listener.Warn(LoggingText.NodesetCompilerExecutableFails)).Callback(delegate { _loggerWrittenOut = true; });
			OppoLogger.RegisterListener(_loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles();

			// Assert
			Assert.IsFalse(result);
			Assert.IsTrue(_loggerWrittenOut);
			Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailure, _projectName, _modelFullName), _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Success_OnGeneratingNodeset()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, null, _fileSystemMock.Object, _modelValidator.Object);

			// Arrange file system
			var modelPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _modelFullName);
			var srcDirectory = Path.Combine(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			var modelName = Path.GetFileNameWithoutExtension(_modelFullName);
			var modelSourcePath = Path.Combine(Constants.DirectoryName.Models, _modelFullName);
			var modelSourceRelativePath = @"../../" + modelSourcePath;
			var modelTargetRelativePath = Path.Combine(Constants.DirectoryName.InformationModels, modelName.ToLower());
			var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) + string.Format(Constants.ExecutableName.NodesetCompilerXml, modelSourceRelativePath, modelTargetRelativePath);

			_fileSystemMock.Setup(x => x.GetExtension(_modelFullName)).Returns(Constants.FileExtension.InformationModel);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName)).Returns(modelPath);
			_fileSystemMock.Setup(x => x.FileExists(modelPath)).Returns(true);
			_modelValidator.Setup(x => x.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(srcDirectory);
			_fileSystemMock.Setup(x => x.GetFileNameWithoutExtension(_modelFullName)).Returns(modelName);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower())).Returns(modelTargetRelativePath);
			_fileSystemMock.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, _modelFullName)).Returns(modelSourcePath);
			_fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs)).Returns(true);

			// Act
			var result = _objectUnderTest.GenerateNodesetSourceCodeFiles();

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(string.Empty, _objectUnderTest.GetOutputMessage());
			_fileSystemMock.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs), Times.Once);
		}
	}
}