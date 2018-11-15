using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class PublishNameStrategyTests
    {
        [Test]
        public void PublishNameStrategy_Should_ImplementICommandOfPublishStrategy()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();

            // Act
            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<PublishStrategy>>(objectUnderTest);
        }

        [Test]
        public void PublishNameStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void PublishNameStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        [Test]
        public void PublishNameStrategy_Should_CreatePublishDirectoryContainingApplicationFiles()
        {
            // Arrange
            const string applicationName = "any-name";
            const string buildDirectory = "build";
            const string publishDirectory = "publish";
            const string applicationSourcePath = "source";
            const string applicationTargetPath = "target";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OpcuaappPublishedSuccess));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.Publish)).Returns(publishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.MesonBuild)).Returns(buildDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.App)).Returns(applicationSourcePath);
            fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.App)).Returns(applicationTargetPath);
            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(new[] {applicationName});

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.OpcuaappPublishSuccess, applicationName), result.Message);
            fileSystemMock.Verify(x => x.CreateDirectory(publishDirectory), Times.Once);
            fileSystemMock.Verify(x => x.CopyFile(applicationSourcePath, applicationTargetPath), Times.Once);
            loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappPublishedSuccess), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [TestCase(null)]
        [TestCase("")]
        public void PublishNameStrategy_Should_IgnoreInvalidInputParams(string applicationName)
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Warn(LoggingText.EmptyOpcuaappName));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = objectUnderTest.Execute(new[] {applicationName});

            // Assert
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(OutputText.OpcuaappPublishFailure, result.Message);
            loggerListenerMock.Verify(x => x.Warn(LoggingText.EmptyOpcuaappName), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}