namespace Oppo.ObjectModel
{
    public interface IFileSystem
    {
        void CreateFile(string filePath, string fileContent);
        
        char[] GetInvalidFileNameChars();

        string LoadTemplateFile(string fileName);
    }    
}