using CSVToJSON.Services.CSVParser.Interfaces;
using CSVToJSON.Services.JSONWriter.Interfaces;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CSVToJSON
{
    internal class Program
    {
        private readonly ILogger<Program> _logger;
        private readonly ICSVParser _csvParser;
        private readonly IJSONWriter _jsonWriter;

        public Program(ILogger<Program> logger, ICSVParser csvParser, IJSONWriter jsonWriter)
        {
            _logger = logger;
            _csvParser = csvParser;
            _jsonWriter = jsonWriter;
        }

        public void Run(string filePath, bool withHeaders)
        {
            var parsedCsvData = _csvParser.ParseCsvFile(filePath);

            var outputFilePath = filePath.Replace($"{Path.GetExtension(filePath)}", ".json");

            _jsonWriter.WriteCsvDataToJsonFile(parsedCsvData, outputFilePath, withHeaders);
        }
    }
}
