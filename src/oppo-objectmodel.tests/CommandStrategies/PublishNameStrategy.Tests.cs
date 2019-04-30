using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System.Linq;
using System.IO;

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
        public void PublishNameStrategy_Should_ProvideSomeHelpText()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.PublishNameArgumentCommandDescription, helpText);
        }

        [Test]
        public void PublishNameStrategy_Should_PublishClientAndServerExecutablesOfClientServerApp()
        {
            // Arrange
            const string applicationName = "any-name";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OpcuaappPublishedSuccess));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var fileSystemMock = new Mock<IFileSystem>();

			var publishDirectory = Path.Combine(applicationName, Constants.DirectoryName.Publish);
            fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.Publish)).Returns(publishDirectory);

			var buildDirectory = Path.Combine(applicationName, Constants.DirectoryName.MesonBuild);
			fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.MesonBuild)).Returns(buildDirectory);

			var clientAppSourcePath = Path.Combine(buildDirectory, Constants.ExecutableName.AppClient);
			fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppSourcePath);

			var serverAppSourcePath = Path.Combine(buildDirectory, Constants.ExecutableName.AppServer);
            fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppSourcePath);

			var clientAppTargetPath = Path.Combine(publishDirectory, Constants.ExecutableName.AppClient);
            fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppTargetPath);

			var serverAppTargetPath = Path.Combine(publishDirectory, Constants.ExecutableName.AppServer);
            fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppTargetPath);

            fileSystemMock.Setup(x => x.FileExists(clientAppSourcePath)).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(serverAppSourcePath)).Returns(true);

            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(new[] { applicationName });

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.OutputMessages);
            Assert.AreEqual(string.Format(OutputText.OpcuaappPublishSuccess, applicationName), result.OutputMessages.First().Key);
            fileSystemMock.Verify(x => x.CreateDirectory(publishDirectory), Times.Once);
            fileSystemMock.Verify(x => x.CopyFile(clientAppSourcePath, clientAppTargetPath), Times.Once);
            fileSystemMock.Verify(x => x.CopyFile(serverAppSourcePath, serverAppTargetPath), Times.Once);
            loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappPublishedSuccess), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

		[Test]
		public void PublishNameStrategy_Should_PublishClientExecutableOfClientApp()
		{
			// Arrange
			const string applicationName = "any-name";

			var loggerListenerMock = new Mock<ILoggerListener>();
			loggerListenerMock.Setup(x => x.Info(LoggingText.OpcuaappPublishedSuccess));
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var fileSystemMock = new Mock<IFileSystem>();

			var publishDirectory = Path.Combine(applicationName, Constants.DirectoryName.Publish);
			fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.Publish)).Returns(publishDirectory);

			var buildDirectory = Path.Combine(applicationName, Constants.DirectoryName.MesonBuild);
			fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.MesonBuild)).Returns(buildDirectory);

			var clientAppSourcePath = Path.Combine(buildDirectory, Constants.ExecutableName.AppClient);
			fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppSourcePath);

			var serverAppSourcePath = Path.Combine(buildDirectory, Constants.ExecutableName.AppServer);
			fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppSourcePath);

			var clientAppTargetPath = Path.Combine(publishDirectory, Constants.ExecutableName.AppClient);
			fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppTargetPath);

			var serverAppTargetPath = Path.Combine(publishDirectory, Constants.ExecutableName.AppServer);
			fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppTargetPath);

			fileSystemMock.Setup(x => x.FileExists(clientAppSourcePath)).Returns(true);
			fileSystemMock.Setup(x => x.FileExists(serverAppSourcePath)).Returns(false);

			var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

			// Act
			var result = objectUnderTest.Execute(new[] { applicationName });

			// Assert
			Assert.IsTrue(result.Success);
			Assert.IsNotNull(result.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.OpcuaappPublishSuccess, applicationName), result.OutputMessages.First().Key);
			fileSystemMock.Verify(x => x.CreateDirectory(publishDirectory), Times.Once);
			fileSystemMock.Verify(x => x.CopyFile(clientAppSourcePath, clientAppTargetPath), Times.Once);
			fileSystemMock.Verify(x => x.CopyFile(serverAppSourcePath, serverAppTargetPath), Times.Never);
			loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappPublishedSuccess), Times.Once);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}

		[Test]
		public void PublishNameStrategy_Should_PublishServerExecutableOfServerApp()
		{

			// Arrange
			const string applicationName = "any-name";

			var loggerListenerMock = new Mock<ILoggerListener>();
			loggerListenerMock.Setup(x => x.Info(LoggingText.OpcuaappPublishedSuccess));
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var fileSystemMock = new Mock<IFileSystem>();

			var publishDirectory = Path.Combine(applicationName, Constants.DirectoryName.Publish);
			fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.Publish)).Returns(publishDirectory);

			var buildDirectory = Path.Combine(applicationName, Constants.DirectoryName.MesonBuild);
			fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.MesonBuild)).Returns(buildDirectory);

			var clientAppSourcePath = Path.Combine(buildDirectory, Constants.ExecutableName.AppClient);
			fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppSourcePath);

			var serverAppSourcePath = Path.Combine(buildDirectory, Constants.ExecutableName.AppServer);
			fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppSourcePath);

			var clientAppTargetPath = Path.Combine(publishDirectory, Constants.ExecutableName.AppClient);
			fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppTargetPath);

			var serverAppTargetPath = Path.Combine(publishDirectory, Constants.ExecutableName.AppServer);
			fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppTargetPath);

			fileSystemMock.Setup(x => x.FileExists(clientAppSourcePath)).Returns(false);
			fileSystemMock.Setup(x => x.FileExists(serverAppSourcePath)).Returns(true);

			var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

			// Act
			var result = objectUnderTest.Execute(new[] { applicationName });

			// Assert
			Assert.IsTrue(result.Success);
			Assert.IsNotNull(result.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.OpcuaappPublishSuccess, applicationName), result.OutputMessages.First().Key);
			fileSystemMock.Verify(x => x.CreateDirectory(publishDirectory), Times.Once);
			fileSystemMock.Verify(x => x.CopyFile(clientAppSourcePath, clientAppTargetPath), Times.Never);
			fileSystemMock.Verify(x => x.CopyFile(serverAppSourcePath, serverAppTargetPath), Times.Once);
			loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappPublishedSuccess), Times.Once);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}

        [Test]
        public void PublishNameStrategy_Should_IgnoreNotExistingBuildFiles()
        {
            const string applicationName = "any-name";
            const string buildDirectory = "build";
            const string publishDirectory = "publish";
            const string clientAppSourcePath = "client-source";
            const string serverAppSourcePath = "server-source";
            const string clientAppTargetPath = "client-target";
            const string serverAppTargetPath = "client-target";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Warn(LoggingText.MissingBuiltOpcuaAppFiles));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.Publish)).Returns(publishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.MesonBuild)).Returns(buildDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppSourcePath);
            fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppSourcePath);
            fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppClient)).Returns(clientAppTargetPath);
            fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.AppServer)).Returns(serverAppTargetPath);
            var objectUnderTest = new PublishNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(new[] { applicationName });

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.OutputMessages);
            Assert.AreEqual(string.Format(OutputText.OpcuaappPublishFailureMissingExecutables, applicationName), result.OutputMessages.First().Key);
            loggerListenerMock.Verify(x => x.Warn(LoggingText.MissingBuiltOpcuaAppFiles), Times.Once);
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
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.OutputMessages);
            Assert.AreEqual(OutputText.OpcuaappPublishFailure, result.OutputMessages.First().Key);
            loggerListenerMock.Verify(x => x.Warn(LoggingText.EmptyOpcuaappName), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}