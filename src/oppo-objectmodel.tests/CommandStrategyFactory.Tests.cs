using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;

namespace Oppo.ObjectModel.Tests
{
    public class CommandStrategyFactoryTests
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
        [TestCase("slN")]
        public void CommandStrategyFactory_ShouldReturn_CommandNotExistentStrategy(string commandName)
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<CommandNotExistentStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_SlnStrategy()
        {
            // Arrange
            const string commandName = Constants.CommandName.Sln;
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<SlnStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_HelloStrategy()
        {
            // Arrange
            const string commandName = Constants.CommandName.Hello;
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<HelloStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_BuildStrategy()
        {
            // Arrange
            const string commandName = Constants.CommandName.Build;
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<BuildStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_PublishStrategy()
        {
            // Arrange
            const string commandName = Constants.CommandName.Publish;
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<PublishStrategy>(strategy);
        }
    }
}