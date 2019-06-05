using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;

namespace Appio.ObjectModel.Tests
{
    public class OpcuaappReferenceShould
    {
        private IOpcuaappReference _defaultopcuaApp, _opcuaapp;
        private string _name = "mvpSmartPump";
        private string _path = "dummy.appioproj";

        [SetUp]
        public void SetupTest()
        {
            _defaultopcuaApp = new OpcuaappReference();
            _opcuaapp = new OpcuaappReference(_name, _path);   
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

        [Test]
        public void BeAsDefaultNullType()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.IsNull(_defaultopcuaApp.Type);
        }

        [Test]
        public void ContainAllPassedInitValues()
        {
            // Arrange

            // Act

            // Assert
            Assert.AreEqual(_name, _opcuaapp.Name);
            Assert.AreEqual(_path, _opcuaapp.Path);
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
            Assert.IsTrue(opcuaappAsJson.Contains(_path)); // don't care where
        }

        [Test]
        public void BeDeSerializableFromJson()
        {
            // Arrange
            var opcuaappAsJson = "" +
                "{" +    
                    "\"name\": \"" + _name + "\"," +
                    "\"path\": \"" +  _path + "\"" +
                "}";

            // Act
            var opcuaapp = JsonConvert.DeserializeObject<OpcuaappReference>(opcuaappAsJson);

            // Assert
            Assert.IsNotNull(opcuaapp);
            Assert.AreEqual(_name, opcuaapp.Name);
            Assert.AreEqual(_path, opcuaapp.Path);
            Assert.IsNull(opcuaapp.Type);
        }
    }
}