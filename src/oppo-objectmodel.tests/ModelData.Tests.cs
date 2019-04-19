using Newtonsoft.Json;
using NUnit.Framework;

namespace Oppo.ObjectModel.Tests
{
    public class ModelDataShould
    {
        private IModelData _defaultModelData, _modelData;
        private string _name = "sampleModelName";
        private string _uri = "sampleModelUri";
		private string _types = "sampleModelTypes";
		private string _namespaceVariable = "sampleModelNamespaceVariable";

        [SetUp]
        public void SetupTest()
        {
			_defaultModelData = new ModelData();
			_modelData = new ModelData(_name, _uri, _types, _namespaceVariable);   
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
		}


		[Test]
        public void BeDeSerializableFromJson()
        {
            // Arrange
            var modelDataAsJson = "" +
                "{" +    
                    "\"name\": \"" + _name + "\"," +
					"\"uri\": \"" + _uri + "\"," +
                    "\"types\": \"" +  _types + "\"," +
					"\"namespaceVariable\": \"" + _namespaceVariable + "\"" +
                "}";

            // Act
            var modelData = JsonConvert.DeserializeObject<ModelData>(modelDataAsJson);

            // Assert
            Assert.IsNotNull(modelData);
            Assert.AreEqual(_name, modelData.Name);
			Assert.AreEqual(_uri, modelData.Uri);
			Assert.AreEqual(_types, modelData.Types);
			Assert.AreEqual(_namespaceVariable, modelData.NamespaceVariable);
		}
	}
}