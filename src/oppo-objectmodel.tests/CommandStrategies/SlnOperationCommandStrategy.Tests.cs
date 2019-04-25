using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using Oppo.Resources.text.output;
using System.Text;
using System.IO;

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
		private Mock<ICommand> _commandMock;
		private SlnOperationData _operationData;
        private SlnOperationCommandStrategy _objectUnderTest;

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
			_commandMock = new Mock<ICommand>();

			_operationData.CommandName			= "anyName";
			_operationData.FileSystem			= _fileSystemMock.Object;
			_operationData.Subcommand			= _commandMock.Object;
			_operationData.SuccessLoggerMessage = "anyText";
			_operationData.SuccessOutputMessage = "anyText";
			_operationData.HelpText				= "anyText";

			_objectUnderTest = new SlnOperationCommandStrategy(_operationData);

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
            Assert.AreEqual(_operationData.CommandName, name);
        }

        [Test]
        public void HaveCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(_operationData.HelpText, helpText);
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
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
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
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
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
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
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
			
			_commandMock.Setup(x => x.Execute(It.IsAny<string[]>())).Returns(new CommandResult(false, new MessageLines()));

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
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
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
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				Assert.AreEqual(_operationData.SuccessOutputMessage, firstMessageLine.Key);
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

			_commandMock.Setup(x => x.Execute(It.IsAny<string[]>())).Returns(new CommandResult(true, new MessageLines()));

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
				
				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);
				
				// Assert
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				Assert.AreEqual(_operationData.SuccessOutputMessage, firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}
    }
}
