/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.ReferenceCommands;
using Appio.Resources.text.output;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Appio.ObjectModel.Tests.CommandStrategies
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

		private readonly string _sampleOpcuaClientAppContent = "{\"name\":\"testClient\",\"type\":\"Client\",\"references\":[]}";
		private readonly string _sampleOpcuaServerAppContent = "{\"name\":\"testServer\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"8413\"}";
		private readonly string _sampleOpcuaClientServerAppContent = "{\"name\":\"testClientServer\",\"type\":\"ClientServer\",\"url\":\"127.0.0.1\",\"port\":\"8413\",\"references\":[]}";
		private readonly string _sampleOpcuaClientAppContentWithServerReference = "{\"name\":\"testClient\",\"type\":\"Client\",\"references\":[{\"name\":\"testServer\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"8413\"}]}";


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
			Assert.AreEqual(Constants.ReferenceCommandArguments.Add, name);
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
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
			AppioLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
			Assert.AreEqual(string.Empty, firstMessageLine.Value);
		}


		[Test]
		public void FailBecauseOfUnkownServerParam([ValueSource(nameof(InvalidInputs_UnknownServerParam))] string[] inputParams)
		{
			// Arrange
			var clientName = inputParams.ElementAtOrDefault(1);
			var serverNameFlag = inputParams.ElementAtOrDefault(2);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);
			
			// Arrange client file
			var appioprojectPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioprojectPath);
			_fileSystemMock.Setup(x => x.FileExists(appioprojectPath)).Returns(true);

			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
			AppioLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
			Assert.AreEqual(string.Empty, firstMessageLine.Value);

		}

		[Test]
		public void FailBecauseOfMissingClientFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange client file
			var appioprojectPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioprojectPath);
			_fileSystemMock.Setup(x => x.FileExists(appioprojectPath)).Returns(false);

			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);


			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
			AppioLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceClientAppioprojFileNotFound), Times.Once);
			Assert.AreEqual(string.Format(OutputText.ReferenceClientAppioprojFileNotFound, appioprojectPath), firstMessageLine.Key);
			Assert.AreEqual(string.Empty, firstMessageLine.Value);
			_fileSystemMock.Verify(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject), Times.Once);
			_fileSystemMock.Verify(x => x.FileExists(appioprojectPath), Times.Once);
		}

		[Test]
		public void FailBecauseOfMissingServerFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var clientName = inputParams.ElementAtOrDefault(1);
			var serverName = inputParams.ElementAtOrDefault(3);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			// Arrange server file
			var appioClientProjectPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientProjectPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientProjectPath)).Returns(true);
			var appioServerProjectPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerProjectPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerProjectPath)).Returns(false);


			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);


			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
			AppioLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceAddServerAppioprojFileNotFound), Times.Once);
			Assert.AreEqual(string.Format(OutputText.ReferenceAddServerAppioprojFileNotFound, appioServerProjectPath), firstMessageLine.Key);
			Assert.AreEqual(string.Empty, firstMessageLine.Value);
			_fileSystemMock.Verify(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject), Times.Once);
			_fileSystemMock.Verify(x => x.FileExists(appioServerProjectPath), Times.Once);
		}

		[Test]
		public void FailOnServerDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var clientName = inputParams.ElementAtOrDefault(1);
			var serverName = inputParams.ElementAtOrDefault(3);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			var appioClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientPath)).Returns(true);
			var appioServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerPath)).Returns(true);

			using (var clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			using (var serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioClientPath)).Returns(clientMemoryStream);
				_fileSystemMock.Setup(x => x.ReadFile(appioServerPath)).Returns(serverMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);
				
				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceAddCouldntDeserliazeServer), Times.Once);
				Assert.AreEqual(string.Format(OutputText.ReferenceAddCouldntDeserliazeServer, appioServerPath), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
				_fileSystemMock.Verify(x => x.ReadFile(appioServerPath), Times.Once);
			}
		}
		[Test]
		public void FailOnClientDeserialization([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			var appioClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientPath)).Returns(true);
			var appioServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerPath)).Returns(true);

			Stream emptyMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty));
			_fileSystemMock.Setup(x => x.ReadFile(appioClientPath)).Returns(emptyMemoryStream);
			Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent));
			_fileSystemMock.Setup(x => x.ReadFile(appioServerPath)).Returns(serverMemoryStream);


			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);


			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
			AppioLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceCouldntDeserliazeClient), Times.Once);
			Assert.AreEqual(string.Format(OutputText.ReferenceCouldntDeserliazeClient, appioClientPath), firstMessageLine.Key);
			Assert.AreEqual(string.Empty, firstMessageLine.Value);
			_fileSystemMock.Verify(x => x.ReadFile(appioClientPath), Times.Once);

			emptyMemoryStream.Close();
			emptyMemoryStream.Dispose();
			serverMemoryStream.Close();
			serverMemoryStream.Dispose();
		}

		[Test]
		public void FailOnAddingServerWhichIsAlreadyClientsReference([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			var appioClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientPath)).Returns(true);
			var appioServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerPath)).Returns(true);

			using (Stream clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContentWithServerReference)))
			using (Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioClientPath)).Returns(clientMemoryStream);
				_fileSystemMock.Setup(x => x.ReadFile(appioServerPath)).Returns(serverMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert 
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceAddServerIsPartOfClientReference), Times.Once);
				Assert.AreEqual(string.Format(OutputText.ReferenceAddServerIsPartOfClientReference, serverName, clientName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}

		[Test]
		public void FailOnAddingClientReference([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			var appioClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientPath)).Returns(true);
			var appioServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerPath)).Returns(true);
			
			using (Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioServerPath)).Returns(serverMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert 
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceAddClientCannotBeReferred), Times.Once);
				Assert.AreEqual(string.Format(OutputText.ReferenceAddClientCannotBeReferred, serverName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}

		[Test]
		public void FailOnAddingReferenceToServer([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			var appioClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientPath)).Returns(true);
			var appioServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerPath)).Returns(true);

			using (Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			using (Stream clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioServerPath)).Returns(serverMemoryStream);
				_fileSystemMock.Setup(x => x.ReadFile(appioClientPath)).Returns(clientMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert 
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Warn(Resources.text.logging.LoggingText.ReferenceAddClientIsAServer), Times.Once);
				Assert.AreEqual(string.Format(OutputText.ReferenceAddClientIsAServer, clientName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
			}
		}

		[Test]
		public void SucceessOnAddingServerReferenceToClientServer([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			var appioClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientPath)).Returns(true);
			var appioServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerPath)).Returns(true);

			using (Stream clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientServerAppContent)))
			using (Stream clientServerMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientServerAppContent)))
			using (Stream serverMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.SetupSequence(x => x.ReadFile(appioClientPath)).Returns(clientMemoryStream).Returns(clientServerMemoryStream);
				_fileSystemMock.Setup(x => x.ReadFile(appioServerPath)).Returns(serverMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert 
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.ReferenceAddSuccess), Times.Once);
				Assert.AreEqual(string.Format(OutputText.RefereneceAddSuccess, serverName, clientName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
				_fileSystemMock.Verify(x => x.ReadFile(appioClientPath), Times.Exactly(2));
				_fileSystemMock.Verify(x => x.ReadFile(appioServerPath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(appioClientPath, It.IsAny<IEnumerable<string>>()), Times.Once);
			}
		}

		[Test]
		public void SuccessOnAddingClientServerReferenceToClient([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrage
			var serverName = inputParams.ElementAtOrDefault(3);
			var clientName = inputParams.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			var appioClientPath = Path.Combine(clientName, clientName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(clientName, clientName + Constants.FileExtension.Appioproject)).Returns(appioClientPath);
			_fileSystemMock.Setup(x => x.FileExists(appioClientPath)).Returns(true);
			var appioServerPath = Path.Combine(serverName, serverName + Constants.FileExtension.Appioproject);
			_fileSystemMock.Setup(x => x.CombinePaths(serverName, serverName + Constants.FileExtension.Appioproject)).Returns(appioServerPath);
			_fileSystemMock.Setup(x => x.FileExists(appioServerPath)).Returns(true);

			using (Stream clientMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientAppContent)))
			using (Stream clientServerMemoryStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaClientServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioClientPath)).Returns(clientMemoryStream);
				_fileSystemMock.Setup(x => x.ReadFile(appioServerPath)).Returns(clientServerMemoryStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert 
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
				AppioLogger.RemoveListener(loggerListenerMock.Object);
				loggerListenerMock.Verify(x => x.Info(Resources.text.logging.LoggingText.ReferenceAddSuccess), Times.Once);
				Assert.AreEqual(string.Format(OutputText.RefereneceAddSuccess, serverName, clientName), firstMessageLine.Key);
				Assert.AreEqual(string.Empty, firstMessageLine.Value);
				_fileSystemMock.Verify(x => x.ReadFile(appioClientPath), Times.Once);
				_fileSystemMock.Verify(x => x.ReadFile(appioServerPath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(appioClientPath, It.IsAny<IEnumerable<string>>()), Times.Once);
			}
		}
	}

}