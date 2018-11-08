using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;

namespace Oppo.ObjectModel.Tests
{
    public abstract class BuildHelpStrategyTestsBase
    {
        protected abstract BuildHelpStrategy InstantiateObjectUnderTest(IWriter writer);
        protected abstract string GetExpectedCommandName();

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
            var objectUnderTest = InstantiateObjectUnderTest(mockWriter.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<BuildStrategy>>(objectUnderTest);
        }

        [Test]
        public void BuildHelpStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange
            var mockWriter = new Mock<IWriter>();
            var objectUnderTest = InstantiateObjectUnderTest(mockWriter.Object);

            // Act
            var commandName = objectUnderTest.Name;

            // Assert
            Assert.AreEqual(GetExpectedCommandName(), commandName);
        }

        [Test]
        public void BuildHelpStrategy_Should_ProvideEmptyHelpText()
        {
            // Arrange
            var mockWriter = new Mock<IWriter>();
            var objectUnderTest = InstantiateObjectUnderTest(mockWriter.Object);

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
            var strategy = InstantiateObjectUnderTest(mockWriter.Object);

            // Act
            var strategyResult = strategy.Execute(new string[] {});

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            mockWriter.Verify(x=>x.WriteLine(It.IsAny<string>()));
        }
    }

    public class BuildHelpStrategyTests : BuildHelpStrategyTestsBase
    {
        protected override BuildHelpStrategy InstantiateObjectUnderTest(IWriter writer)
        {
            return new BuildHelpStrategy(writer);
        }

        protected override string GetExpectedCommandName()
        {
            return Constants.BuildCommandArguments.Help;
        }
    }

    public class BuildVerboseHelpStrategyTests : BuildHelpStrategyTestsBase
    {
        protected override BuildHelpStrategy InstantiateObjectUnderTest(IWriter writer)
        {
            return new BuildVerboseHelpStrategy(writer);
        }

        protected override string GetExpectedCommandName()
        {
            return Constants.BuildCommandArguments.VerboseHelp;
        }
    }
}