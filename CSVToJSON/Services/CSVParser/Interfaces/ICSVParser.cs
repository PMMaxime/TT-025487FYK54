using CSVToJSON.Services.CSVParser.Models;
using System.Collections.Generic;

namespace CSVToJSON.Services.CSVParser.Interfaces
{
    public interface ICSVParser
    {
        public IEnumerable<IEnumerable<CSVBaseValue>> ParseCsvFile(string filePath, char separator);
    }
}
