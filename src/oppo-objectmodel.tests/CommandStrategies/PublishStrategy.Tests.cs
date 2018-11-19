using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class PublishStrategyTests
    {
        private Mock<ICommandFactory<PublishStrategy>> _factoryMock;
        private PublishStrategy _objectUnderTest;

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _factoryMock = new Mock<ICommandFactory<PublishStrategy>>();
            _objectUnderTest = new PublishStrategy(_factoryMock.Object);
        }

        [Test]
        public void PublishStrategy_Should_ImplementICommandOfPublishStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(_objectUnderTest);
        }

        [Test]
        public void PublishStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var commandName = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.CommandName.Publish, commandName);
        }

        [Test]
        public void PublishStrategy_Should_ProvideSomeHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

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
                Returns(new CommandResult(true, new MessageLines() { { expectedCommandResult, string.Empty } }));

            _factoryMock.Setup(x => x.GetCommand(commandName)).Returns(commandMock.Object);

            // Act
            var result = _objectUnderTest.Execute(commandInputParamsMock);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(expectedCommandResult, result.OutputMessages.First().Key);
        }
    }
}
