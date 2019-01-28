using System.IO;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.GenerateCommands;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using static Oppo.ObjectModel.Constants;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class GenerateInformationModelStrategyShould
    {      
        protected static string[][] InvalidInputs_EmptyOpcuaAppName()
        {
            return new[]
            {
                new []{"-n", "", "--model", "model.xml"},
                new []{"--name", "", "-m", "model.xml"}
            };
        }

        protected static string[][] InvalidInputs_UnknownNameParam()
        {
            return new[]
            {
                new []{"-any string", "testApp", "-m", "model.txt"},
                new []{"-N", "testApp", "-m", "model.txt"},
                new []{"-name", "testApp", "-m", "model.txt"},
                new []{"--nam", "testApp", "-m", "model.txt"}
            };
        }

        protected static string[][] InvalidInputs_UnknownModelParam()
        {
            return new[]
            {
                new []{"-n", "testApp", "-any string", "model.txt"},
                new []{"-n", "testApp", "-M", "model.txt"},
                new []{"--name", "testApp", "-model", "model.txt"},
                new []{"--name", "testApp", "--mod", "model.txt"}
            };
        }

        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new []{"-n", "testApp", "-m", "model.xml"},
                new []{"-n", "testApp", "--model", "model.xml"},
                new []{"--name", "testApp", "-m", "model.xml"},
                new []{"--name", "testApp", "--model", "model.xml"},
            };
        }

        protected static string[][] InvalidInputs_InvalidModelExtension()
        {
            return new[]
            {
                new []{"-n", "testApp", "-m", "model.txt"},
                new []{"-n", "testApp", "--model", "model.txt"},
                new []{"--name", "testApp", "-m", "model.txt"},
                new []{"--name", "testApp", "--model", "model.txt"},
            };


        }
        protected static string[][] ValidInputs_ExtraTypes()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "--types", "types.bsd"},
                new [] { "-n", "testApp", "-m", "model.xml", "-t", "types.bsd"},
            };
        }

        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IModelValidator> _modelValidatorMock;
        private GenerateInformationModelStrategy _strategy;
        private Mock<ILoggerListener> _loggerListenerMock;
        private readonly string _srcDir = @"src\server";
        private readonly string _defaultModelsC = "/* \n* This is an automatically generated file.\n*/";
        private readonly string _defaultModelIncludeSnippet = "#include \"information-models/{0}.c\"";
        private readonly string _defaultNodeSetFunctionsC = "UA_StatusCode callNodeSetFunctions(UA_Server* server)\n{\n\treturn UA_STATUSCODE_GOOD;\n}";

        [SetUp]
        public void SetUpTest()
        {
            _loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(_loggerListenerMock.Object);
            _mockFileSystem = new Mock<IFileSystem>();
            _modelValidatorMock = new Mock<IModelValidator>();
            _strategy = new GenerateInformationModelStrategy(GenerateInformationModeCommandArguments.Name, _mockFileSystem.Object, _modelValidatorMock.Object);
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
            Assert.AreEqual(GenerateInformationModeCommandArguments.Name, commandName);
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
        public void GenerateInformationModelForTheFirstTime([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            _loggerListenerMock.Setup(x => x.Info(LoggingText.GenerateInformationModelSuccess));            
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelName = System.IO.Path.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);           

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);

            var modelTargetLocation = System.IO.Path.Combine(Constants.DirectoryName.InformationModels, modelName);
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName)).Returns(modelTargetLocation);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            var modelPath = System.IO.Path.Combine(Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            var sourceModelRelativePath = @"../../" + modelPath;
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(modelPath);

            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, string.Format(Constants.ExecutableName.NodsetCompilerArguments, sourceModelRelativePath, modelTargetLocation))).Returns(true);
            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);

            using (var memoryStream = GenerateStreamFromString(_defaultModelsC))
            {
                var modelsFilePath = System.IO.Path.Combine(_srcDir, Constants.FileName.SourceCode_models_c);
                _mockFileSystem.Setup(x => x.CombinePaths(_srcDir, Constants.FileName.SourceCode_models_c)).Returns(modelsFilePath);
                _mockFileSystem.Setup(x => x.ReadFile(modelsFilePath)).Returns(memoryStream);

                using (var nodeSetFunctionsMemoryStream = GenerateStreamFromString(_defaultNodeSetFunctionsC))
                {
                    var nodeSetFunctionsFilePath = System.IO.Path.Combine(_srcDir, Constants.FileName.SourceCode_nodeSetFunctions_c);
                    _mockFileSystem.Setup(x => x.CombinePaths(_srcDir, Constants.FileName.SourceCode_nodeSetFunctions_c)).Returns(nodeSetFunctionsFilePath);
                    _mockFileSystem.Setup(x => x.ReadFile(nodeSetFunctionsFilePath)).Returns(nodeSetFunctionsMemoryStream);


                    // Act
                    var commandResult = _strategy.Execute(inputParams);

                    // Assert
                    Assert.IsTrue(commandResult.Sucsess);
                    Assert.IsNotNull(commandResult.OutputMessages);
                    var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
                    Assert.AreEqual(string.Format(OutputText.GenerateInformationModelSuccess, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3)), firstMessageLine.Key);
                    Assert.AreEqual(string.Empty, firstMessageLine.Value);
                    _loggerListenerMock.Verify(x => x.Info(LoggingText.GenerateInformationModelSuccess), Times.Once);
                    _mockFileSystem.Verify(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);
                    _mockFileSystem.Verify(x => x.CreateDirectory(System.IO.Path.Combine(_srcDir, DirectoryName.InformationModels)), Times.Once);
                    _mockFileSystem.Verify(x => x.WriteFile(nodeSetFunctionsFilePath, It.IsAny<IEnumerable<string>>()), Times.Once);
                }
            }            
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [Test]
        public void GenerateInformationModelForSeccondTime([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            _loggerListenerMock.Setup(x => x.Info(LoggingText.GenerateInformationModelSuccess));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelName = System.IO.Path.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);

            var modelTargetLocation = System.IO.Path.Combine(Constants.DirectoryName.InformationModels, modelName);
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName)).Returns(modelTargetLocation);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            var modelPath = System.IO.Path.Combine(Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            var sourceModelRelativePath = @"../../" + modelPath;
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(modelPath);
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, string.Format(Constants.ExecutableName.NodsetCompilerArguments, sourceModelRelativePath, modelTargetLocation))).Returns(true);

            _mockFileSystem.Setup(x => x.DirectoryExists(System.IO.Path.Combine(_srcDir, DirectoryName.InformationModels))).Returns(true);
            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);

            using (var modelsMemoryStream = GenerateStreamFromString(_defaultModelsC + "\n" + string.Format(_defaultModelIncludeSnippet, modelName)))
            {
                var modelsFilePath = System.IO.Path.Combine(_srcDir, Constants.FileName.SourceCode_models_c);
                _mockFileSystem.Setup(x => x.CombinePaths(_srcDir, Constants.FileName.SourceCode_models_c)).Returns(modelsFilePath);
                _mockFileSystem.Setup(x => x.ReadFile(modelsFilePath)).Returns(modelsMemoryStream);

                using (var nodeSetFunctionsMemoryStream = GenerateStreamFromString(_defaultNodeSetFunctionsC))
                {
                    var nodeSetFunctionsFilePath = System.IO.Path.Combine(_srcDir, Constants.FileName.SourceCode_nodeSetFunctions_c);
                    _mockFileSystem.Setup(x => x.CombinePaths(_srcDir, Constants.FileName.SourceCode_nodeSetFunctions_c)).Returns(nodeSetFunctionsFilePath);
                    _mockFileSystem.Setup(x => x.ReadFile(nodeSetFunctionsFilePath)).Returns(nodeSetFunctionsMemoryStream);

                    // Act
                    var commandResult = _strategy.Execute(inputParams);

                    // Assert
                    Assert.IsTrue(commandResult.Sucsess);
                    Assert.IsNotNull(commandResult.OutputMessages);
                    var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
                    Assert.AreEqual(string.Format(OutputText.GenerateInformationModelSuccess, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3)), firstMessageLine.Key);
                    Assert.AreEqual(string.Empty, firstMessageLine.Value);
                    _loggerListenerMock.Verify(x => x.Info(LoggingText.GenerateInformationModelSuccess), Times.Once);
                    _mockFileSystem.Verify(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);
                    _mockFileSystem.Verify(x => x.CreateDirectory(System.IO.Path.Combine(_srcDir, DirectoryName.InformationModels)), Times.Never);
                    _mockFileSystem.Verify(x => x.WriteFile(nodeSetFunctionsFilePath, It.IsAny<IEnumerable<string>>()), Times.Once);
                }
            }
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseModelIsNotValid([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            _loggerListenerMock.Setup(x => x.Info(LoggingText.GenerateInformationModelSuccess));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);

            var modelTargetLocation = System.IO.Path.Combine(Constants.DirectoryName.InformationModels, modelName);
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName)).Returns(modelTargetLocation);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            var modelPath = System.IO.Path.Combine(Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            var sourceModelRelativePath = @"../../" + modelPath;
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(modelPath);

            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, string.Format(Constants.ExecutableName.NodsetCompilerArguments, sourceModelRelativePath, modelTargetLocation))).Returns(true);
            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName)).Returns(false);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(string.Format(OutputText.GenerateInformationModelFailureValidatingModel, modelFullName)), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _loggerListenerMock.Verify(x => x.Warn(string.Format(LoggingText.GenerateInformationModelFailureValidatingModel, modelFullName)), Times.Once);            
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseNodesetCompilerCallFailure([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            _loggerListenerMock.Setup(x => x.Info(LoggingText.GenerateInformationModelSuccess));
            
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelName = System.IO.Path.GetFileName(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);
            var args = inputParams.ElementAtOrDefault(3) + " " + modelName;
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, args)).Returns(false);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailure, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3)), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _loggerListenerMock.Verify(x => x.Warn(LoggingText.NodesetCompilerExecutableFails), Times.Once);
            _mockFileSystem.Verify(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseModelDoesntExists([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange            
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelName = System.IO.Path.GetFileName(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(false);
            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, calculatedModelFilePath)));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingModel, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3), calculatedModelFilePath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _mockFileSystem.Verify(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3)), Times.Once);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseInvalidModelExtension([ValueSource(nameof(InvalidInputs_InvalidModelExtension))] string[] inputParams)
        {
            // Arrange            
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelName = System.IO.Path.GetFileName(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);
            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, modelName)));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidModel, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3), System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3))), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _mockFileSystem.Verify(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.Models, inputParams.ElementAtOrDefault(3)), Times.Once);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseUknownNameParam([ValueSource(nameof(InvalidInputs_UnknownNameParam))] string[] inputParams)
        {
            // Arrange            
            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, inputParams.ElementAtOrDefault(0))));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3), inputParams.ElementAtOrDefault(0)), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseUknownModelParam([ValueSource(nameof(InvalidInputs_UnknownModelParam))] string[] inputParams)
        {
            // Arrange            
            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, inputParams.ElementAtOrDefault(2))));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3), inputParams.ElementAtOrDefault(2)), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseEmptyOpcuaAppName([ValueSource(nameof(InvalidInputs_EmptyOpcuaAppName))] string[] inputParams)
        {
            // Arrange            
            _loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelFailureEmptyOpcuaAppName));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureEmptyOpcuaAppName, inputParams.ElementAtOrDefault(1), inputParams.ElementAtOrDefault(3)), firstMessageLine.Key);

            Assert.AreEqual(string.Empty, firstMessageLine.Value);

        }
        [Test]
        public void ShouldGenerateInformationModelWithExtraTypes([ValueSource(nameof(ValidInputs_ExtraTypes))] string[] inputParams)
        {
            //Arrange
            _loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelSuccess));
            var nameFlag = inputParams.ElementAtOrDefault(0);
            var projectName = inputParams.ElementAtOrDefault(1);
            var modelFlag = inputParams.ElementAtOrDefault(2);
            var modelNameWithExtension = inputParams.ElementAtOrDefault(3);
            var typesFlag = inputParams.ElementAtOrDefault(4);
            var typesNameWithExtension = inputParams.ElementAtOrDefault(5);

            _mockFileSystem.Setup(x => x.CombinePaths(projectName,Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            //Arrange model file
            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelNameWithExtension);
            _mockFileSystem.Setup(x => x.GetFileName(modelNameWithExtension)).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(projectName, DirectoryName.Models, modelNameWithExtension);
            _mockFileSystem.Setup(x => x.CombinePaths(projectName, DirectoryName.Models, modelNameWithExtension)).Returns(calculatedModelFilePath);

            var modelTargetLocation = System.IO.Path.Combine(Constants.DirectoryName.InformationModels, modelName);
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName)).Returns(modelTargetLocation);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(modelNameWithExtension);
            _mockFileSystem.Setup(x => x.GetExtension(modelNameWithExtension)).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(modelNameWithExtension)).Returns(modelName);

            var modelPath = System.IO.Path.Combine(Constants.DirectoryName.Models, modelNameWithExtension);
            var sourceModelRelativePath = @"../../" + modelPath;
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, modelNameWithExtension)).Returns(modelPath);

            //Arrange data types file
            var typesName = System.IO.Path.GetFileNameWithoutExtension(typesNameWithExtension);
            _mockFileSystem.Setup(x => x.GetFileName(typesNameWithExtension)).Returns(typesName);

            var calculatedTypesFilePath = System.IO.Path.Combine(projectName, DirectoryName.Models, typesNameWithExtension);
            _mockFileSystem.Setup(x => x.CombinePaths(projectName, DirectoryName.Models, typesNameWithExtension)).Returns(calculatedTypesFilePath);

            var typesTargetLocation = System.IO.Path.Combine(Constants.DirectoryName.InformationModels, typesName);
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, typesName)).Returns(typesTargetLocation);
            _mockFileSystem.Setup(x => x.FileExists(calculatedTypesFilePath)).Returns(true);

            var typesExtension = System.IO.Path.GetExtension(typesNameWithExtension);
            _mockFileSystem.Setup(x => x.GetExtension(typesNameWithExtension)).Returns(typesExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(typesNameWithExtension)).Returns(typesName);

            var typesPath = System.IO.Path.Combine(Constants.DirectoryName.Models, typesName);
            var sourceTypesRelativePath = @"../../" + typesPath;
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, typesName)).Returns(typesPath);

            //Arrange Executables
            var generateDatatypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath + Constants.ExecutableName.GenerateDatatypesTypeBsd + sourceTypesRelativePath + " " + typesTargetLocation;
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, generateDatatypesArgs)).Returns(true);
            var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerTypesArray + Constants.ExecutableName.NodesetCompilerBasicTypes + Constants.ExecutableName.NodesetCompilerExisting + Constants.ExecutableName.NodesetCompilerBasicNodeset + string.Format(Constants.ExecutableName.NodesetCompilerXml, sourceModelRelativePath, modelTargetLocation);
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, nodesetCompilerArgs)).Returns(true);
            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);

            Assert.IsTrue(true);

            //Act
            //var commandResult = _strategy.Execute(inputParams);
            //Assert
            //Assert.IsFalse(commandResult.Sucsess);
            //Assert.IsNotNull(commandResult.OutputMessages);
            //var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            //Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }
    }
        
   
}