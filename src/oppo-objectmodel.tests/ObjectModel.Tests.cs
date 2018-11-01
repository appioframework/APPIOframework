using NUnit.Framework;
using Oppo.ObjectModel;
using Moq;
using System.Collections.Generic;
using System;

namespace Oppo.ObjectModel.Tests
{
    public class ObjectModelTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldGetValidInputParams()
        {
            // Arrange
            var inputParams = new List<string>(){"sln", "new", "-n", "testslns"};

            // Act
            var objectModel = new ObjectModel(inputParams);
                        
            // Assert
            Assert.Pass();
        }

        [Test]
        public void ShouldGetInvalidInputParams()
        {
            // Arrange
            List<string> inputParams = null;

            // Act
                                    
            // Assert
            Assert.Throws<ArgumentNullException>(() => new ObjectModel(inputParams));
        }
    }
}