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
        private readonly Regex matchUnnecessaryQuotes = new Regex(@"[\""]{1}(?![\""])");
        private readonly Regex matchUnquotedLinejumps = new Regex(@"((?!\""[\w\s]*[\""]*[\w\s]*)[\r\n|\r|\n](?![\w\s]*[\""]*[\w\s]*\""))");

        public CSVParser(ILogger<CSVParser> logger, IFileUtils fileUtils)
        {
            _logger = logger;
            _fileUtils = fileUtils;
        }

        /// <summary>
        /// Gets a two dimensional list of parsed values from a csv file.
        /// </summary>
        public IEnumerable<IEnumerable<CSVBaseValue>> ParseCsvFile(string filePath, char separator = SEPARATOR_CHAR)
        {
            _logger.LogInformation($"Begin parsing file at : {filePath} ...");

            var fileText = _fileUtils.ReadAllText(filePath);
            var parsedCsvData = parseRowsFromFileText(fileText, separator);
            validateParsedCsvFile(parsedCsvData);

            _logger.LogInformation($"Successfully parsed {parsedCsvData.Count()} lines.");

            return parsedCsvData;
        }

        /// <summary>
        /// Iterates overs the whole text content of a csv file and returns it's content as parsed rows and columns.
        /// </summary>
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
                    if (matchUnquotedLinejumps.Match(value).Success)
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

        /// <summary>
        /// Checks the consistency of the parsed data by checking the number of columns of each rows. The reference count is the first column's.
        /// </summary>
        private void validateParsedCsvFile(IEnumerable<IEnumerable<CSVBaseValue>> rows)
        {
            var referenceColNb = rows.ElementAt(0).Count();
            var errors = new List<string>();
            var rowCount = 0;

            _logger.LogInformation($"Validating {rows.Count()} lines ...");

            foreach (var row in rows)
            {
                if (row.Count() != referenceColNb)
                {
                    var errorMsg = $"Invalid number of values at line {rowCount + 1} : {row.Count()} values found instead of {referenceColNb} (top line being the reference). The CSV file might be inconsistent in its format.";
                    errors.Add(errorMsg);
                }
                rowCount++;
            }

            if (errors.Any())
            {
                _logger.LogWarning($"{errors.Count()} validation errors found !");
                throw new CSVParserException(String.Join("\r\n\r\n", errors));
            }

            _logger.LogInformation($"CSV file is valid.");
        }

        /// <summary>
        /// Factory-like method that return the right derived class for a CSVBaseValue according to the parsing abilities of the raw value.
        /// </summary>
        private CSVBaseValue TryCastValue(string value)
        {
            if (int.TryParse(value, out int castedValue)) return new CSVIntValue(castedValue);
            else return new CSVStringValue(value);
        }

        /// <summary>
        /// Removes unwanted linejumps chars and quotes from a raw value.
        /// </summary>
        private string cleanValue(string value)
        {
            LINEJUMP_CHARS.ForEach(linejump => value = value.Replace(linejump, string.Empty));
            return matchUnnecessaryQuotes.Replace(value, string.Empty);
        }
    }
}
