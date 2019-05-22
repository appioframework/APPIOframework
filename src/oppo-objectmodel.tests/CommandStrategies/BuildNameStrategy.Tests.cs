using System.Text;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class BuildNameStrategyTests
    {   
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

        protected static bool[][] FailingExecutableStates()
        {
            return new[]
            {
                new[] {false, false},
                new[] {true, false},
                new[] {false, true},
            };
        }

		private readonly string _oppoprojServerContent = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"localhost\",\"port\":\"4840\"}";
		private readonly string _oppoprojClientContent = "{\"name\":\"clientApp\",\"type\":\"Client\",\"references\":[{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"localhost\",\"port\":\"4840\"}]}";

		private readonly string _sampleServerConstantsFileContent = "const char* SERVER_APP_HOSTNAME = \"localhost\";\nconst UA_UInt16 SERVER_APP_PORT = 3000;";

		[Test]
        public void BuildNameStrategy_Should_ImplementICommandOfBuildStrategy()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();

            // Act
            var objectUnderTest = new BuildNameStrategy(string.Empty, fileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<BuildStrategy>>(objectUnderTest);
        }

        [Test]
        public void BuildNameStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new BuildNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void BuildNameStrategy_Should_ProvideExactHelpText()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new BuildNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.BuildNameArgumentCommandDescription, helpText);
        }

        [Test]
        public void ShouldExecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(0);

            var fileSystemMock = new Mock<IFileSystem>();
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(y => y.Warn(It.IsAny<string>()));
            OppoLogger.RegisterListener(loggerListenerMock.Object);
            var buildStrategy = new BuildNameStrategy(string.Empty, fileSystemMock.Object);

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(strategyResult.Success);
            Assert.AreEqual(string.Format(OutputText.OpcuaappBuildFailureProjectDoesNotExist, projectName), strategyResult.OutputMessages.First().Key);
            loggerListenerMock.Verify(y => y.Warn(It.IsAny<string>()),Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void BuildStrategy_ShouldFail_DueToFailingExecutableCalls([ValueSource(nameof(FailingExecutableStates))] bool[] executableStates)
        {
			// Arrange
			var projectName = "anyName";

            var mesonState = executableStates.ElementAt(0);
            var ninjaState = executableStates.ElementAt(1);

            var fileSystemMock = new Mock<IFileSystem>();

			fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);

            fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Meson, It.IsAny<string>(), It.IsAny<string>())).Returns(mesonState);
            fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Ninja, It.IsAny<string>(), It.IsAny<string>())).Returns(ninjaState);

			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);
			fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			var serverConstantsFilePath = Path.Combine();
			fileSystemMock.Setup(x => x.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp, Constants.FileName.SourceCode_constants_h)).Returns(serverConstantsFilePath);

			using (var oppoprojMemoryStreamFirstCall = new MemoryStream(Encoding.ASCII.GetBytes(_oppoprojServerContent)))
			using (var oppoprojMemoryStreamSecondCall = new MemoryStream(Encoding.ASCII.GetBytes(_oppoprojServerContent)))
			using (var serverConstantsMemoryStrean = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				fileSystemMock.SetupSequence(x => x.ReadFile(oppoprojFilePath)).Returns(oppoprojMemoryStreamFirstCall).Returns(oppoprojMemoryStreamSecondCall);
				fileSystemMock.Setup(x => x.ReadFile(serverConstantsFilePath)).Returns(serverConstantsMemoryStrean);

				var buildStrategy = new BuildNameStrategy(string.Empty, fileSystemMock.Object);
				var loggerListenerMock = new Mock<ILoggerListener>();
				loggerListenerMock.Setup(B => B.Warn(It.IsAny<string>()));
				OppoLogger.RegisterListener(loggerListenerMock.Object);

				// Act
				var strategyResult = buildStrategy.Execute(new[] { projectName });

				// Assert
				Assert.IsFalse(strategyResult.Success);
				Assert.AreEqual(OutputText.OpcuaappBuildFailure, strategyResult.OutputMessages.First().Key);
				fileSystemMock.Verify(x => x.ReadFile(oppoprojFilePath), Times.Exactly(2));
				loggerListenerMock.Verify(y => y.Warn(It.IsAny<string>()), Times.Once);
				OppoLogger.RemoveListener(loggerListenerMock.Object);
			}
        }

		[Test]
		public void BuildStrategy_ShouldFail_DueToNotExistingProject([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(0);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var fileSystemMock = new Mock<IFileSystem>();
			
			fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(false);

			var buildStrategy = new BuildNameStrategy(string.Empty, fileSystemMock.Object);

			// Act
			var strategyResult = buildStrategy.Execute(inputParams);

			// Assert
			Assert.IsFalse(strategyResult.Success);
			Assert.AreEqual(string.Format(OutputText.OpcuaappBuildFailureProjectDoesNotExist, projectName), strategyResult.OutputMessages.First().Key);
			fileSystemMock.VerifyAll();
			loggerListenerMock.Verify(y => y.Warn(LoggingText.BuildProjectDoesNotExist), Times.Once);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			fileSystemMock.Verify(x => x.DirectoryExists(projectName), Times.Once);
		}

		[Test]
		public void BuildStrategy_Should_SucceessOnBuildableServerProject([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(0);
			
			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var projectBuildDirectory = Path.Combine(projectName, Constants.DirectoryName.MesonBuild);

			var fileSystemMock = new Mock<IFileSystem>();

			fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);

			fileSystemMock.Setup(x => x.CombinePaths(It.IsAny<string>(), It.IsAny<string>())).Returns(projectBuildDirectory);
			fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild)).Returns(true);
			fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Ninja, projectBuildDirectory, string.Empty)).Returns(true);
			
			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);
			fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			var serverConstantsFilePath = Path.Combine();
			fileSystemMock.Setup(x => x.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp, Constants.FileName.SourceCode_constants_h)).Returns(serverConstantsFilePath);

			using (var oppoprojMemoryStreamFirstCall = new MemoryStream(Encoding.ASCII.GetBytes(_oppoprojServerContent)))
			using (var oppoprojMemoryStreamSecondCall = new MemoryStream(Encoding.ASCII.GetBytes(_oppoprojServerContent)))
			using (var serverConstantsMemoryStrean = new MemoryStream(Encoding.ASCII.GetBytes(_sampleServerConstantsFileContent)))
			{
				fileSystemMock.SetupSequence(x => x.ReadFile(oppoprojFilePath)).Returns(oppoprojMemoryStreamFirstCall).Returns(oppoprojMemoryStreamSecondCall);
				fileSystemMock.Setup(x => x.ReadFile(serverConstantsFilePath)).Returns(serverConstantsMemoryStrean);

				var buildStrategy = new BuildNameStrategy(string.Empty, fileSystemMock.Object);

				// Act
				var strategyResult = buildStrategy.Execute(inputParams);

				// Assert
				Assert.IsTrue(strategyResult.Success);
				Assert.AreEqual(string.Format(OutputText.OpcuaappBuildSuccess, projectName), strategyResult.OutputMessages.First().Key);
				fileSystemMock.VerifyAll();
				loggerListenerMock.Verify(y => y.Info(It.IsAny<string>()), Times.Once);
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				fileSystemMock.Verify(x => x.DirectoryExists(projectName), Times.Once);
			}
		}

		[Test]
		public void BuildStrategy_Should_SucceessOnBuildableClientProject([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(0);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var projectBuildDirectory = Path.Combine(projectName, Constants.DirectoryName.MesonBuild);

			var fileSystemMock = new Mock<IFileSystem>();

			fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);

			fileSystemMock.Setup(x => x.CombinePaths(It.IsAny<string>(), It.IsAny<string>())).Returns(projectBuildDirectory);
			fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild)).Returns(true);
			fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Ninja, projectBuildDirectory, string.Empty)).Returns(true);

			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);
			fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var oppoprojMemoryStreamFirstCall = new MemoryStream(Encoding.ASCII.GetBytes(_oppoprojClientContent)))
			using (var oppoprojMemoryStreamSecondCall = new MemoryStream(Encoding.ASCII.GetBytes(_oppoprojClientContent)))
			{
				fileSystemMock.SetupSequence(x => x.ReadFile(oppoprojFilePath)).Returns(oppoprojMemoryStreamFirstCall).Returns(oppoprojMemoryStreamSecondCall);

				var buildStrategy = new BuildNameStrategy(string.Empty, fileSystemMock.Object);

				// Act
				var strategyResult = buildStrategy.Execute(inputParams);

				// Assert
				Assert.IsTrue(strategyResult.Success);
				Assert.AreEqual(string.Format(OutputText.OpcuaappBuildSuccess, projectName), strategyResult.OutputMessages.First().Key);
				fileSystemMock.VerifyAll();
				loggerListenerMock.Verify(y => y.Info(It.IsAny<string>()), Times.Once);
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				fileSystemMock.Verify(x => x.DirectoryExists(projectName), Times.Once);
			}
		}
	}
}