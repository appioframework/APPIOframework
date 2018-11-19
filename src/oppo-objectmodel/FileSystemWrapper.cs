using System.Diagnostics;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel
{
    [ExcludeFromCodeCoverage]
    public class FileSystemWrapper : IFileSystem
    {
        public string CombinePaths(params string[] paths)
        {
            try
            {
                return Path.Combine(paths);
            }
            catch (System.Exception ex)
            {
                OppoLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }

        public bool CallExecutable(string name, string workingDirectory, string args)
        {
            try
            {
                var info = new ProcessStartInfo(name, args)
                {
                    WorkingDirectory = workingDirectory,
                };

                var process = Process.Start(info);
                process?.WaitForExit();

                return process?.ExitCode == 0;
            }
            catch (System.Exception ex)
            {
                OppoLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }

        public void CopyFile(string source, string target)
        {
            try
            {
                File.Copy(source, target, true);
            }
            catch (System.Exception ex)
            {
                OppoLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }            
        }

        public void DeleteDirectory(string name)
        {
            try
            {
                Directory.Delete(name, true);
            }
            catch (DirectoryNotFoundException ex)
            {
                OppoLogger.Error(LoggingText.DirectoryNotFoundException, ex);
                throw;
            }
            catch (PathTooLongException ex)
            {
                OppoLogger.Error(LoggingText.PathTooLongException, ex);
                throw;
            }
            catch (IOException ex)
            {
                OppoLogger.Error(LoggingText.DirectoryIOException, ex);
                throw;
            }
        }

        public void CreateDirectory(string directoryName)
        {
            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (System.Exception ex)
            {
                OppoLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }            
        }

        public void CreateFile(string filePath, string fileContent)
        {           
            try
            {
                File.WriteAllText(filePath, fileContent);
            }
            catch (System.Exception ex)
            {
                OppoLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
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
            try
            {
                var assembly = typeof(Resources.Resources).Assembly;
                var resourceName = fileName;

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                OppoLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }            
        }
    }
}