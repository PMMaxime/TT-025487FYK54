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
        public class Options
        {
            [Option('h', "headers", Required = false, HelpText = "Are headers present in the csv input file ?")]
            public bool Headers { get; set; }
        }
        public static void Main(string[] args)
        {
            bool withHeaders = false;
            Parser.Default.ParseArguments<Options>(args).WithParsed(opts => withHeaders = opts.Headers);

            using IHost host = ConfigureServices(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Startup>>();
            try
            {
                logger.LogInformation($"--------------------------------------{Environment.NewLine} ... Process started ... {Environment.NewLine}--------------------------------------");
                host.Services.GetRequiredService<Program>().Run(withHeaders);
                logger.LogInformation($"-------------------{Environment.NewLine} ... Process ended without errors... {Environment.NewLine}-------------------");

            }
            catch (Exception e)
            {
                logger.LogError($"--------------------------------------{Environment.NewLine} Process ended unexpectedly for the following reason :{Environment.NewLine} {e.Message} {Environment.NewLine} --------------------------------------");
            }
            logger.LogInformation("Press any key to shut down the application ...");
            Console.ReadLine();
        }

        private static IHostBuilder ConfigureServices(string[] args)
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