using System.Collections;
using NUnit.Framework;

namespace Oppo.ObjectModel.Tests
{
    public class MessageLinesTests
    {

        [Test]
        public void ShouldInstatianteObject()
        {
            // Arrange
            
            // Act
            IEnumerable messageLine = new MessageLines();

            // Assert
            Assert.IsNotNull(messageLine.GetEnumerator());
        }
    }
}