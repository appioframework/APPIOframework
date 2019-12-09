/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using NUnit.Framework;

namespace Appio.ObjectModel.Tests
{
    public class ModelDataShould
    {
        private IModelData _defaultModelData, _modelData;
        private string _name = "sampleModelName";
        private string _uri = "sampleModelUri";
		private string _types = "sampleModelTypes";
        private string _typeDescriptions = "sampleModelTypeDescriptions";
        private string _namespaceVariable = "sampleModelNamespaceVariable";
		private List<string> _requiredModelUris = new List<string>(new string[] { "sampleRequiredModelUri_1", "sampleRequiredModelUri_2" });

        [SetUp]
        public void SetupTest()
        {
			_defaultModelData = new ModelData();
			_modelData = new ModelData(_name, _uri, _types, _typeDescriptions, _namespaceVariable, _requiredModelUris);   
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

        [Test]
        public void BeAsDefaultAnEmptyStructure()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.IsEmpty(_defaultModelData.Name);
			Assert.IsEmpty(_defaultModelData.Uri);
			Assert.IsEmpty(_defaultModelData.Types);
			Assert.IsEmpty(_defaultModelData.NamespaceVariable);
			Assert.IsEmpty(_defaultModelData.RequiredModelUris);
		}

        [Test]
        public void ContainAllPassedInitValues()
        {
            // Arrange

            // Act

            // Assert
            Assert.AreEqual(_name, _modelData.Name);
			Assert.AreEqual(_uri, _modelData.Uri);
			Assert.AreEqual(_types, _modelData.Types);
			Assert.AreEqual(_namespaceVariable, _modelData.NamespaceVariable);
			Assert.AreEqual(_requiredModelUris, _modelData.RequiredModelUris);
		}

        [Test]
        public void BeSerializableToJson()
        {
            // Arrange

            // Act
            var modelDataAsJson = JsonConvert.SerializeObject(_modelData, Formatting.Indented);
            
            // Assert
            Assert.IsNotNull(modelDataAsJson);
            Assert.AreNotEqual(string.Empty, modelDataAsJson);
            Assert.IsTrue(modelDataAsJson.Contains(_name));
			Assert.IsTrue(modelDataAsJson.Contains(_uri));
			Assert.IsTrue(modelDataAsJson.Contains(_types));
			Assert.IsTrue(modelDataAsJson.Contains(_namespaceVariable));
			foreach(var requiredModelUri in _requiredModelUris)
			{
				Assert.IsTrue(modelDataAsJson.Contains(requiredModelUri));
			}
		}


		[Test]
        public void BeDeSerializableFromJson()
        {
            // Arrange
            var modelDataAsJson = "" +
                "{" +    
                    "\"name\": \"" + _name + "\", " +
					"\"uri\": \"" + _uri + "\", " +
                    "\"types\": \"" +  _types + "\", " +
					"\"namespaceVariable\": \"" + _namespaceVariable + "\", " +
					"\"requiredModelUris\": [ \"" + _requiredModelUris[0] + "\", \"" + _requiredModelUris[1] + "\" ]" +
				"}";
			
			// Act
			var modelData = JsonConvert.DeserializeObject<ModelData>(modelDataAsJson);

            // Assert
            Assert.IsNotNull(modelData);
            Assert.AreEqual(_name, modelData.Name);
			Assert.AreEqual(_uri, modelData.Uri);
			Assert.AreEqual(_types, modelData.Types);
			Assert.AreEqual(_namespaceVariable, modelData.NamespaceVariable);
			Assert.AreEqual(_requiredModelUris, modelData.RequiredModelUris);
		}
	}
}