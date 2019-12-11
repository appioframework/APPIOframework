/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.ImportCommands;
using Appio.Resources.text.logging;
using Appio.Resources.text.output;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
	public class ImportInformationModelCommandStrategyTestsShould
	{
		private readonly string _validModelExtension = ".xml";
		private readonly string _invalidModelExtension = ".txt";
		private readonly string _validTypesExtension = ".bsd";
		private readonly string _modelName = "model.xml";

		private static string[][] ValidInputs()
		{
			return new[]
			{
				new[] {"-n", "myApp", "-p", "model.xml"},
				new[] {"-n", "myApp", "--path", "model.xml"},
				new[] {"--name", "myApp", "-p", "model.xml"},
				new[] {"--name", "myApp", "--path", "model.xml"}
			};
		}

		private static string[][] ValidInputsLoadSample()
		{
			return new[]
			{
				new[] {"-n", "myApp", "-s"},
				new[] {"-n", "myApp", "--sample"}
			};
		}

		private static string[][] ValidInputsExtraTypes()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p", "model.xml", "-t", "types.bsd" },
				new[] { "-n", "myApp", "-p", "model.xml", "--types", "types.bsd" },
				new[] { "-n", "myApp", "--path", "model.xml", "-t", "types.bsd" },
				new[] { "-n", "myApp", "--path", "model.xml", "--types", "types.bsd" },
				new[] { "--name", "myApp", "-p", "model.xml", "-t", "types.bsd" },
				new[] { "--name", "myApp", "-p", "model.xml", "--types", "types.bsd" },
				new[] { "--name", "myApp", "--path", "model.xml", "-t", "types.bsd" },
				new[] { "--name", "myApp", "--path", "model.xml", "--types", "types.bsd" }
			};
		}

		private static string[][] InvalidInputsInvalidModelPath()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p", "ab/yx.xml"},
				new[] { "-n", "myApp", "--path", "ab\\yx.xml"},
			};
		}

		private static string[][] InvalidInputsInvalidModelFile()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p", "model.txt"},
				new[] { "-n", "myApp", "--path", "model.xxx"},
				new[] { "-n", "myApp", "--path", "model.xml.txt"}
			};
		}

		private static string[][] InvalidInputsInvalidOpcuaappName()
		{
			return new[]
			{
				new[] { "-n", "myApp/", "-p", "model.xml"},
				new[] { "-n", "my\\App", "--path", "model.xml"}
			};
		}

		private static string[][] InvalidInputsMissingInformationModel()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p"},
				new[] { "-n", "myApp", "--path"}
			};
		}
		
		private static string[][] InvalidInputsMissingPath()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-t", "types.bsd" },
				new[] { "-n", "myApp", "--types", "types.bsd" },
				new[] { "-n", "myApp", "-t", "types.bsd" },
				new[] { "-n", "myApp", "--types", "types.bsd" }
			};
		}

		private static string[][] InvalidInputsInvalidNameArgument()
		{
			return new[]
			{
				new[] { "-N", "myApp", "-p", "model.xml"},
				new[] { "--n", "myApp", "-p", "model.xml"},
				new[] { "-name", "myApp", "--path", "model.xml"},
				new[] { "--Name", "myApp", "--path", "model.xml"}
			};
		}

		private static string[][] InvalidInputsMissingExtraTypesName()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p", "model.xml", "-t" },
				new[] { "-n", "myApp", "-p", "model.xml", "--types" }
			};
		}

		private static string[][] InvalidInputsInvalidExtraTypesExtension()
		{
			return new[]
			{
				new[] { "-n", "myApp", "-p", "model.xml", "-t", "types" },
				new[] { "-n", "myApp", "-p", "model.xml", "-t", "types.txt" },
				new[] { "-n", "myApp", "-p", "model.xml", "-t", "types.xml" },
				new[] { "-n", "myApp", "-p", "model.xml", "-t", "types.typ" }
			};
		}

		private Mock<IFileSystem> _fileSystemMock;
		private Mock<IModelValidator> _modelValidatorMock;
		private ImportInformationModelCommandStrategy _objectUnderTest;

		private readonly string _defaultOpcuaClientAppContent = "{\"name\":\"clientApp\",\"type\":\"Client\",\"references\": []}";
		private readonly string _defaultOpcuaServerAppContent = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[]}";
		private readonly string _defaultOpcuaClientServerAppContent = "{\"name\":\"clientServerApp\",\"type\":\"ClientServer\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"references\": [],\"models\":[]}";

		private readonly string _sampleOpcuaServerAppContentWithName = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[{\"name\":\"model.xml\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"\"}]}";
		private readonly string _sampleOpcuaServerAppContentWithUri = "{\"name\":\"serverApp\",\"type\":\"Server\",\"url\":\"127.0.0.1\",\"port\":\"3000\",\"models\":[{\"name\":\"\",\"uri\": \"sample_namespace\",\"types\": \"\",\"namespaceVariable\": \"\"}]}";

		private readonly string _sampleNodesetContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
														"<UANodeSet xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" LastModified=\"2012-12-31T00:00:00Z\" xmlns=\"http://opcfoundation.org/UA/2011/03/UANodeSet.xsd\">" +
														"<NamespaceUris>" +
														"<Uri>sample_namespace</Uri>" +
														"<Uri>required_namespace</Uri>" +
														"</NamespaceUris>" +
														"<Models>" +
														"<Model ModelUri = \"sample_namespace\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\">" +
														"<RequiredModel ModelUri = \"http://opcfoundation.org/UA/\" Version=\"1.04\" PublicationDate=\"2016-12-31T00:00:00Z\" />" +
														"<RequiredModel ModelUri = \"required_namespace\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\" />" +
														"</Model>" +
														"</Models>" +
														"</UANodeSet>";
		private readonly string _emptyNodesetContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><UANodeSet />";

		[SetUp]
		public void SetupObjectUnderTest()
		{
			_fileSystemMock = new Mock<IFileSystem>();
			_modelValidatorMock = new Mock<IModelValidator>();
			_objectUnderTest = new ImportInformationModelCommandStrategy(_fileSystemMock.Object, _modelValidatorMock.Object);
		}

		[Test]
		public void ImplementICommandOfImportStrategy()
		{
			// Arrange

			// Act

			// Assert
			Assert.IsInstanceOf<ICommand<ImportStrategy>>(_objectUnderTest);
		}

		[Test]
		public void HaveCorrectCommandName()
		{
			// Arrange

			// Act
			var commandName = _objectUnderTest.Name;

			// Assert
			Assert.AreEqual(Constants.ImportCommandArguments.InformationModel, commandName);
		}

		[Test]
		public void ProvideEmptyHelpText()
		{
			// Arrange

			// Act
			var helpText = _objectUnderTest.GetHelpText();

			// Assert
			Assert.AreEqual(string.Empty, helpText);
		}

		[Test]
        public void Success_OnImportingModelToServer([ValueSource(nameof(ValidInputs))]string[] inputParams)
		{
			// Arrange

			var infoWrittenOut = false;
            var projectDirectory = $"{inputParams.ElementAt(1)}";
            var modelsDirectory = "models";
            
            var modelFilePath = $"{inputParams.ElementAt(3)}";
            var modelTargetPath = projectDirectory + "\\" + _modelName;
			var appioprojFilePath = Path.Combine(projectDirectory, projectDirectory + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
            _fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, _modelName)).Returns(modelTargetPath);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectDirectory)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectDirectory + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var serverAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(serverAppioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

					var loggerListenerMock = new Mock<ILoggerListener>();
				loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelFilePath))).Callback(delegate { infoWrittenOut = true; });
				AppioLogger.RegisterListener(loggerListenerMock.Object);

				// Act
				var result = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsTrue(result.Success);
				Assert.IsTrue(infoWrittenOut);
				Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandSuccess, inputParams.ElementAt(3)), result.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelTargetPath), Times.Once);
				AppioLogger.RemoveListener(loggerListenerMock.Object);
			}
		}

		[Test]
		public void Success_OnImportingModelToClientServer([ValueSource(nameof(ValidInputs))]string[] inputParams)
		{
			// Arrange

			var infoWrittenOut = false;
			var projectDirectory = $"{inputParams.ElementAt(1)}";
			var modelsDirectory = "models";

			var modelFilePath = $"{inputParams.ElementAt(3)}";
			var modelTargetPath = projectDirectory + "\\" + _modelName;
			var appioprojFilePath = Path.Combine(projectDirectory, projectDirectory + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, Constants.DirectoryName.Models)).Returns(modelsDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, _modelName)).Returns(modelTargetPath);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectDirectory)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectDirectory, projectDirectory + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var clientServerAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaClientServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(clientServerAppioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

				var loggerListenerMock = new Mock<ILoggerListener>();
				loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelFilePath))).Callback(delegate { infoWrittenOut = true; });
				AppioLogger.RegisterListener(loggerListenerMock.Object);

				// Act
				var result = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsTrue(result.Success);
				Assert.IsTrue(infoWrittenOut);
				Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandSuccess, inputParams.ElementAt(3)), result.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelTargetPath), Times.Once);
				AppioLogger.RemoveListener(loggerListenerMock.Object);
			}
		}

        [Test]
        public void Fail_OnInvalidOpcuaappName([ValueSource(nameof(InvalidInputsInvalidOpcuaappName))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetInvalidFileNameChars()).Returns(new[] { '\\', '/'});
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(LoggingText.InvalidOpcuaappName)).Callback(delegate { warnWrittenOut = true; });
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

		[Test]
		public void Fail_OnCallingNotExistingProject([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var opcuaAppName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var warnWrittenOut = false;
			var loggerListenerMock = new Mock<ILoggerListener>();
			loggerListenerMock.Setup(listener => listener.Warn(LoggingText.InvalidOpcuaappName)).Callback(delegate { warnWrittenOut = true; });
			AppioLogger.RegisterListener(loggerListenerMock.Object);

			// Act
			var result = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(result.Success);
			Assert.IsTrue(warnWrittenOut);
			Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), result.OutputMessages.First().Key);
			_fileSystemMock.Verify(x => x.CopyFile(modelFilePath, Constants.DirectoryName.Models), Times.Never);
			AppioLogger.RemoveListener(loggerListenerMock.Object);
		}

        [Test]
        public void Fail_OnInvalidModelPath([ValueSource(nameof(InvalidInputsInvalidModelPath))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetInvalidPathChars()).Returns(new[] { '\\', '/' });
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelPath, modelFilePath))).Callback(delegate { warnWrittenOut = true; });
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidModelPath, modelFilePath), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void Fail_OnInvalidModelName([ValueSource(nameof(InvalidInputsInvalidModelFile))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";
            
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_invalidModelExtension);
            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(_modelName);
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelExtension, _modelName))).Callback(delegate { warnWrittenOut = true; });
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandInvalidModelExtension, _modelName), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void Fail_OnMissingModelFile([ValueSource(nameof(ValidInputs))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";
            var modelFilePath = $"{inputParams.ElementAtOrDefault(3)}";
            var validModelExtension = ".xml";
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
            _fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(validModelExtension);
            _fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(false);
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(string.Format(LoggingText.InvalidInformationModelNotExistingPath, modelFilePath))).Callback(delegate { warnWrittenOut = true; });
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandNotExistingModelPath, modelFilePath), result.OutputMessages.First().Key);
            _fileSystemMock.Verify(x => x.CopyFile(modelFilePath, modelsDirectory), Times.Never);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void Fail_OnMissingModelFileParam([ValueSource(nameof(InvalidInputsMissingInformationModel))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;
            var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
            var modelsDirectory = "models";           
            
            _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
			_fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }
        
        [Test]
        public void Fail_OnMissingPathParam([ValueSource(nameof(InvalidInputsMissingPath))]string[] inputParams)
        {
	        // Arrange
	        var warnWrittenOut = false;
	        var opcuaAppName = $"{inputParams.ElementAtOrDefault(1)}";
	        var modelsDirectory = "models";           
            
	        _fileSystemMock.Setup(x => x.CombinePaths(opcuaAppName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
	        _fileSystemMock.Setup(x => x.DirectoryExists(opcuaAppName)).Returns(true);

	        var loggerListenerMock = new Mock<ILoggerListener>();
	        loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
	        AppioLogger.RegisterListener(loggerListenerMock.Object);

	        // Act
	        var result = _objectUnderTest.Execute(inputParams);

	        // Assert
	        Assert.IsTrue(warnWrittenOut);
	        Assert.IsFalse(result.Success);
	        AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

        [Test]
        public void Success_OnImportingSampleModel([ValueSource(nameof(ValidInputsLoadSample))]string[] inputParams)
        {
            // Arrange
            var infoWrittenOut = false;
			var projectName = inputParams.ElementAtOrDefault(1);
            var modelsDirectory = projectName + "\\" + "models";
			var appioprojFilePath = projectName + "\\" + projectName + ".appioproj";
			var loadedModel = "anyModelName";
			var loadedTypes = "anyTypesName";
			var sampleModelName = "DiNodeset.xml";
			var sampleTypesName = "DiTypes.bsd";
			var modelTargetPath = modelsDirectory + "\\" + sampleModelName;
			var typesTargetPath = modelsDirectory + "\\" + sampleTypesName;

			 _fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
			_fileSystemMock.Setup(x => x.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName)).Returns(loadedModel);
			_fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, sampleModelName)).Returns(modelTargetPath);
			_fileSystemMock.Setup(x => x.LoadTemplateFile(Resources.Resources.SampleInformationModelTypesFileName)).Returns(loadedTypes);
			_fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, sampleTypesName)).Returns(typesTargetPath);
			
			var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, sampleModelName))).Callback(delegate { infoWrittenOut = true; });
            AppioLogger.RegisterListener(loggerListenerMock.Object);


			using (var serverAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(serverAppioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelTargetPath)).Returns(nodesetFileStream);

				// Act
				var result = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsTrue(result.Success);
				Assert.IsTrue(infoWrittenOut);
				Assert.AreEqual(string.Format(OutputText.ImportSampleInformationModelSuccess, sampleModelName), result.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.CreateFile(modelTargetPath, loadedModel), Times.Once);
				_fileSystemMock.Verify(x => x.CreateFile(typesTargetPath, loadedTypes), Times.Once);
				_fileSystemMock.Verify(x => x.ReadFile(appioprojFilePath), Times.Once);
				_fileSystemMock.Verify(x => x.WriteFile(appioprojFilePath, It.IsAny<IEnumerable<string>>()));
				AppioLogger.RemoveListener(loggerListenerMock.Object);
			}
        }

        [Test]
        public void Fail_OnWrongNameFlag([ValueSource(nameof(InvalidInputsInvalidNameArgument))]string[] inputParams)
        {
            // Arrange
            var warnWrittenOut = false;

            var loggerListenerMock = new Mock<ILoggerListener>();
            loggerListenerMock.Setup(listener => listener.Warn(It.IsAny<string>())).Callback(delegate { warnWrittenOut = true; });
            AppioLogger.RegisterListener(loggerListenerMock.Object);

            // Act
            var result = _objectUnderTest.Execute(inputParams);

            // Assert
            Assert.IsTrue(warnWrittenOut);
            Assert.IsFalse(result.Success);
            AppioLogger.RemoveListener(loggerListenerMock.Object);
        }

		[Test]
		public void Fail_OnImportingToClient([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);
			
			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var clientAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaClientAppContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(clientAppioprojFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(OutputText.ImportInformationModelCommandOpcuaappIsAClient, commandResult.OutputMessages.First().Key);
			}
		}

		[Test]
		public void Fail_OnInvalidAppioprojFile([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var emptyAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(emptyAppioprojFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(OutputText.ImportInforamtionModelCommandFailureCannotReadAppioprojFile, commandResult.OutputMessages.First().Key);
			}
		}

		[Test]
		public void Fail_OnModelDuplication([ValueSource(nameof(ValidInputs))] string [] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);
			var modelName = Path.GetFileName(modelFilePath);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);
			_fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(modelName);

			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var sampleAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContentWithName)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(sampleAppioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandFailureModelNameDuplication, projectName, modelName), commandResult.OutputMessages.First().Key);
			}
		}

		[Test]
		public void Fail_OnUriDuplication([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);
			var modelName = Path.GetFileName(modelFilePath);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);
			_fileSystemMock.Setup(x => x.GetFileName(modelFilePath)).Returns(modelName);

			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var sampleAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleOpcuaServerAppContentWithUri)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(sampleAppioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandFailureModelUriDuplication, projectName, "sample_namespace"), commandResult.OutputMessages.First().Key);
			}
		}

		[Test]
		public void Fail_OnModelValidation([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);

			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.NodesetValidationFailure, modelFilePath), commandResult.OutputMessages.First().Key);
		}

		[Test]
		public void Fail_OnImportingModelWithMissingNamespaceUri([ValueSource(nameof(ValidInputs))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);

			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);

			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);

			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var clientServerAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaClientServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_emptyNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(clientServerAppioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);
				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsFalse(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.ImportInforamtionModelCommandFailureModelMissingUri, modelFilePath), commandResult.OutputMessages.First().Key);
			}
		}

		[Test]
		public void Fail_OnMissingExtraTypesName([ValueSource(nameof(InvalidInputsMissingExtraTypesName))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);
			var typesFilePath = inputParams.ElementAtOrDefault(5);

			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
		}

		[Test]
		public void Fail_OnInvalidExtraTypesExtension([ValueSource(nameof(InvalidInputsInvalidExtraTypesExtension))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);
			var typesFilePath = inputParams.ElementAtOrDefault(5);

			var typesFileExtension = Path.GetExtension(typesFilePath);

			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.GetFileName(typesFilePath)).Returns(typesFilePath);
			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			_fileSystemMock.Setup(x => x.GetExtension(typesFilePath)).Returns(typesFileExtension);

			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandFailureTypesHasInvalidExtension, typesFilePath), commandResult.OutputMessages.First().Key);
		}

		[Test]
		public void Fail_OnMissingExtraTypesFile([ValueSource(nameof(ValidInputsExtraTypes))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);
			var typesFilePath = inputParams.ElementAtOrDefault(5);

			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.GetExtension(typesFilePath)).Returns(_validTypesExtension);
			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			_fileSystemMock.Setup(x => x.FileExists(typesFilePath)).Returns(false);

			// Act
			var commandResult = _objectUnderTest.Execute(inputParams);

			// Assert
			Assert.IsFalse(commandResult.Success);
			Assert.IsNotNull(commandResult.OutputMessages);
			Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandFailureTypesFileDoesNotExist, typesFilePath), commandResult.OutputMessages.First().Key);
		}

		[Test]
		public void Success_OnImportingModelAndExtraTypes([ValueSource(nameof(ValidInputsExtraTypes))] string[] inputParams)
		{
			// Arrange
			var projectName = inputParams.ElementAtOrDefault(1);
			var modelFilePath = inputParams.ElementAtOrDefault(3);
			var typesFilePath = inputParams.ElementAtOrDefault(5);

			var appioprojFilePath = Path.Combine(projectName, projectName + Constants.FileExtension.Appioproject);
			var modelsDirectory = Path.Combine(projectName, Constants.DirectoryName.Models);
			var modelName = Path.GetFileName(modelFilePath);
			var modelTargetPath = Path.Combine(modelsDirectory, modelName);
			var typesName = Path.GetFileName(typesFilePath);
			var typesTargetPath = Path.Combine(modelsDirectory, typesName);

			_fileSystemMock.Setup(x => x.DirectoryExists(projectName)).Returns(true);
			_fileSystemMock.Setup(x => x.FileExists(modelFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetExtension(modelFilePath)).Returns(_validModelExtension);
			_fileSystemMock.Setup(x => x.GetExtension(typesFilePath)).Returns(_validTypesExtension);
			_fileSystemMock.Setup(x => x.FileExists(typesFilePath)).Returns(true);
			_fileSystemMock.Setup(x => x.GetFileName(typesFilePath)).Returns(typesName);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject)).Returns(appioprojFilePath);
			_fileSystemMock.Setup(x => x.CombinePaths(projectName, Constants.DirectoryName.Models)).Returns(modelsDirectory);
			_fileSystemMock.Setup(x => x.CombinePaths(modelsDirectory, typesName)).Returns(typesTargetPath);
			_modelValidatorMock.Setup(x => x.Validate(modelFilePath, It.IsAny<string>())).Returns(true);

			using (var serverAppioprojFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_defaultOpcuaServerAppContent)))
			using (var nodesetFileStream = new MemoryStream(Encoding.ASCII.GetBytes(_sampleNodesetContent)))
			{
				_fileSystemMock.Setup(x => x.ReadFile(appioprojFilePath)).Returns(serverAppioprojFileStream);
				_fileSystemMock.Setup(x => x.ReadFile(modelFilePath)).Returns(nodesetFileStream);

				// Act
				var commandResult = _objectUnderTest.Execute(inputParams);

				// Assert
				Assert.IsTrue(commandResult.Success);
				Assert.IsNotNull(commandResult.OutputMessages);
				Assert.AreEqual(string.Format(OutputText.ImportInformationModelCommandSuccess, modelFilePath), commandResult.OutputMessages.First().Key);
				_fileSystemMock.Verify(x => x.CopyFile(typesFilePath, typesTargetPath), Times.Once);
			}
		}
	}
}