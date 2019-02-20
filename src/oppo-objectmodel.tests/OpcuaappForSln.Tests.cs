using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;

namespace Oppo.ObjectModel.Tests
{
    public class OpcuaappForSlnShould
    {
        private OpcuaappForSln _defaultopcuaAppForSln, _opcuaappForSln;
        private string _name = "mvpSmartPump";
        private string _path = "mvpSmartPump/mvpSmartPump.oppoproj";
        private string _url = "opc.tcp://127.0.1.1:4840";
        private OpcuaappType _type = OpcuaappType.Server;

        [SetUp]
        public void SetupTest()
        {
            _defaultopcuaAppForSln = new OpcuaappForSln();
            _opcuaappForSln = new OpcuaappForSln(new Opcuaapp(_name, _type, _url), _path);   
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

        [Test]
        public void BeAsDefaultOfClientServerType()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.AreEqual(OpcuaappType.ClientServer, _defaultopcuaAppForSln.Type);
        }

        [Test]
        public void ContainAllPassedInitValues()
        {
            // Arrange

            // Act

            // Assert
            Assert.AreEqual(_name, _opcuaappForSln.Name);
            Assert.AreEqual(_path, _opcuaappForSln.Path);
            Assert.AreEqual(_type, _opcuaappForSln.Type);
            Assert.AreEqual(_url, _opcuaappForSln.Url);
        }

        [Test]
        public void BeSerializableToJson()
        {
            // Arrange

            // Act
            var opcuaappAsJson = JsonConvert.SerializeObject(_opcuaappForSln, Formatting.Indented);
            
            // Assert
            Assert.IsNotNull(opcuaappAsJson);
            Assert.AreNotEqual(string.Empty, opcuaappAsJson);
        }

        [Test]
        public void ContainOneProjectAndBeSerializableToJson()
        {
            // Arrange

            // Act
            var opcuaappAsJson = JsonConvert.SerializeObject(_opcuaappForSln, Formatting.Indented);

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
            var opcuaappForSlnAsJson = "" +
                "{" +    
                    "\"Name\": \"" + _name + "\"," +
                    "\"Path\": \"" + _path + "\"," +
                    "\"Type\": \"" +  _type.ToString() + "\"," +
                    "\"Url\": \"" + _url + "\"" +
                "}";

            // Act
            OpcuaappForSln opcuaappForSln = JsonConvert.DeserializeObject<OpcuaappForSln>(opcuaappForSlnAsJson);

            // Assert
            Assert.IsNotNull(opcuaappForSln);
            Assert.AreEqual(_name, opcuaappForSln.Name);
            Assert.AreEqual(_path, opcuaappForSln.Path);
            Assert.AreEqual(_type, opcuaappForSln.Type);
            Assert.AreEqual(_url, opcuaappForSln.Url);
        }

        [Test]
        public void BeOfTypeClientAndDontHaveUrlAndBeSerializableFromJson()
        {
            // Arrange
            var opcuaappForSlnAsJson = "" +
                "{" +
                    "\"Name\": \"" + _name + "\"," +
                    "\"Path\": \"" + _path + "\"," +
                    "\"Type\": \"" + OpcuaappType.Client.ToString() + "\"" +
                "}";

			// Act
			OpcuaappForSln opcuaappForSln = JsonConvert.DeserializeObject<OpcuaappForSln>(opcuaappForSlnAsJson);

            // Assert
            Assert.IsNotNull(opcuaappForSln);
            Assert.AreEqual(_name, opcuaappForSln.Name);
            Assert.AreEqual(_path, opcuaappForSln.Path);
            Assert.AreEqual(OpcuaappType.Client, opcuaappForSln.Type);
            Assert.IsNull(opcuaappForSln.Url);
        }

        [Test]
        public void BeOfTypeClientAndDontHaveUrlAndBeDeSerializableToJson()
        {
			// Arrange
			_opcuaappForSln.Name = _name;
			_opcuaappForSln.Path = _path;
			_opcuaappForSln.Type = OpcuaappType.Client;
			_opcuaappForSln.Url = null;

            // Act
            var opcuaappAsJson = JsonConvert.SerializeObject(_opcuaappForSln, Formatting.Indented);

            // Assert
            Assert.IsNotNull(opcuaappAsJson);
            Assert.AreNotEqual(string.Empty, opcuaappAsJson);
            Assert.IsTrue(opcuaappAsJson.Contains(_name)); // don't care where
            Assert.IsTrue(opcuaappAsJson.Contains(_path)); // don't care where
            Assert.IsTrue(opcuaappAsJson.Contains("\"url\": null")); // don't care where
        }

    }
}