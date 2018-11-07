using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Oppo.ObjectModel.CommandStrategies.HelloCommands;

namespace Oppo.ObjectModel.Tests
{
    public class HelloStrategyTests
    {
        [Test]
        public void HelloStrategy_Should_ImplementICommandStrategy()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();

            // Act
            var objectUnderTest = new HelloStrategy(writerMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommandStrategy>(objectUnderTest);
        }

        [Test]
        public void HelloStrategy_Should_WriteHelloMessage()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var objectUnderTest = new HelloStrategy(writerMock.Object);
            var inputArgsMock = new string[0];

            // Act
            var result = objectUnderTest.Execute(inputArgsMock);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Success, result);
            writerMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldReturnHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var helloStrategy = new HelloStrategy(writerMock.Object);

            // Act
            var helpText = helloStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.HelloCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var helloStrategy = new HelloStrategy(writerMock.Object);

            // Act
            var commandName = helloStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Hello);
        }
    }
}
