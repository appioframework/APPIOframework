using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
	public class NewOpcuaAppCommandStrategyTests
	{
		private static string[][] ValidInputs_ClientServerAppType()
		{
			return new[]
			{
				new[] { "-n", "anyName",	 "-t", "ClientServer",	   "-u", "127.0.0.1",    "-p", "4840" },
				new[] { "-n", "anyName",	 "-t", "ClientServer",	   "-u", "127.0.0.1",	 "--port", "4840" },
				new[] { "-n", "anyName",	 "-t", "ClientServer",	   "--url", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName",	 "-t", "ClientServer",	   "--url", "127.0.0.1", "--port", "4840"  },
				new[] { "-n", "anyName",	 "--type", "ClientServer", "-u", "127.0.0.1",	 "-p", "4840" },
				new[] { "-n", "anyName",	 "--type", "ClientServer", "-u", "127.0.0.1",    "--port", "4840" },
				new[] { "-n", "anyName",	 "--type", "ClientServer", "--url", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName",	 "--type", "ClientServer", "--url", "127.0.0.1", "--port", "4840"  },
				new[] { "--name", "anyName", "-t", "ClientServer",	   "-u", "127.0.0.1",    "-p", "4840" },
				new[] { "--name", "anyName", "-t", "ClientServer",	   "-u", "127.0.0.1",    "--port", "4840" },
				new[] { "--name", "anyName", "-t", "ClientServer",	   "--url", "127.0.0.1", "-p", "4840" },
				new[] { "--name", "anyName", "-t", "ClientServer",	   "--url", "127.0.0.1", "--port", "4840"  },
				new[] { "--name", "anyName", "--type", "ClientServer", "-u", "127.0.0.1",	 "-p", "4840" },
				new[] { "--name", "anyName", "--type", "ClientServer", "-u", "127.0.0.1",	 "--port", "4840" },
				new[] { "--name", "anyName", "--type", "ClientServer", "--url", "127.0.0.1", "-p", "4840" },
				new[] { "--name", "anyName", "--type", "ClientServer", "--url", "127.0.0.1", "--port", "4840"  },
			};
		}

		private static string[][] ValidInputs_ClientAppType()
		{
			return new[]
			{
				new[] { "-n", "anyName", "-t", "Client" },
				new[] { "-n", "anyName", "--type", "Client" },
				new[] { "--name", "anyName", "-t", "Client" },
				new[] { "--name", "anyName", "--type", "Client" },
			};
		}

		private static string[][] ValidInputs_ServerAppType()
		{
			return new[]
			{
				new[] { "-n", "anyName",	 "-t", "Server",	 "-u", "127.0.0.1",	   "-p", "4840" },
				new[] { "-n", "anyName",	 "-t", "Server",	 "-u", "127.0.0.1",	   "--port", "4840" },
				new[] { "-n", "anyName",	 "-t", "Server",	 "--url", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName",	 "-t", "Server",	 "--url", "127.0.0.1", "--port", "4840"  },
				new[] { "-n", "anyName",	 "--type", "Server", "-u", "127.0.0.1",	   "-p", "4840" },
				new[] { "-n", "anyName",	 "--type", "Server", "-u", "127.0.0.1",	   "--port", "4840" },
				new[] { "-n", "anyName",	 "--type", "Server", "--url", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName",	 "--type", "Server", "--url", "127.0.0.1", "--port", "4840"  },
				new[] { "--name", "anyName", "-t", "Server",	 "-u", "127.0.0.1",	   "-p", "4840" },
				new[] { "--name", "anyName", "-t", "Server",	 "-u", "127.0.0.1",	   "--port", "4840" },
				new[] { "--name", "anyName", "-t", "Server",	 "--url", "127.0.0.1", "-p", "4840" },
				new[] { "--name", "anyName", "-t", "Server",	 "--url", "127.0.0.1", "--port", "4840"  },
				new[] { "--name", "anyName", "--type", "Server", "-u", "127.0.0.1",	   "-p", "4840" },
				new[] { "--name", "anyName", "--type", "Server", "-u", "127.0.0.1",	   "--port", "4840" },
				new[] { "--name", "anyName", "--type", "Server", "--url", "127.0.0.1", "-p", "4840" },
				new[] { "--name", "anyName", "--type", "Server", "--url", "127.0.0.1", "--port", "4840"  },
			};
		}

		private static string[][] InvalidInputsFistParam()
		{
			return new[]
			{
				new[] {"--n", "any-name"},
				new[] {"-N", "any-name"},
				new[] {"-name", "any-name"},
				new[] {"--Name", "any-name"},
				new[] {"-N", "ab/yx"},
				new[] {"", ""},
				new[] {""},
				new string[] { }
			};
		}

		private static string[][] InvalidInputsSecondParam()
		{
			return new[]
			{
				new[] {"-n", "ab/yx"},
				new[] {"-n", "ab\\yx"}
			};
		}

		private static string[][] InvalidInputs_UnknownTypeParam()
		{
			return new[]
			{
				new[] { "-n", "anyName", "-T", "ClientServer" },
				new[] { "-n", "anyName", "--t", "ClientServer" },
				new[] { "-n", "anyName", "--Type", "ClientServer" },
				new[] { "-n", "anyName", "-type", "ClientServer" },
			};
		}

		private static string[][] InvalidInputs_UnknownAppType()
		{
			return new[]
			{
				new[] { "-n", "anyName", "-t", "clientserver" },
				new[] { "-n", "anyName", "-t", "Clientserver" },
				new[] { "-n", "anyName", "-t", "clientServer" },
				new[] { "-n", "anyName", "-t", "ServerClient" },
				new[] { "-n", "anyName", "-t", "client" },
				new[] { "-n", "anyName", "-t", "Cln" },
				new[] { "-n", "anyName", "-t", "Serwer" },
				new[] { "-n", "anyName", "-t", "server" },
				new[] { "-n", "anyName", "-t", "Ser" },
				new[] { "-n", "anyName", "-t", "" },
				new[] { "-n", "anyName", "-t", null },
			};
		}

		private static string[][] InvalidInputs_UnknownUrlParam()
		{
			return new[]
			{
				new[] { "-n", "anyName", "-t", "Server", "--u", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-U", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-url", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "--Url", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "--u", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-U", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-url", "127.0.0.1", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "--Url", "127.0.0.1", "-p", "4840" },
			};
		}

		private static string[][] InvalidInputs_UnknownUrlValue()
		{
			return new[]
			{
				new[] { "-n", "anyName", "-t", "Server", "-u", "u r l", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "url\turl", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "url\nurl", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "url\rurl", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "u r l", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "url\turl", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "url\nurl", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "url\rurl", "-p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "", "-p", "4840" },
			};
		}

		private static string[][] InvalidInputs_UnknownPortParam()
		{
			return new[]
			{
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "--p", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "-P", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "-port", "4840" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "--Port", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "--p", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "-P", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "-port", "4840" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "--Port", "4840" },
			};
		}

		private static string[][] InvalidInputs_UnknownPortValue()
		{
			return new[]
			{
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "-p", "PortCanContainOnlyDigits" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "-p", "-1" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "-p", "65536" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "-p", "1 2 3" },
				new[] { "-n", "anyName", "-t", "Server", "-u", "127.0.0.1", "-p", "" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "-p", "PortCanContainOnlyDigits" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "-p", "-1" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "-p", "65536" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "-p", "1 2 3" },
				new[] { "-n", "anyName", "-t", "ClientServer", "-u", "127.0.0.1", "-p", "" },
			};
		}

		private readonly string _clientOppoprojFileContent = "{\r\n  \"name\": \"anyName\",\r\n  \"type\": \"Client\"\r\n}";
		private readonly string _serverOppoprojFileContent = "{\r\n  \"name\": \"anyName\",\r\n  \"type\": \"Server\",\r\n  \"url\": \"127.0.0.1\",\r\n  \"port\": \"4840\"\r\n}";
		private readonly string _clientServerOppoprojFileContent = "{\r\n  \"name\": \"anyName\",\r\n  \"type\": \"ClientServer\",\r\n  \"url\": \"127.0.0.1\",\r\n  \"port\": \"4840\"\r\n}";

		private Mock<IFileSystem> _fileSystemMock;
		private NewOpcuaAppCommandStrategy _objectUnderTest;

		[SetUp]
		public void SetupObjectUnderTest()
		{
			_fileSystemMock = new Mock<IFileSystem>();
			_objectUnderTest = new NewOpcuaAppCommandStrategy(_fileSystemMock.Object);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_ImplementICommandOfNewStrategy()
		{
			// Arrange

			// Act

			// Assert
			Assert.IsInstanceOf<ICommand<NewStrategy>>(_objectUnderTest);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_HaveCorrectCommandName()
		{
			// Arrange

			// Act
			var commandName = _objectUnderTest.Name;

			// Assert
			Assert.AreEqual(Constants.NewCommandName.OpcuaApp, commandName);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_ProvideEmptyHelpText()
		{
			// Arrange

			// Act
			var helpText = _objectUnderTest.GetHelpText();

			// Assert
			Assert.AreEqual(string.Empty, helpText);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_IgnoreInput([ValueSource(nameof(InvalidInputsSecondParam))] string[] inputParams)
		{
			// Arrange
			var loggerListenerMock = new Mock<ILoggerListener>();
			var warnWrittenOut = false;
			loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var invalidNameCharsMock = new[] { '/' };
			var invalidPathCharsMock = new[] { '\\' };
			_fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidNameCharsMock);
			_fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(invalidPathCharsMock);

			// Act
			var result = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsTrue(warnWrittenOut);
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureInvalidProjectName, inputParams.ElementAt(1)), result.OutputMessages.First().Key);
			_fileSystemMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
			_fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(It.IsAny<string>()), Times.Never);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_IgnoreInput_UnknownParams([ValueSource(nameof(InvalidInputsFistParam))] string[] inputParams)
		{
			// Arrange
			var nameFlag = inputParams.ElementAtOrDefault(0);

			var loggerListenerMock = new Mock<ILoggerListener>();
			var warnWrittenOut = false;
			loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var invalidNameCharsMock = new[] { '/' };
			var invalidPathCharsMock = new[] { '\\' };
			_fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidNameCharsMock);
			_fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(invalidPathCharsMock);

			// Act
			var result = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsTrue(warnWrittenOut);
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, nameFlag), result.OutputMessages.First().Key);
			_fileSystemMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
			_fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(It.IsAny<string>()), Times.Never);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_FailOnInvalidTypeParameter([ValueSource(nameof(InvalidInputs_UnknownTypeParam))] string[] inputParam)
		{
			// Arrange
			var typeFlag = inputParam.ElementAtOrDefault(2);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, typeFlag), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(LoggingText.UnknownNewOpcuaappCommandParam), Times.Once);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_FailOnInvalidAppType([ValueSource(nameof(InvalidInputs_UnknownAppType))] string[] inputParam)
		{
			// Arrange
			var applicationType = inputParam.ElementAtOrDefault(3);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureUnknownProjectType, applicationType), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(LoggingText.InvalidOpcuaappType), Times.Once);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_FailOnInvalidUrlParameter([ValueSource(nameof(InvalidInputs_UnknownUrlParam))] string[] inputParam)
		{
			// Arrange
			var urlFlag = inputParam.ElementAtOrDefault(4);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, urlFlag), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(LoggingText.UnknownNewOpcuaappCommandParam), Times.Once);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_FailOnInvalidUrlValue([ValueSource(nameof(InvalidInputs_UnknownUrlValue))] string[] inputParam)
		{
			// Arrange
			var url = inputParam.ElementAtOrDefault(5);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureInvalidServerUrl, url), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(LoggingText.InvalidServerUrl), Times.Once);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_FailOnInvalidPortParameter([ValueSource(nameof(InvalidInputs_UnknownPortParam))] string[] inputParam)
		{
			// Arrange
			var portFlag = inputParam.ElementAtOrDefault(6);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, portFlag), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(LoggingText.UnknownNewOpcuaappCommandParam), Times.Once);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_FailOnInvalidPortValue([ValueSource(nameof(InvalidInputs_UnknownPortValue))] string[] inputParam)
		{
			// Arrange
			var port = inputParam.ElementAtOrDefault(7);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsFalse(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandFailureInvalidServerPort, port), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Warn(LoggingText.InvalidServerPort), Times.Once);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_CreateFilesForClientServerApp([ValueSource(nameof(ValidInputs_ClientServerAppType))] string[] inputParam)
		{
			// Arrange
			var projectName = inputParam.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var projectDirectory = $"{projectName}";
			const string sourceCodeDirectory = "source-directory";
			const string clientSourceDirectory = "client-source-directory";
			const string serverSourceDirectory = "server-source-directory";

			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.SourceCode)).Returns(sourceCodeDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ClientApp)).Returns(clientSourceDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ServerApp)).Returns(serverSourceDirectory);

			var projectFileName = $"{projectName}{Constants.FileExtension.OppoProject}";
			const string projectFilePath = "project-file-path";
			const string mesonBuildFilePath = "meson-build-file-path";
			const string clientMainC = "client-main-c-file";
			const string serverMainC = "server-main-c-file";
			const string serverMesonBuild = "server-meson-build-file";
			const string serverloadInformationModelsC = "server-loadInformationModels-c-file";
			const string modelsDirectory = "models";

			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectFileName)).Returns(projectFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.FileName.SourceCode_meson_build)).Returns(mesonBuildFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(clientSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(clientMainC);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(serverMainC);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_meson_build)).Returns(serverMesonBuild);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_loadInformationModels_c)).Returns(serverloadInformationModelsC);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsTrue(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandSuccess, projectName), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Info(string.Format(LoggingText.NewOpcuaappCommandSuccess, projectName)), Times.Once);

			_fileSystemMock.Verify(x => x.CreateDirectory(projectDirectory), Times.AtLeastOnce);
			_fileSystemMock.Verify(x => x.CreateDirectory(sourceCodeDirectory), Times.AtLeastOnce);
			_fileSystemMock.Verify(x => x.CreateDirectory(clientSourceDirectory), Times.AtLeastOnce);
			_fileSystemMock.Verify(x => x.CreateDirectory(serverSourceDirectory), Times.AtLeastOnce);
			_fileSystemMock.Verify(x => x.CreateDirectory(modelsDirectory), Times.Once);

			_fileSystemMock.Verify(x => x.CreateFile(projectFilePath, _clientServerOppoprojFileContent), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(mesonBuildFilePath, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(clientMainC, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(serverMainC, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(serverMesonBuild, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(serverloadInformationModelsC, It.IsAny<string>()), Times.Once);

			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientServerType_build), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientType_build), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ServerType_build), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_client_c), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_server_c), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_server_build), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_loadInformationModels_server_c), Times.Once);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_CreateFilesForClientApp([ValueSource(nameof(ValidInputs_ClientAppType))] string[] inputParam)
		{
			// Arrange
			var projectName = inputParam.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var projectDirectory = $"{projectName}";
			const string sourceCodeDirectory = "source-directory";
			const string clientSourceDirectory = "client-source-directory";
			const string serverSourceDirectory = "server-source-directory";

			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.SourceCode)).Returns(sourceCodeDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ClientApp)).Returns(clientSourceDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ServerApp)).Returns(serverSourceDirectory);

			var projectFileName = $"{projectName}{Constants.FileExtension.OppoProject}";
			const string projectFilePath = "project-file-path";
			const string mesonBuildFilePath = "meson-build-file-path";
			const string clientMainC = "client-main-c-file";
			const string serverMainC = "server-main-c-file";
			const string serverMesonBuild = "server-meson-build-file";
			const string serverloadInformationModelsC = "server-loadInformationModels-c-file";
			const string modelsDirectory = "models";

			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectFileName)).Returns(projectFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.FileName.SourceCode_meson_build)).Returns(mesonBuildFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(clientSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(clientMainC);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(serverMainC);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_meson_build)).Returns(serverMesonBuild);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_loadInformationModels_c)).Returns(serverloadInformationModelsC);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsTrue(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandSuccess, projectName), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Info(string.Format(LoggingText.NewOpcuaappCommandSuccess, projectName)), Times.Once);

			_fileSystemMock.Verify(x => x.CreateDirectory(projectDirectory), Times.Once);
			_fileSystemMock.Verify(x => x.CreateDirectory(sourceCodeDirectory), Times.Once);
			_fileSystemMock.Verify(x => x.CreateDirectory(clientSourceDirectory), Times.Once);
			_fileSystemMock.Verify(x => x.CreateDirectory(serverSourceDirectory), Times.Never);
			_fileSystemMock.Verify(x => x.CreateDirectory(modelsDirectory), Times.Never);

			_fileSystemMock.Verify(x => x.CreateFile(projectFilePath, _clientOppoprojFileContent), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(mesonBuildFilePath, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(clientMainC, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(serverMainC, It.IsAny<string>()), Times.Never);
			_fileSystemMock.Verify(x => x.CreateFile(serverMesonBuild, It.IsAny<string>()), Times.Never);
			_fileSystemMock.Verify(x => x.CreateFile(serverloadInformationModelsC, It.IsAny<string>()), Times.Never);

			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientServerType_build), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientType_build), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ServerType_build), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_client_c), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_server_c), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_server_build), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_loadInformationModels_server_c), Times.Never);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}

		[Test]
		public void NewOpcuaAppCommandStrategy_Should_CreateFilesForServerApp([ValueSource(nameof(ValidInputs_ServerAppType))] string[] inputParam)
		{
			// Arrange
			var projectName = inputParam.ElementAtOrDefault(1);

			var loggerListenerMock = new Mock<ILoggerListener>();
			OppoLogger.RegisterListener(loggerListenerMock.Object);

			var projectDirectory = $"{projectName}";
			const string sourceCodeDirectory = "source-directory";
			const string clientSourceDirectory = "client-source-directory";
			const string serverSourceDirectory = "server-source-directory";

			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.SourceCode)).Returns(sourceCodeDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ClientApp)).Returns(clientSourceDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(sourceCodeDirectory, Constants.DirectoryName.ServerApp)).Returns(serverSourceDirectory);

			var projectFileName = $"{projectName}{Constants.FileExtension.OppoProject}";
			const string projectFilePath = "project-file-path";
			const string mesonBuildFilePath = "meson-build-file-path";
			const string clientMainC = "client-main-c-file";
			const string serverMainC = "server-main-c-file";
			const string serverMesonBuild = "server-meson-build-file";
			const string serverloadInformationModelsC = "server-loadInformationModels-c-file";
			const string modelsDirectory = "models";

			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectFileName)).Returns(projectFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.FileName.SourceCode_meson_build)).Returns(mesonBuildFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(clientSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(clientMainC);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_main_c)).Returns(serverMainC);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_meson_build)).Returns(serverMesonBuild);
			_fileSystemMock.Setup(x => x.CombinePaths(serverSourceDirectory, Constants.FileName.SourceCode_loadInformationModels_c)).Returns(serverloadInformationModelsC);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);

			// Act
			var result = _objectUnderTest.Execute(inputParam);

			// Assert
			Assert.IsTrue(result.Sucsess);
			Assert.AreEqual(string.Format(OutputText.NewOpcuaappCommandSuccess, projectName), result.OutputMessages.First().Key);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
			loggerListenerMock.Verify(x => x.Info(string.Format(LoggingText.NewOpcuaappCommandSuccess, projectName)), Times.Once);

			_fileSystemMock.Verify(x => x.CreateDirectory(projectDirectory), Times.Once);
			_fileSystemMock.Verify(x => x.CreateDirectory(sourceCodeDirectory), Times.Once);
			_fileSystemMock.Verify(x => x.CreateDirectory(clientSourceDirectory), Times.Never);
			_fileSystemMock.Verify(x => x.CreateDirectory(serverSourceDirectory), Times.Once);
			_fileSystemMock.Verify(x => x.CreateDirectory(modelsDirectory), Times.Once);

			_fileSystemMock.Verify(x => x.CreateFile(projectFilePath, _serverOppoprojFileContent), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(mesonBuildFilePath, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(clientMainC, It.IsAny<string>()), Times.Never);
			_fileSystemMock.Verify(x => x.CreateFile(serverMainC, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(serverMesonBuild, It.IsAny<string>()), Times.Once);
			_fileSystemMock.Verify(x => x.CreateFile(serverloadInformationModelsC, It.IsAny<string>()), Times.Once);

			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientServerType_build), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientType_build), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ServerType_build), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_client_c), Times.Never);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_server_c), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_server_build), Times.Once);
			_fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_loadInformationModels_server_c), Times.Once);
			OppoLogger.RemoveListener(loggerListenerMock.Object);
		}
	}
}