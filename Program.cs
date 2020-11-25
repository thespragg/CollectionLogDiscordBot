using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionLogBot.Helpers;
using CollectionLogBot.Models;
using DSharpPlus;

namespace CollectionLogBot
{
    class Program
    {
        public static DiscordClient _client;
        
        static async Task Main(string[] args)
        {
            await CollectionHandler.LoadFromFile();
            await BankHelper.LoadBank();

            _client = await DiscordHelper.ConnectToClient(CommandParser.ParseMessage);

            await Task.Delay(-1);
        }
    }
}
