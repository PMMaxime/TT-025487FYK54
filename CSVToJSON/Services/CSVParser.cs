using _Tests_CSVToJSON.Utils.Interfaces;
using CSVToJSON.Services.Interfaces;
using CSVToJSON.Services.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSVToJSON.Services
{
    public class CSVParser : ICSVParser
    {
        private readonly IFileUtils _fileUtils;
        private readonly ILogger<CSVParser> _logger;

        private List<string> LINEJUMP_CHARS = new List<string>(3) { "\r\n", "\n", "\r" };
        private const char SEPARATOR_CHAR = ',';
        private const char QUOTATION_CHAR = '\"';
        Regex matchUnnecessaryQuotes = new Regex(@"[\""]{1}(?![\""])");

        public CSVParser(ILogger<CSVParser> logger, IFileUtils fileUtils)
        {
            _logger = logger;
            _fileUtils = fileUtils;
        }

        public IEnumerable<IEnumerable<CSVBaseValue>> ParseCsvFile(string filePath, char separator = SEPARATOR_CHAR)
        {
            var fileText = _fileUtils.ReadAllText(filePath);
            return parseRowsFromFileText(fileText, separator);
        }

        private IEnumerable<IEnumerable<CSVBaseValue>> parseRowsFromFileText(string fileText, char separator)
        {

            var rows = new List<List<CSVBaseValue>>();
            var values = new List<CSVBaseValue>();
            var value = string.Empty;
            var valueIsQuotation = false;

            for (int i = 0; i < fileText.Length; i++)
            {
                var currentChar = fileText[i];
                var nextChar = fileText[i + 1 >= fileText.Length ? fileText.Length - 1 : i + 1];
                if (!valueIsQuotation)
                {
                    if (currentChar == separator)
                    {
                        values.Add(TryCastValue(cleanValue(value)));
                        value = string.Empty;
                        valueIsQuotation = false;
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
                }
                value += currentChar;
                valueIsQuotation = currentChar == QUOTATION_CHAR && nextChar != QUOTATION_CHAR ? !valueIsQuotation : valueIsQuotation;
            }
            values.Add(TryCastValue(cleanValue(value)));
            rows.Add(new List<CSVBaseValue>(values));

            return rows;
        }

        private CSVBaseValue TryCastValue(string value)
        {
            if (int.TryParse(value, out int castedValue)) return new CSVIntValue(castedValue);
            else return new CSVStringValue(value);
        }

        private string cleanValue(string value)
        {
            LINEJUMP_CHARS.ForEach(linejump => value = value.Replace(linejump, string.Empty));
            return matchUnnecessaryQuotes.Replace(value, string.Empty);
        }
    }
}
