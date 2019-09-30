/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Appio.Resources.text.logging;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Appio.ObjectModel
{
    public class ModelValidator : IModelValidator
    {
        private readonly IFileSystem _fileSystem;

        public ModelValidator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool Validate(string filePathToValidate, string fileNameToValidateAgainst)
        {
            AppioLogger.Info(string.Format(LoggingText.ValidatingModel, filePathToValidate, fileNameToValidateAgainst));
            var xsdToValidateAgainst = _fileSystem.LoadTemplateFile(fileNameToValidateAgainst);
            var anyErrors = true;

            var schema = XmlSchema.Read(new StringReader(xsdToValidateAgainst), null); // ValidationEventHandler == null, because xsd is provided by APPIO, so parsing it can't go wrong

            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);

            var streamToValidate = _fileSystem.ReadFile(filePathToValidate);
            var xmlReader = XmlReader.Create(streamToValidate);
            var xDocument = XDocument.Load(xmlReader);

            xDocument.Validate(schemaSet, (o, e) =>
            {
                AppioLogger.Error(string.Format(LoggingText.ValidationError, e.Message), e.Exception);
                anyErrors = false;
            });

            streamToValidate.Close();
            streamToValidate.Dispose();

            return anyErrors;
        }
    }
}