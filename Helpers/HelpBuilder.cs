using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionLogBot.Helpers
{
    public static class HelpBuilder
    {
        public static string BuildHelp(bool isAdmin = false)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Collection Log Bot Help" + Environment.NewLine);

            if (isAdmin)
            {
                builder.AppendLine("===== Config commands =====");
                builder.AppendLine("!set channel <number>");
            }
            
            return builder.ToString();
        }
    }
}
