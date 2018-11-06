﻿using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.Tests
{
    public class NewSlnCommandStrategyTests
    {
        private static string[][] ValidInputs()
        {
            return new[]
            {
                new[] {"-n", "anyName"},
                new[] {"-n", "ABC"},
                new[] {"--name", "ABC"}
            };
        }

        private static string[][] InvalidInputs()
        {
            return new[]
            {
                new[] {"-n", ""},
                new[] {"-n", "ab/yx"},
                new[] {"-n", "ab\\yx"},
                new[] {"-N", "ab/yx"},
                new[] {"", ""},
                new[] {""},
                new[] {"-n"},
                new string[] { }
            };
        }

        [Test]
        public void NewSlnCommandStrategy_ShouldImplement_INewCommandStrategy()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();

            // Act
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<INewCommandStrategy>(objectUnderTest);
        }

        [Test]
        public void NewSlnCommandStrategy_ShouldCreate_SlnAndProjectRelevantFiles([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var slnFileName = $"{inputParams.Skip(1).First()}{Constants.FileExtension.OppoSln}";

            var fileSystemMock = new Mock<IFileSystem>();
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Success, result);
            fileSystemMock.Verify(x => x.CreateFile(slnFileName, It.IsAny<string>()), Times.Once);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName), Times.Once);
        }

        [Test]
        public void NewSlnCommandStrategy_ShouldIgnore_Input([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var invalidCharsMock = new[] { '/', '\\' };
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidCharsMock);
            var objectUnderTest = new NewSlnCommandStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Failure, result);
            fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName), Times.Never);
        }
    }
}