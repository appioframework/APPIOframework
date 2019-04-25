using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using System.Linq;

namespace Oppo.ObjectModel.Tests.CommandStrategies
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
                new NewStrategyFixture(new string[0], Constants.CommandResults.Success),
                new NewStrategyFixture(new[]{ "sln", "-n", "anyName" }, Constants.CommandResults.Success),
                new NewStrategyFixture(new[]{ "abc", "-n", "my-value" }, Constants.CommandResults.Failure),
                new NewStrategyFixture(new[]{ "sln", "--name", "anyName" }, Constants.CommandResults.Success),
            };
        }
        
        [Test]
        public void NewStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var factoryMock = new Mock<ICommandFactory<NewStrategy>>();

            // Act
            var objectUnderTest = new NewStrategy(factoryMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(objectUnderTest);
        }

        [Test]
        public void NewStrategy_Should_ExecuteChildCommand([ValueSource(nameof(Data))] NewStrategyFixture data)
        {
            // Arrange
            var commandMock = new Mock<ICommand<NewStrategy>>();
            commandMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>())).Returns(new CommandResult(true, new MessageLines {{data.Result, string.Empty}}));
            var factoryMock = new Mock<ICommandFactory<NewStrategy>>();
            factoryMock.Setup(x => x.GetCommand(data.Input.FirstOrDefault())).Returns(commandMock.Object);
            var objectUnderTest = new NewStrategy(factoryMock.Object);

            // Act
            var result = objectUnderTest.Execute(data.Input);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(data.Result, result.OutputMessages.First().Key);
        }

        [Test]
        public void ShouldReturnEmptyHelpText()
        {
            // Arrange
            var factoryMock = new Mock<ICommandFactory<NewStrategy>>();
            var newStrategy = new NewStrategy(factoryMock.Object);

            // Act
            var helpText = newStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.NewCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var factoryMock = new Mock<ICommandFactory<NewStrategy>>();
            var newStrategy = new NewStrategy(factoryMock.Object);

            // Act
            var commandName = newStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.New);
        }
    }
}