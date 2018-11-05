using NUnit.Framework;
using Moq;

namespace Oppo.Terminal.Tests
{
    public class OppoTerminalTests
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
            var oppoTerminal = new OppoTerminal(mockWriter.Object, new string[0]);
            
            // Assert
            Assert.IsTrue(messageWrittenOut, "Should be true");
        }
    }
}