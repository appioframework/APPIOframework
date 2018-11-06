using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;

namespace Oppo.ObjectModel.Tests
{
    public class HelpStrategyTests
    {       
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy_Success()
        {
            // Arrange
            var helpStrategy = new HelpStrategy();

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });
            
            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);      
        }
    }
}