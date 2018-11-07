using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;

namespace Oppo.ObjectModel.Tests
{
    public class PublishStrategyTests
    {
        private static string[][] ValidInputsPublishApplication()
        {
            return new[]
            {
                new[] {"-n", "hugo"},
                new[] {"--name", "hugo"},
                new[] {"-n", "another"},
                new[] {"--name", "app-name"},
            };
        }

        private static string[][] InvalidInputsPublishApplication()
        {
            return new[]
            {
                new[] {"-n", ""},
                new[] {"-a", "hugo"},
                new[] {"-a", ""},
                new[] {"--name", ""},
                new[] {"--any", "hugo"},
                new[] {"--any", ""},
                new[] {"-n"},
                new[] {"-x"},
                new[] {"--name"},
                new[] {"--exit"},
            };
        }

        [Test]
        public void PublishStrategy_ShouldImplement_ICommandStrategy()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();

            // Act
            var objectUnderTest = new PublishStrategy(fileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<ICommandStrategy>(objectUnderTest);
        }

        [Test]
        public void PublishStrategy_ShouldCreate_PublishDirectoryContainingApplicationFiles([ValueSource(nameof(ValidInputsPublishApplication))] string[] inputParams)
        {
            // Arrange
            var applicationName = inputParams.ElementAt(1);
            const string buildDirectory = "build";
            const string publishDirectory = "publish";
            const string applicationSourcePath = "source";
            const string applicationTargetPath = "target";
            
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.Publish)).Returns(publishDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(applicationName, Constants.DirectoryName.MesonBuild)).Returns(buildDirectory);
            fileSystemMock.Setup(x => x.CombinePaths(buildDirectory, Constants.ExecutableName.App)).Returns(applicationSourcePath);
            fileSystemMock.Setup(x => x.CombinePaths(publishDirectory, Constants.ExecutableName.App)).Returns(applicationTargetPath);
            var objectUnderTest = new PublishStrategy(fileSystemMock.Object);
            
            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Success, result);
            fileSystemMock.Verify(x => x.CreateDirectory(publishDirectory), Times.Once);
            fileSystemMock.Verify(x => x.CopyFile(applicationSourcePath, applicationTargetPath), Times.Once);
        }

        [Test]
        public void PublishStrategy_ShouldIgnore_MissingOrInvalidArguments([ValueSource(nameof(InvalidInputsPublishApplication))] string[] inputParams)
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new PublishStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Failure, result);
        }

        [Test]
        public void ShouldReturnEmptyHelpText()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var publishStrategy = new PublishStrategy(fileSystemMock.Object);

            // Act
            var helpText = publishStrategy.GetHelpText();

            // Assert
            Assert.AreEqual(helpText, Resources.text.help.HelpTextValues.PublishCommand);
        }

        [Test]
        public void ShouldReturnCommandName()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            var publishStrategy = new PublishStrategy(fileSystemMock.Object);

            // Act
            var commandName = publishStrategy.Name;

            // Assert
            Assert.AreEqual(commandName, Constants.CommandName.Publish);
        }
    }
}