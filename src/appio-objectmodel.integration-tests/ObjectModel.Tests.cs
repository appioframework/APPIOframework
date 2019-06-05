using System;
using System.IO;
using Moq;
using NUnit.Framework;

namespace Appio.ObjectModel.IntegrationTests
{
    public class ObjectModelTests
    {
        private const string LogFileName = "appio.log";
        
        [Test, Sequential]
        public void ObjectModel_ExecuteCommandShould_ThrowAndLogExceptionWhenNullIsPassed()
        {
            // issue with test is that the log file cannot be deleted in teardown
            // due to static logger instance (is cleaned up after Main() exits)
            // research how to integration testing

            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<ObjectModel>>();
            var objectUnderTest = new ObjectModel(commandFactoryMock.Object);

            AppioLogger.RegisterListener(new LoggerListenerWrapper());

            // Act
            Assert.Throws<ArgumentNullException>(() => objectUnderTest.ExecuteCommand(null));

            // Assert
            Assert.IsTrue(File.Exists(LogFileName));
        }
    }
}
