using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.Tests
{
    public class NewCommandNotExistentStrategyTests
    {
        private static string[][] Inputs()
        {
            return new[]
            {
                new[] {"-n", ""},
                new[] {"-n", "ab/yx"},
                new[] {"-n", "ab\\yx"},
                new[] {"-N", "ab/yx"},
                new[] {"", ""},
                new[] {""},
                new[] {"-n"},
                new string[] { }
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExecuteStrategy_Failure([ValueSource(nameof(Inputs))] string[] inputParams)
        {
            // Arrange                       
            var notExistentSlnCommandStrategy = new NewCommandNotExistentStrategy();

            // Act
            var strategyResult = notExistentSlnCommandStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
        }
    }
}