using System;
using System.Threading;
using System.Threading.Tasks;
using CollectionLogBot.Helpers;
using Microsoft.Extensions.Hosting;

namespace CollectionLogBot
{
    public class StartDiscordClient : IHostedService, IDisposable
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Program._client = await DiscordHelper.ConnectToClient(CommandParser.ParseMessage);
            Task.Delay(-1);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Program._client = null;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
