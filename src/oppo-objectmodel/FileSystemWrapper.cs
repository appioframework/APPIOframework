using System.Diagnostics;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class FileSystemWrapper : IFileSystem
    {
        public string CombinePaths(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public bool CallExecutable(string name, string workingDirectory, string args)
        {
            var info = new ProcessStartInfo(name, args)
            {
                WorkingDirectory = workingDirectory,
            };

            var process = Process.Start(info);
            process?.WaitForExit();

            return process?.ExitCode == 0;
        }

        public void CreateDirectory(string directoryName)
        {
            Directory.CreateDirectory(directoryName);
        }

        public void CreateFile(string filePath, string fileContent)
        {
           File.WriteAllText(filePath, fileContent);
        }

        public char[] GetInvalidFileNameChars()
        {
            return Path.GetInvalidFileNameChars();
        }

        public char[] GetInvalidPathChars()
        {
            return Path.GetInvalidPathChars();
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