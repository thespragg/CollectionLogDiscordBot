using System;
using System.Collections.Generic;
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

            builder.AppendLine("!log add <item name>");
            builder.AppendLine("!log <collection name>");
            builder.AppendLine("!log <item name>");

            return builder.ToString();
        }
    }
}
