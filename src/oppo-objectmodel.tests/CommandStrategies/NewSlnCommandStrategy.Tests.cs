using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class NewSlnCommandStrategyTests
    {
        private static string[][] ValidInputs()
        {
            return new[]
            {
                new[] {"-n", "anyName"},
                new[] {"-n", "ABC"},
                new[] {"--name", "ABC"}
            };
        }

        private static object[] InvalidInputsFirstPart =
        {
            new object[] {new[] {"-n", ""}, string.Format(OutputText.ParameterValueMissing, "-n")},
            //new[] {"-n", "ab/yx"},
            //new[] {"-n", "ab\\yx"},
            new object[] {new[] {"-N", "ab/yx"}, string.Format(OutputText.UnknownParameterProvided, "-N", "'-n' or '--name'")},
            new object[] {new[] {"", ""}, string.Format(OutputText.UnknownParameterProvided, string.Empty, "'-n' or '--name'")},
            new object[] {new[] {""}, string.Format(OutputText.UnknownParameterProvided, string.Empty, "'-n' or '--name'")},
            new object[] {new[] {"-n"}, string.Format(OutputText.ParameterValueMissing, "-n")},
            new object[] {new string[] { }, string.Format(OutputText.MissingRequiredParameter, "'-n' or '--name'")}
        };

        private static string[][] InvalidInputsSeccondPart()
        {
            return new[]
            {
                //new[] {"-n", ""},
                new[] {"-n", "ab/yx"},
                new[] {"-n", "ab\\yx"},
                //new[] {"-N", "ab/yx"},
                //new[] {"", ""},
                //new[] {""},
                //new[] {"-n"},
                //new string[] { }
            };
        }

        [Test]
        public void NewSlnCommandStrategy_Should_ImplementICommandOfNewStrategy()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();

            // Act
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<NewStrategy>>(objectUnderTest);
        }

        [Test]
        public void NewSlnCommandStrategy_Should_CreateSlnAndProjectRelevantFiles([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var slnName = inputParams.Skip(1).First();
            var slnFileName = $"{slnName}{Constants.FileExtension.OppoSln}";
            var infoWrittenOut = false;
            var loggerListenerMock = new Mock<ILoggerListener>();            
            loggerListenerMock.Setup(listener => listener.Info(It.IsAny<string>())).Callback(delegate { infoWrittenOut = true; });
            SetupOppoLogger(loggerListenerMock.Object);
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(infoWrittenOut);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(string.Format(OutputText.NewSlnCommandSuccess, slnName), result.OutputMessages.First().Key);
            fileSystemMock.Verify(x => x.CreateFile(slnFileName, It.IsAny<string>()), Times.Once);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName), Times.Once);
            RemoveLoggerListener(loggerListenerMock.Object);
        }

        [TestCaseSource(nameof(InvalidInputsFirstPart))]
        public void NewSlnCommandStrategy_Should_IgnoreInputFirstPart(string[] inputParams, string expectedError)
        {
            // Arrange
            var invalidCharsMock = new[] { '/', '\\' };
            var loggerListenerMock = new Mock<ILoggerListener>();
            var warnWrittenOut = false;
            loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
            SetupOppoLogger(loggerListenerMock.Object);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidCharsMock);
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(expectedError, result.OutputMessages.First().Key);
            fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName), Times.Never);
            RemoveLoggerListener(loggerListenerMock.Object);
        }

        [Test]
        public void NewSlnCommandStrategy_Should_IgnoreInput([ValueSource(nameof(InvalidInputsSeccondPart))] string[] inputParams)
        {
            // Arrange
            var slnName = inputParams.ElementAt(1);
            var invalidCharsMock = new[] { '/', '\\' };
            var loggerListenerMock = new Mock<ILoggerListener>();
            var warnWrittenOut = false;
            loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
            SetupOppoLogger(loggerListenerMock.Object);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidCharsMock);
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.NewSlnCommandFailure, slnName), result.OutputMessages.First().Key);
            fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName), Times.Never);
            RemoveLoggerListener(loggerListenerMock.Object);
        }

        [Test]
        public void NewSlnCommandStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.NewCommandArguments.Sln, commandName);
        }

        [Test]
        public void NewSlnCommandStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        private static void SetupOppoLogger(ILoggerListener loggerListener)
        {
            OppoLogger.RegisterListener(loggerListener);
        }

        private static void RemoveLoggerListener(ILoggerListener loggerListener)
        {
            OppoLogger.RemoveListener(loggerListener);
        }
    }
}