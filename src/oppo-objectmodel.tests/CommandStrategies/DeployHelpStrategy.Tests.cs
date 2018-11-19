using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.DeployCommands;
using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class DeployHelpStrategyTests
    {
        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new []{"-h"},
                new []{"--help"},
            };
        }
        
        [Test]
        public void DeployHelpStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            
            // Act
            var objectUnderTest = new DeployHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Assert
            Assert.IsInstanceOf<ICommand<DeployStrategy>>(objectUnderTest);
        }

        [Test]
        public void DeployHelpStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var objectUnderTest = new DeployHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void DeployHelpStrategy_Should_ProvideExactHelpText()
        {
            // Arrange
            var objectUnderTest = new DeployHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.DeployHelpArgumentCommandDescription, helpText);
        }

        [Test]
        public void DeployHelpStrategy_Should_ExecuteSuccess([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var mockCommand = new Mock<ICommand<DeployStrategy>>();
            mockCommand.Setup(x => x.Name).Returns("deployCommandName");
            mockCommand.Setup(x => x.GetHelpText()).Returns("deployCommandHelp");
            var mockDeployCommandFactory = new Mock<ICommandFactory<DeployStrategy>>();
            mockDeployCommandFactory.Setup(x => x.Commands).Returns(new ICommand<DeployStrategy>[] { mockCommand.Object });
            var strategy = new DeployHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });
            strategy.CommandFactory = mockDeployCommandFactory.Object;

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OppoHelpForBuildCommandCalled));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = strategy.Execute(new string[] { });

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.IsNotNull(strategyResult.OutputMessages);
            loggerListenerMock.Verify(x => x.Info(LoggingText.OppoHelpForDeployCommandCalled), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}