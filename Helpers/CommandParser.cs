using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace CollectionLogBot.Helpers
{
    public static class CommandParser
    {
        public static async Task ParseMessage(MessageCreateEventArgs e)
        {
            if (!e.Message.Content.ToLower().StartsWith("!log")) return;

            var isAdmin = e.Message.Author.Id == 140973520866770946 || e.Message.Author.Id == 329701123000631296;

            //If a channel has been set then respond that the user should use that channel
            if (ConfigHelper.Config.Channel != default && e.Message.Channel.Id != ConfigHelper.Config.Channel)
            {
                await e.Message.RespondAsync($"Please use the #{(await Program._client.GetChannelAsync(ConfigHelper.Config.Channel)).Name} channel.");
                return;
            }

            var msg = e.Message.Content.ToLower().Split(" ");
            var res = "Your command wasn't recognized, please use '!log help' to see all commands";

            if (msg[1].Equals("start")) res = HelpBuilder.BuildGettingStarted();

            if (msg[1].Equals("set") && isAdmin)
            {
                if (msg.Length != 4) return;
                var verb = msg[2];

                switch (verb)
                {
                    case "channel":
                        await ConfigHelper.ChangeChannel(ulong.Parse(msg[3]));
                        res = $"The channel has been set to {msg[3]}";
                        break;
                }
            }

            if (msg[1] == "history" && isAdmin)
            {
                res = msg.Length == 3 ? BankHelper.GetLatestDrops(int.Parse(msg[2])) : BankHelper.GetLatestDrops();
            }

            if (msg[1] == "help") res = HelpBuilder.BuildHelp(isAdmin);
            if (msg[1] == "add")
            {
                if (msg.Length < 3) return;
                var raw = msg.ToList();
                raw.RemoveAt(0);
                raw.RemoveAt(0);
                var joined = string.Join(' ', raw);
                var itemId = CollectionHandler.GetItemId(joined);
                if (itemId != null)
                {
                    await BankHelper.AddItemToBank((int)itemId, e.Author.Username, DateTime.Now,1);
                    res = $"Added {joined} to the collection log";
                }
                else res = "No item with that name found";
            }
            if (msg[1] == "collections") res = string.Join(',', CollectionHandler.CollectionNames());
            if (msg[1] == "bank")
            {
                var bankImage = await BankImageBuilder.CreateBankImage();
                var channel = ConfigHelper.Config.Channel != default ? ConfigHelper.Config.Channel : e.Channel.Id;
                await (await Program._client.GetChannelAsync(channel)).SendFileAsync(bankImage);
                return;
            }

            if (msg[1] == "item")
            {
                var raw = msg.ToList();
                raw.RemoveAt(0);
                raw.RemoveAt(0);
                var joined = string.Join(' ', raw);
                var drops = BankHelper.GetItemFromBank(joined)?.Drops;
                if (drops.Count != 0)
                {
                    var builder = new StringBuilder();

                    var count = 1;
                    foreach (var drop in drops)
                    {
                        builder.AppendLine(
                            $"**{count}** {drop.Username} *{drop.Dropped.ToString("dd/MM/yy hh:mm tt")}*");
                    }

                    res = builder.ToString();
                }
                else
                {
                    res = "No drops recorded for that item";
                }
            }

            if (msg[1] == "remove")
            {
                var raw = msg.ToList();
                raw.RemoveAt(0);
                raw.RemoveAt(0);
                if (raw.Count >= 2)
                {
                    var joined = string.Join(' ',raw.GetRange(0, raw.Count - 1));
                    var drop = raw[raw.Count - 1];

                    var success = int.TryParse(drop, out var dropInt);
                    if (success)
                    {
                        BankHelper.RemoveItemFromBank(joined, dropInt);
                        res = $"Removed item {joined}";
                    }
                }
            }

            if (msg[1] == "collection")
            {
                //Refactor this, its bad
                var raw = msg.ToList();
                raw.RemoveAt(0);
                raw.RemoveAt(0);
                var joined = string.Join(' ', raw);
                var collection = CollectionHandler.GetCollectionItems(joined);
                if (collection == null)
                {
                    await e.Message.RespondAsync(res);
                    return;
                }
                var img = CollectionLogImageBuilder.CreateImage(collection.Type, collection);
                var channel = ConfigHelper.Config.Channel != default ? ConfigHelper.Config.Channel : e.Channel.Id;
                await (await Program._client.GetChannelAsync(channel)).SendFileAsync(img);
                return;
            }

            await e.Message.RespondAsync(res);
        }
    }
}
