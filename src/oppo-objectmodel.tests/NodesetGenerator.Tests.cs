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
			return new string[] { "types.xml", "types.txt", "types.typ" };
		}

		private Mock<IFileSystem> _fileSystemMock;

        [SetUp]
        public void SetupTest()
        {
			_fileSystemMock = new Mock<IFileSystem>();
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

        [Test]
        public void Success_OnGeneratingModelTypes()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, _typesFullName, _fileSystemMock.Object);

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
			Assert.Multiple(() =>
			{
				Assert.IsTrue(result);
				Assert.AreEqual(string.Empty, _objectUnderTest.GetOutputMessage());
				_fileSystemMock.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, generatedTypesArgs), Times.Once);
			});
		}
		
		[Test]
		public void Fail_OnGeneratingTypesFileWithInvalidExtension([ValueSource(nameof(InvalidTypesFullNames))] string typesFullName)
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, typesFullName, _fileSystemMock.Object);

			// Act
			var result = _objectUnderTest.GenerateTypesSourceCodeFiles();

			// Assert
			Assert.Multiple(() =>
			{
				Assert.IsFalse(result);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidFile, _projectName, _modelFullName, typesFullName), _objectUnderTest.GetOutputMessage());
				_fileSystemMock.Verify(x => x.GetExtension(typesFullName), Times.Once);
			});
		}

		[Test]
		public void Fail_OnGeneratingNotExistingTypesFile()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, _typesFullName, _fileSystemMock.Object);

			// Arrange file system
			var typesPath = Path.Combine(_projectName, Constants.DirectoryName.Models, _typesFullName);

			_fileSystemMock.Setup(x => x.GetExtension(_typesFullName)).Returns(Constants.FileExtension.ModelTypes);
			_fileSystemMock.Setup(x => x.CombinePaths(_projectName, Constants.DirectoryName.Models, _typesFullName)).Returns(typesPath);
			_fileSystemMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

			// Act
			var result = _objectUnderTest.GenerateTypesSourceCodeFiles();

			// Assert
			Assert.Multiple(() =>
			{
				Assert.IsFalse(result);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingFile, _projectName, _modelFullName, typesPath), _objectUnderTest.GetOutputMessage());
				_fileSystemMock.Verify(x => x.FileExists(typesPath), Times.Once);
			});
		}

		[Test]
		public void Fail_OnGeneratingTypesWithFailingPythonScript()
		{
			// Arrange tested object
			_objectUnderTest = new NodesetGenerator(_projectName, _modelFullName, _typesFullName, _fileSystemMock.Object);

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

			// Act
			var result = _objectUnderTest.GenerateTypesSourceCodeFiles();

			// Assert
			Assert.Multiple(() =>
			{
				Assert.IsFalse(result);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, _projectName, _modelFullName, _typesFullName), _objectUnderTest.GetOutputMessage());
				_fileSystemMock.Verify(x => x.CallExecutable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			});
		}
	}
}