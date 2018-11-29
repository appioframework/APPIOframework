using NUnit.Framework;
using Oppo.ObjectModel.CommandStrategies.ImportCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.Tests.CommandStrategies
{
    public class ImportHelpStrategyTests
    {
        [Test]
        public void ImportHelpStrategyShouldHaveCorrectCommandName()
        {
            //Arrange 
            var strategy = new ImportHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            //Act
            var commandName = strategy.Name;

            //Assert
            Assert.AreEqual(string.Empty, commandName);
        }

        [Test]
        public void ShouldProvideHelpText()

        {
            //Arrange 
            var strategy = new ImportHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            //Act
            var helpText = strategy.GetHelpText();

            //Assert
            Assert.AreEqual(Resources.text.help.HelpTextValues.ImportHelpArgumentCommandDescription, helpText);
        }

      
        
        [Test]
        public void ShouldExecuteSucces()

        {
            //Arrange 
            var strategy = new ImportHelpStrategy(string.Empty, new MessageLines() { { string.Empty, string.Empty } });

            //Act
            var result = strategy.Execute(new string[0]);

            //Assert
            Assert.IsTrue(result.Sucsess);
        }
    }
}