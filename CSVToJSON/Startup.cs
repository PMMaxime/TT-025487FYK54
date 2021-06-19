using CSVToJSON.Services;
using CSVToJSON.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSVToJSON
{
    internal class Startup
    {
        static void Main(string[] args)
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
                .AddSingleton<ICSVParser, CSVParser>());
        }
    }
}
