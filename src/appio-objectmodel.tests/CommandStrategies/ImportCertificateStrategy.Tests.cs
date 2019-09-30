/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.ImportCommands;
using Appio.Resources.text.logging;
using Appio.Resources.text.output;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
    public class ImportCertificateStrategyTests
    {
        private ImportCertificateStrategy _objectUnderTest;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ILoggerListener> _loggerListenerMock;
        
        private static object[] ValidInputsDER =
        {
            new [] {"--key", "myfile.der", "--certificate", "mycert.der", "-n", "myproject"}
        };
        
        private static object[] ValidInputsPEMFormat =
        {
            new [] {"--key", "myfile.key", "--certificate", "mycert.pem", "-n", "myproject"},
            new [] {"-k", "myfile.key", "-c", "mycert.pem", "-n", "myproject"},
            new [] {"-k", "myfile.key", "--certificate", "mycert.pem", "-n", "myproject"},
            new [] {"-k", "myfile.key", "--certificate", "mycert.crt", "-n", "myproject"}
        };

        private static object[] InvalidInputsCaughtByCommandLineParser =
        {
            new[] {"-Key", "myfile.der", "--certificate", "mycert.der", "-n", "myproject"},
            new[] {"-k", "myfile.der", "--certificate", "mycert.der"},
        };

        private static object[] ValidInputsDERClientServer =
        {
            new [] {"--client", "--key", "myfile.der", "--certificate", "mycert.der", "-n", "myproject"},
            new [] {"--server", "--key", "myfile.der", "--certificate", "mycert.der", "-n", "myproject"}
        };
        
        private static object[] InvalidBothClientServer =
        {
            new [] {"--client", "--server", "--key", "myfile.der", "--certificate", "mycert.der", "-n", "myproject"}
        };
        
        private static readonly string SampleClientServerAppProject = $"{{\"name\": \"App\",\"type\": \"{Constants.ApplicationType.ClientServer}\"}}";
        private static readonly string SampleClientAppProject = $"{{\"name\": \"App\",\"type\": \"{Constants.ApplicationType.Client}\"}}";

        const string AppioprojPath = "appioproj-path";
        const string CertificateFolder = "certificate-folder";
        const string TargetCertPath = "certificate-path";
        const string TargetKeyPath = "key-path";

        [SetUp]
        public void Setup()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _objectUnderTest = new ImportCertificateStrategy(_fileSystemMock.Object);
                    
            _loggerListenerMock = new Mock<ILoggerListener>();
            AppioLogger.RegisterListener(_loggerListenerMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            AppioLogger.RemoveListener(_loggerListenerMock.Object);
        }

        [TestCaseSource(nameof(ValidInputsDER))]
        public void CopiesOverFilesWithRightFileType(string[] inputParams)
        {
            // arrange
            var keyFile = inputParams[1];
            var certFile = inputParams[3];
            var project = inputParams[5];
            
            MockPathCombine(project);
            _fileSystemMock.Setup(fs => fs.CombinePaths(CertificateFolder, Constants.FileName.Certificate)).Returns(TargetCertPath);
            _fileSystemMock.Setup(fs => fs.CombinePaths(CertificateFolder, Constants.FileName.PrivateKeyDER)).Returns(TargetKeyPath);
            var stream = MockFileRead(AppioprojPath, SampleClientAppProject);
            
            // act
            var result = _objectUnderTest.Execute(inputParams);

            // assert
            Assert.IsTrue(result.Success);
            _fileSystemMock.Verify(fs => fs.CreateDirectory(CertificateFolder), Times.Once);
            Assert.AreEqual(OutputText.ImportCertificateCommandSuccess,result.OutputMessages.First().Key);
            _fileSystemMock.Verify(fs => fs.CopyFile(keyFile, TargetKeyPath), Times.Once);
            _fileSystemMock.Verify(fs => fs.CopyFile(certFile, TargetCertPath), Times.Once);
            _loggerListenerMock.Verify(l => l.Info(string.Format(LoggingText.ImportCertificateSuccess, certFile, keyFile)));
            _loggerListenerMock.VerifyNoOtherCalls();
            stream.Close();
        }

        [TestCaseSource(nameof(InvalidInputsCaughtByCommandLineParser))]
        public void FailsWithInvalidInputs(string[] inputParams)
        {
            // arrange
            
            // act
            var result = _objectUnderTest.Execute(inputParams);

            // assert
            Assert.IsFalse(result.Success);
        }
        
        [TestCaseSource(nameof(ValidInputsPEMFormat))]
        public void ConvertsFilesWithExplicitParameter(string[] inputParams)
        {
            // arrange
            var keyFile = inputParams[1];
            var certFile = inputParams[3];
            var project = inputParams[5];
            
            MockPathCombine(project);
            var stream = MockFileRead(AppioprojPath, SampleClientAppProject);
            
            _fileSystemMock.Setup(fs => fs.CombinePaths(CertificateFolder, Constants.FileName.Certificate)).Returns(TargetCertPath);
            _fileSystemMock.Setup(fs => fs.CombinePaths(CertificateFolder, Constants.FileName.PrivateKeyDER)).Returns(TargetKeyPath);
            
            var openSSLCertArgs = string.Format(Constants.ExternalExecutableArguments.OpenSSLConvertCertificateFromPEM, certFile, TargetCertPath);
            var openSSLKeyArgs = string.Format(Constants.ExternalExecutableArguments.OpenSSLConvertKeyFromPEM, keyFile, TargetKeyPath);
            
            // act
            var result = _objectUnderTest.Execute(inputParams);

            // assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(OutputText.ImportCertificateCommandSuccess,result.OutputMessages.First().Key);
            _fileSystemMock.Verify(fs => fs.CreateDirectory(CertificateFolder), Times.Once);
            _fileSystemMock.Verify(fs => fs.CallExecutable(Constants.ExecutableName.OpenSSL, null, openSSLCertArgs), Times.Once);
            _fileSystemMock.Verify(fs => fs.CallExecutable(Constants.ExecutableName.OpenSSL, null, openSSLKeyArgs), Times.Once);
            _fileSystemMock.Verify(fs => fs.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _loggerListenerMock.Verify(l => l.Info(string.Format(LoggingText.ImportCertificateSuccess, certFile, keyFile)));
            _loggerListenerMock.VerifyNoOtherCalls();
            
            stream.Close();
        }
        
        [Test]
        public void CorrectHelpText()
        {
            Assert.AreEqual(string.Empty, _objectUnderTest.GetHelpText());
        }

        [TestCaseSource(nameof(ValidInputsDERClientServer))]
        public void ImportAsCorrectFileInClientServer(string[] inputParams)
        {
            var prefix = string.Concat(inputParams[0].Skip(2)) + "_";
            var keyFile = inputParams[2];
            var certFile = inputParams[4];
            var project = inputParams[6];
            
            _fileSystemMock.Setup(fs => fs.CombinePaths(CertificateFolder, prefix + Constants.FileName.Certificate)).Returns(TargetCertPath);
            _fileSystemMock.Setup(fs => fs.CombinePaths(CertificateFolder, prefix + Constants.FileName.PrivateKeyDER)).Returns(TargetKeyPath);

            // arrange
            MockPathCombine(project);
            
            var stream = MockFileRead(AppioprojPath, SampleClientServerAppProject);
            
            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsTrue(result.Success);
            _fileSystemMock.Verify(fs => fs.CreateDirectory(CertificateFolder), Times.Once);
            Assert.AreEqual(OutputText.ImportCertificateCommandSuccess,result.OutputMessages.First().Key);
            _loggerListenerMock.Verify(l => l.Info(string.Format(LoggingText.ImportCertificateSuccess, certFile, keyFile)));
            _loggerListenerMock.VerifyNoOtherCalls();
            
            _fileSystemMock.Verify(fs => fs.CopyFile(keyFile, TargetKeyPath), Times.Once);
            _fileSystemMock.Verify(fs => fs.CopyFile(certFile, TargetCertPath), Times.Once);
            
            stream.Close();
        }

        [TestCaseSource(nameof(ValidInputsDERClientServer))]
        public void FailWhenSpecifyingInstanceOnClientApp(string[] inputParams)
        {
            // arrange
            var isServer = inputParams[0] == "--server";
            var keyFile = inputParams[2];
            var certFile = inputParams[4];
            var project = inputParams[6];
            
            MockPathCombine(project);
            var stream = MockFileRead(AppioprojPath, SampleClientAppProject);
            
            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.ImportCertificateCommandWrongServerClient, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(fs => fs.CallExecutable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(fs => fs.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _loggerListenerMock.Verify(l => l.Warn(LoggingText.ImportCertificateFailureWrongClientServer));
            _loggerListenerMock.VerifyNoOtherCalls();
            
            stream.Close();
        }
        
        [TestCaseSource(nameof(ValidInputsDER))]
        public void FailWhenServerClientNotSpecified(string[] inputParams)
        {
            // arrange
            var project = inputParams[5];
            
            MockPathCombine(project);
            var stream = MockFileRead(AppioprojPath, SampleClientServerAppProject);
            
            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.ImportCertificateCommandWrongServerClient, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(fs => fs.CallExecutable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(fs => fs.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _loggerListenerMock.Verify(l => l.Warn(LoggingText.ImportCertificateFailureMissingClientServer));
            _loggerListenerMock.VerifyNoOtherCalls();
            stream.Close();
        }
        
        [TestCaseSource(nameof(InvalidBothClientServer))]
        public void FailWhenBothServerClientSpecified(string[] inputParams)
        {
            // arrange
            var project = inputParams[7];
            
            MockPathCombine(project);
            var stream = MockFileRead(AppioprojPath, SampleClientServerAppProject);
            
            // act
            var result = _objectUnderTest.Execute(inputParams);
            
            // assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(OutputText.ImportCertificateCommandWrongServerClient, result.OutputMessages.First().Key);
            _fileSystemMock.Verify(fs => fs.CallExecutable(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _fileSystemMock.Verify(fs => fs.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _loggerListenerMock.Verify(l => l.Warn(LoggingText.ImportCertificateFailureWrongClientServer));
            _loggerListenerMock.VerifyNoOtherCalls();
            stream.Close();
        }
        
        private MemoryStream MockFileRead(string path, string contents)
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(contents));
            _fileSystemMock.Setup(fs => fs.ReadFile(path)).Returns(stream);
            return stream;
        }

        private void MockPathCombine(string project)
        {
            _fileSystemMock.Setup(fs => fs.CombinePaths(project, Constants.DirectoryName.Certificates)).Returns(CertificateFolder);
            _fileSystemMock.Setup(fs => fs.CombinePaths(project, project + Constants.FileExtension.Appioproject)).Returns(AppioprojPath);
        }
    }
}