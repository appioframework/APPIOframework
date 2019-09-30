/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using NUnit.Framework;
using Moq;
using System;
using System.Linq;

namespace Appio.ObjectModel.Tests
{
    public class AppioLoggerTests
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
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Assert: Verify the result of the test.
            Assert.AreEqual(AppioLogger.LoggerListeners.Count(), 1);

            CleanupAppioLogger();
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
            AppioLogger.RegisterListener(loggerListenerMock.Object);
            var loggerCountAfterAdd = AppioLogger.LoggerListeners.Count();                       

            AppioLogger.RemoveListener(loggerListenerMock.Object);
            var loggerCountAfterRemove = AppioLogger.LoggerListeners.Count();

            // Assert: Verify the result of the test.            
            Assert.AreEqual(loggerCountAfterAdd, 1);
            Assert.AreEqual(loggerCountAfterRemove, 0);

            CleanupAppioLogger();
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

            AppioLogger.RegisterListener(loggerListenerMock.Object);
            AppioLogger.RegisterListener(loggerListenerMock2.Object);
            AppioLogger.RegisterListener(loggerListenerMock3.Object);

            // Act: Perform the action of the test.
            AppioLogger.RemoveAllListeners();

            // Assert: Verify the result of the test.
            Assert.AreEqual(AppioLogger.LoggerListeners.Count(), 0);
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
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act: Perform the action of the test.
            AppioLogger.Error(errorMessage, errorException);

            // Assert: Verify the result of the test.            
            Assert.IsTrue(errorWrittenOut);

            CleanupAppioLogger();
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
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act: Perform the action of the test.
            AppioLogger.Warn(warnMessage);

            // Assert: Verify the result of the test.            
            Assert.IsTrue(warnWrittenOut);

            CleanupAppioLogger();
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
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act: Perform the action of the test.
            AppioLogger.Info(infoMessage);

            // Assert: Verify the result of the test.            
            Assert.IsTrue(infoWrittenOut);

            CleanupAppioLogger();
        }

        /// <summary>
        /// Needs to be cleaned-up, because static context.
        /// </summary>
        private static void CleanupAppioLogger()
        {
            AppioLogger.RemoveAllListeners();
            Assert.AreEqual(AppioLogger.LoggerListeners.Count(), 0);
        }
    }
}