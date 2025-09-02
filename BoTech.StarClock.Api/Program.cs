using BoTech.StarClock.Helper;
using Microsoft.OpenApi.Models;

namespace BoTech.StarClock.Api;
public class Program
{
    /// <summary>
    /// This string represents the version of the App.
    /// </summary>
    public static string VersionString { get; private set; }
    public static ApiStatusInfo ApiStatus { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="versionString"> This string represents the version of the App.</param>
    public static void Main(string versionString)
    {
        VersionString = versionString;
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                
                webBuilder.UseStartup<Startup>();
                // Configure https in another way on linux => raspberry os.
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                  /*  webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenLocalhost(7080,
                            listenOptions =>
                            {
                                listenOptions.UseHttps("dev.pfx", "BobiDieBiene.123!"); 
                            });
                    });
                    Console.WriteLine("Init ssl for linux.");*/
                }
                else
                {
                    //webBuilder.UseUrls("https://localhost:7082","http://localhost:5232"); 
                }
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


