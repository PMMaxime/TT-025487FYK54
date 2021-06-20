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

namespace CSVToJSON
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            using IHost host = ConfigureServices(args).Build();
            host.Services.GetRequiredService<Program>().Run();
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
