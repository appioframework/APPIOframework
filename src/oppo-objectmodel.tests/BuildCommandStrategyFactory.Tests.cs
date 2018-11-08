using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;

namespace Oppo.ObjectModel.Tests
{
    public class BuildCommandStrategyFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(null)]
        [TestCase("iioh4ohf")]
        [TestCase("abc")]
        [TestCase("5zh")]
        [TestCase("@/40&")]
        [TestCase("New")]
        [TestCase("CREATE")]
        [TestCase("slN")]
        [TestCase("Help")]
        [TestCase("-Help")]
        [TestCase("-H")]
        public void CommandStrategyFactory_ShouldReturn_BuildCommandNotExistentStrategy(string commandName)
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var mockFileSystem = new Mock<IFileSystem>();
            var factory = new BuildCommandStrategyFactory(writerMock.Object, mockFileSystem.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<BuildCommandNotExistentStrategy>(strategy);
        }
        
        [Test]
        public void CommandStrategyFactory_ShouldReturn_BuildHelpStrategy_Short()
        {
            // Arrange
            const string commandName = Constants.BuildCommandArguments.Help;
            var writerMock = new Mock<IWriter>();
            var mockFileSystem = new Mock<IFileSystem>();
            var factory = new BuildCommandStrategyFactory(writerMock.Object, mockFileSystem.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<BuildHelpStrategy>(strategy);
        }

        [Test]
        public void CommandStrategyFactory_ShouldReturn_BuildHelpStrategy_Verbose()
        {
            // Arrange
            const string commandName = Constants.BuildCommandArguments.VerboseHelp;
            var writerMock = new Mock<IWriter>();
            var mockFileSystem = new Mock<IFileSystem>();
            var factory = new BuildCommandStrategyFactory(writerMock.Object, mockFileSystem.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<BuildHelpStrategy>(strategy);
        }

        [TestCase(Constants.BuildCommandArguments.Name)]
        [TestCase(Constants.BuildCommandArguments.VerboseName)]
        public void CommandStrategyFactory_Should_ReturnBuildByNameStrategy(string commandName)
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var mockFileSystem = new Mock<IFileSystem>();
            var factory = new BuildCommandStrategyFactory(writerMock.Object, mockFileSystem.Object);

            // Act
            var strategy = factory.GetStrategy(commandName);

            // Assert
            Assert.IsInstanceOf<BuildNameStrategy>(strategy);
        }
    }
}