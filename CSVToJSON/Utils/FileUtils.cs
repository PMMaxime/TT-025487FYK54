using _Tests_CSVToJSON.Utils.Interfaces;
using System.IO;


namespace _Tests_CSVToJSON.Utils
{
    public class FileUtils : IFileUtils
    {

        public string ReadAllText(string filePath)
        {
            if (File.Exists(filePath)) return File.ReadAllText(filePath);
            return null;
        }
    }
}
