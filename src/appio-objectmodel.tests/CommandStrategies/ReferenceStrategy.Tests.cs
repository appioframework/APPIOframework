using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.ReferenceCommands;
using System.Collections.Generic;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
    class ReferenceStrategyTests
    {
        private Mock<ICommandFactory<ReferenceStrategy>> _factoryMock;
        private ReferenceStrategy _objectUnderTest;

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _factoryMock = new Mock<ICommandFactory<ReferenceStrategy>>();
            _objectUnderTest = new ReferenceStrategy(_factoryMock.Object);
        }
        [Test]
        public void ReferenceStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(_objectUnderTest);
        }

        [Test]
        public void ReferenceStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var name = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(Constants.CommandName.Reference, name);
        }

        [Test]
        public void ReferenceStrategy_Should_ProvideSomeHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.ReferenceCommand, helpText);
        }

        [Test]
        public void ReferenceStrategy_Should_ExecuteCommand()
        {
            // Arrange
            var inputParams = new[] { "--any-param", "any-value" };
            var commandResultMock = new CommandResult(true, new MessageLines() { { "any-message", string.Empty } });

            var commandMock = new Mock<ICommand<ReferenceStrategy>>();
            commandMock.Setup(x => x.Execute(It.IsAny<string[]>())).Returns(commandResultMock);

            _factoryMock.Setup(x => x.GetCommand("--any-param")).Returns(commandMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.AreEqual(commandResultMock, result);
        }
    }
}

