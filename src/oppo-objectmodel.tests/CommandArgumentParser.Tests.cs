using NUnit.Framework;

namespace Oppo.ObjectModel.Tests
{
    public class CommandArgumentParserTests
    {
        [Test]
        public void Constructor_ShouldConstructFromEmptyArgumentList()
        {
            // Arrange
            var flags = new CommandArgumentParser.ArgumentInfo[] { };
            var arguments = new string[] { };

            // Act
            var obj = new CommandArgumentParser(flags, arguments);

            // Assert
            Assert.IsNotNull(obj);
        }

        [TestCase("-p any-path", "-p")]
        [TestCase("-p any-path", "--path")]
        [TestCase("--path any-path", "-p")]
        [TestCase("--path any-path", "--path")]
        public void IndexOperator_ShouldProvideCorrectInformationForSingleArgumentWithValue(string argumentString, string flag)
        {
            // Arrange
            var argumentInfo = new CommandArgumentParser.ArgumentInfo("-p", "--path", true);

            var flags = new[] { argumentInfo };
            var arguments = argumentString.Split(' ');

            var obj = new CommandArgumentParser(flags, arguments);

            // Act
            var value = obj[flag];

            // Assert
            Assert.AreEqual("any-path", value.Value);
            Assert.IsFalse(value.IsNotExistent);
        }
        
        [TestCase("-p any-path -n any-name -s any-something", "-p -n -s")]
        [TestCase("-p any-path -n any-name -s any-something", "--path --name --something")]
        [TestCase("-n any-name -p any-path -s any-something", "-p -n -s")]
        [TestCase("-n any-name -p any-path -s any-something", "--path --name --something")]
        [TestCase("-s any-something -p any-path -n any-name", "-p -n -s")]
        [TestCase("-s any-something -p any-path -n any-name", "--path --name --something")]
        [TestCase("-p any-path -s any-something -n any-name", "-p -n -s")]
        [TestCase("-p any-path -s any-something -n any-name", "--path --name --something")]
        public void IndexOperator_ShouldProvideCorrectInformationForMultipleArgumentsWithValuesWhenUsingArgumentShortName(string argumentString, string flags)
        {
            // Arrange
            var flagP = flags.Split(' ')[0];
            var flagN = flags.Split(' ')[1];
            var flagS = flags.Split(' ')[2];

            var infos = new[]
            {
                new CommandArgumentParser.ArgumentInfo("-p", "--path", true),
                new CommandArgumentParser.ArgumentInfo("-n", "--name", true),
                new CommandArgumentParser.ArgumentInfo("-s", "--something", true),
            };
            var arguments = argumentString.Split(' ');

            var obj = new CommandArgumentParser(infos, arguments);

            // Act
            var valueForP = obj[flagP];
            var valueForN = obj[flagN];
            var valueForS = obj[flagS];

            // Assert
            Assert.AreEqual("any-path", valueForP.Value);
            Assert.IsFalse(valueForP.IsNotExistent);
            Assert.AreEqual("any-name", valueForN.Value);
            Assert.IsFalse(valueForN.IsNotExistent);
            Assert.AreEqual("any-something", valueForS.Value);
            Assert.IsFalse(valueForS.IsNotExistent);
        }
        
        [TestCase("-p any-path", "-n")]
        [TestCase("-p any-path", "--name")]
        public void IndexOperator_ShouldProvideInformationAboutNotExistentArgument(string argumentString, string flag)
        {
            // Arrange
            var flags = new[]
            {
                new CommandArgumentParser.ArgumentInfo("-p", "--path", true)
            };
            var arguments = argumentString.Split(' ');

            var obj = new CommandArgumentParser(flags, arguments);

            // Act
            var value = obj[flag];

            // Assert
            Assert.AreEqual(string.Empty, value.Value);
            Assert.IsTrue(value.IsNotExistent);
        }

        [TestCase("-s", "-s")]
        [TestCase("-s", "--something")]
        [TestCase("-p any-path -s", "-s")]
        [TestCase("-p any-path -s", "--something")]
        [TestCase("-s -p any-path", "-s")]
        [TestCase("-s -p any-path", "--something")]
        public void IndexOperator_ShouldProvideCorrectInformationForArgumentWithoutValue(string argumentString, string flag)
        {
            // Arrange
            var flags = new[]
            {
                new CommandArgumentParser.ArgumentInfo("-p", "--path", true),
                new CommandArgumentParser.ArgumentInfo("-s", "--something", false),
            };
            var arguments = argumentString.Split(' ');

            var obj = new CommandArgumentParser(flags, arguments);

            // Act
            var value = obj[flag];

            // Assert
            Assert.AreEqual(string.Empty, value.Value);
            Assert.IsFalse(value.IsNotExistent);
        }
    }
}
