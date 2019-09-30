/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Appio.ObjectModel;

namespace Appio.ObjectModel
{
    public abstract class AbstractCertificateGenerator
    {
        public abstract void Generate(string appName,
            string filePrefix,
            uint keySize,
            uint days,
            string organization);

        public virtual void Generate(string appName, string filePrefix = "")
        {
            Generate(appName,
                filePrefix ?? appName,
                Constants.ExternalExecutableArguments.OpenSSLDefaultKeySize,
                Constants.ExternalExecutableArguments.OpenSSLDefaultDays, Constants.ExternalExecutableArguments.OpenSSLDefaultOrganization);
        }
    }
}