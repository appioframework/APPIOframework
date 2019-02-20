using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.Resources.text.output;

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

        private static string[][] InvalidInputsSecondParam()
        {
            return new[]
            {
                new[] {"-n", "ab/yx"},
                new[] {"-n", "ab\\yx"}                
            };
        }

        private static string[][] InvalidInputsFistParam()
        {
            return new[]
            {
                new[] {"-n", ""},
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
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var projectDirectory               = $"{inputParams.ElementAt(1)}";
            const string sourceCodeDirectory   = "source-directory";
            const string clientSourceDirectory = "client-source-directory";
            const string serverSourceDirectory = "server-source-directory";

            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.SourceCode)).Returns(sourceCodeDirectory);
            _fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ClientApp)).Returns(clientSourceDirectory);
            _fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ServerApp)).Returns(serverSourceDirectory);

            var projectFileName                         = $"{inputParams.ElementAt(1)}{Constants.FileExtension.OppoProject}";
            const string projectFilePath                = "project-file-path";
            const string mesonBuildFilePath             = "meson-build-file-path";
            const string clientMainC                    = "client-main-c-file";
            const string serverMainC                    = "server-main-c-file";
            const string serverMesonBuild               = "server-meson-build-file";
            const string serverloadInformationModelsC   = "server-loadInformationModels-c-file";
            const string modelsDirectory                = "models";

            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectFileName)).Returns(projectFilePath);
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.FileName.SourceCode_meson_build)).Returns(mesonBuildFilePath);
            _fileSystemMock.Setup(x => x.CombinePaths(clientSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(clientMainC);
            _fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(serverMainC);
            _fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_meson_build)).Returns(serverMesonBuild);
            _fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_loadInformationModels_c)).Returns(serverloadInformationModelsC);
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(infoWrittenOut);
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandSuccess, inputParams.ElementAt(1)), result.OutputMessages.First().Key);

            _fileSystemMock.Verify(x => x.CreateDirectory(projectDirectory), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.CreateDirectory(sourceCodeDirectory), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.CreateDirectory(clientSourceDirectory), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.CreateDirectory(serverSourceDirectory), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.CreateDirectory(modelsDirectory), Times.Once);

            _fileSystemMock.Verify(x => x.CreateFile(projectFilePath, It.IsAny<string>()), Times.Once);
            _fileSystemMock.Verify(x => x.CreateFile(mesonBuildFilePath, It.IsAny<string>()), Times.Once);
            _fileSystemMock.Verify(x => x.CreateFile(clientMainC, It.IsAny<string>()), Times.Once);
            _fileSystemMock.Verify(x => x.CreateFile(serverMainC, It.IsAny<string>()), Times.Once);
            _fileSystemMock.Verify(x => x.CreateFile(serverMesonBuild, It.IsAny<string>()), Times.Once);
            _fileSystemMock.Verify(x => x.CreateFile(serverloadInformationModelsC, It.IsAny<string>()), Times.Once);
            
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_build), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_client_c), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_server_c), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_server_build), Times.AtLeastOnce);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_loadInformationModels_server_c), Times.AtLeastOnce);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_Should_IgnoreInput([ValueSource(nameof(InvalidInputsSecondParam))] string[] inputParams)
        {
            // Arrange
            var loggerListenerMock = new Mock<ILoggerListener>();
            var warnWrittenOut = false;
            loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var invalidNameCharsMock = new[] { '/' };
            var invalidPathCharsMock = new[] { '\\' };
            _fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidNameCharsMock);
            _fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(invalidPathCharsMock);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailure, inputParams.ElementAt(1)), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(It.IsAny<string>()), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_Should_IgnoreInput_UnknownParams([ValueSource(nameof(InvalidInputsFistParam))] string[] inputParams)
        {
            // Arrange
            var loggerListenerMock = new Mock<ILoggerListener>();
            var warnWrittenOut = false;
            loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var invalidNameCharsMock = new[] { '/' };
            var invalidPathCharsMock = new[] { '\\' };
            _fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidNameCharsMock);
            _fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(invalidPathCharsMock);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(OutputText.NewOpcuaappCommandFailureUnknownParam, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(x => x.LoadTemplateFile(It.IsAny<string>()), Times.Never);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
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
    }
}
