using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class HelpStrategyTests
    {     
        [Test]
        public void HelpStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange

            // Act
            var objectUnderTest = new HelpStrategy(string.Empty);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(objectUnderTest);
        }

        [Test]
        public void HelpStrategy_Should_WriteHelpText()
        {
            // Arrange
            const string commandName = "any-name";
            const string commandHelpText = "any help text \r\n maybe with multiple lines ...";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OppoHelpCalled));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var commandMock = new Mock<ICommand<ObjectModel>>();
            commandMock.Setup(x => x.Name).Returns(commandName);
            commandMock.Setup(x => x.GetHelpText()).Returns(commandHelpText);

            var factoryMock = new Mock<ICommandFactory<ObjectModel>>();
            factoryMock.Setup(x => x.Commands).Returns(new[] {commandMock.Object});

            var helpStrategy = new HelpStrategy(string.Empty);
            helpStrategy.CommandFactory = factoryMock.Object;

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.AreEqual(string.Empty, strategyResult.Message);
            Assert.IsNotNull(strategyResult.OutputText);
            Assert.IsTrue(strategyResult.OutputText.ContainsKey(Resources.text.help.HelpTextValues.HelpStartCommand));
            Assert.IsTrue(strategyResult.OutputText.ContainsValue(commandHelpText));
            Assert.IsTrue(strategyResult.OutputText.ContainsKey(Resources.text.help.HelpTextValues.HelpEndCommand));
            loggerListenerMock.Verify(x => x.Info(LoggingText.OppoHelpCalled), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void HelpStrategy_Should_WriteSparseHelpTextIfNoCommandFactoryIsProvided()
        {
            // Arrange
            var helpStrategy = new HelpStrategy(string.Empty);

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.AreEqual(string.Empty, strategyResult.Message);
            Assert.IsNotNull(strategyResult.OutputText);
            Assert.IsTrue(strategyResult.OutputText.ContainsKey(Resources.text.help.HelpTextValues.HelpStartCommand));
            Assert.IsTrue(strategyResult.OutputText.ContainsKey(Resources.text.help.HelpTextValues.HelpEndCommand));
        }

        [Test]
        public void ShouldReturnHelpText()
        {
            // Arrange
            var helpStrategy = new HelpStrategy(string.Empty);

            // Act
            var helpText = helpStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.HelpCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var helpStrategy = new HelpStrategy(string.Empty);

            // Act
            var commandName = helpStrategy.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }
    }
}