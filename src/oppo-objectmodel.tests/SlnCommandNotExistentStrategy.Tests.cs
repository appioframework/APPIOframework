using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;

namespace Oppo.ObjectModel.Tests
{
    public class SlnCommandNotExistentStrategyTests
    {
        private static string[][] Inputs()
        {
            return new[]
            {
                new []{ "-n", "" },
                new []{ "-n", "ab/yx" },
                new []{ "-n", "ab\\yx" },
                new []{ "-N", "ab/yx" },
                new []{  "", "" },
                new []{ ""},
                new []{ "-n" },
                new string[]{}
            };
        }

    
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy_Failure([ValueSource(nameof(Inputs))] string[] inputParams)
        {
            // Arrange                       
            var notExistendSlnCommandStretegy = new SlnCommandNotExistentStrategy();

            // Act
            var strategyResult = notExistendSlnCommandStretegy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
        }
    }
}