/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.GenerateCommands;
using Appio.Resources.text.logging;
using Appio.Resources.text.output;

namespace Appio.ObjectModel.Tests.CommandStrategies
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
		
		private static string _sampleOpcuaServerAppContent1 = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[" +
																					"{\"name\":\"modelA.xml\",\"uri\": \"namespaceA\",\"types\": \"someTypesA.bsd\",\"namespaceVariable\": \"ns_modelA\", \"requiredModelUris\":[\"namespaceB\"]}," +
																					"{\"name\":\"modelB.xml\",\"uri\": \"namespaceB\",\"types\": \"\",\"namespaceVariable\": \"ns_modelB\", \"requiredModelUris\":[\"namespaceE\",\"namespaceD\"]}," +
																					"{\"name\":\"modelC.xml\",\"uri\": \"namespaceC\",\"types\": \"\",\"namespaceVariable\": \"ns_modelC\", \"requiredModelUris\":[\"namespaceA\",\"namespaceE\",\"namespaceD\"]}," +
																					"{\"name\":\"modelD.xml\",\"uri\": \"namespaceD\",\"types\": \"someTypesD.bsd\",\"namespaceVariable\": \"ns_modelD\", \"requiredModelUris\":[]}," +
																					"{\"name\":\"modelE.xml\",\"uri\": \"namespaceE\",\"types\": \"someTypesD.bsd\",\"namespaceVariable\": \"ns_modelE\", \"requiredModelUris\":[\"namespaceD\"]}," +
																					"]}";

		private static string _sampleOpcuaServerAppContent2 = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[" +
																					"{\"name\":\"modelA.xml\",\"uri\": \"namespaceA\",\"types\": \"\",\"namespaceVariable\": \"ns_modelA\", \"requiredModelUris\":[\"namespaceB\"]}," +
																					"{\"name\":\"modelB.xml\",\"uri\": \"namespaceB\",\"types\": \"\",\"namespaceVariable\": \"ns_modelB\", \"requiredModelUris\":[\"namespaceC\",\"namespaceF\"]}," +
																					"{\"name\":\"modelD.xml\",\"uri\": \"namespaceD\",\"types\": \"\",\"namespaceVariable\": \"ns_modelD\", \"requiredModelUris\":[\"namespaceE\",\"namespaceF\"]}," +
																					"{\"name\":\"modelE.xml\",\"uri\": \"namespaceE\",\"types\": \"\",\"namespaceVariable\": \"ns_modelE\", \"requiredModelUris\":[]}," +
																					"{\"name\":\"modelC.xml\",\"uri\": \"namespaceC\",\"types\": \"\",\"namespaceVariable\": \"ns_modelC\", \"requiredModelUris\":[\"namespaceD\"]}," +
																					"{\"name\":\"modelF.xml\",\"uri\": \"namespaceF\",\"types\": \"\",\"namespaceVariable\": \"ns_modelF\", \"requiredModelUris\":[]}," +
																					"]}";

		protected static string[] ValidOpcuaServerAppContent()
		{
			return new[]
			{
				_sampleOpcuaServerAppContent1,
				_sampleOpcuaServerAppContent2
			};
		}

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
		protected static string[] InvalidOpcuaServerAppContent()
		{
			return new[]
			{
				_opcuaServerAppContentWithDuplicatedModels,
				_opcuaServerAppContentWithModelWhoRequiredItself,
				_opcuaServerAppContentWithMissingRequiredModel
			};
		}
		private readonly string _opcuaServerAppContentWithCircularDependency = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[" +
																					"{\"name\":\"modelA.xml\",\"uri\": \"namespaceA\",\"types\": \"\",\"namespaceVariable\": \"\", \"requiredModelUris\":[\"namespaceB\"]}," +
																					"{\"name\":\"modelB.xml\",\"uri\": \"namespaceB\",\"types\": \"\",\"namespaceVariable\": \"\", \"requiredModelUris\":[]}," +
																					"{\"name\":\"modelC.xml\",\"uri\": \"namespaceC\",\"types\": \"\",\"namespaceVariable\": \"\", \"requiredModelUris\":[\"namespaceD\"]}," +
																					"{\"name\":\"modelD.xml\",\"uri\": \"namespaceD\",\"types\": \"\",\"namespaceVariable\": \"\", \"requiredModelUris\":[\"namespaceE\"]}," +
																					"{\"name\":\"modelE.xml\",\"uri\": \"namespaceE\",\"types\": \"\",\"namespaceVariable\": \"\", \"requiredModelUris\":[\"namespaceC\"]}," +
																					"]}";


		private readonly string _defaultOpcuaClientAppContent = "{\"name\":\"clientApp\",\"type\":\"Client\",\"references\": []}";

		[SetUp]
        public void SetUpTest()
        {
			_fileSystemMock = new Mock<IFileSystem>();
			_modelValidator = new Mock<IModelValidator>();
            _loggerListenerMock = new Mock<ILoggerListener>();
			_loggerWroteOut = false;
			_commandName = Constants.CommandName.Generate + " " + Constants.GenerateCommandOptions.Name;
			_nodesetGenerator = new Mock<INodesetGenerator>();
			_strategy = new GenerateInformationModelStrategy(Constants.GenerateCommandOptions.Name, _fileSystemMock.Object, _modelValidator.Object, _nodesetGenerator.Object);

			AppioLogger.RegisterListener(_loggerListenerMock.Object);
		}

        [TearDown]
        public void CleanUpTest()
        {
            AppioLogger.RemoveListener(_loggerListenerMock.Object);
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
            Assert.AreEqual(Constants.GenerateCommandOptions.Name, commandName);
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
		public void Fail_OnReadingAppioprojFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			using (var appioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(appioprojFileStream);

				// Arrange logger listener
				_loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelFailureCouldntDeserliazeOpcuaapp)).Callback(delegate { _loggerWroteOut = true; });

				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsTrue(_loggerWroteOut);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureCouldntDeserliazeOpcuaapp, projectName, appioprojFilePath), commandResult.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.ReadFile(appioprojFilePath), Times.Once);
			}
		}

		[Test]
		public void Fail_OnReadingAppioprojFileOfAClientApp([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			using (var appioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaClientAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(appioprojFileStream);

				// Arrange logger listener
				_loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelFailuteOpcuaappIsAClient)).Callback(delegate { _loggerWroteOut = true; });

				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsTrue(_loggerWroteOut);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailuteOpcuaappIsAClient, projectName), commandResult.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.ReadFile(appioprojFilePath), Times.Once);
			}
		}

		[Test, Combinatorial]
		public void Success_OnGenerateInformationModel([ValueSource(nameof(ValidInputs))] string[] inputParams, [ValueSource(nameof(ValidOpcuaServerAppContent))] string sampleOpcuaServerAppContent)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);
			var mainCallbacksFilePath = Path.Combine(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp, Constants.FileName.SourceCode_mainCallbacks_c);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp, Constants.FileName.SourceCode_mainCallbacks_c)).Returns(mainCallbacksFilePath);

			using (var appioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(sampleOpcuaServerAppContent)))
			using (var mainCallbacksFileStream = new MemoryStream(Encoding.ASCII.GetBytes("#include \"open62541.h\"\nconst UA_UInt16 ns_modelA = 2;")))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(appioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(mainCallbacksFilePath)).Returns(mainCallbacksFileStream);

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
				_fileSystemMock.Verify(x => x.ReadFile(mainCallbacksFilePath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(mainCallbacksFilePath, It.IsAny<List<string>>()), Times.Once);
			}
		}
		
		[Test, Combinatorial]
		public void Fail_OnInvalidModelList([ValueSource(nameof(ValidInputs))] string[] inputParams, [ValueSource(nameof(InvalidOpcuaServerAppContent))] string sampleOpcuaServerAppContent)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			using (var appioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(sampleOpcuaServerAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(appioprojFileStream);
				
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

		[Test]
		public void Fail_OnModelsCircularDependency([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			using (var appioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_opcuaServerAppContentWithCircularDependency)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(appioprojFileStream);

				// Arrange logger listener
				_loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelCircularDependency)).Callback(delegate { _loggerWroteOut = true; });

				// Act
				var commandResult = _strategy.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsTrue(_loggerWroteOut);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.GenerateInformationModelCircularDependency, projectName), commandResult.OutputMessages.First().Key);
			}
		}

		[Test, Sequential]
		public void Fail_OnCallingNodesetGenerator([Values(false, true)] bool typesGeneratorResult, [Values(true, false)] bool nodesetGeneratorResult, [ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			using (var appioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContent1)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(appioprojFileStream);

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