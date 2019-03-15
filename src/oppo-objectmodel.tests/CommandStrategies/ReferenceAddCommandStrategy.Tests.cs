using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.ReferenceCommands;
using Oppo.Resources.text.output;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
	class ReferenceAddCommandStrategySholud
	{
		protected static string[][] ValidInputs()
		{
			return new[]
			{
				new [] { "-c", "testClient", "-s", "testServer" },
				new [] { "-c", "testClient", "--server", "testServer" },
				new [] { "--client", "testClient", "-s", "testServer" },
				new [] { "--client", "testClient", "--server", "testServer" },
			};
		}
		protected static string[][] InvalidInputs_UnknownClientParam()
		{
			return new[]
			{
				new [] { "--c", "testClient", "-s", "testServer" },
				new [] { "--Client", "testClient", "-s", "testServer" },
				new [] { "-C", "testClient", "-s", "testServer" },
				new [] { "-client", "testClient", "-s", "testServer" },
			};
		}

		protected static string[][] InvalidInputs_UnknownServerParam()
		{
			return new[]
			{
				new [] { "-c", "testClient", "--s", "testserver" },
				new [] { "-c", "testClient", "-server", "testServer" },
				new [] { "-c", "testClient", "-S", "testServer" },
				new [] { "-c", "testClient", "--Server", "testServer" },
			};
		}

		private Mock<IFileSystem> _fileSystemMock;
		private ReferenceAddCommandStrategy _objectUnderTest;

		private readonly string _defaultOpposlnContent = "{\"projects\": []}";
		private readonly string _sampleOpposlnContent = "{\"projects\": [{\"name\":\"testApp\",\"path\":\"testApp.oppoproj\"}]}";

		private readonly string _sampleOpcuaClientAppContent = "{\"name\":\"testApp\",\"type\":\"Client\"}";
		private readonly string _sampleOpcuaServerAppContent = "{\"name\":\"testApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"8413\"}";
		private readonly string _succesfullReferenceClientContent = "{\"name\":\"testApp\",\"type\":\"Client\", \"references\":[{\"name\":\"testApp\",\"type\":\"ClientServer\",\"url\":\"127.0.0.1\",\"port\":\"8413\"}]}";
		private readonly string _sampleOpcuaClientServerAppContent = "{\"name\":\"testApp\",\"type\":\"ClientServer\",\"url\":\"127.0.0.1\",\"port\":\"8413\",\"references\":[]}";


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
		public void FailBecauseOfUnkownClientParam([ValueSource(nameof(InvalidInputs_UnknownClientParam))] string[] inputParams)
		{
			// Arrange
			var clientNameFlag = inputParams.ElementAtOrDefault(0);

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
		public void FailBecauseOfUnkownServerParam([ValueSource(nameof(InvalidInputs_UnknownServerParam))] string[] inputParams)
		{
			// Arrange
			var serverNameFlag = inputParams.ElementAtOrDefault(2);

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
		public void FailBecauseOfMissingClientFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange client file
			var oppoProjectPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoProjectPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoProjectPath)).Returns(false);

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
		public void FailBecauseOfMissingServerFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var clientName = inputParams.ElementAtOrDefault(1);
			var serverName = inputParams.ElementAtOrDefault(3);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange server file
			var oppoClientProjectPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoClientProjectPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoClientProjectPath)).Returns(true);
			var oppoServerProjectPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoServerProjectPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoServerProjectPath)).Returns(false);


			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);


			// Assert
			Assert.IsFalse(commandResult.Sucsess);
			Assert.IsNotNull(commandResult.OutputMessages);
			var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.OppoServerFileNotFound), Times.Once);
			Assert.AreEqual(string.Format(OutputText.ServerNotFound, oppoServerProjectPath), firstMessageLine.Key);
			Assert.AreEqual(string.Empty, firstMessageLine.Value);
			_fileSystemMock.Verify(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject), Times.Once);
			_fileSystemMock.Verify(x => x.FileExists(oppoServerProjectPath), Times.Once);
		}

		[Test]
		public void FailOnServerDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var clientName = inputParams.ElementAtOrDefault(1);
			var serverName = inputParams.ElementAtOrDefault(3);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var oppoClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoClientPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoClientPath)).Returns(true);
			var oppoServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoServerPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoServerPath)).Returns(true);

			using (var clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			using (var serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoClientPath)).Returns(clientMemoryStream);
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
			}
		}
		[Test]
		public void FailOnClientDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var oppoClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoClientPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoClientPath)).Returns(true);
			var oppoServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoServerPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoServerPath)).Returns(true);

			Stream emptyMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty));
			_fileSystemMock.Setup(x => x.ReadFile(oppoClientPath)).Returns(emptyMemoryStream);
			Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent));
			_fileSystemMock.Setup(x => x.ReadFile(oppoServerPath)).Returns(serverMemoryStream);


			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);


			// Assert
			Assert.IsFalse(commandResult.Sucsess);
			Assert.IsNotNull(commandResult.OutputMessages);
			var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.CouldntDeserliazeClient), Times.Once);
			Assert.AreEqual(string.Format(OutputText.CouldntDeserliazeClient, oppoClientPath), firstMessageLine.Key);
			Assert.AreEqual(string.Empty, firstMessageLine.Value);
			_fileSystemMock.Verify(x => x.ReadFile(oppoClientPath), Times.Once);

			emptyMemoryStream.Close();
			emptyMemoryStream.Dispose();
			serverMemoryStream.Close();
			serverMemoryStream.Dispose();
		}


		[Test]
		public void SucceessOnAddingServerReferenceToClientServer([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var oppoClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoClientPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoClientPath)).Returns(true);
			var oppoServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoServerPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoServerPath)).Returns(true);

			using (Stream clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientServerAppContent)))
			using (Stream clientServerMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientServerAppContent)))
			using (Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.SetupSequence(x => x.ReadFile(oppoClientPath)).Returns(clientMemoryStream).Returns(clientServerMemoryStream);

				_fileSystemMock.Setup(x => x.ReadFile(oppoServerPath)).Returns(serverMemoryStream);
				OpcuaClientApp opcuaClient = JsonConvert.DeserializeObject<OpcuaClientApp>(_succesfullReferenceClientContent);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert 
				Assert.IsTrue(commandResult.Sucsess);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.ReferenceAddSuccess), Times.Once);
				Assert.AreEqual(string.Format(OutputText.RefereneceAddSuccess, serverName, clientName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
				_fileSystemMock.Verify(x => x.ReadFile(oppoClientPath), Times.Exactly(2));
				_fileSystemMock.Verify(x => x.ReadFile(oppoServerPath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(oppoClientPath, It.IsAny<IEnumerable<string>>()), Times.Once);
			}
		}

		[Test]
		public void SuccessOnAddingClientServerReferenceToClient([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var oppoClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.OppoProject)).Returns(oppoClientPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoClientPath)).Returns(true);
			var oppoServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.OppoProject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.OppoProject)).Returns(oppoServerPath);
			_fileSystemMock.Setup(x => x.FileExists(oppoServerPath)).Returns(true);

			using (Stream clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			using (Stream clientServerMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoClientPath)).Returns(clientMemoryStream);

				_fileSystemMock.Setup(x => x.ReadFile(oppoServerPath)).Returns(clientServerMemoryStream);
				OpcuaClientApp opcuaClient = JsonConvert.DeserializeObject<OpcuaClientApp>(_succesfullReferenceClientContent);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert 
				Assert.IsTrue(commandResult.Sucsess);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				OppoLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.ReferenceAddSuccess), Times.Once);
				Assert.AreEqual(string.Format(OutputText.RefereneceAddSuccess, serverName, clientName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
				_fileSystemMock.Verify(x => x.ReadFile(oppoClientPath), Times.Once);
				_fileSystemMock.Verify(x => x.ReadFile(oppoServerPath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(oppoClientPath, It.IsAny<IEnumerable<string>>()), Times.Once);
			}
		}
	}

}