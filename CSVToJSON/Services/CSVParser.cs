using CSVToJSON.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSVToJSON.Services
{
    public class CSVParser : ICSVParser
    {
        public readonly ILogger<CSVParser> _logger;

        public CSVParser(ILogger<CSVParser> logger)
        {
            _logger = logger;
        }
    }
}
