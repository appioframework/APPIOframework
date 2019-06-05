using System.Diagnostics;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Appio.Resources.text.logging;
using System.IO.Compression;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Appio.ObjectModel
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
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
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
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                return false;
            }
        }

        public void CopyFile(string source, string target)
        {
            try
            {
                File.Copy(source, target, true);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }            
        }
        
        public void DeleteFile(string name)
        {
            try
            {
                File.Delete(name);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
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
                AppioLogger.Error(LoggingText.DirectoryNotFoundException, ex);
                throw;
            }
            catch (PathTooLongException ex)
            {
                AppioLogger.Error(LoggingText.PathTooLongException, ex);
                throw;
            }
            catch (IOException ex)
            {
                AppioLogger.Error(LoggingText.DirectoryIOException, ex);
                throw;
            }
        }

        public void CreateDirectory(string directoryName)
        {
            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }            
        }

        public void CreateFile(string filePath, string fileContent)
        {           
            try
            {
                File.WriteAllText(filePath, fileContent);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
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
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }            
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public void ExtractFromZip(string source, string target, string resourceFullName)
        {
            try
            {
                if (WriteResourceToFile(resourceFullName, source))
                {
                    ZipFile.ExtractToDirectory(source, target);
                }                
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }            
        }

        public string AppDomainBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private bool WriteResourceToFile(string resourceName, string fileName)
        {
            try
            {
                using (var resource = GetResourceAssembly().GetManifestResourceStream(resourceName))
                {
                    using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        resource.CopyTo(file);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }           
        }

        private static Assembly GetResourceAssembly()
        {
            return typeof(Resources.Resources).Assembly;
        }

        public string GetExtension(string path)
        {
            try
            {
                return Path.GetExtension(path);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }

        public string GetFileName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }

        public string GetFileNameWithoutExtension(string path)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }

        public Stream ReadFile(string path)
        {
            try
            {
                return File.Open(path, FileMode.OpenOrCreate);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }

        public void WriteFile(string path, IEnumerable<string> content)
        {
            try
            {
                File.WriteAllLines(path, content, System.Text.Encoding.Default);
            }
            catch (Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }

        public string[] GetFilesByExtension(string path, string extension)
        {
            try
            {
                return Directory.GetFiles(path, "*" + extension);
            }
            catch(Exception ex)
            {
                AppioLogger.Error(LoggingText.ExceptionOccured, ex);
                throw;
            }
        }
    }
}