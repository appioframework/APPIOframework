/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

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
