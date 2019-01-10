using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.GenerateCommands;
using static Oppo.ObjectModel.Constants;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class GenerateStrategyShould
    {
        protected static string[][] InvalidInputs()
        {
            return new[]
            {
                new []{""},
                new string[0],
                new []{"Information-model"},
                new []{"information-mode", "-n", ""},
                new []{"any string", "-n", ""},
            };
        }

        protected static string[][] ValidInputs()
        {
            return new[]
            {
                new []{"information-model", "-n", "testApp", "-m", "model.xml"},
                new []{"information-model", "--name", "testApp", "-m", "model.xml"},
                new []{"information-model", "--name", "testApp", "--model", "model.xml"},
            };
        }
        private Mock<ICommandFactory<GenerateStrategy>> _mockFactory;
        private GenerateStrategy _strategy;

        [SetUp]
        public void SetUpObjectUnderTest()
        {
            _mockFactory = new Mock<ICommandFactory<GenerateStrategy>>();
            _strategy = new GenerateStrategy(_mockFactory.Object);
        }

        [Test]
        public void ImplementICommandOfGenerateStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(_strategy);
        }

        [Test]
        public void ReturnValidCommandName()
        {
            // Arrange

            // Act
            var commandName = _strategy.Name;

            // Assert
            Assert.AreEqual(Constants.CommandName.Generate, commandName);
        }

        [Test]
        public void ReturnValidHelpText()
        {
            // Arrange

            // Act
            var helpText = _strategy.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.GenerateCommand, helpText);
        }

        [Test]
        public void CallExecuteOnProvidedValidCommand([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var mockCommandResult = new CommandResult(true, new MessageLines());
            var mockCommand = new Mock<ICommand<GenerateStrategy>>();
            mockCommand.Setup(command => command.Execute(It.IsAny<IEnumerable<string>>())).Returns(mockCommandResult);
            _mockFactory.Setup(facory => facory.GetCommand(GenerateInformationModelCommandName.InformationModel)).Returns(mockCommand.Object);
            
            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsTrue(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
        }

        [Test]
        public void CallExecuteOnProvidedInvalidCommand([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var mockCommandResult = new CommandResult(true, new MessageLines());
            var mockCommand = new Mock<ICommand<GenerateStrategy>>();
            mockCommand.Setup(command => command.Execute(It.IsAny<IEnumerable<string>>())).Returns(mockCommandResult);

            var mockInvalidCommandResult = new CommandResult(false, new MessageLines());
            var mockInvalidCommand = new Mock<ICommand<GenerateStrategy>>();
            mockInvalidCommand.Setup(command => command.Execute(It.IsAny<IEnumerable<string>>())).Returns(mockInvalidCommandResult);

            _mockFactory.Setup(facory => facory.GetCommand(GenerateInformationModelCommandName.InformationModel)).Returns(mockCommand.Object);
            _mockFactory.Setup(facory => facory.GetCommand(It.Is<string>(s=>s != GenerateInformationModelCommandName.InformationModel))).Returns(mockInvalidCommand.Object);

            // Act
            var commandResult = _strategy.Execute(inputParams);

            // Assert
            Assert.IsFalse(commandResult.Sucsess);
            Assert.IsNotNull(commandResult.OutputMessages);
        }
    }
}