using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using Oppo.Resources.text.output;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class SlnBuildNameStrategyTestsShould
    {
        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new [] { "-s", "testSln" },
                new [] { "--solution", "testSln" },
            };
        }

        protected static string [][] InvalidInputs_UnknownSolutionParam()
        {
            return new[]
            {
                new [] { "-S", "testSln"},
                new [] { "--Solution", "testSln" },
                new [] { "--s", "testSln" },
                new [] { "-solution", "testSln" },
            };
        }

        private Mock<IFileSystem> _fileSystemMock;
        private SlnBuildCommandStrategy _objectUnderTest;

        private readonly string _defaultOpposlnContent = "{\"projects\": []}";
		private readonly string _sampleOpposlnContentWithOneProject = "{\"projects\": [{\"name\":\"" + _sampleOpcuaClientAppName + "\",\"path\":\"" + _sampleOpcuaClientAppName + "/" + _sampleOpcuaClientAppName + ".oppoproj\"}]}";
		private readonly string _sampleOpposlnContentWithTwoProjects = "{\"projects\": [" +
																			"{\"name\":\"" + _sampleOpcuaClientAppName + "\",\"path\":\"" + _sampleOpcuaClientAppName + "/" + _sampleOpcuaClientAppName + ".oppoproj\"}," +
																			"{\"name\":\"" + _sampleOpcuaServerAppName + "\",\"path\":\"" + _sampleOpcuaServerAppName + "/" + _sampleOpcuaServerAppName + ".oppoproj\"}]}";

		private const string _sampleOpcuaClientAppName = "clientApp";
		private readonly string _sampleOpcuaClientAppContent = "{\"name\":\"" + _sampleOpcuaClientAppName + "\",\"type\":\"Client\"}";

		private const string _sampleOpcuaServerAppName = "serverApp";
		private readonly string _sampleOpcuaServerAppContent = "{\"name\":\"" + _sampleOpcuaServerAppName + "\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"4000\"}";

		[SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new SlnBuildCommandStrategy(_fileSystemMock.Object);
        }

        [Test]
        public void ImplementICommandOfSlnAddStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<SlnStrategy>>(_objectUnderTest);
        }

        [Test]
        public void HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var name = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.SlnCommandName.Build, name);
        }

        [Test]
        public void HaveCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.SlnBuildNameArgumentCommandDescription, helpText);
        }

        [Test]
        public void FailBecauseOfUknownSolutionParam([ValueSource(nameof(InvalidInputs_UnknownSolutionParam))] string[] inputParams)
        {
            // Arrange
            var solutionNameFlag = inputParams.ElementAtOrDefault(0);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnUnknownCommandParam), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnUnknownParameter, solutionNameFlag), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }

        [Test]
        public void FailBecauseOfMissingOpposlnFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var solutionName = inputParams.ElementAtOrDefault(1);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange opposln file
            var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
            _fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(false);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnOpposlnFileNotFound), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnOpposlnNotFound, oppoSlnPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.FileExists(oppoSlnPath), Times.Once);
        }

        [Test]
        public void FailOnOpposlnDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			using (Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);
				
				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Sucsess);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnCouldntDeserliazeSln), Times.Once);
				Assert.AreEqual(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
				_fileSystemMock.Verify(x => x.ReadFile(solutionFullName), Times.Once);
			}
        }

		[Test]
		public void FailOnBuildingNotExisitingProject([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var solutionName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			using (Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpposlnContentWithOneProject)))
			using (var oppoprojMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Sucsess);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.BuildProjectDoesNotExist), Times.Once);
				Assert.AreEqual(string.Format(OutputText.OpcuaappBuildFailureProjectDoesNotExist, _sampleOpcuaClientAppName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}

		[Test]
		public void PassThroughEmptySolution([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var solutionName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);

			using (var slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpposlnContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);
				
				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);
				
				// Assert
				Assert.IsTrue(commandResult.Sucsess);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.SlnBuildSuccess), Times.Once);
				Assert.AreEqual(string.Format(OutputText.SlnBuildSuccess, solutionName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}
		
        [Test]
        public void BuildAllSolutionsProjects([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
			// Arrange
			var solutionName = inputParams.ElementAtOrDefault(1);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);

			using (var slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpposlnContentWithTwoProjects)))
			using (var clientOppoprojMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			using (var serverOppoprojMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

				// Arrange clientApp build command
				_fileSystemMock.Setup(x => x.DirectoryExists(_sampleOpcuaClientAppName)).Returns(true);
				_fileSystemMock.Setup(x => x.CombinePaths(_sampleOpcuaClientAppName, _sampleOpcuaClientAppName + Constants.FileExtension.OppoProject)).Returns(Path.Combine(_sampleOpcuaClientAppName, _sampleOpcuaClientAppName + Constants.FileExtension.OppoProject));
				_fileSystemMock.Setup(x => x.ReadFile(Path.Combine(_sampleOpcuaClientAppName, _sampleOpcuaClientAppName + Constants.FileExtension.OppoProject))).Returns(clientOppoprojMemoryStream);
				_fileSystemMock.Setup(x => x.CombinePaths(_sampleOpcuaClientAppName, Constants.DirectoryName.MesonBuild)).Returns(Path.Combine(_sampleOpcuaClientAppName, Constants.DirectoryName.MesonBuild));
				_fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Meson, _sampleOpcuaClientAppName, Constants.DirectoryName.MesonBuild)).Returns(true);
				_fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Ninja, Path.Combine(_sampleOpcuaClientAppName, Constants.DirectoryName.MesonBuild), string.Empty)).Returns(true);

				// Arrange serverApp build command
				_fileSystemMock.Setup(x => x.DirectoryExists(_sampleOpcuaServerAppName)).Returns(true);
				_fileSystemMock.Setup(x => x.CombinePaths(_sampleOpcuaServerAppName, _sampleOpcuaServerAppName + Constants.FileExtension.OppoProject)).Returns(Path.Combine(_sampleOpcuaServerAppName, _sampleOpcuaServerAppName + Constants.FileExtension.OppoProject));
				_fileSystemMock.Setup(x => x.ReadFile(Path.Combine(_sampleOpcuaServerAppName, _sampleOpcuaServerAppName + Constants.FileExtension.OppoProject))).Returns(serverOppoprojMemoryStream);
				_fileSystemMock.Setup(x => x.CombinePaths(_sampleOpcuaServerAppName, Constants.DirectoryName.MesonBuild)).Returns(Path.Combine(_sampleOpcuaServerAppName, Constants.DirectoryName.MesonBuild));
				_fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Meson, _sampleOpcuaServerAppName, Constants.DirectoryName.MesonBuild)).Returns(true);
				_fileSystemMock.Setup(x => x.CallExecutable(Constants.ExecutableName.Ninja, Path.Combine(_sampleOpcuaServerAppName, Constants.DirectoryName.MesonBuild), string.Empty)).Returns(true);


				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);


				// Assert
				Assert.IsTrue(commandResult.Sucsess);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.SlnBuildSuccess), Times.Once);
				Assert.AreEqual(string.Format(OutputText.SlnBuildSuccess, solutionName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}
    }
}
