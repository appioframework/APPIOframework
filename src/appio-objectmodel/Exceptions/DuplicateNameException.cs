/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Appio.ObjectModel.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class DuplicateNameException : ArgumentException
    {
        public DuplicateNameException(string duplicatedName)
            : base($"The name '{duplicatedName}' was already provided before.")
        {
        }

        protected DuplicateNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
