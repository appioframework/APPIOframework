using NUnit.Framework;
using OPPO.Terminal;
using Moq;

namespace OPPO.Terminal.Tests
{
    public class WriterTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldWriteMessage()
        {
            // Arrange
            var messageWrittenOut = false;
            var mockWriter = new Mock<IWriter>();
            mockWriter.Setup(mw => mw.WriteLine(It.IsAny<string>())).Callback(() => messageWrittenOut = true);
            
            // Act
            var oppoTerminal = new OppoTerminal(mockWriter.Object);
            
            // Assert
            Assert.IsTrue(messageWrittenOut, "Should be true");
        }
    }
}