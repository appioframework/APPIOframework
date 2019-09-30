/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

namespace Appio.ObjectModel
{
    public class ParameterList<TData, TIdent>
    {
        private readonly int _offset;
        private readonly TData[] _parameters;

        public ParameterList(int offset, TData[] parameters)
        {
            _offset = offset;
            _parameters = parameters;
        }

        public TData this[TIdent identifier] => _parameters[(int) (object) identifier - _offset];
    }
}