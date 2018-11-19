using System.Collections;
using System.Linq;
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

        [Test]
        public void ShouldTestCopyConstructor_OK()
        {
            // Arrange
            var msgKey = "key";
            var msgValue = "value";
            var messageLineToCopy = new MessageLines() { { msgKey, msgValue } };

            // Act
            var messageLine = new MessageLines(messageLineToCopy);

            // Assert
            Assert.IsNotNull(messageLine);
            Assert.AreEqual(1, messageLine.Count());
            Assert.IsTrue(messageLine.All(v => v.Key.Contains(msgKey)));
            Assert.IsTrue(messageLine.All(v => v.Value.Contains(msgValue)));
        }

        [Test]
        public void ShouldTestAdd_OK()
        {
            // Arrange
            var msgKey = "key";
            var msgValue = "value";

            var messageLine = new MessageLines();
            var counBeforeAdd = messageLine.Count();

            // Act
            messageLine.Add(msgKey, msgValue);
            var counAfterAdd = messageLine.Count();

            // Assert
            Assert.IsNotNull(messageLine);
            Assert.AreEqual(0, counBeforeAdd);
            Assert.AreEqual(1, counAfterAdd);
            Assert.IsTrue(messageLine.All(v => v.Key.Contains(msgKey)));
            Assert.IsTrue(messageLine.All(v => v.Value.Contains(msgValue)));
        }
    }
}