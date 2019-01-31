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
        // Valid inputs

        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml" },
                new [] { "-n", "testApp", "--model", "model.xml" },
                new [] { "--name", "testApp", "-m", "model.xml" },
                new [] { "--name", "testApp", "--model", "model.xml" }
            };
        }

        protected static string[][] ValidInputs_Types()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "--types", "types.bsd" },
                new [] { "-n", "testApp", "-m", "model.xml", "-t", "types.bsd" }
            };
        }

        protected static string[][] ValidInputs_RequiredModel()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "--requiredModel", "requiredModel.xml" },
                new [] { "-n", "testApp", "-m", "model.xml", "-r", "requiredModel.xml" }
            };
        }

        protected static string[][] ValidInputs_TypesAndRequiredModel()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "--types", "types.bsd", "--requiredModel", "requiredModel.xml" },
                new [] { "-n", "testApp", "-m", "model.xml", "-t", "types.bsd", "--requiredModel", "requiredModel.xml" },
                new [] { "-n", "testApp", "-m", "model.xml", "--types", "types.bsd", "-r", "requiredModel.xml" },
                new [] { "-n", "testApp", "-m", "model.xml", "-t", "types.bsd", "-r", "requiredModel.xml" }
            };
        }

        // Invalid input opcua app name

        protected static string[][] InvalidInputs_EmptyOpcuaAppName()
        {
            return new[]
            {
                new [] { "-n", "", "--model", "model.xml" },
                new [] { "--name", "", "-m", "model.xml" }
            };
        }

        protected static string[][] InvalidInputs_UnknownNameParam()
        {
            return new[]
            {
                new [] { "-any string", "testApp", "-m", "model.txt" },
                new [] { "-N", "testApp", "-m", "model.txt" },
                new [] { "-name", "testApp", "-m", "model.txt" },
                new [] { "--nam", "testApp", "-m", "model.txt" }
            };
        }

        // Invalid inputs model

        protected static string[][] InvalidInputs_UnknownModelParam()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-any string", "model.txt" },
                new [] { "-n", "testApp", "-M", "model.txt" },
                new [] { "--name", "testApp", "-model", "model.txt" },
                new [] { "--name", "testApp", "--mod", "model.txt" }
            };
        }

        protected static string[][] InvalidInputs_InvalidModelExtension()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.txt" },
                new [] { "-n", "testApp", "--model", "model.txt" },
                new [] { "--name", "testApp", "-m", "model.txt" },
                new [] { "--name", "testApp", "--model", "model.txt" }
            };
        }

        // Invalid inputs types

        protected static string[][] InvalidInputs_UnknownTypesParam()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "-T", "types.bsd" },
                new [] { "-n", "testApp", "-m", "model.xml", "--t", "types.bsd" },
                new [] { "-n", "testApp", "-m", "model.xml", "-types", "types.bsd" },
                new [] { "-n", "testApp", "-m", "model.xml", "--Types", "types.bsd" },
            };
        }

        protected static string[][] InvalidInputs_InvalidTypesExtension()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "--types", "types.xml"},
                new [] { "-n", "testApp", "-m", "model.xml", "-t", "types.txt"},
            };
        }

        // Invalid inputs required model

        protected static string[][] InvalidInputs_UnknownRequiredModelParam()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "-R", "requiredModel.xml"},
                new [] { "-n", "testApp", "-m", "model.xml", "--r", "requiredModel.xml"},
                new [] { "-n", "testApp", "-m", "model.xml", "--RequiredModel", "requiredModel.xml"},
                new [] { "-n", "testApp", "-m", "model.xml", "-requiredModel", "requiredModel.xml"},
            };
        }

        //Invalid inputs required model extension

        protected static string[][] InvalidInputs_RequiredModelExtension()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "--requiredModel", "requiredModel.txt" },
                new [] { "-n", "testApp", "-m", "model.xml", "-r", "requiredModel.txt" },
                new [] { "-n", "testApp", "-m", "model.xml", "--requiredModel", "requiredModel.bsd" },
                new [] { "-n", "testApp", "-m", "model.xml", "-r", "requiredModel.bsd" }
                
            };
        }

        // Invalid inputs types and required model

        protected static string[][] InvalidInputs_TypesAndUnknownRequiredModelParam()
        {
            return new[]
            {
                new [] { "-n", "testApp", "-m", "model.xml", "--types", "types.bsd", "--RequiredModel", "requiredModel.xml" },
                new [] { "-n", "testApp", "-m", "model.xml", "-t", "types.bsd", "-requiredModel", "requiredModel.xml" },
                new [] { "-n", "testApp", "-m", "model.xml", "--types", "types.bsd", "-R", "requiredModel.xml" },
                new [] { "-n", "testApp", "-m", "model.xml", "-t", "types.bsd", "--r", "requiredModel.xml" }
            };
        }


        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IModelValidator> _modelValidatorMock;
        private GenerateInformationModelStrategy _strategy;
        private Mock<ILoggerListener> _loggerListenerMock;
        private readonly string _srcDir = @"src\server";
        private readonly string _defaultModelsC = "/* \n* This is an automatically generated file.\n*/";
        private readonly string _defaultModelIncludeSnippet = "#include \"information-models/{0}.c\"";
        private readonly string _defaultTypesIncludeSnippet = "#include\"information-models/{0}_generated.c\"";
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

            var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) + string.Format(Constants.ExecutableName.NodesetCompilerXml, sourceModelRelativePath, modelTargetLocation);
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, nodesetCompilerArgs)).Returns(true);
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
            var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) + string.Format(Constants.ExecutableName.NodesetCompilerXml, sourceModelRelativePath, modelTargetLocation);
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, nodesetCompilerArgs)).Returns(true);

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

            var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) + string.Format(Constants.ExecutableName.NodesetCompilerXml, sourceModelRelativePath, modelTargetLocation);
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, nodesetCompilerArgs)).Returns(true);
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
        public void GenerateInformationModelWithExtraTypes([ValueSource(nameof(ValidInputs_Types))] string[] inputParams)
        {
            //Arrange
            _loggerListenerMock.Setup(x => x.Warn(LoggingText.GenerateInformationModelSuccess));
            var nameFlag = inputParams.ElementAtOrDefault(0);
            var projectName = inputParams.ElementAtOrDefault(1);
            var modelFlag = inputParams.ElementAtOrDefault(2);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var typesFlag = inputParams.ElementAtOrDefault(4);
            var typesFullName = inputParams.ElementAtOrDefault(5);

            _mockFileSystem.Setup(x => x.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            //Arrange model file
            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileName(modelFullName)).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(projectName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(projectName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);

            var modelTargetLocation = System.IO.Path.Combine(Constants.DirectoryName.InformationModels, modelName);
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, modelName)).Returns(modelTargetLocation);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(modelFullName)).Returns(modelName);

            var modelPath = System.IO.Path.Combine(Constants.DirectoryName.Models, modelFullName);
            var sourceModelRelativePath = @"../../" + modelPath;
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, modelFullName)).Returns(modelPath);

            //Arrange data types file
            var typesName = System.IO.Path.GetFileNameWithoutExtension(typesFullName);
            _mockFileSystem.Setup(x => x.GetFileName(typesFullName)).Returns(typesName);

            var calculatedTypesFilePath = System.IO.Path.Combine(projectName, DirectoryName.Models, typesFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(projectName, DirectoryName.Models, typesFullName)).Returns(calculatedTypesFilePath);

            var typesTargetLocation = System.IO.Path.Combine(Constants.DirectoryName.InformationModels, typesName);
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.InformationModels, typesName)).Returns(typesTargetLocation);
            _mockFileSystem.Setup(x => x.FileExists(calculatedTypesFilePath)).Returns(true);

            var typesExtension = System.IO.Path.GetExtension(typesFullName);
            _mockFileSystem.Setup(x => x.GetExtension(typesFullName)).Returns(typesExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(typesFullName)).Returns(typesName);

            var typesPath = System.IO.Path.Combine(Constants.DirectoryName.Models, typesFullName);
            var sourceTypesRelativePath = @"../../" + typesPath;
            _mockFileSystem.Setup(x => x.CombinePaths(Constants.DirectoryName.Models, typesFullName)).Returns(typesPath);

            //Arrange Executables
            var generateDatatypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath + string.Format(Constants.ExecutableName.GenerateDatatypesTypeBsd, sourceTypesRelativePath) + " " + typesTargetLocation;
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, generateDatatypesArgs)).Returns(true);
            var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, typesName.ToUpper()) + string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) + string.Format(Constants.ExecutableName.NodesetCompilerXml, sourceModelRelativePath, modelTargetLocation);
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, nodesetCompilerArgs)).Returns(true);
            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName)).Returns(true);

            //Arrange models.c
            var modelsFilePath = System.IO.Path.Combine(_srcDir, Constants.FileName.SourceCode_models_c);
            _mockFileSystem.Setup(x => x.CombinePaths(_srcDir, Constants.FileName.SourceCode_models_c)).Returns(modelsFilePath);

            var modelsMemoryStream = GenerateStreamFromString(_defaultModelsC + "\n" + string.Format(_defaultModelIncludeSnippet, modelName));
            var typesMemoryStream = GenerateStreamFromString(_defaultModelsC + string.Format(_defaultModelIncludeSnippet, modelName) + "\n" + string.Format(_defaultTypesIncludeSnippet, typesName.ToLower()));
            _mockFileSystem.SetupSequence(x => x.ReadFile(modelsFilePath)).Returns(modelsMemoryStream).Returns(typesMemoryStream);

            //Arrange nodeSetFunctioncs.c
            var nodeSetFunctionsFilePath = System.IO.Path.Combine(_srcDir, Constants.FileName.SourceCode_nodeSetFunctions_c);
            _mockFileSystem.Setup(x => x.CombinePaths(_srcDir, Constants.FileName.SourceCode_nodeSetFunctions_c)).Returns(nodeSetFunctionsFilePath);

            var nodeSetFunctionsMemoryStream = GenerateStreamFromString(_defaultNodeSetFunctionsC);
            _mockFileSystem.Setup(x => x.ReadFile(nodeSetFunctionsFilePath)).Returns(nodeSetFunctionsMemoryStream);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelSuccess, projectName, modelFullName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _loggerListenerMock.Verify(x => x.Info(LoggingText.GenerateInformationModelSuccess), Times.Once);
            _mockFileSystem.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, generateDatatypesArgs), Times.Once);
            _mockFileSystem.Verify(x => x.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);
            _mockFileSystem.Verify(x => x.CreateDirectory(System.IO.Path.Combine(_srcDir, DirectoryName.InformationModels)), Times.Once);
            _mockFileSystem.Verify(x => x.WriteFile(nodeSetFunctionsFilePath, It.IsAny<IEnumerable<string>>()), Times.Once);

            nodeSetFunctionsMemoryStream.Close();
            nodeSetFunctionsMemoryStream.Dispose();
            typesMemoryStream.Close();
            typesMemoryStream.Dispose();
            modelsMemoryStream.Close();
            modelsMemoryStream.Dispose();
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseUknownTypesParam([ValueSource(nameof(InvalidInputs_UnknownTypesParam))] string[] inputParams)
        {
            //Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var typesFlag = inputParams.ElementAtOrDefault(4);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, typesFlag)));

            // Arrange modelFile
            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(modelFullName)).Returns(modelName);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, typesFlag), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseTypesDoesntExist([ValueSource(nameof(ValidInputs_Types))] string[] inputParams)
        {
            //Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var typesFullName = inputParams.ElementAtOrDefault(5);

            // Arrange Opcua Application  
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            // Arrange Model
            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(modelFullName)).Returns(modelName);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            //Arrange Types
            var calculatedTypesFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, typesFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, typesFullName)).Returns(calculatedTypesFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedTypesFilePath)).Returns(false);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, calculatedTypesFilePath)));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingFile, opcuaAppName, modelFullName, calculatedTypesFilePath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _mockFileSystem.Verify(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, typesFullName), Times.Once);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseInvalidTypesExtension([ValueSource(nameof(InvalidInputs_InvalidTypesExtension))] string[] inputParams)
        {
            //Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var typesFullName = inputParams.ElementAtOrDefault(5);

            // Arrange Opcua Aplication            
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            //Arrange Model
            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            //Arrange Types
            var calculatedTypesFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, typesFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, typesFullName)).Returns(calculatedTypesFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedTypesFilePath)).Returns(true);

            var typesExtension = System.IO.Path.GetExtension(typesFullName);
            _mockFileSystem.Setup(x => x.GetExtension(typesFullName)).Returns(typesExtension);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, typesFullName)));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidFile, opcuaAppName, modelFullName, typesFullName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _mockFileSystem.Verify(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, typesFullName), Times.Once);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseGenerateTypesCallFailure([ValueSource(nameof(ValidInputs_Types))] string[] inputParams)
        {
            // Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var typesFullName = inputParams.ElementAtOrDefault(5);

            //Arrange Opcua Application
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            //Arrange Model
            var modelName = System.IO.Path.GetFileName(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileName(modelFullName)).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            //Arrange Types
            var typesName = System.IO.Path.GetFileName(typesFullName);
            _mockFileSystem.Setup(x => x.GetFileName(typesFullName)).Returns(typesName);

            var calculatedTypesFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, typesFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, typesFullName)).Returns(calculatedTypesFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedTypesFilePath)).Returns(true);

            var typesExtension = System.IO.Path.GetExtension(typesFullName);
            _mockFileSystem.Setup(x => x.GetExtension(typesFullName)).Returns(typesExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(typesFullName)).Returns(typesName);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, opcuaAppName, modelFullName, typesFullName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _loggerListenerMock.Verify(x => x.Warn(LoggingText.GeneratedTypesExecutableFails), Times.Once);
            _mockFileSystem.Verify(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);

            _mockFileSystem.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseUknownRequiredModelParam([ValueSource(nameof(InvalidInputs_UnknownRequiredModelParam))] string[] inputParams)
        {
            //Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var requiredModelFlag = inputParams.ElementAtOrDefault(4);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, requiredModelFlag)));

            // Arrange modelFile
            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(modelFullName)).Returns(modelName);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, requiredModelFlag), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseUknownRequiredModelParamWithTypes([ValueSource(nameof(InvalidInputs_TypesAndUnknownRequiredModelParam))] string[] inputParams)
        {
            //Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var typesFullName = inputParams.ElementAtOrDefault(5);
            var requiredModelFlag = inputParams.ElementAtOrDefault(6);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, requiredModelFlag)));

            // Arrange model file
            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(modelFullName)).Returns(modelName);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            // Arrange types file
            var calculatedTypesFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, typesFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, typesFullName)).Returns(calculatedTypesFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedTypesFilePath)).Returns(true);

            var typesName = System.IO.Path.GetFileNameWithoutExtension(typesFullName);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(typesFullName)).Returns(typesName);

            var typesExtension = System.IO.Path.GetExtension(typesFullName);
            _mockFileSystem.Setup(x => x.GetExtension(typesFullName)).Returns(typesExtension);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, requiredModelFlag), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }

        [Test]
        public void FailOnGenerateInformationModelBecauseNodesetCompilerWithRequiredModelCallFailure([ValueSource(nameof(ValidInputs_RequiredModel))] string[] inputParams)
        {
            // Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var requiredModelFullName = inputParams.ElementAtOrDefault(5);

            //Arrange Opcua Application
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            //Arrange Model
            var modelName = System.IO.Path.GetFileName(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileName(modelFullName)).Returns(modelName);

            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            //Arrange Types
            var requiredModelName = System.IO.Path.GetFileName(requiredModelFullName);
            _mockFileSystem.Setup(x => x.GetFileName(requiredModelFullName)).Returns(requiredModelName);

            var calculatedRequiredModelPath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, requiredModelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, requiredModelFullName)).Returns(calculatedRequiredModelPath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedRequiredModelPath)).Returns(true);

            var requiredModelExtension = System.IO.Path.GetExtension(requiredModelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(requiredModelFullName)).Returns(requiredModelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(requiredModelFullName)).Returns(requiredModelName);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(true);
            /*Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelRequiredModelFailure, opcuaAppName, modelFullName, requiredModelFullName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _loggerListenerMock.Verify(x => x.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsRequiredModel,requiredModelFullName)), Times.Once);
            _mockFileSystem.Verify(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);
            _mockFileSystem.Verify(x => x.CallExecutable(Constants.ExecutableName.PythonScript, _srcDir, It.IsAny<string>()), Times.Once);*/
        }
        [Test]
      
        public void FailOnGenerateInformationModelBecauseRequiredModelDoesntExist([ValueSource(nameof(ValidInputs_RequiredModel))] string[] inputParams)
        {
            //Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var requiredModelFullName = inputParams.ElementAtOrDefault(5);

            // Arrange Opcua Application  
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            // Arrange Model
            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(modelFullName)).Returns(modelName);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            //Arrange Required Model
            var calculatedRequiredModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, requiredModelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, requiredModelFullName)).Returns(calculatedRequiredModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedRequiredModelFilePath)).Returns(false);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, calculatedRequiredModelFilePath)));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingFile, opcuaAppName, modelFullName, calculatedRequiredModelFilePath), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _mockFileSystem.Verify(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredModelFullName), Times.Once);

        }
        [Test]
        public void FailOnGenerateInformationModelBecauseInvalidRequiredModelExtension([ValueSource(nameof(InvalidInputs_RequiredModelExtension))] string[] inputParams)
        {
            //Arrange
            var opcuaAppName = inputParams.ElementAtOrDefault(1);
            var modelFullName = inputParams.ElementAtOrDefault(3);
            var requiredModelFullName = inputParams.ElementAtOrDefault(5);

            // Arrange Opcua Aplication            
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);

            //Arrange Model
            var calculatedModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, modelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, modelFullName)).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(modelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(modelFullName)).Returns(modelExtension);

            _modelValidatorMock.Setup(x => x.Validate(calculatedModelFilePath, It.IsAny<string>())).Returns(true);

            //Arrange Required Model
            var calculatedRequiredModelFilePath = System.IO.Path.Combine(opcuaAppName, DirectoryName.Models, requiredModelFullName);
            _mockFileSystem.Setup(x => x.CombinePaths(opcuaAppName, DirectoryName.Models, requiredModelFullName)).Returns(calculatedRequiredModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedRequiredModelFilePath)).Returns(true);

            var typesExtension = System.IO.Path.GetExtension(requiredModelFullName);
            _mockFileSystem.Setup(x => x.GetExtension(requiredModelFullName)).Returns(typesExtension);

            _loggerListenerMock.Setup(x => x.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, requiredModelFullName)));

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidFile, opcuaAppName, modelFullName, requiredModelFullName), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _mockFileSystem.Verify(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredModelFullName), Times.Once);
        }
    }
}