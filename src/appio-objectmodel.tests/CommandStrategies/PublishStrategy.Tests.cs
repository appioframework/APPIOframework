using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.PublishCommands;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
    public class PublishStrategyTests
    {
        private Mock<ICommandFactory<PublishStrategy>> _factoryMock;
        private PublishStrategy _objectUnderTest;

        private static string[][] ValidData()
        {
            return new[]
            {
                new string[0],
                new[] {"--any-name", "any", "sub", "parameters"}
            };
        }

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
        public void PublishStrategy_Should_ExecuteCorrectSubCommandByName([ValueSource(nameof(ValidData))] string[] inputParams)
        {
            // Arrange
            const string expectedCommandResult = "any-result";

            var commandMock = new Mock<ICommand<PublishStrategy>>();
            commandMock.Setup(x => x.Execute(It.Is<IEnumerable<string>>(p => p.SequenceEqual(inputParams.Skip(1).ToArray())))).
                Returns(new CommandResult(true, new MessageLines { { expectedCommandResult, string.Empty } }));

            _factoryMock.Setup(x => x.GetCommand(inputParams.FirstOrDefault())).Returns(commandMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(expectedCommandResult, result.OutputMessages.First().Key);
        }
    }
}
