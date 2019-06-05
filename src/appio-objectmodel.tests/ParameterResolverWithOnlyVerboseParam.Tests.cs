using NUnit.Framework;
using static Appio.ObjectModel.Tests.ParameterResolverTest;

namespace Appio.ObjectModel.Tests
{
    public class ParameterResolverWithOnlyVerboseParamShould
    {
        private enum Identifiers { Server, Client, Url, Quiet }
        private ParameterResolver<Identifiers> _resolver;
        
        [SetUp]
        public void Setup()
        {
            _resolver = new ParameterResolver<Identifiers>(string.Empty, new[]
            {
                new StringParameterSpecification<Identifiers> { Identifier = Identifiers.Server, Short = "-S", Verbose = "--server" },
                new StringParameterSpecification<Identifiers> { Identifier = Identifiers.Client, Short = "-C", Verbose = "--client" },
                new StringParameterSpecification<Identifiers> { Identifier = Identifiers.Url, Verbose = "--url" },
                new StringParameterSpecification<Identifiers> { Identifier = Identifiers.Quiet, Short = "-q" }
            });
        }

        private static object[] _goodParams =
        {
            new[] { "--server", "testServer", "-C", "exampleClient", "--url", "someUrl", "-q", "dummy" },
            new[] { "-C", "exampleClient", "--url", "someUrl", "-q", "dummy", "--server", "testServer" },
            new[] { "-q", "dummy", "--url", "someUrl", "-C", "exampleClient", "--server", "testServer" }
        };

        private static object[] _badDataMissingParameter =
        {
            new object[] {new[] {"--server", "testServer", "-C", "exampleClient", "-q", "dummy"}, "'--url'"},
            new object[] {new[] {"--server", "testServer", "-C", "exampleClient", "--url", "someUrl"}, "'-q'"}
        };

        private static object[] _badParamsExpectedRecognizedFlag =
        {
            new object[] {new[] {"--server", "someArg", "someOtherArg", "-C"}, "someOtherArg"}
        };
        
        [TestCaseSource(nameof(_goodParams))]
        public void GiveCorrectParametersOnGoodInputs(string[] parameters)
        {
            var stringParameters = _resolver.ResolveParams(parameters).StringParameters;
            Assert.AreEqual("testServer", stringParameters[Identifiers.Server]);
            Assert.AreEqual("exampleClient", stringParameters[Identifiers.Client]);
            Assert.AreEqual("someUrl", stringParameters[Identifiers.Url]);
            Assert.AreEqual("dummy", stringParameters[Identifiers.Quiet]);
        }
        
        [TestCaseSource(nameof(_badParamsExpectedRecognizedFlag))]
        public void ThrowExceptionWhenFlagExpected(string[] parameters, string expectedBadFlag)
        {
            AssertFailWith(_resolver, parameters, $"Unrecognized parameter '{expectedBadFlag}'. Expected parameter '-S', '--server', '-C', '--client', '--url' or '-q' instead");
        }
                 
        [TestCaseSource(nameof(_badDataMissingParameter))]
        public void ThrowExceptionWhenParameterMissing(string[] parameters, string expectedBadFlagPair)
        {
            AssertFailWith(_resolver, parameters, $"Missing required parameter {expectedBadFlagPair}");
        }

    }
}