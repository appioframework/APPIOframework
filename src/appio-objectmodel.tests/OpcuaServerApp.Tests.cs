using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Appio.ObjectModel.Tests
{
    public class OpcuaServerAppShould
    {
        private IOpcuaServerApp _defaultopcuaApp, _opcuaapp;
        private string _name = "mvpSmartPump";
        private string _type = "Server";
        private string _url = "localhost";
		private string _port = "4840";
		private List<IModelData> _models = new List<IModelData>();


        [SetUp]
        public void SetupTest()
        {
            _defaultopcuaApp = new OpcuaServerApp();
            _opcuaapp = new OpcuaServerApp(_name, _url, _port);   
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

        [Test]
        public void BeAsDefaultOfServeType()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.AreEqual(_type, _defaultopcuaApp.Type);
        }

        [Test]
        public void ContainAllPassedInitValues()
        {
            // Arrange

            // Act

            // Assert
            Assert.AreEqual(_name, _opcuaapp.Name);
            Assert.AreEqual(_type, _opcuaapp.Type);
			Assert.AreEqual(_url, _opcuaapp.Url);
			Assert.AreEqual(_port, _opcuaapp.Port);
			Assert.AreEqual(_models, _opcuaapp.Models);
        }

        [Test]
        public void BeSerializableToJson()
        {
            // Arrange

            // Act
            var opcuaappAsJson = JsonConvert.SerializeObject(_opcuaapp, Formatting.Indented);
            
            // Assert
            Assert.IsNotNull(opcuaappAsJson);
            Assert.AreNotEqual(string.Empty, opcuaappAsJson);
            Assert.IsTrue(opcuaappAsJson.Contains(_name)); // don't care where
			Assert.IsTrue(opcuaappAsJson.Contains(_type));
            Assert.IsTrue(opcuaappAsJson.Contains(_url)); // don't care where
			Assert.IsTrue(opcuaappAsJson.Contains(_port)); // don't care where
		}

        [Test]
        public void BeDeSerializableFromJson()
        {
            // Arrange
            var opcuaappAsJson = "" +
                "{" +    
                    "\"name\": \"" + _name + "\"," +
                    "\"type\": \"" +  _type + "\"," +
                    "\"url\": \"" + _url + "\"," +
					"\"port\": \"" + _port + "\"," +
					"\"models\": []" +
                "}";

            // Act
            var opcuaapp = JsonConvert.DeserializeObject<OpcuaServerApp>(opcuaappAsJson);

            // Assert
            Assert.IsNotNull(opcuaapp);
            Assert.AreEqual(_name, opcuaapp.Name);
            Assert.AreEqual(_type, opcuaapp.Type);
			Assert.AreEqual(_url, opcuaapp.Url);
			Assert.AreEqual(_port, opcuaapp.Port);
			Assert.AreEqual(_models, opcuaapp.Models);
		}
    }
}