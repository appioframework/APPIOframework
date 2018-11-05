using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class SlnSrategyTests
    {
        private static string[][] InvalidInputs()
        {
            return new[] 
            { 
                new []{ "new", "", "" },
                new []{ "new" },
                new []{ "new", "-n" },
                new []{ "foo", "-n" },
                new string[]{ }
            };
        }

        private static string[][] ValidInputs()
        {
            return new[]
            {
                new []{ "new", "-n", "anyName" },
                new []{ "new", "-n", "ABC" },
                new []{ "new", "--name", "ABC" }
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy_Success([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var mockSlnCommandsStrategyFactory = new Mock<ISlnCommandStrategyFactory>();
            var slnStretegy = new SlnStrategy(mockSlnCommandsStrategyFactory.Object);
            var mockSlnNewCommandStrategy = new Mock<ISlnCommandStrategy>();
            mockSlnCommandsStrategyFactory.Setup(factory => factory.GetStrategy(inputParams.FirstOrDefault())).Returns(mockSlnNewCommandStrategy.Object);
            mockSlnNewCommandStrategy.Setup(slnNewCommandStrategy => slnNewCommandStrategy.Execute(It.IsAny<IEnumerable<string>>())).Returns(Constants.CommandResults.Success);

            // Act
            var strategyResult = slnStretegy.Execute(inputParams);
            
            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);      
        }

        [Test]
        public void ShouldExcecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var mockSlnCommandsStrategyFactory = new Mock<ISlnCommandStrategyFactory>();
            var slnStretegy = new SlnStrategy(mockSlnCommandsStrategyFactory.Object);
            var mockSlnNewCommandStrategy = new Mock<ISlnCommandStrategy>();
            mockSlnCommandsStrategyFactory.Setup(factory => factory.GetStrategy(inputParams.FirstOrDefault())).Returns(mockSlnNewCommandStrategy.Object);
            mockSlnNewCommandStrategy.Setup(slnNewCommandStrategy => slnNewCommandStrategy.Execute(It.IsAny<IEnumerable<string>>())).Returns(Constants.CommandResults.Failure);

            // Act
            var strategyResult = slnStretegy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
        }
    }
}