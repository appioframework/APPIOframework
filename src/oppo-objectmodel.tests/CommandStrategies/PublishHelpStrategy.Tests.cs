using System.Collections.Generic;
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
            var writerMock = new Mock<IWriter>();

            // Act
            var objectUnderTest = new PublishHelpStrategy(string.Empty, writerMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<PublishStrategy>>(objectUnderTest);
        }

        [Test]
        public void PublishHelpStrategy_Should_ProvideCorrectCommandName()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = new PublishHelpStrategy(string.Empty, writerMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void PublishHelpStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = new PublishHelpStrategy(string.Empty, writerMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        [Test]
        public void PublishHelpStrategy_Should_WriteHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = new PublishHelpStrategy(string.Empty, writerMock.Object);

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OpcuaappPublishHelpCalled));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = objectUnderTest.Execute(new string[0]);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(string.Empty, result.Message);
            writerMock.Verify(x=>x.WriteLine(It.IsAny<string>()));
            writerMock.Verify(x=>x.WriteLines(It.IsAny<Dictionary<string, string>>()));
            loggerListenerMock.Verify(x => x.Info(LoggingText.OpcuaappPublishHelpCalled), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }   
}