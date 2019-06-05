using NUnit.Framework;

namespace Appio.Terminal.Tests
{
    public class ReflectionWrapperTests
    {
        [Test]
        public void ReflectionWrapper_Should_Construct()
        {
            // Arrange

            // Act
            var objectUnderTest = new ReflectionWrapper();

            // Assert
            Assert.IsInstanceOf<ReflectionWrapper>(objectUnderTest);
        }

        [Test]
        public void ReflectionWrapper_Should_ProvideValidInfosForResourcesObjectModelAndTerminal()
        {
            // Arrange
            var objectUnderTest = new ReflectionWrapper();

            // Act
            var infos = objectUnderTest.GetAppioAssemblyInfos();

            // Assert
            Assert.IsNotNull(infos);
            Assert.AreEqual(3, infos.Length);

            foreach (var info in infos)
            {
                Assert.IsNotNull(info.AssemblyName);
                Assert.IsNotNull(info.AssemblyFileVersion);
                Assert.IsNotNull(info.AssemblyVersion);
            }
        }
    }
}
