using NUnit.Framework;
using static Appio.ObjectModel.Tests.ParameterResolverTest;

namespace Appio.ObjectModel.Tests
{
    public class ParameterResolverWithOptionalParametersShould
    {
        private enum Identifiers { Server, Client, Type }
        private ParameterResolver<Identifiers> _resolver;

        [SetUp]
        public void Setup()
        {
            _resolver = new ParameterResolver<Identifiers>(string.Empty, new[]
            {
                new StringParameterSpecification<Identifiers>{ Identifier = Identifiers.Server, Short = "-S", Verbose = "--server" },
                new StringParameterSpecification<Identifiers>{ Identifier = Identifiers.Client, Short = "-C", Verbose = "--client" },
                new StringParameterSpecification<Identifiers>{ Identifier = Identifiers.Type, Short = "-t", Verbose = "--type", Default = "ClientServer" }
                
            });
        }
        
        private static object[] _goodData =
        {
            new object[] {new[] { "--server", "testServer", "-C", "exampleClient" }, new[] {"testServer", "exampleClient", "ClientServer"}},
            new object[] {new[] { "-C", "exampleClient", "--type", "Client", "--server", "testServer" }, new[] {"testServer", "exampleClient", "Client"}},
            new object[] {new[] { "-t", "Server", "-C", "exampleClient", "--server", "testServer" }, new[] {"testServer", "exampleClient", "Server"}}
        };
        
        private static object[] _badParamsMissingParameter =
        {
            new[] {"-S", "testServer"},
            new[] {"-t", "Server", "--server", "testServer"}
        };
        
        [TestCaseSource(nameof(_goodData))]
        public void GiveCorrectParametersOnGoodInputs(string[] parameters, string[] expectedResult)
        {
            var stringParameters = _resolver.ResolveParams(parameters).StringParameters;
            Assert.AreEqual(expectedResult[(int) Identifiers.Server], stringParameters[Identifiers.Server]);
            Assert.AreEqual(expectedResult[(int) Identifiers.Client], stringParameters[Identifiers.Client]);
            Assert.AreEqual(expectedResult[(int) Identifiers.Type], stringParameters[Identifiers.Type]);
        }
         
        [TestCaseSource(nameof(_badParamsMissingParameter))]
        public void ThrowExceptionWhenParameterMissing(string[] parameters)
        {
            AssertFailWith(_resolver, parameters, "Missing required parameter '-C' or '--client'");
        }
    }
}