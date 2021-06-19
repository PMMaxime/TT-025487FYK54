using CSVToJSON.Services.Models;
using System.Collections.Generic;

namespace CSVToJSON.Services.Interfaces
{
    public interface ICSVParser
    {
        public IEnumerable<IEnumerable<CSVBaseValue>> ParseCsvFile(string filePath, char separator);
    }
}
