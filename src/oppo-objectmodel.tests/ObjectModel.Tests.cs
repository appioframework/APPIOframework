using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq;
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
            commandMock.Setup(s=>s.Execute(slnInputParams)).Returns(new CommandResult(true, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("anyMsg", string.Empty) }));

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
    }
}