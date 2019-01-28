using NUnit.Framework;
using Moq;
using System;
using Oppo.Resources.text.logging;
using System.IO;

namespace Oppo.ObjectModel.Tests
{
    public class ModelValidatorShould
    {
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ILoggerListener> _loggerListenerMock;
        private readonly string _filePathToValidate = "anyFile.xml";
        private readonly string _fileNameToValidateAgainst = "anyFile.xsd";
        IModelValidator _validator;

        #region real testing file content

        private readonly string _xsdToValidateAgainst = Resources.StringResources.xsdFileToValidateAgainst;
        private readonly string _xmlToValidate_Valid = Resources.StringResources.xmlFileToValidate_Valid;
        private readonly string _xmlToValidate_Invalid = Resources.StringResources.xmlFileToValidate_Invalid;

        #endregion

        [SetUp]
        public void SetupTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _validator = new ModelValidator(_fileSystemMock.Object);
            _loggerListenerMock = new Mock<ILoggerListener>();
            OppoLogger.RegisterListener(_loggerListenerMock.Object);
        }

        [TearDown]
        public void CleanUpTest()
        {
            OppoLogger.RemoveListener(_loggerListenerMock.Object);
        }

        [Test]
        public void ValidateFileWithoutError()
        {
            // arrange          
            _fileSystemMock.Setup(f => f.LoadTemplateFile(_fileNameToValidateAgainst)).Returns(_xsdToValidateAgainst);

            using (var xmlToValidateStream = new MemoryStream())
            {
                var streamWriter = new StreamWriter(xmlToValidateStream);
                streamWriter.Write(_xmlToValidate_Valid);
                streamWriter.Flush();
                xmlToValidateStream.Position = 0;
                
                _fileSystemMock.Setup(f => f.ReadFile(_filePathToValidate)).Returns(xmlToValidateStream);

                // act
                var result = _validator.Validate(_filePathToValidate, _fileNameToValidateAgainst);
                
                // assert
                Assert.IsTrue(result);
                _loggerListenerMock.Verify(x => x.Info(string.Format(LoggingText.ValidatingModel, _filePathToValidate, _fileNameToValidateAgainst)), Times.Once);
                _loggerListenerMock.Verify(x => x.Error(string.Format(LoggingText.ValidationError, It.IsAny<string>()), It.IsAny<Exception>()), Times.Never);
            }           
        }

        [Test]
        public void ValidateFileWithError()
        {
            // arrange          
            _fileSystemMock.Setup(f => f.LoadTemplateFile(_fileNameToValidateAgainst)).Returns(_xsdToValidateAgainst);

            using (var xmlToValidateStream = new MemoryStream())
            {
                var streamWriter = new StreamWriter(xmlToValidateStream);
                streamWriter.Write(_xmlToValidate_Invalid);
                streamWriter.Flush();
                xmlToValidateStream.Position = 0;

                _fileSystemMock.Setup(f => f.ReadFile(_filePathToValidate)).Returns(xmlToValidateStream);

                // act
                var result = _validator.Validate(_filePathToValidate, _fileNameToValidateAgainst);

                // assert
                Assert.IsFalse(result);
                _loggerListenerMock.Verify(x => x.Info(string.Format(LoggingText.ValidatingModel, _filePathToValidate, _fileNameToValidateAgainst)), Times.Once);
                _loggerListenerMock.Verify(x => x.Error(string.Format(LoggingText.ValidationError, It.IsAny<string>()), It.IsAny<Exception>()), Times.Once);
            }
        }
    }
}