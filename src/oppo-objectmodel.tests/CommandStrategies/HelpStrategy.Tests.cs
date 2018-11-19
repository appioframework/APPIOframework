using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using Oppo.Resources.text.logging;
using System.Collections.Generic;
using System.Linq;

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
            Assert.IsNotNull(strategyResult.OutputMessages);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
           
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(Resources.text.help.HelpTextValues.HelpStartCommand, string.Empty)));
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(commandName, commandHelpText)));
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(Resources.text.help.HelpTextValues.HelpEndCommand, string.Empty)));
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
            Assert.IsNotNull(strategyResult.OutputMessages);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(Resources.text.help.HelpTextValues.HelpStartCommand, string.Empty)));
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(Resources.text.help.HelpTextValues.HelpEndCommand, string.Empty)));
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