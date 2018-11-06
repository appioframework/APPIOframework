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
        public struct NewStrategyTestsFixture
        {
            public NewStrategyTestsFixture(string[] input, string result)
            {
                Input = input;
                Result = result;
            }

            public string[] Input;
            public string Result;
        }

        private static NewStrategyTestsFixture[] Data()
        {
            return new[]
            {
                new NewStrategyTestsFixture(new[]{ "sln", "-n", "anyName" }, Constants.CommandResults.Success),
                new NewStrategyTestsFixture(new[]{ "abc", "-n", "my-value" }, Constants.CommandResults.Failure),
                new NewStrategyTestsFixture(new[]{ "sln", "--name", "anyName" }, Constants.CommandResults.Success),
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
        public void NewStrategy_ShouldCall_ChildCommand([ValueSource(nameof(Data))] NewStrategyTestsFixture data)
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
    }
}