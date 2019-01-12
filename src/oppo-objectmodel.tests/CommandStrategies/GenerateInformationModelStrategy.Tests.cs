using System.Collections.Generic;
using System.Linq;
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
        protected static string[][] InvalidInputs()
        {
            return new[]
            {
                new []{""},
                new string[0],
                new []{"any string"},
                new []{"-n", ""},
                new []{"-name", ""},
                new []{"-n", "testApp"},
                new []{"-n", "testApp", "-M"},
                new []{"-n", "testApp", "-m"},
                new []{"-n", "testApp", "-m", ""},
                new []{"-n", "testApp", "-m", "model.txt"},
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

        private Mock<IFileSystem> _mockFileSystem;
        private GenerateInformationModelStrategy _strategy;
        private Mock<ILoggerListener> _loggerListenerMock;
        private readonly string _srcDir = @"src/server";

        [SetUp]
        public void SetUpTest()
        {
            _loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(_loggerListenerMock.Object);
            _mockFileSystem = new Mock<IFileSystem>();
            _strategy = new GenerateInformationModelStrategy(GenerateInformationModeCommandArguments.Name, _mockFileSystem.Object);
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
            Assert.AreEqual(Resources.text.help.HelpTextValues.GenerateInformationModelArgumentCommandDescription, helpText);
        }

        [Test]
        public void GenerateInformationModelForTheFirstTime([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            _loggerListenerMock.Setup(x => x.Info(LoggingText.GenerateInformationModelSuccess));
            
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelName = System.IO.Path.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3));
            var args = inputParams.ElementAtOrDefault(3) + " " + modelName;
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.NodsetCompiler, _srcDir, args)).Returns(true);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelSuccess, inputParams.ElementAtOrDefault(3)), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _loggerListenerMock.Verify(x => x.Info(LoggingText.GenerateInformationModelSuccess), Times.Once);
            _mockFileSystem.Verify(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);
            _mockFileSystem.Verify(x => x.CreateDirectory(System.IO.Path.Combine(_srcDir, DirectoryName.InformationModels)), Times.Once);
        }

        [Test]
        public void GenerateInformationModelForSeccondTime([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            _loggerListenerMock.Setup(x => x.Info(LoggingText.GenerateInformationModelSuccess));

            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp)).Returns(_srcDir);
            var modelName = System.IO.Path.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetFileName(inputParams.ElementAtOrDefault(3))).Returns(modelName);
            var args = inputParams.ElementAtOrDefault(3) + " " + modelName;
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.NodsetCompiler, _srcDir, args)).Returns(true);
            _mockFileSystem.Setup(x => x.DirectoryExists(System.IO.Path.Combine(_srcDir, DirectoryName.InformationModels))).Returns(true);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);
            _mockFileSystem.Setup(x => x.GetFileNameWithoutExtension(inputParams.ElementAtOrDefault(3))).Returns(modelName);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelSuccess, inputParams.ElementAtOrDefault(3)), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
            _loggerListenerMock.Verify(x => x.Info(LoggingText.GenerateInformationModelSuccess), Times.Once);
            _mockFileSystem.Verify(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp), Times.Once);
            _mockFileSystem.Verify(x => x.CreateDirectory(System.IO.Path.Combine(_srcDir, DirectoryName.InformationModels)), Times.Never);
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
            _mockFileSystem.Setup(x => x.CallExecutable(Constants.ExecutableName.NodsetCompiler, _srcDir, args)).Returns(false);

            var calculatedModelFilePath = System.IO.Path.Combine(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.CombinePaths(inputParams.ElementAtOrDefault(1), DirectoryName.Models, inputParams.ElementAtOrDefault(3))).Returns(calculatedModelFilePath);
            _mockFileSystem.Setup(x => x.FileExists(calculatedModelFilePath)).Returns(true);

            var modelExtension = System.IO.Path.GetExtension(inputParams.ElementAtOrDefault(3));
            _mockFileSystem.Setup(x => x.GetExtension(inputParams.ElementAtOrDefault(3))).Returns(modelExtension);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
            var firstMessageLine = commandResult.OutputMessages.FirstOrDefault();
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailure, inputParams.ElementAtOrDefault(3)), firstMessageLine.Key);
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
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureMissingModel, inputParams.ElementAtOrDefault(3), calculatedModelFilePath), firstMessageLine.Key);
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
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureInvalidModel, modelName), firstMessageLine.Key);
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
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, inputParams.ElementAtOrDefault(0)), firstMessageLine.Key);
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
            Assert.AreEqual(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, inputParams.ElementAtOrDefault(2)), firstMessageLine.Key);
            Assert.AreEqual(string.Empty, firstMessageLine.Value);
        }

        //[Test]
        //public void CallExecuteOnProvidedInvalidCommand([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        //{
        //    // Arrange
        //    var mockCommandResult = new CommandResult(true, new MessageLines());
        //    var mockCommand = new Mock<ICommand<GenerateStrategy>>();
        //    mockCommand.Setup(command => command.Execute(It.IsAny<IEnumerable<string>>())).Returns(mockCommandResult);

        //    var mockInvalidCommandResult = new CommandResult(false, new MessageLines());
        //    var mockInvalidCommand = new Mock<ICommand<GenerateStrategy>>();
        //    mockInvalidCommand.Setup(command => command.Execute(It.IsAny<IEnumerable<string>>())).Returns(mockInvalidCommandResult);

        //    _mockFactory.Setup(facory => facory.GetCommand(GenerateInformationModelCommandName.InformationModel)).Returns(mockCommand.Object);
        //    _mockFactory.Setup(facory => facory.GetCommand(It.Is<string>(s=>s != GenerateInformationModelCommandName.InformationModel))).Returns(mockInvalidCommand.Object);

        //    // Act
        //    var commandResult = _strategy.Execute(inputParams);

        //    // Assert
        //    Assert.IsFalse(commandResult.Sucsess);
        //    Assert.IsNotNull(commandResult.OutputMessages);
        //}
    }
}