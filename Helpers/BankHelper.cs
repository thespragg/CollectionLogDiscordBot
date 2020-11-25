using CollectionLogBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CollectionLogBot.Helpers
{
    public class BankHelper
    {
        private static readonly List<Item> _items = new List<Item>();
        private static readonly string baseDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Data"), "Bank");

        public static async Task LoadBank()
        {
            if (!Directory.Exists(baseDir)) CreateBank();
            foreach (var file in Directory.GetFiles(baseDir))
            {
                var raw = await File.ReadAllTextAsync(file);
                var item = JsonSerializer.Deserialize<Item>(raw);
                _items.Add(item);
            }
        }

        private static void CreateBank()
        {
            Directory.CreateDirectory(baseDir);
        }

        public static List<Item> GetFullBank()
        {
            return _items;
        }

        public static async Task AddItemToBank(int id, string username, DateTime dropped)
        {
            Item item;
            var path = Path.Combine(baseDir, $"{id}.json");

            if (_items.Any(x=>x.Id == id)) item = _items.FirstOrDefault(x => x.Id == id);
            else {
                item = CollectionHandler.GetItem(id);
                _items.Add(item);
            }

            item!.Quantity += 1;
            item.Drops.Add(new Drop(username, dropped));
            item.Obtained = true;

            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(item));
        }

        public static Item GetItemFromBank(int id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }

        public static IEnumerable<Item> GetItemsFromBank(IEnumerable<int> ids)
        {
            return _items.Where(x => ids.Contains(x.Id));
        }

        public static string GetLatestDrops(int amount = 5)
        {
            if (amount > 30) return "Please choose a smaller number to show";
            var res =  _items.SelectMany(x => x.Drops).OrderByDescending(x => x.Dropped).Take(amount).ToList();
            var builder = new StringBuilder();

            var count = 1;
            foreach (var x in res)
            {
                builder.AppendLine($"**{count}.**    *{x.Username}*    {x.Dropped}");
            }

            return builder.ToString();
        }
    }
}
