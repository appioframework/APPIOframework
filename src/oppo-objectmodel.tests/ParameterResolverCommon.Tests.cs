using System;
using NUnit.Framework;

namespace Oppo.ObjectModel.Tests
{
    public class ParameterResolverTest
    {
        public static void AssertFailWith<T>(ParameterResolver<T> resolver, string[] parameters, string errorMessage) where T : struct, IConvertible
        {
            Assert.AreEqual(errorMessage, resolver.ResolveParams(parameters).ErrorMessage);
        }
    }
}