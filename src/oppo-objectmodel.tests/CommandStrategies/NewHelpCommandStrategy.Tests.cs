using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class NewHelpCommandStrategyTests
    {     
        [Test]
        public void NewHelpCommandStrategy_Should_ImplementICommandOfNewStrategy()
        {
            // Arrange

            // Act
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, new[] { new KeyValuePair<string, string>(string.Empty, string.Empty) });

            // Assert
            Assert.IsInstanceOf<ICommand<NewStrategy>>(objectUnderTest);
        }

        [Test]
        public void NewHelpCommandStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, new[] { new KeyValuePair<string, string>(string.Empty, string.Empty) });

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void NewHelpCommandStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, new[] { new KeyValuePair<string, string>(string.Empty, string.Empty) });

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        [Test]
        public void NewHelpCommandStrategy_Should_WriteHelpText()
        {
            // Arrange
            var inputParamsMock = new string[0];
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, new[] { new KeyValuePair<string, string>(string.Empty, string.Empty) });
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OppoHelpForNewCommandCalled));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParamsMock);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.IsNotNull(result.OutputMessages);
            loggerListenerMock.Verify(x => x.Info(LoggingText.OppoHelpForNewCommandCalled), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}