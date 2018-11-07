using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;

namespace Oppo.ObjectModel.Tests
{
    public class BuildCommandNotExistentStrategyTests
    {      

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BuildCommandNotExistentStrategy_Should_Excecute_Failure()
        {
            // Arrange
            var strategy = new BuildCommandNotExistentStrategy();

            // Act
            var strategyResult = strategy.Execute(new string[] {});

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
        }
    }
}