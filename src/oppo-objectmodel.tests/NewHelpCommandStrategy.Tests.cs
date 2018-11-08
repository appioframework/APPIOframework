using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.Tests
{
    public abstract class NewHelpCommandStrategyTestsBase
    {
        protected abstract NewHelpCommandStrategy InstantiateObjectUnderTest(IWriter writer);
        protected abstract string GetExpectedCommandName();

        [Test]
        public void NewHelpCommandStrategy_Should_ImplementICommandOfNewStrategy()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();

            // Act
            var objectUnderTest = InstantiateObjectUnderTest(writerMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<NewStrategy>>(objectUnderTest);
        }

        [Test]
        public void NewHelpCommandStrategy_Should_HaveCorrectCommandName()
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
        public void NewHelpCommandStrategy_Should_ProvideEmptyHelpText()
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
        public void NewHelpCommandStrategy_Should_WriteHelpText()
        {
            // Arrange
            var inputParamsMock = new string[0];
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = InstantiateObjectUnderTest(writerMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParamsMock);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Success, result);
            writerMock.Verify(x => x.WriteLine(It.IsAny<string>()));
            writerMock.Verify(x => x.WriteLines(It.IsAny<Dictionary<string, string>>()));
        }
    }

    public class NewHelpCommandStrategyTests : NewHelpCommandStrategyTestsBase
    {
        protected override NewHelpCommandStrategy InstantiateObjectUnderTest(IWriter writer)
        {
            return new NewHelpCommandStrategy(writer);
        }

        protected override string GetExpectedCommandName()
        {
            return Constants.NewCommandName.Help;
        }
    }

    public class NewVerboseHelpCommandStrategyTests : NewHelpCommandStrategyTestsBase
    {
        protected override NewHelpCommandStrategy InstantiateObjectUnderTest(IWriter writer)
        {
            return new NewVerboseHelpCommandStrategy(writer);
        }

        protected override string GetExpectedCommandName()
        {
            return Constants.NewCommandName.VerboseHelp;
        }
    }
}
