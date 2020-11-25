using System;
using System.Linq;
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
                if (msg.Length != 3) return;
                var itemId = CollectionHandler.GetItemId(msg[2]);
                if (itemId != null)
                {
                    await BankHelper.AddItemToBank((int)itemId, e.Author.Username, DateTime.Now);
                    res = $"Added {msg[2]} to the collection log";
                }
                else res = "No item with that name found";
            }
            if (msg[1] == "collections") res = string.Join(',', CollectionHandler.CollectionNames());
            if (msg[1] == "bank") res = string.Join(',', BankHelper.GetFullBank().Select(x => x.Name));



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
                await (await Program._client.GetChannelAsync(channel)).SendFileAsync(img,$"Collection log for {joined}");
                return;
            }

            await e.Message.RespondAsync(res);
        }
    }
}
