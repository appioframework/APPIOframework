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
    public class SlnAddNameStrategyTestsShould
    {
        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new [] { "-s", "testSln", "-p", "testProj" },
                new [] { "-s", "testSln", "--project", "testProj" },
                new [] { "--solution", "testSln", "-p", "testProj" },
                new [] { "--solution", "testSln", "--project", "testProj" },
            };
        }
        
        protected static object[] BadFlagParams =
        {
	        new [] { "-s", "testSln", "--p", "testProj" },
	        new [] { "-s", "testSln", "--Project", "testProj" },
	        new [] { "-solution", "testSln", "-p", "testProj" },
	        new [] { "--Solution", "testSln", "--project", "testProj" },
        };

        private Mock<IFileSystem> _fileSystemMock;
        private SlnAddCommandStrategy _objectUnderTest;

        private readonly string _defaultOpposlnContent = "{\"projects\": []}";
        private readonly string _sampleOpposlnContent = "{\"projects\": [{\"name\":\"clientApp\",\"path\":\"clientApp/clientApp.oppoproj\"}]}";

        private readonly string _sampleOpcuaClientAppContent = "{\"name\":\"clientApp\",\"type\":\"Client\",\"references\": []}";
		private readonly string _sampleOpcuaServerAppContent = "{\"name\":\"serverApp\",\"type\":\"Server\"}";

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new SlnAddCommandStrategy(_fileSystemMock.Object);
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
            Assert.AreEqual(Constants.SlnCommandName.Add, name);
        }

        [Test]
        public void HaveCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.SlnAddNameArgumentCommandDescription, helpText);
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
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnOpposlnFileNotFound), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnOpposlnNotFound, oppoSlnPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.FileExists(oppoSlnPath), Times.Once);
        }

        [Test]
        public void FailBecauseOfMissingOppoprojFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange opposln file
            var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
            _fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

            // Arrange oppoproj file
            var oppoProjPath = Path.Combine(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject)).Returns(oppoProjPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjPath)).Returns(false);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnAddOppoprojFileNotFound), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnAddOpcuaappNotFound, oppoProjPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject), Times.Once);
            _fileSystemMock.Verify(x => x.FileExists(oppoProjPath), Times.Once);
        }

        [Test]
        public void FailOnOpposlnDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

            // Arrange oppoproj file
            var oppoProjPath = Path.Combine(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject)).Returns(oppoProjPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjPath)).Returns(true);
            

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnCouldntDeserliazeSln), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.ReadFile(solutionFullName), Times.Once);

			slnMemoryStream.Close();
			slnMemoryStream.Dispose();
        }

        [Test]
        public void FailOnOppoprojDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpposlnContent));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

            // Arrange oppoproj file
            var oppoProjPath = Path.Combine(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject)).Returns(oppoProjPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjPath)).Returns(true);

            Stream opcuaappMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty));
            _fileSystemMock.Setup(x => x.ReadFile(oppoProjPath)).Returns(opcuaappMemoryStream);

			
            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnAddCouldntDeserliazeOpcuaapp), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnAddCouldntDeserliazeOpcuaapp, opcuaappName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.ReadFile(oppoProjPath), Times.Once);

			slnMemoryStream.Close();
			slnMemoryStream.Dispose();
			opcuaappMemoryStream.Close();
			opcuaappMemoryStream.Dispose();
		}

        [Test]
        public void FailOnSlnAlreadyContainsOpcuaapp([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpposlnContent));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

            // Arrange oppoproj file
            var oppoProjPath = Path.Combine(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject)).Returns(oppoProjPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjPath)).Returns(true);

            Stream opcuaappMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent));
            _fileSystemMock.Setup(x => x.ReadFile(oppoProjPath)).Returns(opcuaappMemoryStream);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.SlnAddContainsOpcuaapp), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnAddContainsOpcuaapp, solutionName, opcuaappName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);

			slnMemoryStream.Close();
			slnMemoryStream.Dispose();
			opcuaappMemoryStream.Close();
			opcuaappMemoryStream.Dispose();
		}

        [Test]
        public void AddOpcuaClientAppToDefaultSln([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
			// Arrange
			var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpposlnContent));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

            // Arrange oppoproj file
            var oppoProjPath = Path.Combine(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject)).Returns(oppoProjPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjPath)).Returns(true);
            
            Stream opcuaappMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent));
            _fileSystemMock.Setup(x => x.ReadFile(oppoProjPath)).Returns(opcuaappMemoryStream);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsTrue(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.SlnAddSuccess), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnAddSuccess, opcuaappName, solutionName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.WriteFile(solutionFullName, It.IsAny<IEnumerable<string>>()), Times.Once);

			slnMemoryStream.Close();
			slnMemoryStream.Dispose();
			opcuaappMemoryStream.Close();
			opcuaappMemoryStream.Dispose();
		}

        [Test]
        public void AddOpcuaappToNotEmptySln([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
			// Arrange
			var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange opposln file
			var oppoSlnPath = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			_fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln)).Returns(oppoSlnPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoSlnPath)).Returns(true);

			var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.OppoSln);
			Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpposlnContent));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

            // Arrange oppoproj file
            var oppoProjPath = Path.Combine(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.OppoProject)).Returns(oppoProjPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjPath)).Returns(true);

            Stream opcuaappMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent));
            _fileSystemMock.Setup(x => x.ReadFile(oppoProjPath)).Returns(opcuaappMemoryStream);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsTrue(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.SlnAddSuccess), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnAddSuccess, opcuaappName, solutionName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.WriteFile(solutionFullName, It.IsAny<IEnumerable<string>>()), Times.Once);

			slnMemoryStream.Close();
			slnMemoryStream.Dispose();
			opcuaappMemoryStream.Close();
			opcuaappMemoryStream.Dispose();
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
