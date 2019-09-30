/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System.Linq;
using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.NewCommands;

namespace Appio.ObjectModel.Tests
{
     public class ParameterResolverWithArgumentsShould
    {
        public enum Subcommand { Sln, Opcuaapp }

        private static Mock<ICommandFactory<NewStrategy>> _factoryMock;
        private static Mock<ICommand<NewStrategy>> _opcuaAppMock;
        private static Mock<ICommand<NewStrategy>> _slnMock;

        private static object[] _goodData =
        {
            new object[] {new[] {"sln"}, Subcommand.Sln},
            new object[] {new[] {"opcuaapp"}, Subcommand.Opcuaapp},
            new object[] {new[] {"sln", "--server", "testServer", "-C", "exampleClient"}, Subcommand.Sln},
            new object[] {new[] {"opcuaapp", "-C", "exampleClient", "--server", "testServer"}, Subcommand.Opcuaapp}
        };

        [SetUp]
        public void Setup()
        {
            _factoryMock = new Mock<ICommandFactory<NewStrategy>>();
            _opcuaAppMock = new Mock<ICommand<NewStrategy>>();
            _slnMock = new Mock<ICommand<NewStrategy>>();

            _opcuaAppMock.Setup(c => c.Execute(It.IsAny<string[]>())).Returns(new CommandResult(true, new MessageLines()));
            _slnMock.Setup(c => c.Execute(It.IsAny<string[]>())).Returns(new CommandResult(true, new MessageLines()));
            
            _factoryMock.Setup(f => f.GetCommand("opcuaapp")).Returns(_opcuaAppMock.Object);
            _factoryMock.Setup(f => f.GetCommand("sln")).Returns(_slnMock.Object);
        }

        [TestCaseSource(nameof(_goodData))]
        public void CallCorrectClassWithGoodInputs(string[] parameters, Subcommand expectedSubcommand)
        {
            var restParams = parameters.Skip(1).ToArray();
            Assert.IsTrue(ParameterResolver.DelegateToSubcommands(_factoryMock.Object, parameters).Success);
            var expectedCommandMock = expectedSubcommand == Subcommand.Sln ? _slnMock : _opcuaAppMock;
            expectedCommandMock.Verify(c => c.Execute(restParams), Times.Once);
        }
    }
}