using NUnit.Framework;
using Oppo.ObjectModel;
using Moq;
using System.Collections.Generic;
using System;

namespace Oppo.ObjectModel.Tests
{
    public class ObjectModelTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldGetValidInputParams()
        {
            // Arrange
            var strategyMock = new Mock<ICommandStrategy>();           
            var mockCommandStrategyFactory = new Mock<ICommandStrategyFactory>();
            var inputParams = new List<string>(){"sln", "new", "-n", "testslns"};
            var slnInputParams = new List<string>(){"new", "-n", "testslns"};
            var objectModel = new ObjectModel(mockCommandStrategyFactory.Object);
            mockCommandStrategyFactory.Setup(factory => factory.GetStrategy("sln")).Returns(strategyMock.Object);
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