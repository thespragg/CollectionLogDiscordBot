using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CollectionLogBot.Models;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace CollectionLogBot.Helpers
{
    public static class DiscordHelper
    {
        public static async Task<DiscordClient> ConnectToClient(AsyncEventHandler<MessageCreateEventArgs> parserFunc)
        {
            var conf = await ConfigHelper.LoadConfig();
            var client = CreateClient(conf);
            client.MessageCreated += parserFunc;
            await client.ConnectAsync();
            return client;
        }

        private static DiscordClient CreateClient(Config conf)
        {
            return new DiscordClient(new DiscordConfiguration
            {
                Token = conf.Token,
                TokenType = TokenType.Bot
            });
        }
    }
}
