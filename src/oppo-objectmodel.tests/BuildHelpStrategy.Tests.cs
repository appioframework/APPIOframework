using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;

namespace Oppo.ObjectModel.Tests
{
    public class BuildHelpStrategyTestsBase
    {
        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new []{"-h"},
                new []{"--help"},
            };
        }
        
        [Test]
        public void BuildHelpStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var mockWriter = new Mock<IWriter>();

            // Act
            var objectUnderTest = new BuildHelpStrategy(string.Empty, mockWriter.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<BuildStrategy>>(objectUnderTest);
        }

        [Test]
        public void BuildHelpStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var mockWriter = new Mock<IWriter>();
            var objectUnderTest = new BuildHelpStrategy(string.Empty, mockWriter.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void BuildHelpStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var mockWriter = new Mock<IWriter>();
            var objectUnderTest = new BuildHelpStrategy(string.Empty, mockWriter.Object);

            // Act
            var helpText = objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(string.Empty, helpText);
        }

        [Test]
        public void BuildHelpStrategy_Should_ExecuteSuccess([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var mockWriter = new Mock<IWriter>();
            var strategy = new BuildHelpStrategy(string.Empty, mockWriter.Object);

            // Act
            var strategyResult = strategy.Execute(new string[] {});

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            mockWriter.Verify(x=>x.WriteLine(It.IsAny<string>()));
        }
    }
}