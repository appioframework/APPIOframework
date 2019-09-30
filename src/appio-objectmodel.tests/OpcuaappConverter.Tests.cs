/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Appio.ObjectModel.Tests
{
    public class OpcuaappConverterShould
    {
        private OpcuaappConverter<IOpcuaapp, OpcuaappReference> _converter;

        [SetUp]
        public void SetupTest()
        {
			_converter = new OpcuaappConverter<IOpcuaapp, OpcuaappReference>();   
        }

        [TearDown]
        public void CleanUpTest()
        {
        }

		[Test]
		public void ReturnFalseForCanWriteProperty()
		{
			// Arrange

			// Act
			var result = _converter.CanWrite;

			// Assert
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ReturnTrueForCanReadProperty()
		{
			// Arrange

			// Act
			var result = _converter.CanRead;

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
        public void ReturnTrueWhenCallingCanConvertWithIOpcuaappStructure()
        {
			// Arrange

			// Act
			var result = _converter.CanConvert(typeof(List<IOpcuaapp>));

			// Assert
			Assert.IsTrue(result);
        }

		[Test]
		public void ReturnInvalidOperationExceptionWhenCallingWriteJson()
		{
			// Arrange
			InvalidOperationException expectedException = null;

			// Act
			try
			{
				_converter.WriteJson(null, null, null);
			}
			catch(InvalidOperationException ex)
			{
				expectedException = ex;
			}

			// Assert
			Assert.IsNotNull(expectedException);
			Assert.AreEqual("Use default serialization.", expectedException.Message);
		}

	}
}