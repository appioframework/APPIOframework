using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.Exceptions;

namespace Oppo.ObjectModel.Tests
{
    public class CommandFactoryTests
    {
        public class CommandFactoryFixture
        {
            public CommandFactoryFixture(string[] commandNames, string nameOfDefaultCommand, string nameOfDesiredCommand)
            {
                CommandNames = commandNames;
                NameOfDefaultCommand = nameOfDefaultCommand;
                NameOfDesiredCommand = nameOfDesiredCommand;
            }

            public string[] CommandNames;
            public string NameOfDefaultCommand;
            public string NameOfDesiredCommand;
        }

        private static CommandFactoryFixture[] CommandSelectionSuccessful()
        {
            return new[]
            {
                new CommandFactoryFixture(new []{"hello", "new", "exit"}, "hello", "hello"), 
                new CommandFactoryFixture(new []{"hello", "new", "exit"}, "hello", "exit"),
                new CommandFactoryFixture(new []{"hello", "new", "exit", "build", "re-run"}, "hello", "build"),
                new CommandFactoryFixture(new []{"hello", "new", "exit", "build", "re-run", "exploit", "destruct", "smile"}, "hello", "re-run"),
            };
        }

        private static CommandFactoryFixture[] CommandSelectionDuplicates()
        {
            return new[]
            {
                new CommandFactoryFixture(new []{"hello", "new", "exit", "hello"}, "hello", "hello"),
                new CommandFactoryFixture(new []{"hello", "new", "exit", "new"}, "hello", "exit"),
                new CommandFactoryFixture(new []{"hello", "new", "exit", "build", "re-run", "re-run"}, "hello", "build"),
                new CommandFactoryFixture(new []{"hello", "new", "exit", "build", "re-run", "exploit", "destruct", "smile", "exploit"}, "hello", "re-run"),
            };
        }

        private static CommandFactoryFixture[] CommandSelectionInvalidNameOfDefaultCommand()
        {
            return new[]
            {
                new CommandFactoryFixture(new []{"hello", "exit"}, "invalid", "hello"),
                new CommandFactoryFixture(new []{"hello", "exit", "build", "wipe"}, "wipe-2", "hello"),
                new CommandFactoryFixture(new []{"hello", "exit", "build", "wipe", "thrash", "smile"}, "smil", "hello"),
            };
        }

        [Test]
        public void CommandFactory_Should_ThrowArgumentNullExceptionWhenAnyArgumentsAreNull()
        {
            // Arrange

            // Act
            Assert.Throws<ArgumentNullException>(() => new CommandFactory<object>(null as ICommand<object>[], null as string));
            Assert.Throws<ArgumentNullException>(() => new CommandFactory<object>(new ICommand<object>[0], null as string));
            Assert.Throws<ArgumentNullException>(() => new CommandFactory<object>(null as ICommand<object>[], "hello"));

            // Assert
        }

        [Test]
        public void CommandFactory_Should_ImplementICommandFactoryGeneric()
        {
            // Arrange
            var commandArrayMock = new ICommand<object>[0];

            // Act
            var objectUnderTest = new CommandFactory<object>(commandArrayMock, string.Empty);

            // Assert
            Assert.IsInstanceOf<ICommandFactory<object>>(objectUnderTest);
        }

        [Test]
        public void CommandFactory_Should_ReturnFallbackCommandOnInvalidCommandName()
        {
            // Arrange
            var commandArrayMock = new ICommand<object>[0];
            var objectUnderTest = new CommandFactory<object>(commandArrayMock, string.Empty);

            // Act
            var command = objectUnderTest.GetCommand("any-name");

            // Assert
            Assert.IsInstanceOf<ICommand<object>>(command);
        }

        [Test]
        public void CommandFactory_Should_AlwaysProvideValidSequenceOfCommands([ValueSource(nameof(CommandSelectionSuccessful))] CommandFactoryFixture data)
        {
            // Arrange
            var commandMockArray = new Mock<ICommand<object>>[data.CommandNames.Length];

            for (var i = 0; i < data.CommandNames.Length; ++i)
            {
                var name = data.CommandNames[i];

                commandMockArray[i] = new Mock<ICommand<object>>();
                commandMockArray[i].Setup(x => x.Name).Returns(name);
            }

            var commandArrayMock = commandMockArray.Select(x => x.Object).ToArray();

            var objectUnderTest = new CommandFactory<object>(commandArrayMock, data.NameOfDefaultCommand);

            // Act
            var commands = objectUnderTest.Commands;

            // Assert
            foreach (var command in commandArrayMock)
            {
                Assert.IsTrue(commands.Any(x => x == command));
            }
        }

        [Test]
        public void CommandFactoryFallbackCommand_Should_ThrowNotSupportedExceptionOnAllInteractions()
        {
            // Arrange
            var commandArrayMock = new ICommand<object>[0];
            var objectUnderTest = new CommandFactory<object>(commandArrayMock, string.Empty);
            var command = objectUnderTest.GetCommand("any-name");

            // Act
            Assert.Throws<NotSupportedException>(() => command.Name.ToString());
            Assert.Throws<NotSupportedException>(() => command.Execute(null));
            Assert.Throws<NotSupportedException>(() => command.GetHelpText());

            // Assert

        }

        [Test]
        public void CommandFactory_Should_ReturnAppropriateCommandSelectedByName([ValueSource(nameof(CommandSelectionSuccessful))] CommandFactoryFixture data)
        {
            // Arrange
            var commandMockArray = new Mock<ICommand<object>>[data.CommandNames.Length];
            var expectedCommand = null as ICommand<object>;

            for (var i = 0; i < data.CommandNames.Length; ++i)
            {
                var name = data.CommandNames[i];

                commandMockArray[i] = new Mock<ICommand<object>>();
                commandMockArray[i].Setup(x => x.Name).Returns(name);

                if (name == data.NameOfDesiredCommand)
                {
                    expectedCommand = commandMockArray[i].Object;
                }
            }

            var commandArrayMock = commandMockArray.Select(x => x.Object).ToArray();

            var objectUnderTest = new CommandFactory<object>(commandArrayMock, data.NameOfDefaultCommand);

            // Act
            var command = objectUnderTest.GetCommand(data.NameOfDesiredCommand);

            // Assert
            Assert.AreEqual(expectedCommand, command);
        }

        [Test]
        public void CommandFactory_Should_ThrowDuplicateNameExceptionWhenCommandsWithEqualNamesArePassed([ValueSource(nameof(CommandSelectionDuplicates))] CommandFactoryFixture data)
        {
            // Arrange
            var commandMockArray = new Mock<ICommand<object>>[data.CommandNames.Length];

            for (var i = 0; i < data.CommandNames.Length; ++i)
            {
                var name = data.CommandNames[i];

                commandMockArray[i] = new Mock<ICommand<object>>();
                commandMockArray[i].Setup(x => x.Name).Returns(name);
            }

            var commandArrayMock = commandMockArray.Select(x => x.Object).ToArray();

            // Act
            Assert.Throws<DuplicateNameException>(() => new CommandFactory<object>(commandArrayMock, data.NameOfDefaultCommand));

            // Assert

        }

        [Test]
        public void CommandFactory_Should_ThrowArgumentOutOfRangeExceptionOnInvalidNameOfDefaultCommand([ValueSource(nameof(CommandSelectionInvalidNameOfDefaultCommand))] CommandFactoryFixture data)
        {
            // Arrange
            var commandMockArray = new Mock<ICommand<object>>[data.CommandNames.Length];

            for (var i = 0; i < data.CommandNames.Length; ++i)
            {
                var name = data.CommandNames[i];

                commandMockArray[i] = new Mock<ICommand<object>>();
                commandMockArray[i].Setup(x => x.Name).Returns(name);
            }

            var commandArrayMock = commandMockArray.Select(x => x.Object).ToArray();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => new CommandFactory<object>(commandArrayMock, data.NameOfDefaultCommand));

            // Assert
        }

        [Test]
        public void CommandFactory_Should_ReturnAppropriateDefaultCommand([ValueSource(nameof(CommandSelectionSuccessful))] CommandFactoryFixture data)
        {
            // Arrange
            var commandMockArray = new Mock<ICommand<object>>[data.CommandNames.Length];
            var expectedCommand = null as ICommand<object>;

            for (var i = 0; i < data.CommandNames.Length; ++i)
            {
                var name = data.CommandNames[i];

                commandMockArray[i] = new Mock<ICommand<object>>();
                commandMockArray[i].Setup(x => x.Name).Returns(name);

                if (name == data.NameOfDefaultCommand)
                {
                    expectedCommand = commandMockArray[i].Object;
                }
            }

            var commandArrayMock = commandMockArray.Select(x => x.Object).ToArray();

            var objectUnderTest = new CommandFactory<object>(commandArrayMock, data.NameOfDefaultCommand);

            // Act
            var command = objectUnderTest.GetCommand(null as string);

            // Assert
            Assert.AreEqual(expectedCommand, command);
        }
    }
}
