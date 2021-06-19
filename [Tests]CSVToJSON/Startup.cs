using CSVToJSON.Services;
using CSVToJSON.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace _Tests_CSVToJSON
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ICSVParser, CSVParser>();
        }
    }
}


