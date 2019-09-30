/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.HelpCommands;
using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
    public class HelpStrategyTests
    {
        [Test]
        public void HelpData_Clone_ShouldPerformDeepClone()
        {
            // Arrange
            const string commandName = "any-name";
            var helpTextFirstLine = new MessageLines { {"any-header-begin", "any-message-begin"} };
            var helpTextLastLine = new MessageLines { {"any-header-end", "any-message-end"} };
            const string helpText = "any-help-text";
            const string logMessage = "any-log-message";

            var obj = new HelpData
            {
                CommandName = commandName,
                HelpTextFirstLine = helpTextFirstLine,
                HelpTextLastLine = helpTextLastLine,
                HelpText = helpText,
                LogMessage = logMessage,
            };

            // Act
            var clone = obj.Clone();

            // Assert
            Assert.AreEqual(commandName, clone.CommandName);
            Assert.AreNotSame(helpTextFirstLine, clone.HelpTextFirstLine);
            Assert.AreEqual(1, helpTextFirstLine.Count());
            Assert.AreEqual(1, clone.HelpTextFirstLine.Count());
            Assert.AreEqual(1, helpTextLastLine.Count());
            Assert.AreEqual(1, clone.HelpTextLastLine.Count());
            Assert.AreEqual(helpTextFirstLine.First().Key, clone.HelpTextFirstLine.First().Key);
            Assert.AreEqual(helpTextFirstLine.First().Value, clone.HelpTextFirstLine.First().Value);
            Assert.AreEqual(helpTextLastLine.First().Key, clone.HelpTextLastLine.First().Key);
            Assert.AreEqual(helpTextLastLine.First().Value, clone.HelpTextLastLine.First().Value);
            Assert.AreNotSame(helpTextLastLine, clone.HelpTextLastLine);
            Assert.AreEqual(helpText, clone.HelpText);
            Assert.AreEqual(logMessage, clone.LogMessage);
        }

        [Test]
        public void HelpStrategy_Should_ImplementICommandOfAnyType()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName       = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine  = { { "any-other-text", "" } },
                LogMessage        = "any-message",
                HelpText          = "any-text",
            };

            // Act
            var objectUnderTest = new HelpStrategy<object>(helpData);

            // Assert
            Assert.IsInstanceOf<ICommand<object>>(objectUnderTest);
        }


		private Mock<ILoggerListener> _loggerListenerMock;
		private Mock<ICommand<object>> _commandMock;
		private Mock<ICommandFactory<object>> _factoryMock;

		private readonly string sampleCommandName			= "sampleCommandName";
		private readonly string sampleFirstLineText			= "sampleFirstLineText";
		private readonly string sampleOption				= "sampleOption";
		private readonly string	sampleOptionDescription		= "sampleOptionDescription";
		private readonly string sampleArgument				= "sampleArgument";
		private readonly string sampleArgumentDescription	= "sampleArgumentDescription";
		private readonly string sampleLastLineText			= "sampleLastLineText";
		private readonly string sampleLogMessage			= "sampleLogMessage";
		private readonly string sampleHelpText				= "sampleHelpText";

		[SetUp]
		public void HelpStrategy_Setup()
		{
			_loggerListenerMock = new Mock<ILoggerListener>();
			_commandMock = new Mock<ICommand<object>>();
			_commandMock.Setup(x => x.Name).Returns("any name");
			_commandMock.Setup(x => x.GetHelpText()).Returns("any \n help \n text");
			_factoryMock = new Mock<ICommandFactory<object>>();
			_factoryMock.Setup(x => x.Commands).Returns(new[] { _commandMock.Object });
		}

		[Test]
		public void HelpStrategy_Should_WriteHelpTextWithArguments()
		{
			// Arrange help data
			var helpData = new HelpData
			{
				CommandName = sampleCommandName,
				HelpTextFirstLine = { { sampleFirstLineText, string.Empty } },
				Arguments = { { sampleArgument, sampleArgumentDescription } },
				HelpTextLastLine = { { string.Empty, sampleLastLineText } },
				LogMessage = sampleLogMessage,
				HelpText = sampleHelpText,
			};

			AppioLogger.RegisterListener(_loggerListenerMock.Object);

			var helpStrategy = new HelpStrategy<object>(helpData);
			helpStrategy.CommandFactory = _factoryMock.Object;

			// Act
			var strategyResult = helpStrategy.Execute(new string[] { });

			// Assert
			Assert.IsTrue(strategyResult.Success);
			Assert.IsNotNull(strategyResult.OutputMessages);

			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(sampleFirstLineText, string.Empty)));
			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>("Arguments:", string.Empty)));
			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(sampleArgument, sampleArgumentDescription)));
			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(string.Empty, sampleLastLineText)));

			_loggerListenerMock.Verify(x => x.Info(sampleLogMessage), Times.Once);
			AppioLogger.RemoveListener(_loggerListenerMock.Object);
		}

		[Test]
		public void HelpStrategy_Should_WriteHelpTextWithOptions()
		{
			// Arrange help data
			var helpData = new HelpData
			{
				CommandName = sampleCommandName,
				HelpTextFirstLine = { { sampleFirstLineText, string.Empty } },
				Options = { { sampleOption, sampleOptionDescription } },
				HelpTextLastLine = { { string.Empty, sampleLastLineText } },
				LogMessage = sampleLogMessage,
				HelpText = sampleHelpText,
			};

			AppioLogger.RegisterListener(_loggerListenerMock.Object);
			
			var helpStrategy = new HelpStrategy<object>(helpData);
			helpStrategy.CommandFactory = _factoryMock.Object;

			// Act
			var strategyResult = helpStrategy.Execute(new string[] { });

			// Assert
			Assert.IsTrue(strategyResult.Success);
			Assert.IsNotNull(strategyResult.OutputMessages);

			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(sampleFirstLineText, string.Empty)));
			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>("Options:", string.Empty)));
			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(sampleOption, sampleOptionDescription)));
			Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(string.Empty, sampleLastLineText)));

			_loggerListenerMock.Verify(x => x.Info(sampleLogMessage), Times.Once);
			AppioLogger.RemoveListener(_loggerListenerMock.Object);
		}

        [Test]
        public void HelpStrategy_Should_WriteSparseHelpTextIfNoCommandFactoryIsProvided()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine = { { "", "any-other-text" } },
                LogMessage = "any-message",
                HelpText = "any-text",
            };

            var helpStrategy = new HelpStrategy<object>(helpData);

            // Act
            var strategyResult = helpStrategy.Execute(new string[] { });

            // Assert
            Assert.IsTrue(strategyResult.Success);
            Assert.IsNotNull(strategyResult.OutputMessages);
            Assert.AreEqual(string.Empty, strategyResult.OutputMessages.First().Value);
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>("any-text", string.Empty)));
            Assert.IsTrue(strategyResult.OutputMessages.Contains(new KeyValuePair<string, string>(string.Empty, "any-other-text")));
        }

        [Test]
        public void ShouldReturnHelpText()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine = { { "any-other-text", "" } },
                LogMessage = "any-message",
                HelpText = "any-text",
            };

            var helpStrategy = new HelpStrategy<object>(helpData);

            // Act
            var helpText = helpStrategy.GetHelpText();

            // Assert
            Assert.AreEqual("any-text", helpText);
        }
        
        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var helpData = new HelpData
            {
                CommandName = "any-name",
                HelpTextFirstLine = { { "any-text", "" } },
                HelpTextLastLine = { { "any-other-text", "" } },
                LogMessage = "any-message",
                HelpText = "any-text",
            };

            var helpStrategy = new HelpStrategy<object>(helpData);

            // Act
            var commandName = helpStrategy.Name;

            // Assert
            Assert.AreEqual("any-name", commandName);
        }
    }
}