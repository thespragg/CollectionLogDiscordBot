using CollectionLogBot.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CollectionLogBot.Helpers
{
    public class SendDropService : IHostedService, IDisposable
    {
        private Timer MessageTimer { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            MessageTimer = new Timer(SendDrops, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private async void SendDrops(object o)
        {
            var drops = BankHelper.Drops;
            foreach (var drop in drops)
            {
                var added = await BankHelper.AddItemToBank(drop);
                if (ConfigHelper.Config.Channel == default || !added) return;
                await (await Program._client.GetChannelAsync(ConfigHelper.Config.Channel)).SendMessageAsync(
                    $"Added item to bank from runelite! {drop.Username} {drop.ItemName} {drop.ItemQuantity} {drop.EventName} {drop.ItemId}");
            }

            drops = new List<GameDrop>();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            MessageTimer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            MessageTimer?.Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
