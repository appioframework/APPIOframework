using NUnit.Framework;
using static Appio.ObjectModel.Tests.ParameterResolverTest;

namespace Appio.ObjectModel.Tests
{
    public class ParameterResolverWithBooleanParametersShould
    {
        private enum Identifiers { Server, Client, Help }

        private ParameterResolver<Identifiers> _resolver;
        
        private static object[] _goodParams =
        {
            new object[]
            {
                new string[0],
                new [] {"NoServer", "NoClient"},
                false
            },
            new object[]
            {
                new[] {"-C", "exampleClient"},
                new [] {"NoServer", "exampleClient"},
                false
            },
            new object[]
            {
                new[] {"-C", "exampleClient", "--server", "testServer"}, 
                new [] {"testServer", "exampleClient"}, 
                false
            },
            new object[]
            {
                new[] {"--help"}, 
                new [] {"NoServer", "NoClient"}, 
                true
            },
            new object[]
            {
                new[] {"-C", "exampleClient", "--help"}, 
                new [] {"NoServer", "exampleClient"}, 
                true
            },
            new object[]
            {
                new[] {"--help", "--server", "testServer"}, 
                new [] {"testServer", "NoClient"}, 
                true
            },
            new object[]
            {
                new[] {"-C", "exampleClient", "--help", "--server", "testServer"}, 
                new [] {"testServer", "exampleClient"}, 
                true
            }
        };

        private static object[] _badParamsExpectedFlag =
        {
            new[] {"badArg", "--help"},
            new[] {"--help", "badArg"},
            new[] {"--help", "--server", "someServer", "badArg"},
            new[] {"--server", "someServer", "badArg", "--help"}
        };
        
        [SetUp]
        public void Setup()
        {
            _resolver = new ParameterResolver<Identifiers>(string.Empty, new[]
            {
                new StringParameterSpecification<Identifiers>{ Identifier = Identifiers.Server, Short = "-S", Verbose = "--server", Default = "NoServer"},
                new StringParameterSpecification<Identifiers>{ Identifier = Identifiers.Client, Short = "-C", Verbose = "--client", Default = "NoClient"}
            }, new[]
            {
                new BoolParameterSpecification<Identifiers>{ Identifier = Identifiers.Help, Short = "-h", Verbose = "--help" }    
            });
        }
        
        [TestCaseSource(nameof(_goodParams))]
        public void GiveCorrectParametersOnGoodInputs(string[] parameters, string[] expectedStrings, bool expectedBool)
        {
            var (_, stringParams, boolParams) = _resolver.ResolveParams(parameters);
            
            Assert.AreEqual(expectedStrings[(int) Identifiers.Client], stringParams[Identifiers.Client]);
            Assert.AreEqual(expectedStrings[(int) Identifiers.Server], stringParams[Identifiers.Server]);
            Assert.AreEqual(expectedBool, boolParams[Identifiers.Help]);
        }
        
        [TestCaseSource(nameof(_badParamsExpectedFlag))]
        public void ThrowExceptionWhenFlagExpected(string[] parameters)
        {
            AssertFailWith(_resolver, parameters,"Unrecognized parameter 'badArg'. Expected parameter '-S', '--server', '-C', '--client', '-h' or '--help' instead");
        }
    }
}