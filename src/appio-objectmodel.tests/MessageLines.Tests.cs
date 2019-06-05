using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace Appio.ObjectModel.Tests
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

        [Test]
        public void Sort_ShouldSortContent()
        {
            // Arrange
            var obj = new MessageLines
            {
                { "-x", "any-text" },
                { "--a", "any-other-text" },
                { "d", "any-other-text-374573" },
                { "---c", "any-other-text-2" },
            };

            // Act
            obj.Sort();

            // Assert
            Assert.AreEqual(4, obj.Count());
            Assert.AreEqual("--a", obj.ElementAt(0).Key);
            Assert.AreEqual("any-other-text", obj.ElementAt(0).Value);
            Assert.AreEqual("---c", obj.ElementAt(1).Key);
            Assert.AreEqual("any-other-text-2", obj.ElementAt(1).Value);
            Assert.AreEqual("d", obj.ElementAt(2).Key);
            Assert.AreEqual("any-other-text-374573", obj.ElementAt(2).Value);
            Assert.AreEqual("-x", obj.ElementAt(3).Key);
            Assert.AreEqual("any-text", obj.ElementAt(3).Value);
        }
    }
}