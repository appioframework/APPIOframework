using NUnit.Framework;
using Moq;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.Tests
{
    public class NewCommandStrategyFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void NewCommandStrategyFactory_ShouldImplement_INewCommandStrategyFactory()
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();

            // Act
            var objectUnderTest = new NewCommandStrategyFactory(mockFileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<INewCommandStrategyFactory>(objectUnderTest);
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
        public void NewCommandStrategyFactory_ShouldReturn_CommandNotExistentStrategy(string commandName)
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();
            var factory = new NewCommandStrategyFactory(mockFileSystemMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<NewCommandNotExistentStrategy>(strategy);
        }

        [Test]
        public void NewCommandStrategyFactory_ShouldReturn_NewSlnStrategy()
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();
            const string commandName = Constants.NewCommandName.Sln;
            var factory = new NewCommandStrategyFactory(mockFileSystemMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<NewSlnCommandStrategy>(strategy);
        }
    }
}