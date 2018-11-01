using System;
using System.IO;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class FileSystemWrapper : IFileSystem
    {
        public void CreateFile(string filePath)
        {
            using(var fileStream = File.Create(filePath))
            {
                var msg = new UTF8Encoding().GetBytes("Hello from OPPO");
                fileStream.Write(msg);
            }
            
            
        }
    }
}