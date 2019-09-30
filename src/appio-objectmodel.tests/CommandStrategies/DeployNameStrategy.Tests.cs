/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.DeployCommands;
using Appio.Resources.text.logging;
using Appio.Resources.text.output;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
	public class DeployNameStrategyTests
	{
		const string _deployDirectory = "deploy";
		const string _appClientPublishLocation = "publish\\clientApp";
		const string _appServerPublishLocation = "publish\\serverApp";
		const string _appClientDeployLocation = "deploy\\clientApp";
		const string _appServerDeployLocation = "deploy\\serverApp";
		const string _deployTempDirectory = "deploy\\temp";
		const string _appClientDeployTempLocation = "deploy\\temp\\appio-opcuaapp\\usr\\bin\\clientApp";
		const string _appServerDeployTempLocation = "deploy\\temp\\appio-opcuaapp\\usr\\bin\\serverApp";

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

		protected static bool[][] FilesExistanceFlags()
		{
			return new[]
			{
				new [] { true, true },
				new [] { true, false },
				new [] { false, true }
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
        public void DeployStrategy_Should_SucceedOnDeployProject([ValueSource(nameof(ValidInputs))] string[] inputParams, [ValueSource(nameof(FilesExistanceFlags))] bool[] existanceFlags)
        {
            // Arrange
            var projectDirectoryName = inputParams.ElementAt(0);
            var projectPublishDirectory = Path.Combine(projectDirectoryName, Constants.DirectoryName.Publish);
			var publishedClientExistanceFlag = existanceFlags.ElementAtOrDefault(0);
			var publishedServerExistanceFlag = existanceFlags.ElementAtOrDefault(1);
			
			var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Publish)).Returns(projectPublishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerPublishLocation);
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Deploy)).Returns(_deployDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppClient)).Returns(_appClientDeployLocation);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.ExecutableName.AppServer)).Returns(_appServerDeployLocation);
            
            fileSystemMock.Setup(x => x.CombinePaths(projectDirectoryName, Constants.DirectoryName.Deploy)).Returns(_deployDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployDirectory, Constants.DirectoryName.Temp)).Returns(_deployTempDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(_deployTempDirectory, Constants.DirectoryName.OpcuaappInstaller, Constants.DirectoryName.Usr, Constants.DirectoryName.Bin, Constants.ExecutableName.AppClient)).Returns(_appClientDeployTempLocation);
            fileSystemMock.Setup(x => x.CombinePaths(_deployTempDirectory, Constants.DirectoryName.OpcuaappInstaller, Constants.DirectoryName.Usr, Constants.DirectoryName.Bin, Constants.ExecutableName.AppServer)).Returns(_appServerDeployTempLocation);
                          
            // conitue work here

            fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.CreateDebianInstaller, It.IsAny<string>(), Constants.ExecutableName.CreateDebianInstallerArguments)).Returns(true);
            fileSystemMock.Setup(x => x.FileExists(_appClientPublishLocation)).Returns(publishedClientExistanceFlag);
            fileSystemMock.Setup(x => x.FileExists(_appServerPublishLocation)).Returns(publishedServerExistanceFlag);

            var deployStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Info(It.IsAny<string>()));
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = deployStrategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(strategyResult.Success);
            Assert.AreEqual(string.Format(OutputText.OpcuaappDeploySuccess, projectDirectoryName), strategyResult.OutputMessages.First().Key);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            fileSystemMock.Verify(x => x.CreateDirectory(_deployDirectory), Times.Once);
			if (publishedClientExistanceFlag)
			{
				fileSystemMock.Verify(x => x.CopyFile(_appClientPublishLocation, _appClientDeployTempLocation), Times.Once);
			}
			else
			{
				fileSystemMock.Verify(x => x.CopyFile(_appClientPublishLocation, _appClientDeployTempLocation), Times.Never);
			}
			if (publishedServerExistanceFlag)
			{
				fileSystemMock.Verify(x => x.CopyFile(_appServerPublishLocation, _appServerDeployTempLocation), Times.Once);
			}
			else
			{
				fileSystemMock.Verify(x => x.CopyFile(_appServerPublishLocation, _appServerDeployTempLocation), Times.Never);
			}
            loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappDeploySuccess), Times.Once);
            loggerListenerMock.Verify(y => y.Info(It.IsAny<string>()), Times.Once);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void DeployStrategy_Should_FailOnDeployProject_MissingPublishedFiles([ValueSource(nameof(ValidInputs))] string[] inputParams)
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
            fileSystemMock.Setup(x => x.FileExists(_appServerPublishLocation)).Returns(false);

            var deployStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Info(It.IsAny<string>()));
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = deployStrategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(strategyResult.Success);
            Assert.AreEqual(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectDirectoryName), strategyResult.OutputMessages.First().Key);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            fileSystemMock.Verify(x => x.CreateDirectory(_deployDirectory), Times.Never);
            fileSystemMock.Verify(x => x.CopyFile(_appClientPublishLocation, _appClientDeployLocation), Times.Never);
            fileSystemMock.Verify(x => x.CopyFile(_appServerPublishLocation, _appServerDeployLocation), Times.Never);
            loggerListenerMock.Verify(x => x.Warn(LoggingText.MissingPublishedOpcuaAppFiles), Times.Once);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void ShouldExecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Warn(It.IsAny<string>()));
            AppioLogger.RegisterListener(loggerListenerMock.Object);
            var deployStrategy = new DeployNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var strategyResult = deployStrategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(strategyResult.Success);
            Assert.AreEqual(OutputText.OpcuaappDeployFailure, strategyResult.OutputMessages.First().Key);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            loggerListenerMock.Verify(y => y.Warn(It.IsAny<string>()), Times.Once);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
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
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = buildStrategy.Execute(new[] { projectDirectoryName });

            // Assert
            Assert.IsFalse(strategyResult.Success);
            Assert.AreEqual(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectDirectoryName), strategyResult.OutputMessages.First().Key);

            loggerListenerMock.Verify(y => y.Warn(LoggingText.CreateDebianInstallerFails), Times.Once);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}