using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class ObjectModelTests
    {

        private static string[][] ValidInputs()
        {
            return new[]
            {
                new []{ "new", "sln", "-n", "anyName" },
                new []{ "new", "opcuaapp", "-n", "ABC" },
                new []{ "new", "sln", "--name", "ABC" },
                new string[]{ }
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldGetValidInputParams([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var strategyMock = new Mock<ICommandStrategy>();           
            var mockCommandStrategyFactory = new Mock<ICommandStrategyFactory>();

            var slnInputParams = inputParams.Skip(1);
            var objectModel = new ObjectModel(mockCommandStrategyFactory.Object);
            mockCommandStrategyFactory.Setup(factory => factory.GetStrategy(inputParams.FirstOrDefault())).Returns(strategyMock.Object);
            strategyMock.Setup(s=>s.Execute(slnInputParams)).Returns(Constants.CommandResults.Success);
            
            // Act
            var executionResult = objectModel.ExecuteCommand(inputParams);
                        
            // Assert
            Assert.AreEqual(executionResult, Constants.CommandResults.Success);
        }

        [Test]
        public void ShouldGetInvalidInputParams()
        {
            // Arrange
            var mockCommandStrategyFactory = new Mock<ICommandStrategyFactory>();
            List<string> inputParams = null;

            var objectModel = new ObjectModel(mockCommandStrategyFactory.Object);

            // Act
                                    
            // Assert
            Assert.Throws<ArgumentNullException>(() => objectModel.ExecuteCommand(inputParams));
        }
    }
}