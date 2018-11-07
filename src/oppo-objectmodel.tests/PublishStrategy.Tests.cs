using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class PublishStrategyTests
    {
        private static string[][] InvalidInputs()
        {
            return new[] 
            { 
                new []{ "Publish", "", "" },
                new []{ "", "" },
                new []{ "publish" },
                new []{ "p", "all" },
                new []{ "Publish", "-all" },
                new []{ "-All" },
                new []{ "-aLL" },
                new []{ "-R" },
                new string[]{ }
            };
        }

        private static string[][] ValidInputs()
        {
            return new[]
            {
                new []{"-all"}                
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy_Success([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var publishStrategy = new PublishStrategy();

            // Act
            var strategyResult = publishStrategy.Execute(inputParams);
            
            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);      
        }

        [Test]
        public void ShouldExcecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var publishStrategy = new PublishStrategy();

            // Act
            var strategyResult = publishStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
        }

        [Test]
        public void ShouldReturnEmptyHelpText()
        {
            // Arrange
            var publishStrategy = new PublishStrategy();

            // Act
            var helpText = publishStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.PublishCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var publishStrategy = new PublishStrategy();

            // Act
            var commandName = publishStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Publish);
        }
    }
}