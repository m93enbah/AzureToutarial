using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Sqlapp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                webBuilder.ConfigureAppConfiguration(config => {
                    var settings = config.Build();
                    config.AddAzureAppConfiguration("Endpoint=https://enbehappconfig.azconfig.io;Id=V8yt-l9-s0:f9OiHcz11rHrsNfKgNm0;Secret=0EDZBj8xxOh4ktqM+rrrU4EBHt+iDNqeSDseNanLMr8=");
                }).UseStartup<Startup>());
    }
}
