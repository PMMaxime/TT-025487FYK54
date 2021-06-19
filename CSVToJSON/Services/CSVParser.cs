using _Tests_CSVToJSON.Utils.Interfaces;
using CSVToJSON.Services.Interfaces;
using CSVToJSON.Services.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSVToJSON.Services
{
    public class CSVParser : ICSVParser
    {
        public readonly IFileUtils _fileUtils;
        public readonly ILogger<CSVParser> _logger;

        private List<string> LINEJUMP_CHARS = new List<string>(3) { "\r\n", "\n", "\r" };
        private const char SEPARATOR_CHAR = ',';

        public CSVParser(ILogger<CSVParser> logger, IFileUtils fileUtils)
        {
            _logger = logger;
            _fileUtils = fileUtils;
        }

        public IEnumerable<IEnumerable<CSVBaseValue>> ParseCsvFile(string filePath, char separator = SEPARATOR_CHAR)
        {
            var fileText = _fileUtils.ReadAllText(filePath);
            var parsedFile = parseRowsFromFileText(fileText, separator);
            return parsedFile;
        }

        private IEnumerable<IEnumerable<CSVBaseValue>> parseRowsFromFileText(string fileText, char separator)
        {
            var rows = new List<List<CSVBaseValue>>();
            var values = new List<CSVBaseValue>();
            var value = string.Empty;

            for (int i = 0; i < fileText.Length; i++)
            {
                if (fileText[i] == separator)
                {
                    values.Add(TryCastValue(cleanValue(value)));
                    value = string.Empty;
                    continue;
                }
                if (LINEJUMP_CHARS.Any(linejump => value.Contains(linejump)))
                {
                    values.Add(TryCastValue(cleanValue(value)));
                    value = string.Empty;
                    rows.Add(new List<CSVBaseValue>(values));
                    values.Clear();
                    continue;
                }

                value += fileText[i];
            }
            values.Add(TryCastValue(cleanValue(value)));
            rows.Add(new List<CSVBaseValue>(values));

            return rows;
        }

        private CSVBaseValue TryCastValue(string value)
        {
            if (int.TryParse(value, out int castedValue))
                return new CSVIntValue(castedValue);

            else return new CSVStringValue(value);
        }

        private string cleanValue(string value)
        {
            LINEJUMP_CHARS.ForEach(linejump => value = value.Replace(linejump, string.Empty));
            return value;
        }
    }
}
