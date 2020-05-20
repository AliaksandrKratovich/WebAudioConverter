using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NAudio.MediaFoundation;
using WebAudioConverter.Helpers;

namespace WebAudioConverter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FilesHelpers.ClearAllData();
            MediaFoundationApi.Startup();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
