using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.DeployCommands;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class DeployNameStrategyTests
    {
        const string _deployDirectory = "deploy";
        const string _appClientPublishLocation = "publish\\clientApp";
        const string _appServerPublishLocation = "publish\\serverApp";
        const string _appClientDeployLocation = "deploy\\clientApp";
        const string _appServerDeployLocation = "deploy\\serverApp";
        const string _deployTempDirectory = "deploy\\temp";
        const string _appClientDeployTempLocation = "deploy\\temp\\oppo-opcuaapp\\usr\\bin\\clientApp";
        const string _appServerDeployTempLocation = "deploy\\temp\\oppo-opcuaapp\\usr\\bin\\serverApp";

        protected static string[][] InvalidInputs()
        {
            return new[]
            {
                new []{""},
                new string[0],
            };
        }

        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new []{"hugo"}
            };
        }
               
        [Test]
        public void DeployNameStrategy_Should_ImplementICommandOfBuildStrategy()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();

            // Act
            var objectUnderTest = new DeployNameStrategy(string.Empty, fileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<DeployStrategy>>(objectUnderTest);
        }

        [Test]
        public void DeployameStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new DeployNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void DeployNameStrategy_Should_ProvideExactHelpText()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new DeployNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.DeployNameArgumentCommandDescription, helpText);
        }

        [Test]
        public void DeployStrategy_Should_SucceedOnDeployProject([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var projectDirectoryName = inputParams.ElementAt(0);
            var projectPublishDirectory = Path.Combine(projectDirectoryName, Constants.DirectoryName.Publish);
            
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Publish)).Returns(projectPublishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Deploy)).Returns(_deployDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientDeployLocation);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerDeployLocation);

            
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Deploy)).Returns(_deployDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.DirectoryName.Temp)).Returns(_deployTempDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployTempDirectory, "oppo-opcuaapp", "usr", "bin", Constants.ExecutableName.AppClient)).Returns(_appClientDeployTempLocation);
            fileSystemMock.Setup(x => x.CombinePaths(_deployTempDirectory, "oppo-opcuaapp", "usr", "bin", Constants.ExecutableName.AppServer)).Returns(_appServerDeployTempLocation);
                          
            // conitue work here

            fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.CreateDebianInstaller, It.IsAny<string>(), Constants.ExecutableName.CreateDebianInstallerArguments)).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(_appClientPublishLocation)).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(_appServerPublishLocation)).Returns(true);

            var deployStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Info(It.IsAny<string>()));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = deployStrategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.AreEqual(string.Format(OutputText.OpcuaappDeploySuccess, projectDirectoryName), strategyResult.OutputMessages.First().Key);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            fileSystemMock.Verify(x => x.CreateDirectory(_deployDirectory), Times.Once);
            //fileSystemMock.Verify(x => x.CopyFile(_appClientPublishLocation, _appClientDeployLocation), Times.Once);
            //fileSystemMock.Verify(x => x.CopyFile(_appServerPublishLocation, _appServerDeployLocation), Times.Once);
            fileSystemMock.Verify(x => x.CopyFile(_appClientPublishLocation, _appClientDeployTempLocation), Times.Once); 
            fileSystemMock.Verify(x => x.CopyFile(_appServerPublishLocation, _appServerDeployTempLocation), Times.Once);
            loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappDeploySuccess), Times.Once);
            loggerListenerMock.Verify(y => y.Info(It.IsAny<string>()), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void DeployStrategy_Should_FailOnDeployProject_MissingPublishedClientFiles([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var projectDirectoryName = inputParams.ElementAt(0);
            var projectPublishDirectory = Path.Combine(projectDirectoryName, Constants.DirectoryName.Publish);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Publish)).Returns(projectPublishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Deploy)).Returns(_deployDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientDeployLocation);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerDeployLocation);

            fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.CreateDebianInstaller, _deployDirectory, It.IsAny<string>())).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(_appClientPublishLocation)).Returns(false);
            fileSystemMock.Setup(x => x.FileExists(_appServerPublishLocation)).Returns(true);

            var deployStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Info(It.IsAny<string>()));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = deployStrategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(strategyResult.Sucsess);
            Assert.AreEqual(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectDirectoryName), strategyResult.OutputMessages.First().Key);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            fileSystemMock.Verify(x => x.CreateDirectory(_deployDirectory), Times.Never);
            fileSystemMock.Verify(x => x.CopyFile(_appClientPublishLocation, _appClientDeployLocation), Times.Never);
            fileSystemMock.Verify(x => x.CopyFile(_appServerPublishLocation, _appServerDeployLocation), Times.Never);
            loggerListenerMock.Verify(x => x.Warn(LoggingText.MissingPublishedOpcuaAppFiles), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void DeployStrategy_Should_FailOnDeployProject_MissingPublishedServerFiles([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var projectDirectoryName = inputParams.ElementAt(0);
            var projectPublishDirectory = Path.Combine(projectDirectoryName, Constants.DirectoryName.Publish);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Publish)).Returns(projectPublishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Deploy)).Returns(_deployDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientDeployLocation);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerDeployLocation);

            fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.CreateDebianInstaller, _deployDirectory, It.IsAny<string>())).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(_appClientPublishLocation)).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(_appServerPublishLocation)).Returns(false);

            var deployStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Info(It.IsAny<string>()));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = deployStrategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(strategyResult.Sucsess);
            Assert.AreEqual(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectDirectoryName), strategyResult.OutputMessages.First().Key);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            fileSystemMock.Verify(x => x.CreateDirectory(_deployDirectory), Times.Never);
            fileSystemMock.Verify(x => x.CopyFile(_appClientPublishLocation, _appClientDeployLocation), Times.Never);
            fileSystemMock.Verify(x => x.CopyFile(_appServerPublishLocation, _appServerDeployLocation), Times.Never);
            loggerListenerMock.Verify(x => x.Warn(LoggingText.MissingPublishedOpcuaAppFiles), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ShouldExecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Warn(It.IsAny<string>()));
            OppoLogger.RegisterListener(loggerListenerMock.Object);
            var deployStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var strategyResult = deployStrategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(strategyResult.Sucsess);
            Assert.AreEqual(OutputText.OpcuaappDeployFailure, strategyResult.OutputMessages.First().Key);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            loggerListenerMock.Verify(y => y.Warn(It.IsAny<string>()), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void DeployStrategy_ShouldFail_DueToFailingExecutableCalls([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var projectDirectoryName = inputParams.ElementAt(0);
            var projectPublishDirectory = Path.Combine(projectDirectoryName, Constants.DirectoryName.Publish);
           
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.CreateDebianInstaller, _deployDirectory, It.IsAny<string>())).Returns(false);
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Publish)).Returns(projectPublishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Deploy)).Returns(_deployDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientDeployLocation);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerDeployLocation);
            fileSystemMock.Setup(x => x.FileExists(_appClientPublishLocation)).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(_appServerPublishLocation)).Returns(true);

            var buildStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(B => B.Warn(It.IsAny<string>()));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = buildStrategy.Execute(new[] { projectDirectoryName });

            // Assert
            Assert.IsFalse(strategyResult.Sucsess);
            Assert.AreEqual(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectDirectoryName), strategyResult.OutputMessages.First().Key);

            loggerListenerMock.Verify(y => y.Warn(LoggingText.CreateDebianInstallerFails), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}