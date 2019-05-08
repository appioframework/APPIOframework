using NUnit.Framework;
using static Oppo.ObjectModel.Tests.ParameterResolverTest;

namespace Oppo.ObjectModel.Tests
{
    public class ParameterResolverShould
    {
        private enum Identifiers { Server, Client }

        private ParameterResolver<Identifiers> _resolver;
        
        private static object[] _goodParams =
        {
            new[] {"--server", "testServer", "-C", "exampleClient"},
            new[] {"-C", "exampleClient", "--server", "testServer"},
        };
        
        private static object[] _badDataMissingParameter =
        {
            new object[] {new[] {"--server", "testServer"}, "'-C' or '--client'"},
            new object[] {new[] {"-C", "exampleClient"}, "'-S' or '--server'"}
        };

        private static object[] _badParamsExpectedRecognizedFlag =
        {
            new object[] { new[] {"--server", "someArg", "someOtherArg", "-C"}, "someOtherArg" },
            new object[] { new[] {"someArg", "--server", "someArg", "someOtherArg"}, "someArg" },
            new object[] { new[] {"someArg", "--server", "-S", "someOtherArg"}, "someArg" },
            new object[] { new[] {"--unknown", "someArg", "--server", "someOtherArg"}, "--unknown" },
        };
        
        private static object[] _badParamsDuplicates =
        {
            new object[] { new[] {"--server", "someServer", "--client", "someClient", "--client", "someClient"}, "--client" },
            new object[] { new[] {"--server", "someServer", "-S", "someServer"}, "-S" },
            new object[] { new[] {"--server", "someServer", "--client", "someClient", "-S", "someServer"}, "-S" }
        };

        [SetUp]
        public void Setup()
        {
            _resolver = new ParameterResolver<Identifiers>(string.Empty, new[]
            {
                new StringParameterSpecification<Identifiers>{ Identifier = Identifiers.Server, Short = "-S", Verbose = "--server" },
                new StringParameterSpecification<Identifiers>{ Identifier = Identifiers.Client, Short = "-C", Verbose = "--client" }
            });
        }
        
        [TestCaseSource(nameof(_goodParams))]
        public void GiveCorrectParametersOnGoodInputs(string[] parameters)
        {
            var stringParams = _resolver.ResolveParams(parameters).StringParameters;
            Assert.AreEqual("testServer", stringParams[Identifiers.Server]);
            Assert.AreEqual("exampleClient", stringParams[Identifiers.Client]);
        }

        [TestCaseSource(nameof(_badParamsExpectedRecognizedFlag))]
        public void ThrowExceptionWhenFlagExpected(string[] parameters, string expectedBadFlag)
        {
            AssertFailWith(_resolver, parameters, $"Unrecognized parameter '{expectedBadFlag}'. Expected parameter '-S', '--server', '-C' or '--client' instead");
        }
        
        [TestCaseSource(nameof(_badDataMissingParameter))]
        public void ThrowExceptionWhenParameterMissing(string[] parameters, string expectedBadFlagPair)
        {
            AssertFailWith(_resolver, parameters, $"Missing required parameter {expectedBadFlagPair}");
        }
        
        [TestCaseSource(nameof(_badParamsDuplicates))]
        public void ThrowExceptionWhenDuplicateFlagsProvided(string[] parameters, string expectedBadFlag)
        {
            AssertFailWith(_resolver, parameters, $"Duplicate parameter '{expectedBadFlag}'");
        }
    }
}