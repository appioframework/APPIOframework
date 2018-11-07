using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;

namespace Oppo.ObjectModel.Tests
{
    public class CommandStrategyFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [TestCase("iioh4ohf")]
        [TestCase("abc")]
        [TestCase("5zh")]
        [TestCase("@/40&")]
        [TestCase("New")]
        [TestCase("CREATE")]
        [TestCase("slN")]
        [TestCase("Help")]
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

        [Test]
        public void CommandStrategyFactory_ShouldReturn_HelpStrategy()
        {
            // Arrange
            const string commandName = Constants.CommandName.Help;
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<HelpStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_HelpStrategy_Short()
        {
            // Arrange
            const string commandName = Constants.CommandName.ShortHelp;
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<HelpStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_HelpStrategy_PassingNull()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(null as string);

            // Assert
            Assert.IsInstanceOf<HelpStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_NewStrategy()
        {
            // Arrange
            const string commandName = Constants.CommandName.New;
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<NewStrategy>(strategy);
        }
    }
}