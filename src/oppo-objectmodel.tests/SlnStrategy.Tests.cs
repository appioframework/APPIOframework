using NUnit.Framework;
using Oppo.ObjectModel;
using Oppo.ObjectModel.CommandStrategies;
using Moq;
using System.Collections.Generic;

namespace Oppo.ObjectModel.Tests
{
    public class SlnSrategyTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldExcecuteStrategy()
        {
            // Arrange
            var mockFileSystemMock = new Mock<IFileSystem>();
            var slnStretegy = new SlnStrategy(mockFileSystemMock.Object);
            
            // Act
            var strategyResult = slnStretegy.Execute(new List<string>());
            
            // Assert
            Assert.AreEqual(strategyResult, Constants.CommandResults.Success);      
            mockFileSystemMock.Verify(mf=>mf.CreateFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);       
        }
    }
}