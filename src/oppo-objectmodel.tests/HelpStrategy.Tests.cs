using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests
{
    public class HelpStrategyTestsBase
    {     
        [Test]
        public void HelpStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();

            // Act
            var objectUnderTest = new HelpStrategy(string.Empty, writerMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(objectUnderTest);
        }

        [Test]
        public void HelpStrategy_Should_WriteHelpText()
        {
            // Arrange
            const string commandName = "any-name";
            const string commandHelpText = "any help text \r\n maybe with multiple lines ...";

            var writerMock = new Mock<IWriter>();
            var commandMock = new Mock<ICommand<ObjectModel>>();
            commandMock.Setup(x => x.Name).Returns(commandName);
            commandMock.Setup(x => x.GetHelpText()).Returns(commandHelpText);

            var factoryMock = new Mock<ICommandFactory<ObjectModel>>();
            factoryMock.Setup(x => x.Commands).Returns(new[] {commandMock.Object});

            var helpStrategy = new HelpStrategy(string.Empty, writerMock.Object);
            helpStrategy.CommandFactory = factoryMock.Object;

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            writerMock.Verify(x => x.WriteLines(It.Is<Dictionary<string, string>>(d => d.ContainsValue(commandHelpText))), Times.Once);
            writerMock.Verify(x => x.WriteLine(It.Is<string>(l => l.Equals(Resources.text.help.HelpTextValues.HelpStartCommand))), Times.Once);
            writerMock.Verify(x => x.WriteLine(It.Is<string>(l => l.Equals(Resources.text.help.HelpTextValues.HelpEndCommand))), Times.Once);
        }

        [Test]
        public void HelpStrategy_Should_WriteSparseHelpTextIfNoCommandFactoryIsProvided()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var helpStrategy = new HelpStrategy(string.Empty, writerMock.Object);

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            writerMock.Verify(x => x.WriteLine(It.Is<string>(l => l.Equals(Resources.text.help.HelpTextValues.HelpStartCommand))), Times.Once);
            writerMock.Verify(x => x.WriteLine(It.Is<string>(l => l.Equals(Resources.text.help.HelpTextValues.HelpEndCommand))), Times.Once);
        }

        [Test]
        public void ShouldReturnHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();            
            var helpStrategy = new HelpStrategy(string.Empty, writerMock.Object);

            // Act
            var helpText = helpStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.HelpCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var helpStrategy = new HelpStrategy(string.Empty, writerMock.Object);

            // Act
            var commandName = helpStrategy.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }
    }
}