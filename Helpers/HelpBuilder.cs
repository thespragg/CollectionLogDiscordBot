using System;
using System.Text;

namespace CollectionLogBot.Helpers
{
    public static class HelpBuilder
    {
        public static string BuildHelp(bool isAdmin)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Collection Log Bot Help" + Environment.NewLine);

            if (isAdmin)
            {
                builder.AppendLine("===== Config commands =====");
                builder.AppendLine("!log set channel <number>");
                builder.Append("!log history");
            }

            builder.AppendLine("Helpers");
            builder.AppendLine("!log bank");
            builder.AppendLine("!log collections -- Lists the collections");
            builder.AppendLine();

            builder.AppendLine("!log add <item name> -- Adds an item to the log");
            builder.AppendLine("!log collection <collection name> -- Shows log page");
            builder.AppendLine("!log item <item name> -- Shows drop history of the item");
            builder.AppendLine("!log remove <item name> <drop number> -- Removes the specified drop from the item");

            return builder.ToString();
        }

        public static string BuildGettingStarted()
        {
            var builder = new StringBuilder();
            builder.AppendLine("```diff");
            builder.AppendLine("++ GETTING STARTED ++");
            builder.AppendLine();
            builder.AppendLine("++ USING DISCORD ++");
            builder.AppendLine("To use discord for logging drops all you need is the following chat commands:");
            builder.AppendLine("-!log add <username> <item name>");
            builder.AppendLine("-!log collection <collection name>");
            builder.AppendLine("All further commands can be found under !log help");
            builder.AppendLine();
            builder.AppendLine("++ USING THE RUNELITE PLUGIN ++ ");
            builder.AppendLine("Setting up the runelite plugin takes a few extra steps than just using discord.");
            builder.AppendLine("First download 'Loot Logger External' from the runelite plugin hub");
            builder.AppendLine("Open the config for the plugin");
            builder.AppendLine("Next run the following command in discord:");
            builder.AppendLine("-!log register");
            builder.AppendLine("This will return a url with all the info filled out, paste this url into the 'Endpoint URL' box of the plugin config");
            builder.AppendLine("Next run this command in discord:");
            builder.AppendLine("-!log items");
            builder.AppendLine("Paste the response from that into the 'Items to track' box of the runelite plugin");
            builder.AppendLine("Leave the NPC's to track box empty");
            builder.AppendLine("```");

            return builder.ToString();
        }
    }
}
