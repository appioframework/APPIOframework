using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.BuildCommands;
using Appio.Resources.text.output;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
    public class BuildStrategyTests
    {
        private Mock<ICommandFactory<BuildStrategy>> _factoryMock;
        private BuildStrategy _objectUnderTest;

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _factoryMock = new Mock<ICommandFactory<BuildStrategy>>();
            _objectUnderTest = new BuildStrategy(_factoryMock.Object);
        }

        [Test]
        public void BuildStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(_objectUnderTest);
        }

        [Test]
        public void BuildStrategy_Should_ExecuteCommand()
        {
            // Arrange
            const string commandName = "any-name";
            const string commandArgs = "any-args";
            var inputParams = new[] { commandName, commandArgs };

            var expectedResult = new CommandResult(true, new MessageLines());

            var commandMock = new Mock<ICommand<BuildStrategy>>();
            commandMock.Setup(x => x.Execute(It.IsAny<string[]>())).Returns(expectedResult);

            _factoryMock.Setup(x => x.GetCommand(commandName)).Returns(commandMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            commandMock.Verify(x => x.Execute(It.Is<string[]>(p => p.Length == 1 && p[0] == commandArgs)), Times.Once);
            Assert.AreEqual(expectedResult, result);
        }
        
        [Test]
        public void BuildStrategy_Should_ReturnCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.BuildCommand, helpText);
        }

        [Test]
        public void BuildStrategy_Should_CorrectCommandName()
        {
            // Arrange

            // Act
            var commandName = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Build);
        }
    }
}