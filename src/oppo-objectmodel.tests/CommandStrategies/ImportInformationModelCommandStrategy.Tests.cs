using System.Linq;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
	public class ImportInformationModelCommandStrategyTests
	{
		private readonly string _validModelExtension = ".xml";
		private readonly string _invalidModelExtension = ".txt";
		private readonly string _modelName = "model.xml";

		private static string[][] ValidInputs()
		{
			return new[]
			{
				new[] {"-n", "myApp", "-p", "model.xml"},
				new[] {"-n", "myApp", "--path", "model.xml"},
				new[] {"--name", "myApp", "-p", "model.xml"},
				new[] {"--name", "myApp", "--path", "model.xml"}
			};
		}

		private static string[][] ValidInputsLoadSample()
		{
			return new[]
			{
				new[] {"-n", "myApp", "-s"},
				new[] {"-n", "myApp", "--sample"}
			};
		}

		private static string[][] InvalidInputsInvalidModelPath()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p", "ab/yx.xml"},
				new[] { "-n", "myApp", "--path", "ab\\yx.xml"},
			};
		}

		private static string[][] InvalidInputsInvalidModelFile()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p", "model.txt"},
				new[] { "-n", "myApp", "--path", "model.xxx"},
				new[] { "-n", "myApp", "--path", "model.xml.txt"}
			};
		}

		private static string[][] InvalidInputsInvalidPathFlag()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-P", ""},
				new[] { "-n", "myApp", "--Path", ""},
				new[] {"-N", "ab/yx"},
				new[] {"-n", "myApp", "-P" }
			};
		}

		private static string[][] InvalidInputsMissingOpcuaappName()
		{
			return new[]
			{
				new[] { "-n", "", "-p", "model.xml"},
				new[] { "--name", "", "--path", "model.xml"}
			};
		}

		private static string[][] InvalidInputsInvalidOpcuaappName()
		{
			return new[]
			{
				new[] { "-n", "myApp/", "-p", "model.xml"},
				new[] { "-n", "my\\App", "--path", "model.xml"}
			};
		}

		private static string[][] InvalidInputsMissingInformationModel()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p"},
				new[] { "-n", "myApp", "--path"}
			};
		}

		private static string[][] InvalidInputsInvalidNameArgument()
		{
			return new[]
			{
				new[] { "-N", "myApp", "-p", "model.xml"},
				new[] { "-name", "myApp", "--path", "model.xml"}
			};
		}

		private Mock<IFileSystem> _fileSystemMock;
		private ImportInformationModelCommandStrategy _objectUnderTest;

		private readonly string _defaultOpcuaClientAppContent = "{\"name\":\"clientApp\",\"type\":\"Client\",\"references\": []}";
		private readonly string _defaultOpcuaServerAppContent = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[]}";
		private readonly string _defaultOpcuaClientServerAppContent = "{\"name\":\"clientServerApp\",\"type\":\"ClientServer\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"references\": [],\"models\":[]}";

		private readonly string _sampleOpcuaServerAppContent = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[{\"name\":\"model.xml\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"ns_model\"}]}";

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
														"</UANodeSet>";

		[SetUp]
		public void SetupObjectUnderTest()
		{
			_fileSystemMock = new Mock<IFileSystem>();
			_objectUnderTest = new ImportInformationModelCommandStrategy(_fileSystemMock.Object);
		}

		[Test]
		public void ImportInformationModelCommandStrategy_Should_ImplementICommandOfImportStrategy()
		{
			// Arrange

			// Act

			// Assert
			Assert.IsInstanceOf<ICommand<ImportStrategy>>(_objectUnderTest);
		}

		[Test]
		public void ImportInformationModelCommandStrategy_Should_HaveCorrectCommandName()
		{
			// Arrange

			// Act
			var commandName = _objectUnderTest.Name;

			// Assert
			Assert.AreEqual(Constants.ImportInformationModelCommandName.InformationModel, commandName);
		}

		[Test]
		public void ImportInformationModelCommandStrategy_Should_ProvideEmptyHelpText()
		{
			// Arrange

			// Act
			var helpText = _objectUnderTest.GetHelpText();

			// Assert
			Assert.AreEqual(string.Empty, helpText);
		}

		[Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModelToServer([ValueSource(nameof(ValidInputs))]string[] inputParams)
		{
			// Arrange

			var infoWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAt(1)}";
            var modelsDirectory = "models";
            
            var modelFilePath = $"{inputParams.ElementAt(3)}";
            var modelTargetPath = projectDirectory + "\\" + _modelName;
			var oppoprojFilePath = Path.Combine(projectDirectory, projectDirectory + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, _modelName)).Returns(modelTargetPath);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectDirectory)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectDirectory + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var serverOppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(serverOppoprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

					var loggerListenerMock = new Mock<ILoggerListener>();
				loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelFilePath))).Callback(delegate { infoWrittenOut = true; });
				OppoLogger.RegisterListener(loggerListenerMock.Object);

				// Act
				var result = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsTrue(infoWrittenOut);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandSuccess, inputParams.ElementAt(3)), result.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelTargetPath), Times.Once);
				OppoLogger.RemoveListener(loggerListenerMock.Object);
			}
		}

		[Test]
		public void ImportInformationModelCommandStrategy_Should_ImportModelToClientServer([ValueSource(nameof(ValidInputs))]string[] inputParams)
		{
			// Arrange

			var infoWrittenOut = false;
			var projectDirectory = $"{inputParams.ElementAt(1)}";
			var modelsDirectory = "models";

			var modelFilePath = $"{inputParams.ElementAt(3)}";
			var modelTargetPath = projectDirectory + "\\" + _modelName;
			var oppoprojFilePath = Path.Combine(projectDirectory, projectDirectory + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, _modelName)).Returns(modelTargetPath);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectDirectory)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectDirectory + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var clientServerOppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaClientServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(clientServerOppoprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

				var loggerListenerMock = new Mock<ILoggerListener>();
				loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelFilePath))).Callback(delegate { infoWrittenOut = true; });
				OppoLogger.RegisterListener(loggerListenerMock.Object);

				// Act
				var result = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsTrue(infoWrittenOut);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandSuccess, inputParams.ElementAt(3)), result.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelTargetPath), Times.Once);
				OppoLogger.RemoveListener(loggerListenerMock.Object);
			}
		}

		[Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_Failure([ValueSource(nameof(InvalidInputsInvalidPathFlag))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";
			_fileSystemMock.Setup(x => x.DirectoryExists(projectDirectory)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.UnknownImportInfomrationModelCommandParam)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.ImportInformationModelCommandUnknownParamFailure, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_MissingOpcuaappName_Failure([ValueSource(nameof(InvalidInputsMissingOpcuaappName))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.EmptyOpcuaappName)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.ImportInformationModelCommandUnknownParamFailure, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_InvalidOpcuaappName_Failure([ValueSource(nameof(InvalidInputsInvalidOpcuaappName))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(new[] { '\\', '/'});
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.InvalidOpcuaappName)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

		[Test]
		public void ImportInformationModelCommandStrategy_Should_Fail_OnCallingNotExistingProject([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var opcuaAppName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var warnWrittenOut = false;
			var loggerListenerMock = new Mock<ILoggerListener>();
			loggerListenerMock.Setup(listener => listener.Warn(LoggingText.InvalidOpcuaappName)).Callback(delegate { warnWrittenOut = true; });
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(result.Success);
			Assert.IsTrue(warnWrittenOut);
			Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), result.OutputMessages.First().Key);
			_fileSystemMock.Verify(x => x.CopyFile(modelFilePath, Constants.DirectoryName.Models), Times.Never);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_InvalidModelPath_Failure([ValueSource(nameof(InvalidInputsInvalidModelPath))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(new[] { '\\', '/' });
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelPath, modelFilePath))).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidModelPath, modelFilePath), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_InvalidModelName_Failure([ValueSource(nameof(InvalidInputsInvalidModelFile))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";
            
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_invalidModelExtension);
            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelExtension, _modelName))).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidModelExtension, _modelName), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_ModelFileMissing_Failure([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";
            var validModelExtension = ".xml";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(validModelExtension);
            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(false);
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelNotExistingPath, modelFilePath))).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandNotExistingModelPath, modelFilePath), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_ModelFileParamMissing_Failure([ValueSource(nameof(InvalidInputsMissingInformationModel))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";           
            
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.InvalidInformationModelMissingModelFile)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.ImportInformationModelCommandMissingModelPath, result.OutputMessages.First().Key);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportSampleModel([ValueSource(nameof(ValidInputsLoadSample))]string[] inputParams)
        {
            // Arrange
            var infoWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAt(1)}";
            var modelsDirectory = "models";
            var loadedModel = "anyString";
            var modelFilePath = Constants.FileName.SampleInformationModelFile;
            var modelTargetPath = projectDirectory + "\\" + _modelName;

            _fileSystemMock.Setup(x => x.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName)).Returns(loadedModel);

            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectDirectory)).Returns(true);

			_fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, Constants.FileName.SampleInformationModelFile)).Returns(modelTargetPath);            


            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelFilePath))).Callback(delegate { infoWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(infoWrittenOut);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportSampleInformationModelSuccess, modelFilePath), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CreateFile(modelTargetPath, loadedModel), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_WrongNameFlag_Failure([ValueSource(nameof(InvalidInputsInvalidNameArgument))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.UnknownImportInfomrationModelCommandParam)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.ImportInformationModelCommandUnknownParamFailure, result.OutputMessages.First().Key);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

		[Test]
		public void ImportInformationModelCommandStrategy_Should_Fail_OnImportingToClient([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var clientOppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaClientAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(clientOppoprojFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(OutputText.ImportInformationModelCommandOpcuaappIsAClient, commandResult.OutputMessages.First().Key);
			}
		}

		[Test]
		public void ImportInformationModelCommandStrategy_Should_Fail_OnInvalidOppoprojFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var emptyOppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(emptyOppoprojFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(OutputText.ImportInforamtionModelCommandFailureCannotReadOppoprojFile, commandResult.OutputMessages.First().Key);
			}
		}

		[Test]
		public void ImportInformationModelCommandStrategy_Should_Fail_OnModelDuplication([ValueSource(nameof(ValidInputs))] string [] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);
			var modelName = Path.GetFileName(modelFilePath);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);
			_fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(modelName);

			using (var sampleOppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(sampleOppoprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandFailureModelDuplication, projectName, modelName), commandResult.OutputMessages.First().Key);
			}
		}
	}
}