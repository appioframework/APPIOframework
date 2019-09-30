/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Moq;
using NUnit.Framework;
using Appio.ObjectModel.CommandStrategies.ReferenceCommands;

namespace Appio.ObjectModel.Tests.CommandStrategies
{
	class ReferenceBaseTestClass : ReferenceBase
	{
		public ReferenceBaseTestClass(IFileSystem fileSystem) : base(fileSystem) { }
	}

	class ReferenceBaseSholud
	{
		private Mock<IFileSystem> _fileSystemMock;
		private ReferenceBase _objectUnderTest;
		
		[SetUp]
		public void SetUp_ObjectUnderTest()
		{
			_fileSystemMock = new Mock<IFileSystem>();
			_objectUnderTest = new ReferenceBaseTestClass(_fileSystemMock.Object);
		}
		[Test]
		public void ImplementICommandOfReferenceAddStrategy()
		{
			// Arrange

			// Act

			// Assert
			Assert.IsInstanceOf<ICommand<ReferenceStrategy>>(_objectUnderTest);
		}

		[Test]
		public void HaveEmptyCommandName()
		{
			// Arrange

			// Act
			var name = _objectUnderTest.Name;

			// Assert
			Assert.AreEqual(string.Empty, name);
		}

		[Test]
		public void HaveEmptyHelpText()
		{
			// Arrange

			// Act
			var helpText = _objectUnderTest.GetHelpText();

			// Assert
			Assert.AreEqual(string.Empty, helpText);
		}

		[Test]
		public void ReturnNullWhenExecute()
		{
			// Arrange

			// Act
			var result = _objectUnderTest.Execute(null);

			// Assert
			Assert.AreEqual(null, result);
		}
	}
}