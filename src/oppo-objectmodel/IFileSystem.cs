using System.IO;
using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface IFileSystem
    {
        void CreateFile(string filePath, string fileContent);
        
        char[] GetInvalidFileNameChars();

        char[] GetInvalidPathChars();

        string LoadTemplateFile(string fileName);
        void CreateDirectory(string directoryName);
        string CombinePaths(params string[] paths);

        bool CallExecutable(string name, string workingDirectory, string args);

        void CopyFile(string source, string target);

        void DeleteFile(string name);
        void DeleteDirectory(string name);
        bool FileExists(string filePath);
        bool DirectoryExists(string directoryPath);
        void ExtractFromZip(string source, string target, string resourceFullName);
        string AppDomainBaseDirectory();
        string GetExtension(string path);
        string GetFileName(string path);
        string GetFileNameWithoutExtension(string path);
        Stream ReadFile(string path);
        void WriteFile(string path, IEnumerable<string> content);
        string[] GetFilesByExtension(string path, string extension);
    }    
}