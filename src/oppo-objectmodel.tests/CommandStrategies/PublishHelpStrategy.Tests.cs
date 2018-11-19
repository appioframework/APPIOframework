using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class PublishHelpStrategyTests
    {      
        [Test]
        public void PublishHelpStrategy_Should_ImplementICommandOfPublishStrategy()
        {
            // Arrange

            // Act
            var objectUnderTest = new PublishHelpStrategy(string.Empty, new MessageLines(){ { string.Empty, string.Empty } });

            // Assert
            Assert.IsInstanceOf<ICommand<PublishStrategy>>(objectUnderTest);
        }

        [Test]
        public void PublishHelpStrategy_Should_ProvideCorrectCommandName()
        {
            // Arrange
            var objectUnderTest = new PublishHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void PublishHelpStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var objectUnderTest = new PublishHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        [Test]
        public void PublishHelpStrategy_Should_WriteHelpText()
        {
            // Arrange
            var objectUnderTest = new PublishHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OpcuaappPublishHelpCalled));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = objectUnderTest.Execute(new string[0]);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.IsNotNull(result.OutputMessages);
            loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappPublishHelpCalled), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }   
}