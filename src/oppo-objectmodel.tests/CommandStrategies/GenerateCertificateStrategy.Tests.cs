using System;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.GenerateCommands;
using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class GenerateCertificateStrategyTests
    {
        private const uint DefKeySize = Constants.ExternalExecutableArguments.OpenSSLDefaultKeySize;
        private const uint DefDays = Constants.ExternalExecutableArguments.OpenSSLDefaultDays;
        private const string DefOrg = Constants.ExternalExecutableArguments.OpenSSLDefaultOrganization;

        private const string ServerDirectory = "server-directory";
        private const string ClientDirectory = "client-directory";
        private const string OppoprojFile = "oppoproj-file";
        
        private static readonly string SampleClientServerAppProject = $"{{\"name\": \"App\",\"type\": \"{Constants.ApplicationType.ClientServer}\"}}";
        private static readonly string SampleClientAppProject = $"{{\"name\": \"App\",\"type\": \"{Constants.ApplicationType.Client}\"}}";

        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ILoggerListener> _listenerMock;
        private Mock<AbstractCertificateGenerator> _certGenMock;
        private GenerateCertificateStrategy _objectUnderTest;
        
        // app name | key size | days | org name
        private static object[] _validInput =
        {
            new object[] {new [] {"-n", "MyApp"}, "MyApp", DefKeySize, DefDays, DefOrg},
            new object[] {new [] {"-n", "MyOtherApp", "--keysize", "2048"}, "MyOtherApp", 2048u, DefDays, DefOrg},
            new object[] {new [] {"-n", "MyThirdApp", "--keysize", "2048", "--organization", "TheOrg", "--days", "30"}, "MyThirdApp", 2048u, 30u, "TheOrg"}
        };
        
        private static object[] _invalidInputBadFlags =
        {
            new string[0],
            new [] {"-n", "MyApp", "--someFlag"},
            new [] {"-n", "MyApp", "--KeySize", "1024"}
        };
        
        private static object[] _invalidInputArgumentsNotParsable =
        {
            new [] {"-n", "MyApp", "--keysize", "ten-twenty-four"},
            new [] {"-n", "MyApp", "--days", "thirty"}
        };

        [SetUp]
        public void Setup()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _certGenMock = new Mock<AbstractCertificateGenerator>();
            _objectUnderTest = new GenerateCertificateStrategy(_fileSystemMock.Object, _certGenMock.Object);
            _listenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(_listenerMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            OppoLogger.RemoveListener(_listenerMock.Object);
        }

        [TestCaseSource(nameof(_validInput))]
        public void GenerateCorrectCertificateOnValidInput(string[] inputParams, string expectedAppName, uint expectedKeySize, uint expectedDays, string expectedOrg)
        {
            // arrange
            MockPathCombine(expectedAppName);
            var stream = MockFileRead(OppoprojFile, SampleClientAppProject);
            
            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(string.Format(OutputText.GenerateCertificateCommandSuccess, expectedAppName), result.OutputMessages.First().Key);
            _certGenMock.Verify(c => c.Generate(expectedAppName, string.Empty, expectedKeySize, expectedDays, expectedOrg), Times.Once);
            _listenerMock.Verify(l => l.Info(LoggingText.GenerateCertificateSuccess));
            
            stream.Close();
        }

        [TestCaseSource(nameof(_invalidInputBadFlags))]
        public void FailOnInvalidInputAndNotGenerateAnything(string[] inputParams)
        {
            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsFalse(result.Success);
            _certGenMock.Verify(c => c.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<string>()), Times.Never);
            _listenerMock.Verify(l => l.Warn(It.IsAny<string>()));
        }
        
        [TestCaseSource(nameof(_validInput))]
        public void GenerateCorrectCertificateOnValidInputWithClientServerApp(string[] inputParams, string expectedAppName, uint expectedKeySize, uint expectedDays, string expectedOrg)
        {
            // arrange
            MockPathCombine(expectedAppName);
            var stream = MockFileRead(OppoprojFile, SampleClientServerAppProject);

            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(string.Format(OutputText.GenerateCertificateCommandSuccess, expectedAppName), result.OutputMessages.First().Key);
            _certGenMock.Verify(c => c.Generate(expectedAppName, Constants.FileName.ClientCryptoPrefix, expectedKeySize, expectedDays, expectedOrg), Times.Once);
            _certGenMock.Verify(c => c.Generate(expectedAppName, Constants.FileName.ServerCryptoPrefix, expectedKeySize, expectedDays, expectedOrg), Times.Once);
            _listenerMock.Verify(l => l.Info(LoggingText.GenerateCertificateSuccess));
            
            stream.Close();
        }
        
        [TestCaseSource(nameof(_invalidInputArgumentsNotParsable))]
        public void FailWhenArgumentsAreNotParsable(string[] inputParams)
        {
            // arrange
            MockPathCombine("MyApp");
            var stream = MockFileRead(OppoprojFile, SampleClientAppProject);

            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.GenerateCertificateCommandFailureNotParsable, result.OutputMessages.First().Key);
            _certGenMock.Verify(c => c.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<string>()), Times.Never);
            _listenerMock.Verify(l => l.Warn(LoggingText.GenerateCertificateFailureNotParsable));

            stream.Close();
        }

        [Test]
        public void FailWhenProjectNotFound()
        {
            // arrange
            const string project = "UnknownProject";
            string[] inputParams = {"-n", project};
            MockPathCombine(project);
            _fileSystemMock.Setup(fs => fs.ReadFile(OppoprojFile)).Throws<Exception>();
            
            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.GenerateCertificateCommandFailureNotFound, project), result.OutputMessages.First().Key);
            _certGenMock.Verify(c => c.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<string>()), Times.Never);
            _listenerMock.Verify(l => l.Warn(string.Format(LoggingText.GenerateCertificateFailureNotFound, project)));
        }

        [Test]
        public void CorrectHelpText()
        {
            Assert.AreEqual(string.Empty, _objectUnderTest.GetHelpText());
        }

        private MemoryStream MockFileRead(string path, string contents)
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(contents));
            _fileSystemMock.Setup(fs => fs.ReadFile(path)).Returns(stream);
            return stream;
        }

        private void MockPathCombine(string appName)
        {
            _fileSystemMock.Setup(fs => fs.CombinePaths(appName, Constants.DirectoryName.ServerApp)).Returns(ServerDirectory);
            _fileSystemMock.Setup(fs => fs.CombinePaths(appName, Constants.DirectoryName.ClientApp)).Returns(ClientDirectory);
            _fileSystemMock.Setup(fs => fs.CombinePaths(appName, appName + Constants.FileExtension.OppoProject)).Returns(OppoprojFile);
        }
    }
}