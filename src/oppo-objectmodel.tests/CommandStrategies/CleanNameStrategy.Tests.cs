using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.CleanCommands;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class CleanNameStrategyTests
    {
        private static string[][] ValidInputs()
        {
            return new[]
            {
                new[]
                {
                    "-n",
                    "any-name",
                },
                new[]
                {
                    "--name",
                    "any-name",
                },
            };
        }

        private static string[][] InvalidInputs()
        {
            return new[]
            {
                new[]
                {
                    "-n",
                },
                new[]
                {
                    "-N",
                    "any-name",
                },
                new[]
                {
                    "-n",
                    "",
                },
                new[]
                {
                    "--name",
                },
                new[]
                {
                    "--Name",
                    "any-name",
                },
                new[]
                {
                    "--Name",
                    "",
                },
                new string[0], 
            };
        }

        private const string AnyCommandName = "any-name";

        private Mock<IFileSystem> _fileSystemMock;
        private CleanNameStrategy _objectUnderTest;

        [SetUp]
        public void SetUp_ObjectUnderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new CleanNameStrategy(AnyCommandName, _fileSystemMock.Object);
        }

        [Test]
        public void CleanNameStrategy_Should_ImplementICommandOfCleanStrategy()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsInstanceOf<ICommand<CleanStrategy>>(_objectUnderTest);
        }

        [Test]
        public void CleanNameStrategy_Should_HaveCorrectCommandName()
        {
            // Arrange

            // Act
            var name = _objectUnderTest.Name;

            // Assert
            Assert.AreEqual(AnyCommandName, name);
        }

        [Test]
        public void CleanNameStrategy_Should_HaveCorrectHelpText()
        {
            // Arrange

            // Act
            var helpText = _objectUnderTest.GetHelpText();

            // Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.CleanNameArgumentCommandDescription, helpText);
        }

        [Test]
        public void CleanNameStrategy_Should_CleanProjectOnValidProjectName([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            const string projectBuildDirectory = "build-dir";
            var projectName = inputParams.ElementAt(1);
            var resultMessage = string.Format(OutputText.OpcuaappCleanSuccess, projectName);

            _fileSystemMock.Setup(x => x.CombinePaths(projectName, Constants.DirectoryName.MesonBuild)).Returns(projectBuildDirectory);
            _fileSystemMock.Setup(x => x.DeleteDirectory(projectBuildDirectory));

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(result.Sucsess);
            Assert.AreEqual(resultMessage, result.Message);
        }

        [Test]
        public void CleanNameStrategy_Should_IgnoreInvalidParameters([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsFalse(result.Sucsess);
            Assert.AreEqual(OutputText.OpcuaappCleanFailure, result.Message);
        }
    }
}
