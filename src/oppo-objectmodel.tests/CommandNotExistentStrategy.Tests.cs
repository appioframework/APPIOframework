using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using System;
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
        public void ShouldExecuteStrategy()
        {
            // Arrange
            var strategy = new CommandNotExistentStrategy();

            // Act
            var strategyResult = strategy.Execute(new List<string>());
            
            // AssertP
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);             
        }

        [Test]
        public void ShouldThrowNotSupportedExceptionOnGetHelpText()
        {
            // Arrange
            var strategy = new CommandNotExistentStrategy();

            // Act            

            // Assert
            Assert.Throws<NotSupportedException>(() => strategy.GetHelpText());
        }
    }
}