/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

namespace Appio.ObjectModel
{
    public abstract class ParameterSpecification<TIdent>
    {
        public TIdent Identifier { get; set; }
        public string Short { get;  set; }
        public string Verbose { get;  set; }

        public void Deconstruct(out TIdent identifier, out string shortName, out string verboseName)
        {
            identifier = Identifier;
            shortName = Short;
            verboseName = Verbose;
        }
    }
    
    public class StringParameterSpecification<TIdent> : ParameterSpecification<TIdent>
    {
        public string Default { get; set; }
    }
    
    public class BoolParameterSpecification<TIdent> : ParameterSpecification<TIdent>
    {
    }
}