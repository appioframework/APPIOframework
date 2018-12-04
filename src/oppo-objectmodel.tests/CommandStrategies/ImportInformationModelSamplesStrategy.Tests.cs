using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class ImportInformationModelSamplesStrategyTest
    {
        //load embeded resource
        //write resource to file

        [Test]
        public void ShouldImplementICommandOfImportStrategy()
        {
            //Arange

            //Act
            var obj = new ImportInformationModelSamplesStrategy(String.Empty);

            //Assert
            Assert.IsInstanceOf<ICommand<ImportStrategy>>(obj);
        }
        [Test]
        public void ShouldProvideCorrectName()
        {
            //Arange
            const string expectedName = "Any Name!";
            var obj = new ImportInformationModelSamplesStrategy(expectedName);

            //Act
            var name = obj.Name;

            //Assert
            Assert.AreEqual(expectedName,name);
        }

        [Test]
        public void ShouldProvideCorrectHelpText()
        {
            //Arange
            var obj = new ImportInformationModelSamplesStrategy(String.Empty);

            //Act 
            var helpText = obj.GetHelpText();

            //Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.ImportSamplesArgumentCommandDescription,helpText);
        }





    }
}
