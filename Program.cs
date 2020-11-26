using System.Threading.Tasks;
using CollectionLogBot.Helpers;
using DSharpPlus;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace CollectionLogBot
{
    class Program
    {
        public static DiscordClient _client;
        
        static async Task Main(string[] args)
        {
            await CollectionHandler.LoadFromFile();
            await BankHelper.LoadBank();
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
