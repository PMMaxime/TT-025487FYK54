using CommandLine;
using CSVToJSON.Services.CSVParser;
using CSVToJSON.Services.CSVParser.Interfaces;
using CSVToJSON.Services.JSONWriter;
using CSVToJSON.Services.JSONWriter.Interfaces;
using CSVToJSON.Utils;
using CSVToJSON.Utils.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CSVToJSON
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            using IHost host = configureServices(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Startup>>();

            try
            {
                parseArgs(args, out string filePath, out bool withHeaders);
                logger.LogInformation($"--------------------------------------{Environment.NewLine} ... Process started ... {Environment.NewLine}--------------------------------------");
                host.Services.GetRequiredService<Program>().Run(filePath, withHeaders);
                logger.LogInformation($"-------------------{Environment.NewLine} ... Process ended without errors... {Environment.NewLine}-------------------");

            }
            catch (Exception e)
            {
                logger.LogError($"--------------------------------------{Environment.NewLine} Process ended unexpectedly for the following reason :{Environment.NewLine} {e.Message} {Environment.NewLine} --------------------------------------");
            }
            logger.LogInformation("Press any key to shut down the application ...");
            Console.ReadLine();
        }


        private static void parseArgs(string[] args, out string filePath, out bool withHeaders)
        {
            string _filePath = null;
            bool? _withHeaders = null;

            Parser.Default.ParseArguments<CmdLineArgs>(args).WithParsed(opts =>
            {
                _withHeaders = opts.Headers;
                _filePath = opts.FilePath;
            });

            if (string.IsNullOrEmpty(_filePath))
                throw new ArgumentNullException("The file path (-f) MUST be passed as an argument when executing this process.");

            filePath = _filePath;
            withHeaders = _withHeaders.HasValue ? _withHeaders.Value : false;

        }

        private static IHostBuilder configureServices(string[] args)
        {
            return
                Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();
                    var env = hostingContext.HostingEnvironment;
                    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    var configurationRoot = configuration.Build();
                })
                .ConfigureServices((_, services) => services
                .AddLogging(conf => conf.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddSingleton<Program>()
                .AddTransient<IFileUtils, FileUtils>()
                .AddSingleton<ICSVParser, CSVParser>()
                .AddSingleton<IJSONWriter, JSONWriter>());
        }
    }
}