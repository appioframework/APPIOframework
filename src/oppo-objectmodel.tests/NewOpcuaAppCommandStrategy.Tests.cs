using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.Tests
{
    public class NewOpcuaAppCommandStrategyTests
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
            var objectUnderTest = new NewOpcuaAppCommandStrategy(fileSystemMock.Object);

            // Assert
            Assert.IsInstanceOf<INewCommandStrategy>(objectUnderTest);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_ShouldCreate_SlnAndProjectRelevantFiles([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var projectDirectoryName = $"{inputParams.Skip(1).First()}";
            var opcuaSourceCode = Path.Combine(projectDirectoryName, Constants.DirectoryName.SourceCode);
            var projectFileName = $"{inputParams.Skip(1).First()}{Constants.FileExtension.OppoProject}";
            var projectFilePath = Path.Combine(projectDirectoryName, projectFileName);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(f => f.CombinePaths(projectDirectoryName, projectFileName)).Returns(projectFilePath);
            fileSystemMock.Setup(f => f.CombinePaths(projectDirectoryName, Constants.DirectoryName.SourceCode)).Returns(opcuaSourceCode);
            fileSystemMock.Setup(f => f.CombinePaths(opcuaSourceCode, Constants.FileName.SourceCode_main_c)).Returns(Path.Combine(opcuaSourceCode, Resources.Resources.OppoOpcuaAppTemplateFileName_main_c));
            fileSystemMock.Setup(f => f.CombinePaths(opcuaSourceCode, Constants.FileName.SourceCode_open62541_c)).Returns(Path.Combine(opcuaSourceCode, Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_c));
            fileSystemMock.Setup(f => f.CombinePaths(opcuaSourceCode, Constants.FileName.SourceCode_open62541_h)).Returns(Path.Combine(opcuaSourceCode, Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_h));
            

            var objectUnderTest = new NewOpcuaAppCommandStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Success, result);
            fileSystemMock.Verify(x => x.CreateDirectory(projectDirectoryName), Times.Once);
            fileSystemMock.Verify(x => x.CreateDirectory(opcuaSourceCode), Times.Once);
            fileSystemMock.Verify(x => x.CreateFile(It.Is<string>(s=>s.StartsWith(opcuaSourceCode)), It.IsAny<string>()), Times.Exactly(3));
            fileSystemMock.Verify(x => x.CreateFile(projectFilePath, It.IsAny<string>()), Times.Once);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName), Times.Once);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_c), Times.Once);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_c), Times.Once);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_open62541_h), Times.Once);
        }

        [Test]
        public void NewOpcuaAppCommandStrategy_ShouldIgnore_Input([ValueSource(nameof(InvalidInputs))] string[] inputParams)
        {
            // Arrange
            var invalidNameCharsMock = new[] { '/' };
            var invalidPathCharsMock = new[] { '\\' };
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(invalidNameCharsMock);
            fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(invalidPathCharsMock);
            var objectUnderTest = new NewOpcuaAppCommandStrategy(fileSystemMock.Object);

            // Act
            var result = objectUnderTest.Execute(inputParams);

            // Assert
            Assert.AreEqual(Constants.CommandResults.Failure, result);
            fileSystemMock.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
            fileSystemMock.Verify(x => x.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName), Times.Never);
        }
    }
}
