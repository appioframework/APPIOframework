using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;

namespace Oppo.ObjectModel.Tests
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

        private static bool[][] FailingExecutableStates()
        {
            return new[]
            {
                new[] {false, false},
                new[] {true, false},
                new[] {false, true},
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BuildStrategy_Should_Execute_HelpCommand_Success([ValueSource(nameof(ValidBuildHelpInputs))] string[] inputParams)
        {
            // Arrange
            var mockBuildStrategy = new Mock<IBuildStrategy>();
            mockBuildStrategy.Setup(x => x.Execute(new string[] { })).Returns(Constants.CommandResults.Success);
            var mockBuildFactory = new Mock<IBuildCommandStrategyFactory>();
            mockBuildFactory.Setup(f => f.GetStrategy(Constants.BuildCommandArguments.Help)).Returns(mockBuildStrategy.Object);
            mockBuildFactory.Setup(f => f.GetStrategy(Constants.BuildCommandArguments.VerboseHelp)).Returns(mockBuildStrategy.Object);

            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
        }

        [Test]
        public void BuildStrategy_Should_SucceedOnBuildableProject([ValueSource(nameof(ValidBuildNameInputs))] string[] inputParams)
        {
            // Arrange
            var mockBuildStrategy = new Mock<IBuildStrategy>();
            mockBuildStrategy.Setup(x => x.Execute(new string[] { inputParams[1] })).Returns(Constants.CommandResults.Success);

            var mockBuildFactory = new Mock<IBuildCommandStrategyFactory>();
            mockBuildFactory.Setup(f => f.GetStrategy(Constants.BuildCommandArguments.Name)).Returns(mockBuildStrategy.Object);
            mockBuildFactory.Setup(f => f.GetStrategy(Constants.BuildCommandArguments.VerboseName)).Returns(mockBuildStrategy.Object);

            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
        }

        [Test]
        public void ShouldExecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var mockBuildStrategy = new Mock<IBuildStrategy>();
            mockBuildStrategy.Setup(x => x.Execute(It.IsAny<IEnumerable<string>>())).Returns(Constants.CommandResults.Failure);
            
            var mockBuildFactory = new Mock<IBuildCommandStrategyFactory>();
            mockBuildFactory.Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(mockBuildStrategy.Object);

            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
        }      

        [Test]
        public void ShouldReturnEmptyHelpText()
        {
            // Arrange
            var mockBuildFactory = new Mock<IBuildCommandStrategyFactory>();
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
            var mockBuildFactory = new Mock<IBuildCommandStrategyFactory>();
            var buildStrategy = new BuildStrategy(mockBuildFactory.Object);

            // Act
            var commandName = buildStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Build);
        }
    }
}