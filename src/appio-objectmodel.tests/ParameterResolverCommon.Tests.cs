/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using NUnit.Framework;

namespace Appio.ObjectModel.Tests
{
    public class ParameterResolverTest
    {
        public static void AssertFailWith<T>(ParameterResolver<T> resolver, string[] parameters, string errorMessage) where T : struct, IConvertible
        {
            Assert.AreEqual(errorMessage, resolver.ResolveParams(parameters).ErrorMessage);
        }
    }
}