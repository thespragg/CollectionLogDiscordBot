using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace CollectionLogBot.Helpers
{
    public static class CommandParser
    {
        public static async Task ParseMessage(MessageCreateEventArgs e)
        {
            if (e.Message.Content.StartsWith("!rhs")) return;
            if (!e.Message.Content.StartsWith("!")) return;
            //If a channel has been set then respond that the user should use that channel
            if (ConfigHelper.Config.Channel != default && e.Message.Channel.Id != ConfigHelper.Config.Channel)
            {
                await e.Message.RespondAsync("Please use the correct channel");
                return;
            }

            var msg = e.Message.Content.Split(" ");
            var res = "Your command wasn't recognized, please use '!help' to see all commands";

            if (msg[0].Equals("!set"))
            {
                if (msg.Length != 3) return;
                var verb = msg[1];

                switch (verb)
                {
                    case "channel":
                        await ConfigHelper.ChangeChannel(ulong.Parse(msg[2]));
                        res = $"The channel has been set to {msg[2]}";
                        break;
                }
                
            }

            if (msg[0] == "!help") res = HelpBuilder.BuildHelp();

            await e.Message.RespondAsync(res);
        }
    }
}
