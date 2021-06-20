using CSVToJSON.Services.CSVParser.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSVToJSON
{
    internal class Program
    {
        private readonly ILogger<Program> _logger;
        private readonly ICSVParser _csvParser;

        public Program(ILogger<Program> logger, ICSVParser csvParser)
        {
            _logger = logger;
            _csvParser = csvParser;
        }

        public void Run()
        {
            _logger.LogInformation("... Application Started ...");
        }
    }
}
