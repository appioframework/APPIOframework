using System.IO;
using System.Linq;
using System.Collections.Generic;
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

        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IModelValidator> _modelValidatorMock;
        private GenerateInformationModelStrategy _strategy;
        private Mock<ILoggerListener> _loggerListenerMock;
        private readonly string _srcDir = @"src\server";
        private readonly string _defaultServerMesonBuild        = "server_app_sources += [\n]";
        private readonly string _defaultLoadInformationModelsC  = "UA_StatusCode loadInformationModels(UA_Server* server)\n{\n\treturn UA_STATUSCODE_GOOD;\n}";
		private readonly string _uaMethodSample					= "<?xml version=\"1.0\" encoding=\"utf-8\"?><UANodeSet><NamespaceUris><Uri>test</Uri></NamespaceUris><UAMethod NodeId=\"ns=1;i=1000\" BrowseName=\"sampleBrowseName\"></UAMethod></UANodeSet>";
		private readonly string _defaultMainCallbacsC			= "UA_StatusCode addCallbacks(UA_Server* server)\n{\n\treturn UA_STATUSCODE_GOOD;\n}";

		[SetUp]
        public void SetUpTest()
        {
            _loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(_loggerListenerMock.Object);
            _mockFileSystem = new Mock<IFileSystem>();
            _modelValidatorMock = new Mock<IModelValidator>();
            _strategy = new GenerateInformationModelStrategy(Constants.GenerateInformationModeCommandArguments.Name, _mockFileSystem.Object, _modelValidatorMock.Object);
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
        public void FailOnGenerateInformationModelBecauseUknownNameParam([ValueSource(nameof(InvalidInputs_UnknownNameParam))] string[] inputParams)
        {
			// Arrange       
			var projectNameFlag = inputParams.ElementAtOrDefault(0);

			_loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, inputParams.ElementAtOrDefault(0))));

            // Act
            var commandResult = _strategy.Execute(inputParams);

			// Assert
			Assert.Multiple(() =>
			{
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.UnknownParameterProvided, projectNameFlag, "'-n' or '--name'"), commandResult.OutputMessages.FirstOrDefault().Key);
			});
		}
		
        [Test]
        public void FailOnGenerateInformationModelBecauseEmptyOpcuaAppName([ValueSource(nameof(InvalidInputs_EmptyOpcuaAppName))] string[] inputParams)
        {
			// Arrange
			var projectNameFlag = inputParams.ElementAtOrDefault(0);

            _loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelFailureEmptyOpcuaAppName));

            // Act
            var commandResult = _strategy.Execute(inputParams);

			// Assert
			Assert.Multiple(() =>
			{
				Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.ParameterValueMissing, projectNameFlag), commandResult.OutputMessages.FirstOrDefault().Key);
			});
        }

		[Test]
		public void SuccessOnGenerateInformationModel([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);

			_loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelSuccess));
			
			// Act
			var commandResult = _strategy.Execute(inputParams);

			// Assert
			Assert.Multiple(() =>
			{
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelSuccess, projectName), commandResult.OutputMessages.First().Key);
			});
		}
    }
}