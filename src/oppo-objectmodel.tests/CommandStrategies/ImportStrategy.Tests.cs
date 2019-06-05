using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using System.Linq;
using Oppo.Resources.text.help;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class ImportStrategyTest
    {
        public struct ImportStrategyFixture
        {
            public ImportStrategyFixture(string[] input, string result)
            {
                Input = input;
                Result = result;
            }

            public string[] Input;
            public string Result;
        }

        private static ImportStrategyFixture[] Data()
        {
            return new[]
            {
                new ImportStrategyFixture(new string[0], Constants.CommandResults.Success),
                new ImportStrategyFixture(new[]{ "-h"}, Constants.CommandResults.Success),
                new ImportStrategyFixture(new[]{ "information-model", "myApp", "-p", "path" }, Constants.CommandResults.Success),
                new ImportStrategyFixture(new[]{ "information-model", "myApp", "--path", "path" }, Constants.CommandResults.Success),
                new ImportStrategyFixture(new[]{ "abc", "myApp", "--path", "path" }, Constants.CommandResults.Failure),
            };
        }

        [Test]
        public void ImportStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var factoryMock = new Mock<ICommandFactory<ImportStrategy>>();

            // Act
            var strategy = new ImportStrategy(factoryMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(strategy);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var factoryMock = new Mock<ICommandFactory<ImportStrategy>>();
            var strategy = new ImportStrategy(factoryMock.Object);

            // Act
            var commandName = strategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Import);
        }

        [Test]
        public void ShouldReturnValidHelpText()
        {
            // Arrange
            var factoryMock = new Mock<ICommandFactory<ImportStrategy>>();
            var strategy = new ImportStrategy(factoryMock.Object);

            // Act
            var helpText = strategy.GetHelpText();
            
            // Assert
            Assert.AreEqual(helpText, HelpTextValues.ImportCommand);
        }

        [Test]
        public void ShouldSuccedOnExecute([ValueSource(nameof(Data))] ImportStrategyFixture data)
        {
            // Arrange
            var commandMock = new Mock<ICommand<ImportStrategy>>();
            commandMock.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>())).Returns(new CommandResult(true, new MessageLines { { data.Result, string.Empty } }));

            var factoryMock = new Mock<ICommandFactory<ImportStrategy>>();            
            factoryMock.Setup(x => x.GetCommand(data.Input.FirstOrDefault())).Returns(commandMock.Object);

            var strategy = new ImportStrategy(factoryMock.Object);

            // Act
            var result = strategy.Execute(data.Input);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(data.Result, result.OutputMessages.First().Key);
        }
    }
}