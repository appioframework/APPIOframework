using NUnit.Framework;
using Moq;
using Oppo.ObjectModel;

namespace Oppo.Terminal.Tests
{
    public class OppoTerminalTests
    {
        public struct ExecuteCommandFixture
        {
            public ExecuteCommandFixture(string[] input, string result)
            {
                Input = input;
                Result = result;
            }

            public string[] Input;
            public string Result;
        }

        public static ExecuteCommandFixture[] CommandFixture()
        {
            return new[]
            {
                new ExecuteCommandFixture(new[] {"sln"}, Constants.CommandResults.Success),
                new ExecuteCommandFixture(new[] {"abc"}, Constants.CommandResults.Success),
                new ExecuteCommandFixture(new[] {"374f47h"}, Constants.CommandResults.Failure),
                new ExecuteCommandFixture(new[] {":)"}, Constants.CommandResults.Failure),
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void OppoTerminal_Should_Construct()
        {
            // Arrange
            var objectModelMock = new Mock<IObjectModel>();
            
            // Act
            var oppoTerminal = new OppoTerminal(objectModelMock.Object);
            
            // Assert
            Assert.IsInstanceOf<OppoTerminal>(oppoTerminal);
        }

        [Test]
        public void OppoTerminal_Should_ExecuteObjectModelCommand([ValueSource(nameof(CommandFixture))] ExecuteCommandFixture data)
        {
            // Arrange
            var objectModelMock = new Mock<IObjectModel>();
            objectModelMock.Setup(x => x.ExecuteCommand(It.IsAny<string[]>())).Returns(data.Result);

            var oppoTerminal = new OppoTerminal(objectModelMock.Object);

            // Act
            var result = oppoTerminal.Execute(data.Input);

            // Assert
            Assert.AreEqual(data.Result, result);
        }
    }
}