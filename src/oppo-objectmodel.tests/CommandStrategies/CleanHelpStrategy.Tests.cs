using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.CleanCommands;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class CleanHelpStrategyTests
    {
        private const string AnyCommandName = "any-name";

        private static readonly IEnumerable<KeyValuePair<string, string>> AnyHelpText = new[]
        {
            new KeyValuePair<string, string>("any-key", "any-value"),
        };

        private CleanHelpStrategy _objectUnderTest;

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _objectUnderTest = new CleanHelpStrategy(AnyCommandName, AnyHelpText);
        }

        [Test]
        public void CleanHelpStrategy_Should_ImplementICommandOfCleanStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<CleanStrategy>>(_objectUnderTest);
        }

        [Test]
        public void CleanHelpStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var name = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(AnyCommandName, name);
        }

        [Test]
        public void CleanHelpStrategy_Should_ProvideSomeHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.CleanHelpArgumentCommandDescription, helpText);
        }

        [Test]
        public void CleanHelpStrategy_Should_ReturnHelpTextWhenExecuted()
        {
            // Arrange
            const string commandName = "any-name";
            const string commandHelpText = "any help text";

            var commandMock = new Mock<ICommand<CleanStrategy>>();
            commandMock.Setup(x => x.Name).Returns(commandName);
            commandMock.Setup(x => x.GetHelpText()).Returns(commandHelpText);

            var factoryMock = new Mock<ICommandFactory<CleanStrategy>>();
            factoryMock.Setup(x => x.Commands).Returns(new[] {commandMock.Object});

            _objectUnderTest.CommandFactory = factoryMock.Object;

            // Act
            var result = _objectUnderTest.Execute(new string[0]);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.IsEmpty(result.Message);
            Assert.IsNotNull(result.OutputText);

            factoryMock.Verify(x => x.Commands, Times.AtLeastOnce);
            commandMock.Verify(x => x.Name, Times.AtLeastOnce);
            commandMock.Verify(x => x.GetHelpText(), Times.AtLeastOnce);
        }
    }
}
