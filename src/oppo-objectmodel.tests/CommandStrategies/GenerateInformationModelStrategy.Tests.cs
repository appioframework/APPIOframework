using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.GenerateCommands;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class GenerateInformationModelStrategyShould
    {
        // Valid inputs

        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new [] { "-n", "testApp" },
                new [] { "--name", "testApp" },
            };
        }

        // Invalid input opcua app name

        protected static string[][] InvalidInputs_EmptyOpcuaAppName()
        {
            return new[]
            {
                new [] { "-n", "" },
                new [] { "--name", "" }
            };
        }

        protected static string[][] InvalidInputs_UnknownNameParam()
        {
            return new[]
            {
                new [] { "-any string" },
                new [] { "-N", "testApp" },
                new [] { "-name", "testApp" },
                new [] { "--nam", "testApp" }
            };
        }

        private GenerateInformationModelStrategy _strategy;
		private Mock<IFileSystem> _fileSystemMock;
		private Mock<IModelValidator> _modelValidator;
        private Mock<ILoggerListener> _loggerListenerMock;
		private bool _loggerWroteOut;
		private string _commandName;
		private Mock<INodesetGenerator> _nodesetGenerator;
		private readonly string _srcDir = @"src\server";
        private readonly string _defaultServerMesonBuild        = "server_app_sources += [\n]";
        private readonly string _defaultLoadInformationModelsC  = "UA_StatusCode loadInformationModels(UA_Server* server)\n{\n\treturn UA_STATUSCODE_GOOD;\n}";
		private readonly string _uaMethodSample					= "<?xml version=\"1.0\" encoding=\"utf-8\"?><UANodeSet><NamespaceUris><Uri>test</Uri></NamespaceUris><UAMethod NodeId=\"ns=1;i=1000\" BrowseName=\"sampleBrowseName\"></UAMethod></UANodeSet>";
		private readonly string _defaultMainCallbacsC			= "UA_StatusCode addCallbacks(UA_Server* server)\n{\n\treturn UA_STATUSCODE_GOOD;\n}";
		
		private readonly string _sampleOpcuaServerAppContent = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[{\"name\":\"model.xml\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"\"}]}";
		private static string _opcuaServerAppContentWithDuplicatedModels = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[" +
																					"{\"name\":\"model.xml\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"\"}," +
																					"{\"name\":\"model.xml\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"\"}" +
																					"]}";
		private static string _opcuaServerAppContentWithModelWhoRequiredItself = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[" +
																					"{\"name\":\"model.xml\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"\", \"requiredModelUris\":[" +
																					"\"sample_namespace\"" +
																					"]}]}";
		private static string _opcuaServerAppContentWithMissingRequiredModel = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[" +
																					"{\"name\":\"model.xml\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"\", \"requiredModelUris\":[" +
																					"\"sample_namespace1\"" +
																					"]}]}";
		private static string[] InvalidOpcuaServerAppContent()
		{
			return new[]
			{
				_opcuaServerAppContentWithDuplicatedModels,
				_opcuaServerAppContentWithModelWhoRequiredItself,
				_opcuaServerAppContentWithMissingRequiredModel
			};
		}
		private readonly string _defaultOpcuaClientAppContent = "{\"name\":\"clientApp\",\"type\":\"Client\",\"references\": []}";

		[SetUp]
        public void SetUpTest()
        {
			_fileSystemMock = new Mock<IFileSystem>();
			_modelValidator = new Mock<IModelValidator>();
            _loggerListenerMock = new Mock<ILoggerListener>();
			_loggerWroteOut = false;
			_commandName = Constants.CommandName.Generate + " " + Constants.GenerateInformationModeCommandArguments.Name;
			_nodesetGenerator = new Mock<INodesetGenerator>();
			_strategy = new GenerateInformationModelStrategy(Constants.GenerateInformationModeCommandArguments.Name, _fileSystemMock.Object, _modelValidator.Object, _nodesetGenerator.Object);

			OppoLogger.RegisterListener(_loggerListenerMock.Object);
		}

        [TearDown]
        public void CleanUpTest()
        {
            OppoLogger.RemoveListener(_loggerListenerMock.Object);
        }

        [Test]
        public void ImplementICommandOfGenerateStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<GenerateStrategy>>(_strategy);
        }

        [Test]
        public void ReturnValidCommandName()
        {
            // Arrange

            // Act
            var commandName = _strategy.Name;

            // Assert
            Assert.AreEqual(Constants.GenerateInformationModeCommandArguments.Name, commandName);
        }

        [Test]
        public void ReturnValidHelpText()
        {
            // Arrange

            // Act
            var helpText = _strategy.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.GenerateInformationModelCommandDescription, helpText);
        }
		
        [Test]
        public void Fail_OnGenerateInformationModelBecauseUknownNameParam([ValueSource(nameof(InvalidInputs_UnknownNameParam))] string[] inputParams)
        {
			// Arrange       
			var projectNameFlag = inputParams.ElementAtOrDefault(0);
			
			_loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.UnknownParameterProvided, _commandName))).Callback(delegate { _loggerWroteOut = true; });

			// Act
			var commandResult = _strategy.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsTrue(_loggerWroteOut);
			Assert.IsNotNull(commandResult.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.UnknownParameterProvided, projectNameFlag, "'-n' or '--name'"), commandResult.OutputMessages.FirstOrDefault().Key);
		}
		
        [Test]
        public void Fail_OnGenerateInformationModelBecauseEmptyOpcuaAppName([ValueSource(nameof(InvalidInputs_EmptyOpcuaAppName))] string[] inputParams)
        {
			// Arrange
			var projectNameFlag = inputParams.ElementAtOrDefault(0);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.ParameterValueMissing, _commandName))).Callback(delegate { _loggerWroteOut = true; });

			// Act
			var commandResult = _strategy.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsTrue(_loggerWroteOut);
			Assert.IsNotNull(commandResult.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.ParameterValueMissing, projectNameFlag), commandResult.OutputMessages.FirstOrDefault().Key);
        }

		[Test]
		public void Fail_OnReadingOppoprojFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var oppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(oppoprojFileStream);

				// Arrange logger listener
				_loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelFailureCouldntDeserliazeOpcuaapp)).Callback(delegate { _loggerWroteOut = true; });

				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsTrue(_loggerWroteOut);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureCouldntDeserliazeOpcuaapp, projectName, oppoprojFilePath), commandResult.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.ReadFile(oppoprojFilePath), Times.Once);
			}
		}

		[Test]
		public void Fail_OnReadingOppoprojFileOfAClientApp([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var oppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaClientAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(oppoprojFileStream);

				// Arrange logger listener
				_loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelFailuteOpcuaappIsAClient)).Callback(delegate { _loggerWroteOut = true; });

				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsTrue(_loggerWroteOut);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailuteOpcuaappIsAClient, projectName), commandResult.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.ReadFile(oppoprojFilePath), Times.Once);
			}
		}

		[Test]
		public void Success_OnGenerateInformationModel([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var oppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(oppoprojFileStream);

				_nodesetGenerator.Setup(x => x.GenerateTypesSourceCodeFiles(It.IsAny<string>(), It.IsAny<IModelData>())).Returns(true);
				_nodesetGenerator.Setup(x => x.GenerateNodesetSourceCodeFiles(It.IsAny<string>(), It.IsAny<IModelData>(), It.IsAny<List<RequiredModelsData>>())).Returns(true);

				// Arrange logger listener
				_loggerListenerMock.Setup(x => x.Info(LoggingText.GenerateInformationModelSuccess)).Callback(delegate { _loggerWroteOut = true; });

				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsTrue(commandResult.Success);
				Assert.IsTrue(_loggerWroteOut);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelSuccess, projectName), commandResult.OutputMessages.First().Key);
			}
		}
		
		[Test, Combinatorial]
		public void Fail_OnInvalidModelList([ValueSource(nameof(ValidInputs))] string[] inputParams, [ValueSource(nameof(InvalidOpcuaServerAppContent))] string sampleOpcuaServerAppContent)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var oppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(oppoprojFileStream);
				
				// Arrange logger listener
				_loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelInvalidModelsList)).Callback(delegate { _loggerWroteOut = true; });

				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsTrue(_loggerWroteOut);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelInvalidModelsList, projectName), commandResult.OutputMessages.First().Key);
			}
		}

		[Test, Sequential]
		public void Fail_OnCallingNodesetGenerator([Values(false, true)] bool typesGeneratorResult, [Values(true, false)] bool nodesetGeneratorResult, [ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var oppoprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.OppoProject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject)).Returns(oppoprojFilePath);

			using (var oppoprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(oppoprojFilePath)).Returns(oppoprojFileStream);

				_nodesetGenerator.Setup(x => x.GenerateTypesSourceCodeFiles(It.IsAny<string>(), It.IsAny<IModelData>())).Returns(typesGeneratorResult);
				_nodesetGenerator.Setup(x => x.GenerateNodesetSourceCodeFiles(It.IsAny<string>(), It.IsAny<IModelData>(), It.IsAny<List<RequiredModelsData>>())).Returns(nodesetGeneratorResult);
			
				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.IsNotEmpty(commandResult.OutputMessages.First().Key);
			}
		}
    }
}