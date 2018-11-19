using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class BuildHelpStrategyTests
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
        public void BuildHelpStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            
            // Act
            var objectUnderTest = new BuildHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Assert
            Assert.IsInstanceOf<ICommand<BuildStrategy>>(objectUnderTest);
        }

        [Test]
        public void BuildHelpStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var objectUnderTest = new BuildHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void BuildHelpStrategy_Should_ProvideExactHelpText()
        {
            // Arrange
            var objectUnderTest = new BuildHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.BuildHelpArgumentCommandDescription, helpText);
        }

        [Test]
        public void BuildHelpStrategy_Should_ExecuteSuccess([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var mockCommand = new Mock<ICommand<BuildStrategy>>();
            mockCommand.Setup(x => x.Name).Returns("buildCommandName");
            mockCommand.Setup(x => x.GetHelpText()).Returns("buildCommandHelp");
            var mockBuildCommandFactory = new Mock<ICommandFactory<BuildStrategy>>();
            mockBuildCommandFactory.Setup(x => x.Commands).Returns(new ICommand<BuildStrategy>[] { mockCommand.Object});
            var strategy = new BuildHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });
            strategy.CommandFactory = mockBuildCommandFactory.Object;

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.OppoHelpForBuildCommandCalled));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var strategyResult = strategy.Execute(new string[] {});

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.IsNotNull(strategyResult.OutputMessages);
            loggerListenerMock.Verify(x => x.Info(LoggingText.OppoHelpForBuildCommandCalled), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }
    }
}