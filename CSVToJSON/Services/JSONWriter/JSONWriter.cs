using CSVToJSON.Services.CSVParser.Models;
using CSVToJSON.Services.JSONWriter.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVToJSON.Services.JSONWriter
{
    public class JSONWriter : IJSONWriter
    {
        private readonly ILogger<JSONWriter> _logger;

        public JSONWriter(ILogger<JSONWriter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Outputs parsed two dimensional IEnumerable data to a json object in a file.
        /// </summary>
        public void WriteCsvDataToJsonFile(IEnumerable<IEnumerable<CSVBaseValue>> csvData, string filepath, bool withHeaders = false)
        {
            dynamic jsonData;

            _logger.LogInformation($"Begin writing parsed data to json file at : {filepath} ...");

            if (withHeaders)
                jsonData = GetStructuredJsonDataFromCsvData(csvData);
            else
                jsonData = GetFlatJsonDataFromCsvData(csvData);

            File.WriteAllText(filepath, JsonConvert.SerializeObject(jsonData));

            _logger.LogInformation($"{csvData.Count()} json elements Successfully written. ");
        }

        public IEnumerable<JObject> GetStructuredJsonDataFromCsvData(IEnumerable<IEnumerable<CSVBaseValue>> csvData)
        {
            var jsonData = new List<JObject>();
            var headers = csvData.First();

            for (var row = 1; row < csvData.Count(); row++)
            {
                dynamic jsonRow = new JObject();

                for (var col = 0; col < headers.Count(); col++)
                    jsonRow[headers.ElementAt(col).CastedValue] = csvData.ElementAt(row).ElementAt(col).CastedValue;

                jsonData.Add(jsonRow);
            }

            return jsonData;
        }

        public List<List<JValue>> GetFlatJsonDataFromCsvData(IEnumerable<IEnumerable<CSVBaseValue>> csvData)
        {
            var jsonData = new List<List<JValue>>();

            foreach (var row in csvData)
                jsonData.Add(new List<JValue>(row.Select(v => new JValue(v.CastedValue))));

            return jsonData;
        }
    }
}
