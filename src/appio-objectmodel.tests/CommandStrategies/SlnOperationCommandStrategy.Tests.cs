using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.SlnCommands;
using Appio.Resources.text.output;
using System.Text;
using System.IO;

namespace Appio.ObjectModel.Tests.CommandStrategies
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

        protected static object[] BadFlagParams =
        {
	        new [] { "--s", "testSln" },
	        new [] { "-Solution", "testSln" }
        };

        private Mock<IFileSystem> _fileSystemMock;
		private Mock<ICommand> _commandMock;
		private SlnOperationData _operationData;
        private SlnOperationCommandStrategy _objectUnderTest;

        private readonly string _defaultAppioslnContent = "{\"projects\": []}";
		private readonly string _sampleAppioslnContentWithOneProject = "{\"projects\": [{\"name\":\"" + _sampleOpcuaClientAppName + "\",\"path\":\"" + _sampleOpcuaClientAppName + "/" + _sampleOpcuaClientAppName + ".appioproj\"}]}";
		private readonly string _sampleAppioslnContentWithTwoProjects = "{\"projects\": [" +
																			"{\"name\":\"" + _sampleOpcuaClientAppName + "\",\"path\":\"" + _sampleOpcuaClientAppName + "/" + _sampleOpcuaClientAppName + ".appioproj\"}," +
																			"{\"name\":\"" + _sampleOpcuaServerAppName + "\",\"path\":\"" + _sampleOpcuaServerAppName + "/" + _sampleOpcuaServerAppName + ".appioproj\"}]}";

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
        public void FailBecauseOfMissingAppioslnFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var solutionName = inputParams.ElementAtOrDefault(1);

            var loggerListenerMock = new Mock<ILoggerListener>();
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange appiosln file
            var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
            _fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
            _fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(false);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            AppioLogger.RemoveListener(loggerListenerMock.Object);
            Assert.AreEqual(string.Format(OutputText.SlnAppioslnNotFound, appioslnPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.FileExists(appioslnPath), Times.Once);
        }

        [Test]
        public void FailOnAppioslnDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);

            var loggerListenerMock = new Mock<ILoggerListener>();
            AppioLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange appiosln file
			var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
			_fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
			using (Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);
				
				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
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
			AppioLogger.RegisterListener(loggerListenerMock.Object);
			
			_commandMock.Setup(x => x.Execute(It.IsAny<string[]>())).Returns(new CommandResult(false, new MessageLines()));

			// Arrange appiosln file
			var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
			_fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
			using (Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleAppioslnContentWithOneProject)))
			using (var appioprojMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
			}
		}

		[Test]
		public void PassThroughEmptySolution([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var solutionName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange appiosln file
			var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
			_fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.Appiosln);

			using (var slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultAppioslnContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);
				
				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);
				
				// Assert
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
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
            AppioLogger.RegisterListener(loggerListenerMock.Object);

			_commandMock.Setup(x => x.Execute(It.IsAny<string[]>())).Returns(new CommandResult(true, new MessageLines()));

			// Arrange appiosln file
			var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
			_fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.Appiosln);

			using (var slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleAppioslnContentWithTwoProjects)))
			using (var clientAppioprojMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			using (var serverAppioprojMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);
				
				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);
				
				// Assert
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
				Assert.AreEqual(_operationData.SuccessOutputMessage, firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}
        
        [Test]
        public void FailOnIncorrectFlags([ValueSource(nameof(BadFlagParams))] string[] inputParams)
        {
	        var result = _objectUnderTest.Execute(inputParams);
            
	        Assert.IsFalse(result.Success);
	        var unknownParameterStart = string.Join(" ", OutputText.UnknownParameterProvided.Split().Take(2));
	        Assert.That(() => result.OutputMessages.First().Key.StartsWith(unknownParameterStart));
        }
    }
}
