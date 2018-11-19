using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class BuildStrategyTests
    {
        private static string[][] InvalidInputs()
        {
            return new[]
            {
                new []{"-g"},
                new []{"-y"},
                new []{"-n"},
                new []{"-n", ""},
                new []{"--something"},
                new []{"--different"},
                new []{"--name"},
                new []{"--name", ""},
                new []{""},
                new string[0],
            };
        }

        private static string[][] ValidBuildNameInputs()
        {
            return new[]
            {
                new []{"-n", "hugo"},
                new []{"--name", "talsen"}
            };
        }

        private static string[][] ValidBuildHelpInputs()
        {
            return new[]
            {
                new []{"--help"},
                new []{"-h"}
            };
        }

        [Test]
        public void BuildStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var mockBuildFactory = new Mock<ICommandFactory<BuildStrategy>>();

            // Act
            var objectUnderTest = new BuildStrategy(mockBuildFactory.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(objectUnderTest);
        }

        [Test]
        public void BuildStrategy_Should_Execute_HelpCommand_Success([ValueSource(nameof(ValidBuildHelpInputs))] string[] inputParams)
        {
            // Arrange
            var mockBuildStrategy = new Mock<ICommand<BuildStrategy>>();
            mockBuildStrategy.Setup(x => x.Execute(new string[] { })).Returns(new CommandResult(true, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(string.Empty, string.Empty)}));
            var mockBuildFactory = new Mock<ICommandFactory<BuildStrategy>>();
            mockBuildFactory.Setup(f => f.GetCommand(Constants.BuildCommandArguments.Help)).Returns(mockBuildStrategy.Object);
            mockBuildFactory.Setup(f => f.GetCommand(Constants.BuildCommandArguments.VerboseHelp)).Returns(mockBuildStrategy.Object);

            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Key);
        }

        [Test]
        public void BuildStrategy_Should_SucceedOnBuildableProject([ValueSource(nameof(ValidBuildNameInputs))] string[] inputParams)
        {
            // Arrange
            var mockBuildStrategy = new Mock<ICommand<BuildStrategy>>();
            mockBuildStrategy.Setup(x => x.Execute(new string[] { inputParams[1] })).Returns(new CommandResult(true, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(OutputText.OpcuaappBuildSuccess, string.Empty) }));

            var mockBuildFactory = new Mock<ICommandFactory<BuildStrategy>>();
            mockBuildFactory.Setup(f => f.GetCommand(Constants.BuildCommandArguments.Name)).Returns(mockBuildStrategy.Object);
            mockBuildFactory.Setup(f => f.GetCommand(Constants.BuildCommandArguments.VerboseName)).Returns(mockBuildStrategy.Object);

            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(strategyResult.Sucsess);
            Assert.AreEqual(OutputText.OpcuaappBuildSuccess, strategyResult.OutputMessages.First().Key);
        }

        [Test]
        public void ShouldExecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var mockBuildStrategy = new Mock<ICommand<BuildStrategy>>();
            mockBuildStrategy.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>())).Returns(new CommandResult(false, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(OutputText.OpcuaappBuildFailure, string.Empty) }));
            
            var mockBuildFactory = new Mock<ICommandFactory<BuildStrategy>>();
            mockBuildFactory.Setup(f => f.GetCommand(It.IsAny<string>())).Returns(mockBuildStrategy.Object);

            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(strategyResult.Sucsess);
            Assert.AreEqual(OutputText.OpcuaappBuildFailure, strategyResult.OutputMessages.First().Key);
        } 

        [Test]
        public void ShouldReturnEmptyHelpText()
        {
            // Arrange
            var mockBuildFactory = new Mock<ICommandFactory<BuildStrategy>>();
            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);            

            // Act
            var helpText = buildStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.BuildCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var mockBuildFactory = new Mock<ICommandFactory<BuildStrategy>>();
            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var commandName = buildStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Build);
        }
    }
}