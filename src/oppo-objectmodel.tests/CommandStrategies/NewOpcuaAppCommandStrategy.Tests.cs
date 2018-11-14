using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class NewOpcuaAppCommandStrategyTests
    {
        private static string[][] ValidInputs()
        {
            return new[]
            {
                new[] {"-n", "anyName"},
                new[] {"-n", "ABC"},
                new[] {"--name", "ABC"}
            };
        }

        private static string[][] InvalidInputs()
        {
            return new[]
            {
                new[] {"-n", ""},
                new[] {"-n", "ab/yx"},
                new[] {"-n", "ab\\yx"},
                new[] {"-N", "ab/yx"},
                new[] {"", ""},
                new[] {""},
                new[] {"-n"},
                new string[] { }
            };
        }

        private Mock<IFileSystem> _fileSystemMock;
        private NewOpcuaAppCommandStrategy _objectUnderTest;

        [SetUp]
        public void SetupObjectUnderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new NewOpcuaAppCommandStrategy(_fileSystemMock.Object);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_Should_ImplementICommandOfNewStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<NewStrategy>>(_objectUnderTest);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_Should_CreateSlnAndProjectRelevantFiles([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var infoWrittenOut = false;
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Info(It.IsAny<string>())).Callback(delegate { infoWrittenOut = true; });
            SetupOppoLogger(loggerListenerMock.Object);

            var projectDirectoryName = $"{inputParams.ElementAt(1)}";
            var opcuaSourceCode = Path.Combine(projectDirectoryName, Constants.DirectoryName.SourceCode);
            var projectFileName = $"{inputParams.ElementAt(1)}{Constants.FileExtension.OppoProject}";
            var projectFilePath = Path.Combine(projectDirectoryName, projectFileName);
            var mesonBuildFilePath = Path.Combine(projectDirectoryName, Resources.Resources.OppoOpcuaAppTemplateFileName_meson_build);
            var maincFile = Path.Combine(opcuaSourceCode, Resources.Resources.OppoOpcuaAppTemplateFileName_main_c);
            var open62541cFile = Path.Combine(opcuaSourceCode, Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_c);
            var open62541hFile = Path.Combine(opcuaSourceCode, Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_h);

            _fileSystemMock.Setup(f => f.CombinePaths(projectDirectoryName, projectFileName)).Returns(projectFilePath);
            _fileSystemMock.Setup(f => f.CombinePaths(projectDirectoryName, Constants.DirectoryName.SourceCode)).Returns(opcuaSourceCode);
            _fileSystemMock.Setup(f => f.CombinePaths(projectDirectoryName, Constants.FileName.SourceCode_meson_build)).Returns(mesonBuildFilePath);
            _fileSystemMock.Setup(f => f.CombinePaths(opcuaSourceCode, Constants.FileName.SourceCode_main_c)).Returns(maincFile);
            _fileSystemMock.Setup(f => f.CombinePaths(opcuaSourceCode, Constants.FileName.SourceCode_open62541_c)).Returns(open62541cFile);
            _fileSystemMock.Setup(f => f.CombinePaths(opcuaSourceCode, Constants.FileName.SourceCode_open62541_h)).Returns(open62541hFile);
            

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(infoWrittenOut);
            Assert.AreEqual(Constants.CommandResults.Success, result);
            _fileSystemMock.Verify(x => x.CreateDirectory(projectDirectoryName), Times.Once);
            _fileSystemMock.Verify(x => x.CreateDirectory(opcuaSourceCode), Times.Once);
            _fileSystemMock.Verify(x => x.CreateFile(It.Is<string>(s=>s.StartsWith(opcuaSourceCode)), It.IsAny<string>()), Times.Exactly(3));
            _fileSystemMock.Verify(x => x.CreateFile(projectFilePath, It.IsAny<string>()), Times.Once);
            _fileSystemMock.Verify(x => x.CreateFile(mesonBuildFilePath, It.IsAny<string>()), Times.Once);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName), Times.Once);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_c), Times.Once);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_build), Times.Once);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_c), Times.Once);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_h), Times.Once);
            RemoveLoggerListener(loggerListenerMock.Object);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_Should_IgnoreInput([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var loggerListenerMock = new Mock<ILoggerListener>();
            var warnWrittenOut = false;
            loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
            SetupOppoLogger(loggerListenerMock.Object);

            var invalidNameCharsMock = new[] { '/' };
            var invalidPathCharsMock = new[] { '\\' };
            _fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidNameCharsMock);
            _fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(invalidPathCharsMock);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.AreEqual(Constants.CommandResults.Failure, result);
            _fileSystemMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName), Times.Never);
            RemoveLoggerListener(loggerListenerMock.Object);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var commandName = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.NewCommandName.OpcuaApp, commandName);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        private static void SetupOppoLogger(ILoggerListener loggerListener)
        {
            OppoLogger.RegisterListener(loggerListener);
        }
        
        private static void RemoveLoggerListener(ILoggerListener loggerListener)
        {
            OppoLogger.RemoveListener(loggerListener);
        }
    }
}
