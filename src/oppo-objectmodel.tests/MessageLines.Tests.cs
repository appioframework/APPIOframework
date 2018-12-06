using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace Oppo.ObjectModel.Tests
{
    public class MessageLinesTests
    {
        [Test]
        public void ShouldInstantiateObject()
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
            var messageLineToCopy = new MessageLines { { msgKey, msgValue } };

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
            var countBeforeAdd = messageLine.Count();

            // Act
            messageLine.Add(msgKey, msgValue);
            var countAfterAdd = messageLine.Count();

            // Assert
            Assert.IsNotNull(messageLine);
            Assert.AreEqual(0, countBeforeAdd);
            Assert.AreEqual(1, countAfterAdd);
            Assert.IsTrue(messageLine.All(v => v.Key.Contains(msgKey)));
            Assert.IsTrue(messageLine.All(v => v.Value.Contains(msgValue)));
        }

        [Test]
        public void Add_ShouldCopyOtherMessageLines()
        {
            // Arrange
            var msgKey = "key";
            var msgValue = "value";
            var messageLineToAdd = new MessageLines { { msgKey, msgValue } };

            var messageLine = new MessageLines();
            var countBeforeAdd = messageLine.Count();

            // Act
            messageLine.Add(messageLineToAdd);
            var countAfterAdd = messageLine.Count();

            // Assert
            Assert.IsNotNull(messageLine);
            Assert.AreEqual(0, countBeforeAdd);
            Assert.AreEqual(1, countAfterAdd);
            Assert.IsTrue(messageLine.All(v => v.Key.Contains(msgKey)));
            Assert.IsTrue(messageLine.All(v => v.Value.Contains(msgValue)));
        }
    }
}