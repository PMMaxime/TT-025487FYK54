using CSVToJSON.Services.CSVParser.Exceptions;
using CSVToJSON.Services.CSVParser.Interfaces;
using CSVToJSON.Services.CSVParser.Models;
using CSVToJSON.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSVToJSON.Services.CSVParser
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
            var parsedCsvFile = parseRowsFromFileText(fileText, separator);
            validateParsedCsvFile(parsedCsvFile);
            return parsedCsvFile;
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

        private void validateParsedCsvFile(IEnumerable<IEnumerable<CSVBaseValue>> rows)
        {
            var referenceColNb = rows.ElementAt(0).Count();
            var errors = new List<string>();
            var rowCount = 0;
            foreach (var row in rows)
            {
                if (row.Count() != referenceColNb)
                {
                    var errorMsg = $"Invalid number of values at line {rowCount} : {row.Count()} values found instead of {referenceColNb} (top line being the reference). The CSV file might be inconsistent in its format.";
                    errors.Add(errorMsg);
                    _logger.LogError(errorMsg);
                }
                rowCount++;
            }

            if (errors.Any()) throw new CSVParserException(String.Join("\r\n", errors));
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
