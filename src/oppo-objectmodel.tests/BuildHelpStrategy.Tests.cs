using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;

namespace Oppo.ObjectModel.Tests
{
    public class BuildHelpStrategyTests
    {
        private static string[][] InvalidInputs()
        {
            return new[]
            {
                new []{"-g"},
                new []{"-H"},
                new []{"--Help"},
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

        private static string[][] ValidInputs()
        {
            return new[]
            {
                new []{"-h"},
                new []{"--help"},
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BuildHelpStrategy_Should_Excecute_Seccess([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var mockWriter = new Mock<IWriter>();
            var strategy = new BuildHelpStrategy(mockWriter.Object);

            // Act
            var strategyResult = strategy.Execute(new string[] {});

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            mockWriter.Verify(x=>x.WriteLine(It.IsAny<string>()));
        }
    }
}