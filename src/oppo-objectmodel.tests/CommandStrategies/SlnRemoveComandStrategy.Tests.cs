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
        protected static string[][] InvalidInputs_EmptyProjectName()
        {
            return new[]
            {
                new [] { "-s", "testSln", "-p", "" },
                new [] { "-s", "testSln", "--project", "" },
  
            };

        }
        protected static string[][] InvalidInputs_UnknownProjectParam()
        {
            return new[]
            {
                new [] { "-s", "testSln", "-P", "testApp" },
                new [] { "-s", "testSln", "--Project", "testApp" },
                new [] { "-s", "testSln", "--p", "testApp" },
                new [] { "-s", "testSln", "-project", "testApp" },
            };
        }

        protected static string [][] InvalidInputs_UnknownSolutionParam()
        {
            return new[]
            {
                new [] { "-S", "testSln", "-p", "testProj" },
                new [] { "--Solution", "testSln", "-p", "testProj" },
                new [] { "--s", "testSln", "-p", "testProj" },
                new [] { "-solution", "testSln", "-p", "testProj" },
            };
        }

        private Mock<IFileSystem> _fileSystemMock;
        private SlnRemoveCommandStrategy _objectUnderTest;

        private readonly string _defaultOpposlnContent = "{\"projects\": []}";
        private readonly string _sampleOpposlnContent = "{\"projects\": [{\"name\":\"testApp\",\"path\":\"testApp.oppoproj\",\"type\":\"ClientServer\",\"url\":\"opc.tcp://127.0.0.1:4840/\"}]}";
        private readonly string _sampleOppoprojContent1 = "{\"name\":\"testApp\",\"path\":\"testApp.oppoproj\",\"type\":\"ClientServer\",\"url\":\"opc.tcp://127.0.0.1:4840/\"}";
        private readonly string _sampleOppoprojContent2 = "{\"name\":\"myApp\",\"path\":\"myApp.oppoproj\",\"type\":\"Server\",\"url\":\"opc.tcp://127.0.0.1:4841/\"}";

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
            Assert.AreEqual(Constants.SlnCommandName.Remove, name);
        }

        [Test]
        public void HaveCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.SlnNameArgumentCommandDescription, helpText);
        }

        [Test]
        public void FailOnUnknownSolutionParametar([ValueSource(nameof(InvalidInputs_UnknownSolutionParam))] string[] inputParams)     
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
        public void FailOnUnknownProjectParametar([ValueSource(nameof(InvalidInputs_UnknownProjectParam))] string[] inputParams)
        {
            // Arrange
            var projectNameFlag = inputParams.ElementAtOrDefault(2);

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
            Assert.AreEqual(string.Format(OutputText.SlnUnknownParameter, projectNameFlag), firstMessageLine.Key);
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
            Assert.AreEqual(string.Format(OutputText.SlnOppoSlnNotFound, oppoSlnPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.CombinePaths(solutionName + Constants.FileExtension.OppoSln), Times.Once);
            _fileSystemMock.Verify(x => x.FileExists(oppoSlnPath), Times.Once);
        }

        [Test]
        public void FailBecauseOfEmptyOppoprojName([ValueSource(nameof(InvalidInputs_EmptyProjectName))] string[] inputParams)
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

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.SlnRemoveOppoprojNameEmpty), Times.Once);
            Assert.AreEqual(OutputText.SlnRemoveOpcuaappNameEmpty,firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
         
        }
    }

}

