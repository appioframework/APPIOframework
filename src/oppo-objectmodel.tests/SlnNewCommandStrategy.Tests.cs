using NUnit.Framework;
using Moq;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class SlnNewCommandStrategyTests
    {
        private static string[][] InvalidInputs()
        {
            return new[]
            {
                new []{ "-n", "" },
                new []{ "-n", "ab/yx" },
                new []{ "-n", "ab\\yx" },
                new []{ "-N", "ab/yx" },
                new []{  "", "" },
                new []{ ""},
                new []{ "-n" },
                new string[]{}
            };
        }

        private static string[][] ValidInputs()
        {
            return new[]
            {
                new []{ "-n", "anyName" },
                new []{ "-n", "ABC" },
                new []{ "--name", "ABC" }
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy_Success([ValueSource(nameof(ValidInputs))] string[] inputParams)
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();                        
            var slnNewCommandStrategy = new SlnNewCommandStrategy(mockFileSystemMock.Object);
            var calculatedFileName = $"{inputParams.Skip(1).FirstOrDefault()}{Constants.FileExtension.OppoSln}";

            // Act
            var strategyResult = slnNewCommandStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);
            mockFileSystemMock.Verify(mf => mf.CreateFile(calculatedFileName, It.IsAny<string>()), Times.Once);
            mockFileSystemMock.Verify(mf => mf.LoadTemplateFile(Oppo.Resources.Resources.OppoSlnTemplateFileName), Times.Once);
        }

        [Test]
        public void ShouldExcecuteStrategy_Fail_MissingParameter([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();            
            var slnNewCommandStrategy = new SlnNewCommandStrategy(mockFileSystemMock.Object);
            mockFileSystemMock.Setup(fileWrapper => fileWrapper.GetInvalidFileNameChars()).Returns(new[] { '/', '\\' });

            // Act
            var strategyResult = slnNewCommandStrategy.Execute(inputParams);

            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Failure);
            mockFileSystemMock.Verify(mf => mf.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockFileSystemMock.Verify(mf => mf.LoadTemplateFile(Oppo.Resources.Resources.OppoSlnTemplateFileName), Times.Never);
        }
    }
}