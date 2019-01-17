using Oppo.Resources.text.logging;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Oppo.ObjectModel
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
            OppoLogger.Info(string.Format(LoggingText.ValidatingModel, filePathToValidate, fileNameToValidateAgainst));
            var xsdToValidateAgainst = _fileSystem.LoadTemplateFile(fileNameToValidateAgainst);
            var anyErrors = true;

            var schema = XmlSchema.Read(new StringReader(xsdToValidateAgainst), null); // ValidationEventHandler == null, because xsd is provided by OPPO, so parsing it can't go wrong

            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);

            var streamToValidate = _fileSystem.ReadFile(filePathToValidate);
            var xmlReader = XmlReader.Create(streamToValidate);
            var xDocument = XDocument.Load(xmlReader);

            xDocument.Validate(schemaSet, (o, e) =>
            {
                OppoLogger.Error(string.Format(LoggingText.ValidationError, e.Message), e.Exception);
                anyErrors = false;
            });

            streamToValidate.Close();
            streamToValidate.Dispose();

            return anyErrors;
        }
    }
}