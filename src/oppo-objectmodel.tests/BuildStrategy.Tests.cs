using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class BuildStrategyTests
    {
        private static string[][] InvalidInputs()
        {
            return new[] 
            { 
                new []{ "Build", "", "" },
                new []{ "", "" },
                new []{ "built" },
                new []{ "b", "all" },
                new []{ "Build", "-all" },
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
                new []{"-all"},
                new []{"-r"},
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
            var buildStrategy = new BuildStrategy();

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);
            
            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);      
        }

        [Test]
        public void ShouldExcecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var buildStrategy = new BuildStrategy();

            // Act
            var strategyResult = buildStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
        }
    }
}