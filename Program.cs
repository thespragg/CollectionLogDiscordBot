using System;
using System.Threading.Tasks;
using CollectionLogBot.Helpers;
using DSharpPlus;

namespace CollectionLogBot
{
    class Program
    {
        private static DiscordClient _client;
        static async Task Main(string[] args)
        {
            _client = await DiscordHelper.ConnectToClient(CommandParser.ParseMessage);
            await Task.Delay(-1);
        }
    }
}
