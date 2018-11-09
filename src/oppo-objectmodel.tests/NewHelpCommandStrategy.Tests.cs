using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.Tests
{
    public class NewHelpCommandStrategyTests
    {     
        [Test]
        public void NewHelpCommandStrategy_Should_ImplementICommandOfNewStrategy()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();

            // Act
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, writerMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<NewStrategy>>(objectUnderTest);
        }

        [Test]
        public void NewHelpCommandStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, writerMock.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void NewHelpCommandStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, writerMock.Object);

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
            var objectUnderTest = new NewHelpCommandStrategy(string.Empty, writerMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParamsMock);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Success, result);
            writerMock.Verify(x => x.WriteLine(It.IsAny<string>()));
            writerMock.Verify(x => x.WriteLines(It.IsAny<Dictionary<string, string>>()));
        }
    }
}