using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.VersionCommands;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class VersionStrategyTests
    {
        private static string[][] Inputs()
        {
            return new[]
            {
                new string[0],
                new[] {"whatever"},
                new[] {"-x"},
                new[] {"--any"},
            };
        }

        private static AssemblyInfo[][] AssemblyInfos()
        {
            return new[]
            {
                new[]
                {
                    new AssemblyInfo("assembly-1", new Version("0.0.0.0"), new Version("0.0.0.0")),
                },
                new[]
                {
                    new AssemblyInfo("assembly-1", new Version("0.0.0.0"), new Version("0.0.4.0")),
                    new AssemblyInfo("assembly-2", new Version("0.2.0.0"), new Version("0.0.0.0")),
                },
                new[]
                {
                    new AssemblyInfo("assembly-1", new Version("0.0.0.0"), new Version("0.0.4.0")),
                    new AssemblyInfo("assembly-4", new Version("0.5.0.6"), new Version("0.0.3.0")),
                    new AssemblyInfo("assembly-2", new Version("0.2.0.0"), new Version("0.0.0.7")),
                    new AssemblyInfo("assembly-5", new Version("9.0.0.0"), new Version("3.0.0.0")),
                    new AssemblyInfo("assembly-3", new Version("0.0.1.0"), new Version("0.1.0.0")),
                    new AssemblyInfo("assembly-6", new Version("0.1.4.3"), new Version("0.1.0.0")),
                },
            };
        }

        [Test]
        public void VersionStrategy_Should_ImplementICommandOfObjectModel()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var reflectionMock = new Mock<IReflection>();

            // Act
            var objectUnderTest = new VersionStrategy(reflectionMock.Object, writerMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommand<ObjectModel>>(objectUnderTest);
        }

        [Test]
        public void VersionStrategy_Should_PrintVersionInformation([ValueSource(nameof(Inputs))] string[] inputParams, [ValueSource(nameof(AssemblyInfos))] AssemblyInfo[] assemblyInfos)
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var reflectionMock = new Mock<IReflection>();
            reflectionMock.Setup(x => x.GetOppoAssemblyInfos()).Returns(assemblyInfos);
            var objectUnderTest = new VersionStrategy(reflectionMock.Object, writerMock.Object);
            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(x => x.Info(LoggingText.VersionCommandCalled));
            OppoLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            var versionRegex = new Regex(@"^\d+.\d+.\d+$");
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(string.Empty, result.Message);
            writerMock.Verify(x => x.WriteLines(It.Is<Dictionary<string, string>>(d => d.Values.All(v => versionRegex.IsMatch(v)))), Times.AtLeastOnce);
            loggerListenerMock.Verify(x => x.Info(LoggingText.VersionCommandCalled), Times.Once);
            OppoLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void VersionStrategy_Should_ReturnEmptyHelpText()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var reflectionMock = new Mock<IReflection>();
            var newStrategy = new VersionStrategy(reflectionMock.Object, writerMock.Object);

            // Act
            var helpText = newStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.VersionCommand);
        }

        [Test]
        public void VersionStrategy_Should_ReturnCommandName()
        {
            // Arrange
            var writerMock = new Mock<IWriter>();
            var reflectionMock = new Mock<IReflection>();
            var newStrategy = new VersionStrategy(reflectionMock.Object, writerMock.Object);

            // Act
            var commandName = newStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Version);
        }
    }
}
