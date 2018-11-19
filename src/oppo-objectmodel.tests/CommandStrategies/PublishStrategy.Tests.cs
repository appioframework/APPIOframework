using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class PublishStrategyTests
    {
        [Test]
        public void PublishStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<PublishStrategy>>();

            // Act
            var objectUnderTest = new PublishStrategy(commandFactoryMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(objectUnderTest);
        }

        [Test]
        public void PublishStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<PublishStrategy>>();
            var objectUnderTest = new PublishStrategy(commandFactoryMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.CommandName.Publish, commandName);
        }

        [Test]
        public void PublishStrategy_Should_ProvideSomeHelpText()
        {
            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<PublishStrategy>>();
            var objectUnderTest = new PublishStrategy(commandFactoryMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.IsNotEmpty(helpText);
        }

        [Test]
        public void PublishStrategy_Should_ExecuteCorrectSubCommandByName()
        {
            // Arrange
            const string commandName = "--any-name";
            const string expectedCommandResult = "any-result";
            var commandInputParamsMock = new[] {commandName, "any", "sub", "parameters"};
            var subCommandInputParamsMock = new[] {"any", "sub", "parameters"};

            var commandMock = new Mock<ICommand<PublishStrategy>>();
            commandMock.Setup(x => x.Execute(It.Is<IEnumerable<string>>(p => p.SequenceEqual(subCommandInputParamsMock)))).
                Returns(new CommandResult(true, new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(expectedCommandResult, string.Empty)
                }));

            var commandFactoryMock = new Mock<ICommandFactory<PublishStrategy>>();
            commandFactoryMock.Setup(x => x.GetCommand(commandName)).Returns(commandMock.Object);
            var objectUnderTest = new PublishStrategy(commandFactoryMock.Object);

            // Act
            var result = objectUnderTest.Execute(commandInputParamsMock);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(expectedCommandResult, result.OutputMessages.First().Key);
        }
    }
}
