using NUnit.Framework;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using Moq;

namespace Oppo.ObjectModel.Tests
{
    public class SlnCommandStrategyFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase( null )]
        [TestCase( "" )]
        [TestCase( "iioh4ohf" )]
        [TestCase( "abc" )]
        [TestCase( "5zh" )]
        [TestCase( "@/40&" )]
        [TestCase( "New" )]
        [TestCase( "CREATE" )]        
    
        public void SlnCommandStrategyFactory_ShouldReturn_CommandNotExistentStrategy( string commandName )
        {
            // Arrange
            var factory = new SlnCommandStrategyFactory();

            // Act
            var strategy = factory.GetStrategy( commandName );
            
            // Assert
            Assert.IsInstanceOf< SlnCommandNotExistentStrategy >( strategy );
        }

        [Test]
        public void SlnCommandStrategyFactory_ShouldReturn_SlnNewStrategy()
        {
            // Arrange
            const string commandName = Constants.SlnCommandName.New;
            var factory = new SlnCommandStrategyFactory();

            // Act
            var strategy = factory.GetStrategy( commandName );
            
            // Assert
            Assert.IsInstanceOf< SlnNewCommandStrategy >( strategy );
        }
    }
}