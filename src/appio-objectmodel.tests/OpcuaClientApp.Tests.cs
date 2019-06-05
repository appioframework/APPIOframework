using Newtonsoft.Json;
using NUnit.Framework;

namespace Appio.ObjectModel.Tests
{
    public class OpcuaClientAppShould
    {
        private IOpcuaClientApp _defaultopcuaApp, _opcuaapp;
        private string _name = "mvpSmartPump";
        private string _type = "Client";

        [SetUp]
        public void SetupTest()
        {
            _defaultopcuaApp = new OpcuaClientApp();
            _opcuaapp = new OpcuaClientApp(_name);   
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

        [Test]
        public void BeAsDefaultOfClientType()
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
        }

      
        [Test]
        public void BeDeSerializableFromJson()
        {
            // Arrange
            var opcuaappAsJson = "" +
                "{" +    
                    "\"name\": \"" + _name + "\"," +
                    "\"type\": \"" +  _type + "\"" +
                "}";

            // Act
            var opcuaapp = JsonConvert.DeserializeObject<OpcuaClientApp>(opcuaappAsJson);

            // Assert
            Assert.IsNotNull(opcuaapp);
            Assert.AreEqual(_name, opcuaapp.Name);
            Assert.AreEqual(_type, opcuaapp.Type);
        }
    }
}