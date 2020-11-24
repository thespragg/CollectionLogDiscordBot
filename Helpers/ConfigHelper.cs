using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CollectionLogBot.Models;

namespace CollectionLogBot.Helpers
{
    public static class ConfigHelper
    {
        public static Config Config { get; set; }

        public static async Task<Config> LoadConfig()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "conf.json");
            Config = JsonSerializer.Deserialize<Config>(await File.ReadAllTextAsync(path));
            return Config;
        }

        public static async Task ChangeChannel(ulong channel)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "conf.json");
            Config.Channel = channel;
            var serializedConfig = JsonSerializer.Serialize(Config);
            await File.WriteAllTextAsync(path,serializedConfig);
        }
    }
}
