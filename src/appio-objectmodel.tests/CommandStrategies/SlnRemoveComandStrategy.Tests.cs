using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.SlnCommands;
using Appio.Resources.text.output;
using System.Text;
using System.IO;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
    public class SlnRemoveComandStrategyTestsShould
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
            new[] {"-s", "testSln", "-P", "testProj"},
            new[] {"-s", "testSln", "-Project", "testProj"},
            new[] {"--s", "testSln", "-p", "testProj"},
            new[] {"--Solution", "testSln", "--project", "testProj"},
        };
        
        private Mock<IFileSystem> _fileSystemMock;
        private SlnRemoveCommandStrategy _objectUnderTest;

        private readonly string _defaultAppioslnContent = "{\"projects\": []}";
        private readonly string _sampleAppioslnContent = "{\"projects\": [{\"name\":\"testProj\",\"path\":\"testProj.appioproj\",\"type\":\"ClientServer\",\"url\":\"opc.tcp://127.0.0.1:4840/\"}]}";
       

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new SlnRemoveCommandStrategy(_fileSystemMock.Object);
        }

        [Test]
        public void ImplementICommandOfSlnRemoveStrategy()
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
            Assert.AreEqual(Constants.SlnCommandArguments.Remove, name);
        }

        [Test]
        public void HaveCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.SlnRemoveNameArgumentCommandDescription, helpText);
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
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnAppioslnFileNotFound), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnAppioslnNotFound, appioslnPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln), Times.Once);
            _fileSystemMock.Verify(x => x.FileExists(appioslnPath), Times.Once);
        }

        [Test]
        public void FailOnAppioslnDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange appiosln file
            var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
            _fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
            _fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(true);

            var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
            Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

            // Arrange appioproj file
            var appioprojPath = Path.Combine(opcuaappName, opcuaappName + Constants.FileExtension.Appioproject);
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaappName, opcuaappName + Constants.FileExtension.Appioproject)).Returns(appioprojPath);
            _fileSystemMock.Setup(x => x.FileExists(appioprojPath)).Returns(true);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            AppioLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnCouldntDeserliazeSln), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnCouldntDeserliazeSln, solutionName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.ReadFile(solutionFullName), Times.Once);

            slnMemoryStream.Close();
            slnMemoryStream.Dispose();

        }

        [Test]
        public void FailOnSlnDoesNotContainOpcuaapp([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange appiosln file
            var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
            _fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
            _fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(true);

            var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
            Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultAppioslnContent));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);
			
            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            AppioLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnRemoveOpcuaappIsNotInSln), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnRemoveOpcuaappIsNotInSln, opcuaappName, solutionName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);

            slnMemoryStream.Close();
            slnMemoryStream.Dispose();
        }
        [Test]
        public void RemoveOpcuaappFormSln([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var solutionName = inputParams.ElementAtOrDefault(1);
            var opcuaappName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange appiosln file
            var appioslnPath = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
            _fileSystemMock.Setup(x => x.CombinePaths(solutionName + Constants.FileExtension.Appiosln)).Returns(appioslnPath);
            _fileSystemMock.Setup(x => x.FileExists(appioslnPath)).Returns(true);

            var solutionFullName = Path.Combine(solutionName + Constants.FileExtension.Appiosln);
            Stream slnMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleAppioslnContent));
            _fileSystemMock.Setup(x => x.ReadFile(solutionFullName)).Returns(slnMemoryStream);

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsTrue(commandResult.Success);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            AppioLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.SlnRemoveSuccess), Times.Once);
            Assert.AreEqual(string.Format(OutputText.SlnRemoveSuccess, opcuaappName, solutionName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);

            slnMemoryStream.Close();
            slnMemoryStream.Dispose();
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

