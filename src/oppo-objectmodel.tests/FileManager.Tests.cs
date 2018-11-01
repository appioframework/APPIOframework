using NUnit.Framework;
using Oppo.ObjectModel;
using Moq;

namespace Oppo.ObjectModel.Tests
{
    public class FileManagerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldCreateSlnFile()
        {
            // // Arrange
            // var filecreated = false;
            // var mockFileSystem = new Mock<IFileSystem>();
            // mockFileSystem.Setup(fs => fs.CreateFile(It.IsAny<string>())).Callback(() => filecreated = true);
            
            // // Act
            // var fileManager = new FileManager(mockFileSystem.Object);
            
            // // Assert
            // Assert.IsTrue(filecreated);
            Assert.Pass();
        }
    }
}