using System;
using System.IO;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class FileSystemWrapper : IFileSystem
    {
        public void CreateFile(string filePath, string fileContent)
        {
           File.WriteAllText(filePath, fileContent);
        }

        public char[] GetInvalidFileNameChars()
        {
            return Path.GetInvalidFileNameChars();
        }

        public string LoadTemplateFile(string fileName)
        {
            var assembly = typeof(Oppo.Resources.Resources).Assembly;
            var resourceName = fileName;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}