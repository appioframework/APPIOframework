using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;

namespace Oppo.ObjectModel.Tests
{
    public abstract class PublishHelpStrategyTestsBase
    {
        protected abstract PublishHelpStrategy InstantiateObjectUnderTest(IWriter writer);
        protected abstract string GetExpectedCommandName();

        [Test]
        public void PublishHelpStrategy_Should_ImplementICommandOfPublishStrategy()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();

            // Act
            var objectUnderTest = InstantiateObjectUnderTest(writerMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<PublishStrategy>>(objectUnderTest);
        }

        [Test]
        public void PublishHelpStrategy_Should_ProvideCorrectCommandName()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = InstantiateObjectUnderTest(writerMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(GetExpectedCommandName(), commandName);
        }

        [Test]
        public void PublishHelpStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = InstantiateObjectUnderTest(writerMock.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        [Test]
        public void PublishHelpStrategy_Should_WriteHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = InstantiateObjectUnderTest(writerMock.Object);

            // Act
            var result = objectUnderTest.Execute(new string[0]);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Success, result);
            writerMock.Verify(x=>x.WriteLine(It.IsAny<string>()));
            writerMock.Verify(x=>x.WriteLines(It.IsAny<Dictionary<string, string>>()));
        }
    }

    public class PublishHelpStrategyTests : PublishHelpStrategyTestsBase
    {
        protected override PublishHelpStrategy InstantiateObjectUnderTest(IWriter writer)
        {
            return new PublishHelpStrategy(writer);
        }

        protected override string GetExpectedCommandName()
        {
            return Constants.PublishCommandArguments.Help;
        }
    }

    public class PublishVerboseHelpStrategyTests : PublishHelpStrategyTestsBase
    {
        protected override PublishHelpStrategy InstantiateObjectUnderTest(IWriter writer)
        {
            return new PublishVerboseHelpStrategy(writer);
        }

        protected override string GetExpectedCommandName()
        {
            return Constants.PublishCommandArguments.VerboseHelp;
        }
    }
}
