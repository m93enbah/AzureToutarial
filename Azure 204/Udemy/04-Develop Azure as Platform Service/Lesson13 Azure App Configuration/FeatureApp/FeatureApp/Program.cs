using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FeatureApp
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

                webBuilder.ConfigureAppConfiguration(config =>
                {
                    var settings = config.Build();
                    config.AddAzureAppConfiguration(
                        opt => opt.Connect("Endpoint=https://enbehconfig.azconfig.io;Id=RQqB-l9-s0:kx+/Am4URhLDhZbVZ81a;Secret=1OEbqNa7h8ftoNtK1Yz84hMYRt9digJTAZ4JVJTCsoA=")
                        .UseFeatureFlags());
                }).UseStartup<Startup>());
    }
}
