namespace Oppo.ObjectModel
{
    public class FileManager
    {
        private readonly IFileSystem _fileSystem = null;
        
        public FileManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void CreateFile(string filePath, string fileContent)
        {
            _fileSystem.CreateFile(filePath, fileContent);
        }
    }
}