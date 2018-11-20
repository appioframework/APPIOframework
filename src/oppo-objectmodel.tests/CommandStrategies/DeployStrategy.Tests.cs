﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.DeployCommands;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class DeployStrategyTests
    {
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
        public void DeployStrategy_Should_ExecuteCorrectSubCommandByName()
        {
            // Arrange
            const string commandName = "--any-name";
            const string expectedCommandResult = "any-result";
            var commandInputParamsMock = new[] {commandName, "any", "sub", "parameters"};
            var subCommandInputParamsMock = new[] {"any", "sub", "parameters"};

            var commandMock = new Mock<ICommand<DeployStrategy>>();
            commandMock.Setup(x => x.Execute(It.Is<IEnumerable<string>>(p => p.SequenceEqual(subCommandInputParamsMock)))).
                Returns(new CommandResult(true, new MessageLines() { { expectedCommandResult, string.Empty } }));

            var commandFactoryMock = new Mock<ICommandFactory<DeployStrategy>>();
            commandFactoryMock.Setup(x => x.GetCommand(commandName)).Returns(commandMock.Object);
            var objectUnderTest = new DeployStrategy(commandFactoryMock.Object);

            // Act
            var result = objectUnderTest.Execute(commandInputParamsMock);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(expectedCommandResult, result.OutputMessages.First().Key);
        }
    }
}