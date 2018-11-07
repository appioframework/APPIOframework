using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.CommandNotExistent;
using Oppo.ObjectModel.CommandStrategies.HelloCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.ObjectModel.CommandStrategies.VersionCommands;

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
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

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
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

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
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

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
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

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
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

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
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<HelpStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_HelpStrategy_PassingNull()
        {
            // Arrange
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

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
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<NewStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_VersionStrategy()
        {
            // Arrange
            const string commandName = Constants.CommandName.Version;
            var reflectionMock = new Mock<IReflection>();
            var writerMock = new Mock<IWriter>();
            var factory = new CommandStrategyFactory(reflectionMock.Object, writerMock.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<VersionStrategy>(strategy);
        }
    }
}