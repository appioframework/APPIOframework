using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class OpcuaServerAppShould
    {
        private IOpcuaServerApp _defaultopcuaApp, _opcuaapp;
        private string _name = "mvpSmartPump";
        private string _type = "Server";
        private string _url = "localhost:4848";

        [SetUp]
        public void SetupTest()
        {
            _defaultopcuaApp = new OpcuaServerApp();
            _opcuaapp = new OpcuaServerApp(_name, _url);   
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
        }

        [Test]
        public void BeDeSerializableFromJson()
        {
            // Arrange
            var opcuaappAsJson = "" +
                "{" +    
                    "\"name\": \"" + _name + "\"," +
                    "\"type\": \"" +  _type + "\"," +
                    "\"url\": \"" + _url + "\"" +
                "}";

            // Act
            var opcuaapp = JsonConvert.DeserializeObject<OpcuaServerApp>(opcuaappAsJson);

            // Assert
            Assert.IsNotNull(opcuaapp);
            Assert.AreEqual(_name, opcuaapp.Name);
            Assert.AreEqual(_type, opcuaapp.Type);
        }
    }
}