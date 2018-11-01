using NUnit.Framework;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;


namespace Oppo.ObjectModel.Tests
{
    public class CommandNotExistentStrategyTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy()
        {
            // Arrange
            var strategy = new CommandNotExistentStrategy();

            // Act
            var strategyResult = strategy.Execute(new List<string>());
            
            // AssertP
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);             
        }
    }
}