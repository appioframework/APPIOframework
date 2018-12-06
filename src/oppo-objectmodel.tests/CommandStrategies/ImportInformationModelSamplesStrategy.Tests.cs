using Moq;
using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class ImportInformationModelSamplesStrategyTest
    {
      

        [Test]
        public void ShouldImplementICommandOfImportStrategy()
        {
            //Arange
            var fileSystemMock = new Mock<IFileSystem>();
            //Act
            var obj = new ImportInformationModelSamplesStrategy(fileSystemMock.Object, String.Empty);

            //Assert
            Assert.IsInstanceOf<ICommand<ImportStrategy>>(obj);
        }
        [Test]
        public void ShouldProvideCorrectName()
        {
            //Arange
            var fileSystemMock = new Mock<IFileSystem>();
            const string expectedName = "Any Name!";
            var obj = new ImportInformationModelSamplesStrategy(fileSystemMock.Object, expectedName);

            //Act
            var name = obj.Name;

            //Assert
            Assert.AreEqual(expectedName,name);
        }

        [Test]
        public void ShouldProvideCorrectHelpText()
        {
            //Arange
            var fileSystemMock = new Mock<IFileSystem>();
            var obj = new ImportInformationModelSamplesStrategy(fileSystemMock.Object, String.Empty);

            //Act 
            var helpText = obj.GetHelpText();

            //Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.ImportSamplesArgumentCommandDescription,helpText);
        }

        [Test]
        public void ShouldImportSamplesOnExecution()
        {
            //Arange
            const string content = "AnyContent";
            const string projectName = "AnyProjectName";
            const string relativeModelsDirPath = "AnyModel";
            const string relativeModelFilePath = "AnyFilePath";
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x=>x.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName)).Returns(content);
            fileSystemMock.Setup(x=>x.CreateFile(relativeModelFilePath,content));
            fileSystemMock.Setup(x => x.CombinePaths(projectName,Constants.DirectoryName.Models)).Returns(relativeModelsDirPath);
            fileSystemMock.Setup(x => x.CombinePaths(relativeModelsDirPath, Constants.FileName.SampleInformationModelFile)).Returns(relativeModelFilePath);
            var obj = new ImportInformationModelSamplesStrategy(fileSystemMock.Object, String.Empty);
            //Act

            var result = obj.Execute(new string[] {projectName});
            //Assert
            Assert.IsTrue(result.Sucsess);
            fileSystemMock.Verify(x => x.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName), Times.Once);
            fileSystemMock.Verify(x => x.CreateFile(relativeModelFilePath, content), Times.Once);
        }

        




    }
}
