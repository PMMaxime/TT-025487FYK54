using _Tests_CSVToJSON.Utils.Interfaces;
using CSVToJSON.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<IEnumerable<string>> ParseCsvFile(string filePath)
        {
            var fileText = _fileUtils.ReadAllText(filePath);
            var parsedFile = parseRowsFromFileText(fileText);
            return parsedFile;
        }

        private IEnumerable<IEnumerable<string>> parseRowsFromFileText(string fileText, char separator = SEPARATOR_CHAR)
        {
            var rows = new List<List<string>>();
            var values = new List<string>();
            var value = string.Empty;

            for (int i = 0; i < fileText.Length; i++)
            {
                if (fileText[i] == separator)
                {
                    values.Add(cleanValue(value));
                    value = string.Empty;
                    continue;
                }
                if (LINEJUMP_CHARS.Any(linejump => value.Contains(linejump)))
                {
                    values.Add(cleanValue(value));
                    value = string.Empty;
                    rows.Add(new List<string>(values));
                    values.Clear();
                    continue;
                }

                value += fileText[i];
            }
            values.Add(value);
            rows.Add(new List<string>(values));

            return rows;
        }


        private string cleanValue(string value)
        {
            LINEJUMP_CHARS.ForEach(linejump => value = value.Replace(linejump, string.Empty));
            return value;
        }
    }
}
