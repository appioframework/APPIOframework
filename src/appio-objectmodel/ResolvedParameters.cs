/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;

namespace Appio.ObjectModel
{
    public class ResolvedParameters<TIdent> where TIdent : struct, IConvertible
    {
        public string ErrorMessage { get; set; }
        public ParameterList<string, TIdent> StringParameters { get; set; }
        public ParameterList<bool, TIdent> BoolParameters { get; set; }

        public void Deconstruct(out string error, out ParameterList<string, TIdent> stringParams, out ParameterList<bool, TIdent> boolParams)
        {
            error = ErrorMessage;
            stringParams = StringParameters;
            boolParams = BoolParameters;
        }
    }
}