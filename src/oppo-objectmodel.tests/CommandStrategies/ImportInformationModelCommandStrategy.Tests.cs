using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class ImportInformationModelCommandStrategyTests
    {
        private static string[][] ValidInputs()
        {
            return new[]
            {
                new[] {"myApp", "-p", "model.xml"},
                new[] {"myApp", "--path", "model.xml"}
            };
        }

        private Mock<IFileSystem> _fileSystemMock;
        private ImportInformationModelCommandStrategy _objectUnderTest;

        [SetUp]
        public void SetupObjectUnderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new ImportInformationModelCommandStrategy(_fileSystemMock.Object);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImplementICommandOfImportStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<ImportStrategy>>(_objectUnderTest);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var commandName = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.ImportInformationModelCommandName.IModel, commandName);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        [Test]
        public void ImportInformationModelCommandStrategy_Should_ImportModel([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var infoWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAt(0)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            var modelFilePath = $"{inputParams.ElementAt(2)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelFilePath))).Callback(delegate { infoWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);         

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(infoWrittenOut);
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandSuccess, inputParams.ElementAt(2)), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}