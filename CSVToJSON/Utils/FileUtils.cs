using CSVToJSON.Utils.Interfaces;
using System.IO;


namespace CSVToJSON.Utils
{
    /// <summary>
    /// This class is a basic abstraction for the native File class. It allows for more flexibility regarding testing and mocking, among other things.
    /// </summary>
    public class FileUtils : IFileUtils
    {

        public string ReadAllText(string filePath)
        {
            if (File.Exists(filePath)) return File.ReadAllText(filePath);
            return null;
        }
    }
}
