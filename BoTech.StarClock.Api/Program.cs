using BoTech.StarClock.Helper;
using Microsoft.OpenApi.Models;

namespace BoTech.StarClock.Api;
public class Program
{
    public static ApiStatusInfo ApiStatus { get; private set; }

    public static void Main()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("https://localhost:7084","http://localhost:5234"); 
                webBuilder.UseStartup<Startup>();
            });
        var host = builder.Build();
        host.Start();
    }
    /// <summary>
    /// Contains info about the api.
    /// </summary>
    /// <param name="urls"></param>
    /// <param name="environment"></param>
    public class ApiStatusInfo(List<string> urls, IWebHostEnvironment environment)
    {
        /// <summary>
        /// The list of URLs that the HTTP server is bound to.
        /// </summary>
        public List<string> Urls { get; private set; } = urls;
        /// <summary>
        /// The application's configured IWebHostEnvironment.
        /// </summary>
        public IWebHostEnvironment Environment { get; private set; } = environment;
    }
}


