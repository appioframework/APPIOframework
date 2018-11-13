using System;
using System.IO;
using Moq;
using NUnit.Framework;

namespace Oppo.ObjectModel.IntegrationTests
{
    public class ObjectModelTests
    {
        private const string LogFileName = "oppo.log";
        
        [Test, Sequential]
        public void ObjectModel_ExecuteCommandShould_ThrowAndLogExceptionWhenNullIsPassed()
        {
            // issue with test is that the log file cannot be deleted in teardown
            // due to static logger instance (is cleaned up after Main() exits)
            // research how to integration testing

            // Arrange
            var commandFactoryMock = new Mock<ICommandFactory<ObjectModel>>();
            var logger = new LoggerWrapper();
            var objectUnderTest = new ObjectModel(commandFactoryMock.Object, logger);

            // Act
            Assert.Throws<ArgumentNullException>(() => objectUnderTest.ExecuteCommand(null));

            // Assert
            Assert.IsTrue(File.Exists(LogFileName));
        }
    }
}
