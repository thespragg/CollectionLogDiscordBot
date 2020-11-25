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
            }

            builder.AppendLine("Helpers");
            builder.AppendLine("!log collections -- Lists the collections");
            builder.AppendLine();

            builder.AppendLine("!log add <item name> -- Adds an item to the log");
            builder.AppendLine("!log collection <collection name> -- Shows log page");
            builder.AppendLine("!log item <item name> -- Shows drop history of the item");

            return builder.ToString();
        }
    }
}
