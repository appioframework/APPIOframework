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
            mockCommand.Setup(x => x.Name).Returns(Constants.CommandName.Build);
            mockCommand.Setup(x => x.GetHelpText()).Returns(Resources.text.help.HelpTextValues.BuildCommand);
            var commands = new List<ICommandStrategy>();
            commands.Add(mockCommand.Object);
            var helpStrategy = new HelpStrategy(writerMock.Object, commands);

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });
            
            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            writerMock.Verify(x => x.WriteLines(It.Is<Dictionary<string, string>>(d => d.ContainsValue(Resources.text.help.HelpTextValues.BuildCommand))), Times.Once);
            writerMock.Verify(x => x.WriteLine(It.Is<string>(l=>l.Equals(Resources.text.help.HelpTextValues.HelpStartCommand))), Times.Once);
            writerMock.Verify(x => x.WriteLine(It.Is<string>(l => l.Equals(Resources.text.help.HelpTextValues.HelpEndCommand))), Times.Once);
        }

        [Test]
        public void ShouldReturnHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();            
            var commands = new List<ICommandStrategy>();            
            var helpStrategy = new HelpStrategy(writerMock.Object, commands);

            // Act
            var helpText = helpStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.HelpCommand);
        }
    }
}