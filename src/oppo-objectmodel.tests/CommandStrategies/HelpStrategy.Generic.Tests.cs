using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class HelpStrategyTests
    {
        [Test]
        public void HelpStrategy_Should_ImplementICommandOfAnyType()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName       = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine  = { { "any-other-text", "" } },
                LogMessage        = "any-message",
                HelpText          = "any-text",
            };

            // Act
            var objectUnderTest = new HelpStrategy<object>(helpData);

            // Assert
            Assert.IsInstanceOf<ICommand<object>>(objectUnderTest);
        }

        [Test]
        public void HelpStrategy_Should_WriteHelpText()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine = { { "", "any-other-text" } },
                LogMessage = "any-message",
                HelpText = "any-text",
            };

            const string commandName = "any-name-2";
            const string commandHelpText = "any help text \r\n maybe with multiple lines ...";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info("any-message"));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            var commandMock = new Mock<ICommand<ObjectModel>>();
            commandMock.Setup(x => x.Name).Returns(commandName);
            commandMock.Setup(x => x.GetHelpText()).Returns(commandHelpText);

            var factoryMock = new Mock<ICommandFactory<ObjectModel>>();
            factoryMock.Setup(x => x.Commands).Returns(new[] {commandMock.Object});

            var helpStrategy = new HelpStrategy<object>(helpData);
            helpStrategy.CommandFactory = factoryMock.Object;

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.IsNotNull(strategyResult.OutputMessages);
           
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>("any-text", string.Empty)));
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(commandName, commandHelpText)));
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(string.Empty, "any-other-text")));

            loggerListenerMock.Verify(x => x.Info("any-message"), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void HelpStrategy_Should_WriteSparseHelpTextIfNoCommandFactoryIsProvided()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine = { { "", "any-other-text" } },
                LogMessage = "any-message",
                HelpText = "any-text",
            };

            var helpStrategy = new HelpStrategy<object>(helpData);

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.IsNotNull(strategyResult.OutputMessages);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>("any-text", string.Empty)));
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(string.Empty, "any-other-text")));
        }

        [Test]
        public void ShouldReturnHelpText()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine = { { "any-other-text", "" } },
                LogMessage = "any-message",
                HelpText = "any-text",
            };

            var helpStrategy = new HelpStrategy<object>(helpData);

            // Act
            var helpText = helpStrategy.GetHelpText();

            // Assert
            Assert.AreEqual("any-text", helpText);
        }
        
        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine = { { "any-other-text", "" } },
                LogMessage = "any-message",
                HelpText = "any-text",
            };

            var helpStrategy = new HelpStrategy<object>(helpData);

            // Act
            var commandName = helpStrategy.Name;

            // Assert
            Assert.AreEqual("any-name", commandName);
        }
    }
}