using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Appio.ObjectModel.Tests
{
    public class OpcuaClientServerAppShould
    {
        private IOpcuaClientServerApp _defaultopcuaApp, _opcuaapp;
        private string _name = "mvpSmartPump";
        private string _type = "ClientServer";
        private string _url = "localhost";
		private string _port = "4840";
		private List<IOpcuaServerApp> _references = new List<IOpcuaServerApp>();
		private List<IModelData> _models = new List<IModelData>();
		
        [SetUp]
        public void SetupTest()
        {
            _defaultopcuaApp = new OpcuaClientServerApp();
			_opcuaapp = new OpcuaClientServerApp(_name, _url, _port);
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

        [Test]
        public void BeAsDefaultOfClientServeType()
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
			Assert.AreEqual(_references, _opcuaapp.ServerReferences);
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
            Assert.IsTrue(opcuaappAsJson.Contains(_url)); // don't care where
			Assert.IsTrue(opcuaappAsJson.Contains(_port)); // don't care where
		}

        [Test]
        public void BeDeserializableFromJson()
        {
            // Arrange
            var opcuaappAsJson = "" +
                "{" +    
                    "\"name\": \"" + _name + "\"," +
                    "\"type\": \"" +  _type + "\"," +
                    "\"url\": \"" + _url + "\"," +
					"\"port\": \"" + _port + "\"," +
					"\"references\": []," +
					"\"models\": []" +
				"}";

            // Act
            var opcuaapp = JsonConvert.DeserializeObject<OpcuaClientServerApp>(opcuaappAsJson);

            // Assert
            Assert.IsNotNull(opcuaapp);
            Assert.AreEqual(_name, opcuaapp.Name);
            Assert.AreEqual(_type, opcuaapp.Type);
			Assert.AreEqual(_url, opcuaapp.Url);
			Assert.AreEqual(_port, opcuaapp.Port);
			Assert.AreEqual(_references, opcuaapp.ServerReferences);
			Assert.AreEqual(_models, opcuaapp.Models);
		}
    }
}