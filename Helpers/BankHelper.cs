using CollectionLogBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CollectionLogBot.Helpers
{
    public class BankHelper
    {
        private static List<Item> _items = new List<Item>();
        private static string baseDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"), "Bank");

        public static async Task LoadBank()
        {
            foreach(var file in Directory.GetFiles(baseDir))
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
            if (!Directory.Exists(baseDir)) CreateBank();
            Item item;
            var path = Path.Combine(baseDir, $"{id}.json");

            if (_items.Any(x=>x.Id == id))
            {
                item = _items.FirstOrDefault(x => x.Id == id);
            }
            else {
                item = CollectionHandler.GetItem(id);
                _items.Add(item);
            }
            item.Quantity += 1;
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
    }
}
