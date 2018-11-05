using NUnit.Framework;
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

        [TestCase(null)]
        [TestCase("")]
        [TestCase("iioh4ohf")]
        [TestCase("abc")]
        [TestCase("5zh")]
        [TestCase("@/40&")]
        [TestCase("New")]
        [TestCase("CREATE")]
        [TestCase("Build")]
        [TestCase("built")]

        public void SlnCommandStrategyFactory_ShouldReturn_CommandNotExistentStrategy(string commandName)
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();
            var factory = new SlnCommandStrategyFactory(mockFileSystemMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<SlnCommandNotExistentStrategy>(strategy);
        }

        [Test]
        public void SlnCommandStrategyFactory_ShouldReturn_SlnNewStrategy()
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();
            const string commandName = Constants.SlnCommandName.New;
            var factory = new SlnCommandStrategyFactory(mockFileSystemMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<SlnNewCommandStrategy>(strategy);
        }
    }
}