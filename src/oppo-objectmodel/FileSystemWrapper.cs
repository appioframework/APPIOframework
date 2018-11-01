using System;
using System.IO;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class FileSystemWrapper : IFileSystem
    {
        public void CreateFile(string filePath, string fileContent)
        {
           File.WriteAllText(filePath, fileContent);
        }
    }
}