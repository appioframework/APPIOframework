using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.DeployCommands;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
    public class DeployStrategyTests
    {
        private static string[][] ValidData()
        {
            return new[]
            {
                new string[0],
                new[] {"--any-name", "any", "sub", "parameters"}
            };
        }

        [Test]
        public void DeployStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<DeployStrategy>>();

            // Act
            var objectUnderTest = new DeployStrategy(commandFactoryMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(objectUnderTest);
        }

        [Test]
        public void DeployStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<DeployStrategy>>();
            var objectUnderTest = new DeployStrategy(commandFactoryMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.CommandName.Deploy, commandName);
        }

        [Test]
        public void DeployStrategy_Should_ProvideSomeHelpText()
        {
            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<DeployStrategy>>();
            var objectUnderTest = new DeployStrategy(commandFactoryMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.IsNotEmpty(helpText);
        }

        [Test]
        public void DeployStrategy_Should_ExecuteCorrectSubCommandByName([ValueSource(nameof(ValidData))] string[] inputParams)
        {
            // Arrange
            const string expectedCommandResult = "any-result";

            var commandMock = new Mock<ICommand<DeployStrategy>>();
            commandMock.Setup(x => x.Execute(It.Is<IEnumerable<string>>(p => p.SequenceEqual(inputParams.Skip(1).ToArray())))).
                Returns(new CommandResult(true, new MessageLines() { { expectedCommandResult, string.Empty } }));

            var commandFactoryMock = new Mock<ICommandFactory<DeployStrategy>>();
            commandFactoryMock.Setup(x => x.GetCommand(inputParams.FirstOrDefault())).Returns(commandMock.Object);
            var objectUnderTest = new DeployStrategy(commandFactoryMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(expectedCommandResult, result.OutputMessages.First().Key);
        }
    }
}