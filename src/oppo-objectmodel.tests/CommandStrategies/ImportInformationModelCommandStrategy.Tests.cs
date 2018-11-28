using System.Linq;
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
                new[] {"myApp", "-p", "model.xml"},
                new[] {"myApp", "--path", "model.xml"}
            };
        }

        private static string[][] InvalidInputsInvalidModelPath()
        {
            return new[]
            {
                new[] {"myApp", "-p", "ab/yx.xml"},
                new[] {"myApp", "--path", "ab\\yx.xml"},
            };
        }

        private static string[][] InvalidInputsInvalidModelFile()
        {
            return new[]
            {
                new[] {"myApp", "-p", "model.txt"},
                new[] {"myApp", "--path", "model.xxx"},
                new[] {"myApp", "--path", "model.xml.txt"}
            };
        }

        private static string[][] InvalidInputsInvalidPathFlag()
        {
            return new[]
            {
                new[] {"myApp", "-P", ""},
                new[] {"myApp", "--Path", ""},
                new[] {"-N", "ab/yx"},
                new[] {"-n"}                
            };
        }

        private static string[][] InvalidInputsMissingOpcuaappName()
        {
            return new[]
            {
                new string[] { },
                new[] {"", "-p", "model.xml"},
                new[] {"", "--path", "model.xml"}
            };
        }

        private static string[][] InvalidInputsInvalidOpcuaappName()
        {
            return new[]
            {
                new[] { "myApp/", "-p", "model.xml"},
                new[] { "my\\App", "--path", "model.xml"}
            };
        }        

        private Mock<IFileSystem> _fileSystemMock;
        private ImportInformationModelCommandStrategy _objectUnderTest;

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
        public void ImportInformationModelCommandStrategy_Should_ImportModel([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var infoWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAt(0)}";
            var modelsDirectory = "models";
            
            var modelFilePath = $"{inputParams.ElementAt(2)}";
            var modelTargetPath = projectDirectory + "\\" + _modelName;
            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, _modelName)).Returns(modelTargetPath);         
            
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelFilePath))).Callback(delegate { infoWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);         

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(infoWrittenOut);
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandSuccess, inputParams.ElementAt(2)), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelTargetPath), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_Failure([ValueSource(nameof(InvalidInputsInvalidPathFlag))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAtOrDefault(0)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            var modelFilePath = $"{inputParams.ElementAtOrDefault(2)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.UnknownImportInfomrationModelCommandParam)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(OutputText.ImportInforamtionModelCommandUnknownParamFailure, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_MissingOpcuaappName_Failure([ValueSource(nameof(InvalidInputsMissingOpcuaappName))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAtOrDefault(0)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            var modelFilePath = $"{inputParams.ElementAtOrDefault(2)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.EmptyOpcuaappName)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(OutputText.ImportInforamtionModelCommandUnknownParamFailure, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_InvalidOpcuaappName_Failure([ValueSource(nameof(InvalidInputsInvalidOpcuaappName))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(0)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(new[] { '\\', '/'});
            var modelFilePath = $"{inputParams.ElementAtOrDefault(2)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.InvalidOpcuaappName)).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandInvalidOpcuaappName, opcuaAppName), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_InvalidModelPath_Failure([ValueSource(nameof(InvalidInputsInvalidModelPath))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(0)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(new[] { '\\', '/' });
            var modelFilePath = $"{inputParams.ElementAtOrDefault(2)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelPath, modelFilePath))).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandInvalidModelPath, modelFilePath), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_InvalidModelName_Failure([ValueSource(nameof(InvalidInputsInvalidModelFile))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(0)}";
            var modelsDirectory = "models";
            var modelFilePath = $"{inputParams.ElementAtOrDefault(2)}";
            
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_invalidModelExtension);
            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelExtension, _modelName))).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandInvalidModelExtension, _modelName), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel_ModelFileMissing_Failure([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(0)}";
            var modelsDirectory = "models";
            var modelFilePath = $"{inputParams.ElementAtOrDefault(2)}";
            var validModelExtension = ".xml";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(validModelExtension);
            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(false);

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelNotExistingPath, modelFilePath))).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandNotExistingModelPath, modelFilePath), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}