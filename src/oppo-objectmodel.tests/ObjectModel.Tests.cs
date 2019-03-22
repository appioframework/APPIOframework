using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.CleanCommands;
using Oppo.ObjectModel.CommandStrategies.DeployCommands;
using Oppo.ObjectModel.CommandStrategies.GenerateCommands;
using Oppo.ObjectModel.CommandStrategies.HelloCommands;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.ObjectModel.CommandStrategies.ReferenceCommands;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using Oppo.ObjectModel.CommandStrategies.VersionCommands;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests
{
    public class ObjectModelTests
    {   
        private static string[][] ValidInputs()
        {
            return new[]
            {
                new []{ "new", "sln", "-n", "anyName" },
                new []{ "new", "opcuaapp", "-n", "ABC" },
                new []{ "new", "sln", "--name", "ABC" },
                new string[]{ }
            };
        }

        [Test]
        public void ShouldGetValidInputParams([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var commandMock = new Mock<ICommand<ObjectModel>>();           
            var factoryMock = new Mock<ICommandFactory<ObjectModel>>();

            var slnInputParams = inputParams.Skip(1);
            var objectModel = new ObjectModel(factoryMock.Object);
            factoryMock.Setup(factory => factory.GetCommand(inputParams.FirstOrDefault())).Returns(commandMock.Object);
            commandMock.Setup(s=>s.Execute(slnInputParams)).Returns(new CommandResult(true, new MessageLines(){ {"anyMsg", string.Empty } }));

            // Act
            var executionResult = objectModel.ExecuteCommand(inputParams);

            // Assert
            Assert.IsTrue(executionResult.Sucsess);
            Assert.AreEqual("anyMsg", executionResult.OutputMessages.First().Key);
        }

        [Test]
        public void ShouldGetInvalidInputParams()
        {
            // Arrange
            var factoryMock = new Mock<ICommandFactory<ObjectModel>>();
            List<string> inputParams = null;            
                    
            var objectModel = new ObjectModel(factoryMock.Object);
            var errorWrittenOut = false;
            var loggerListener = new Mock<ILoggerListener>();            
            loggerListener.Setup(logger => logger.Error(LoggingText.NullInputParams_Msg, It.IsAny<ArgumentNullException>())).Callback(delegate { errorWrittenOut = true; });

            OppoLogger.RegisterListener(loggerListener.Object);

            // Act                       
            Assert.Throws<ArgumentNullException>(() => objectModel.ExecuteCommand(inputParams));

            // Assert
            Assert.IsTrue(errorWrittenOut);            
        }

        [Test]
        public void ShouldCreateCommandFactory()
        {
            // Arrange
            var reflectionWrapperMock = new Mock<IReflection>();

            // Act
            var commandFactory = ObjectModel.CreateCommandFactory(reflectionWrapperMock.Object);

            // Assert
            Assert.IsNotNull(commandFactory);
            Assert.IsNotNull(commandFactory.Commands);

            var commands = commandFactory.Commands.ToArray();
            Assert.IsTrue(commands.Length > 0);
            Assert.IsTrue(commands.Any(x => x is BuildStrategy));
            Assert.IsTrue(commands.Any(x => x is CleanStrategy));
            Assert.IsTrue(commands.Any(x => x is DeployStrategy));
			Assert.IsTrue(commands.Any(x => x is GenerateStrategy));
			Assert.IsTrue(commands.Any(x => x is HelloStrategy));
            Assert.IsTrue(commands.Any(x => x is HelpStrategy<ObjectModel>));
            Assert.IsTrue(commands.Any(x => x is ImportStrategy));
            Assert.IsTrue(commands.Any(x => x is NewStrategy));
            Assert.IsTrue(commands.Any(x => x is PublishStrategy));
			Assert.IsTrue(commands.Any(x => x is ReferenceStrategy));
			Assert.IsTrue(commands.Any(x => x is SlnStrategy));
            Assert.IsTrue(commands.Any(x => x is VersionStrategy));
        }

        [Test]
        public void PrepareCommandFailureOutputText_ShouldCreateCorrectlyFormattedText()
        {
            // Arrange
            var argsMock = new[] { "build", "--exit", "any-value" };

            const string expectedResult = "Command \"oppo build --exit any-value\" failed!";

            var factoryMock = new Mock<ICommandFactory<ObjectModel>>();

            var objectModel = new ObjectModel(factoryMock.Object);

            // Act
            var result = objectModel.PrepareCommandFailureOutputText(argsMock);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}