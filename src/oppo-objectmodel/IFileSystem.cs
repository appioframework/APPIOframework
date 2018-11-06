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
    }    
}