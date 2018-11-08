using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class NewStrategyTests
    {
        public struct NewStrategyFixture
        {
            public NewStrategyFixture(string[] input, string result)
            {
                Input = input;
                Result = result;
            }

            public string[] Input;
            public string Result;
        }

        private static NewStrategyFixture[] Data()
        {
            return new[]
            {
                new NewStrategyFixture(new[]{ "sln", "-n", "anyName" }, Constants.CommandResults.Success),
                new NewStrategyFixture(new[]{ "abc", "-n", "my-value" }, Constants.CommandResults.Failure),
                new NewStrategyFixture(new[]{ "sln", "--name", "anyName" }, Constants.CommandResults.Success),
            };
        }
        
        [Test]
        public void NewStrategy_ShouldImplement_ICommandStrategy()
        {
            // Arrange
            var commandFactoryMock = new Mock<INewCommandStrategyFactory>();

            // Act
            var objectUnderTest = new NewStrategy(commandFactoryMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommandStrategy>(objectUnderTest);
        }

        [Test]
        public void NewStrategy_ShouldCall_ChildCommand([ValueSource(nameof(Data))] NewStrategyFixture data)
        {
            // Arrange
            var commandFactoryMock = new Mock<INewCommandStrategyFactory>();
            var newSlnCommandMock = new Mock<INewCommandStrategy>();
            var objectUnderTest = new NewStrategy(commandFactoryMock.Object);
            commandFactoryMock.Setup(x => x.GetStrategy(data.Input.First())).Returns(newSlnCommandMock.Object);
            newSlnCommandMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>())).Returns(data.Result);

            // Act
            var result = objectUnderTest.Execute(data.Input);

            // Assert
            Assert.AreEqual(data.Result, result);
        }

        [Test]
        public void ShouldReturnEmptyHelpText()
        {
            // Arrange
            var commandFactoryMock = new Mock<INewCommandStrategyFactory>();
            var newStrategy = new NewStrategy(commandFactoryMock.Object);

            // Act
            var helpText = newStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.NewCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var commandFactoryMock = new Mock<INewCommandStrategyFactory>();
            var newStrategy = new NewStrategy(commandFactoryMock.Object);

            // Act
            var commandName = newStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.New);
        }
    }
}