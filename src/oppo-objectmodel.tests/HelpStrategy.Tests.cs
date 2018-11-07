using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests
{
    public class HelpStrategyTests
    {       
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy_Success()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var mockCommand = new Mock<ICommandStrategy>();
            mockCommand.Setup(x => x.GetHelpText()).Returns(Resources.text.help.HelpText.BuildCommand);
            var commands = new List<ICommandStrategy>();
            commands.Add(mockCommand.Object);
            var helpStrategy = new HelpStrategy(writerMock.Object, commands);

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });
            
            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            writerMock.Verify(x => x.WriteLines(It.Is<List<string>>(l => l.Contains(Resources.text.help.HelpText.HelpStartCommand))), Times.Once);
            writerMock.Verify(x => x.WriteLines(It.Is<List<string>>(l=>l.Contains(Resources.text.help.HelpText.BuildCommand))), Times.Once);
            writerMock.Verify(x => x.WriteLines(It.Is<List<string>>(l => l.Contains(Resources.text.help.HelpText.HelpEndCommand))), Times.Once);
        }

        [Test]
        public void ShouldReturnEmptyHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();            
            var commands = new List<ICommandStrategy>();            
            var helpStrategy = new HelpStrategy(writerMock.Object, commands);

            // Act
            var helpText = helpStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, string.Empty);
        }
    }
}