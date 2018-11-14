using NUnit.Framework;
using Moq;
using System;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class OppoLoggerTests
    {

        /// <summary>
        /// Registers one LoggerListener.
        /// </summary>
        [Test]
        public void Should_Register_One_LoggerListener()
        {
            // Follow the AAA pattern
            // Arrange: Set up data for the test.
            var loggerListenerMock = new Mock<ILoggerListener>();

            // Act: Perform the action of the test.
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Assert: Verify the result of the test.
            Assert.AreEqual(OppoLogger.LoggerListeners.Count(), 1);

            CleanupOppoLogger();
        }        

        /// <summary>
        /// Removes a previous registered LoggerListener.
        /// </summary>
        [Test]
        public void Should_Register_And_Remove_One_LoggerListener()
        {
            // Follow the AAA pattern
            // Arrange: Set up data for the test.
            var loggerListenerMock = new Mock<ILoggerListener>();

            // Act: Perform the action of the test.
            OppoLogger.RegisterListener(loggerListenerMock.Object);
            var loggerCountAfterAdd = OppoLogger.LoggerListeners.Count();                       

            OppoLogger.RemoveListener(loggerListenerMock.Object);
            var loggerCountAfterRemove = OppoLogger.LoggerListeners.Count();

            // Assert: Verify the result of the test.            
            Assert.AreEqual(loggerCountAfterAdd, 1);
            Assert.AreEqual(loggerCountAfterRemove, 0);

            CleanupOppoLogger();
        }

        /// <summary>
        /// Removes all LogerListeners
        /// </summary>
        [Test]
        public void Should_RemoveAll_LoggerListener()
        {
            // Follow the AAA pattern
            // Arrange: Set up data for the test.
            var loggerListenerMock = new Mock<ILoggerListener>();
            var loggerListenerMock2 = new Mock<ILoggerListener>();
            var loggerListenerMock3 = new Mock<ILoggerListener>();

            OppoLogger.RegisterListener(loggerListenerMock.Object);
            OppoLogger.RegisterListener(loggerListenerMock2.Object);
            OppoLogger.RegisterListener(loggerListenerMock3.Object);

            // Act: Perform the action of the test.
            OppoLogger.RemoveAllListeners();

            // Assert: Verify the result of the test.
            Assert.AreEqual(OppoLogger.LoggerListeners.Count(), 0);
        }

        /// <summary>
        /// Logs error message using LoggerListener
        /// </summary>
        [Test]
        public void Should_LogErrorMessage()
        {
            // Follow the AAA pattern
            // Arrange: Set up data for the test.
            var errorWrittenOut = false;
            var errorMessage = "errorMsg";
            var errorException = new Exception();
            
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(loggerListener => loggerListener.Error(errorMessage, errorException)).Callback(delegate { errorWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act: Perform the action of the test.
            OppoLogger.Error(errorMessage, errorException);

            // Assert: Verify the result of the test.            
            Assert.IsTrue(errorWrittenOut);

            CleanupOppoLogger();
        }

        /// <summary>
        /// Logs warn message using LoggerListener
        /// </summary>
        [Test]
        public void Should_LogWarnMessage()
        {
            // Follow the AAA pattern
            // Arrange: Set up data for the test.
            var warnWrittenOut = false;
            var warnMessage = "warnMsg";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(loggerListener => loggerListener.Warn(warnMessage)).Callback(delegate {warnWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act: Perform the action of the test.
            OppoLogger.Warn(warnMessage);

            // Assert: Verify the result of the test.            
            Assert.IsTrue(warnWrittenOut);

            CleanupOppoLogger();
        }

        /// <summary>
        /// Logs info message using LoggerListener
        /// </summary>
        [Test]
        public void Should_logInfoMessage()
        {
            // Follow the AAA pattern
            // Arrange: Set up data for the test.
            var infoWrittenOut = false;
            var infoMessage = "infoMsg";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(loggerListener => loggerListener.Info(infoMessage)).Callback(delegate { infoWrittenOut = true; });
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act: Perform the action of the test.
            OppoLogger.Info(infoMessage);

            // Assert: Verify the result of the test.            
            Assert.IsTrue(infoWrittenOut);

            CleanupOppoLogger();
        }

        /// <summary>
        /// Needs to be cleaned-up, because static context.
        /// </summary>
        private static void CleanupOppoLogger()
        {
            OppoLogger.RemoveAllListeners();
            Assert.AreEqual(OppoLogger.LoggerListeners.Count(), 0);
        }
    }
}