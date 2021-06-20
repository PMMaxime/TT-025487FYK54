using CSVToJSON.Services.CSVParser.Models;
using System.Collections.Generic;

namespace CSVToJSON.Services.JSONWriter.Interfaces
{
    public interface IJSONWriter
    {
        void WriteCsvDataToJsonFile(IEnumerable<IEnumerable<CSVBaseValue>> data, string filepath, bool withHeaders);
    }
}
