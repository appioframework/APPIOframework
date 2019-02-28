using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.ReferenceCommands;
using Oppo.Resources.text.output;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    class ReferenceAddCommandStrategySholud
    {
        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new [] { "-S", "testServer", "-C", "testClient" },
                new [] { "-S", "testServer", "--client", "testClient" },
                new [] { "--server", "testServer", "-C", "testClient" },
                new [] { "--server", "testServer", "--client", "testClient" },
            };
        }
        protected static string[][] InvalidInputs_UnknownClientParam()
        {
            return new[]
            {
                new [] { "-S", "testServer", "--c", "testClient" },
                new [] { "-S", "testServer", "--Client", "testClient" },
                new [] { "--server", "testServer", "-c", "testClient" },
                new [] { "--server", "testServer", "-client", "testClient" },
            };
        }

        protected static string[][] InvalidInputs_UnknownServerParam()
        {
            return new[]
            {
                new [] { "--S", "testserver", "-C", "testClient" },
                new [] { "-server", "testServer", "-c", "testClient" },
                new [] { "--S", "testServer", "-c", "testClient" },
                new [] { "-server", "testServer", "-c", "testClient" },
            };
        }
        private Mock<IFileSystem> _fileSystemMock;
        private ReferenceAddCommandStrategy _objectUnderTest;

        private readonly string _defaultOpposlnContent = "{\"projects\": []}";
        private readonly string _sampleOpposlnContent = "{\"projects\": [{\"name\":\"testApp\",\"path\":\"testApp.oppoproj\"}]}";

        private readonly string _sampleOpcuaClientAppContent = "{\"name\":\"testApp\",\"type\":\"Client\"}";
        private readonly string _sampleOpcuaServerAppContent = "{\"name\":\"testApp\",\"type\":\"Server\",\"url\":\"opc.tcp://127.0.0.1:4841/\"}";
        private readonly string _sampleOpcuaClientServerAppContent = "{\"name\":\"testApp\",\"type\":\"ClientServer\",\"url\":\"opc.tcp://127.0.0.1:4841/\"}";
        private readonly string _sampleOppoprojContent2 = "{\"name\":\"myApp\",\"path\":\"myApp.oppoproj\",\"type\":\"Server\",\"url\":\"opc.tcp://127.0.0.1:4841/\"}";

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new ReferenceAddCommandStrategy(_fileSystemMock.Object);
        }
        [Test]
        public void ImplementICommandOfReferenceAddStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<ReferenceStrategy>>(_objectUnderTest);
        }

        [Test]
        public void HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var name = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.ReferenceCommandName.Add, name);
        }

        [Test]
        public void HaveCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.ReferenceAddNameArgumentCommandDescription, helpText);
        }
        [Test]
        public void FailBecauseOfUnkownServerParam([ValueSource(nameof(InvalidInputs_UnknownServerParam))] string[] inputParams)
        {
            // Arrange
            var serverNameFlag = inputParams.ElementAtOrDefault(0);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceUnknownCommandParam), Times.Once);
            Assert.AreEqual(string.Format(OutputText.ReferenceUnknownParameter, serverNameFlag), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);

        }
        [Test]
        public void FailBecauseOfUnkownClientParam([ValueSource(nameof(InvalidInputs_UnknownClientParam))] string[] inputParams)
        {
            // Arrange
            var clientNameFlag = inputParams.ElementAtOrDefault(2);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceUnknownCommandParam), Times.Once);
            Assert.AreEqual(string.Format(OutputText.ReferenceUnknownParameter, clientNameFlag), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
           

        }
       
        [Test]
        public void FailBecauseOfMissingServerFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var serverName = inputParams.ElementAtOrDefault(1);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange server file
            var oppoProjectPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoProjectPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjectPath)).Returns(false);


            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.OppoServerFileNotFound), Times.Once);
            Assert.AreEqual(string.Format(OutputText.ServerNotFound, oppoProjectPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject), Times.Once);
            _fileSystemMock.Verify(x => x.FileExists(oppoProjectPath), Times.Once);
        }

        [Test]
        public void FailBecauseOfMissingClientFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var serverName = inputParams.ElementAtOrDefault(1);
            var clientName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Arrange client file
            var oppoProjectPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoProjectPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoProjectPath)).Returns(false);
            var oppoServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoServerPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoServerPath)).Returns(true);

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.OppoClientFileNotFound), Times.Once);
            Assert.AreEqual(string.Format(OutputText.ClientNotFound, oppoProjectPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject), Times.Once);
            _fileSystemMock.Verify(x => x.FileExists(oppoProjectPath), Times.Once);
        }
        [Test]
        public void FailOnServerDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrage
            var serverName = inputParams.ElementAtOrDefault(1);
            var clientName = inputParams.ElementAtOrDefault(3);

            var loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var oppoClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoClientPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoClientPath)).Returns(true);
            var oppoServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
            _fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoServerPath);
            _fileSystemMock.Setup(x => x.FileExists(oppoServerPath)).Returns(true);

            Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty));
            _fileSystemMock.Setup(x => x.ReadFile(oppoServerPath)).Returns(serverMemoryStream);
         

            // Act
            var commandResult = _objectUnderTest.Execute(inputParams);


            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            OppoLogger.RemoveListener(loggerListenerMock.Object);
            loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.CouldntDeserliazeServer), Times.Once);
            Assert.AreEqual(string.Format(OutputText.CouldntDeserliazeServer, oppoServerPath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _fileSystemMock.Verify(x => x.ReadFile(oppoServerPath), Times.Once);

            serverMemoryStream.Close();
            serverMemoryStream.Dispose();
        }
    }

}
