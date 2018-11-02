using NUnit.Framework;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies;
using Moq;

namespace Oppo.ObjectModel.Tests
{
    public class CommandStrategyFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_CommandNotExistentStrategy()
        {
            // Arrange
            var factory = new CommandStrategyFactory();

            // Act
            var strategy = factory.GetStrategy( null );
            
            // Assert
            Assert.IsInstanceOf< CommandNotExistentStrategy >( strategy );
        }
    }
}