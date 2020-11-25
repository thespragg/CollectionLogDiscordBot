using CollectionLogBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CollectionLogBot.Helpers
{
    public static class CollectionHandler
    {
        private static List<Collection> _collections;

        public static async Task LoadFromFile()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Data/collection-log.json");
            _collections = JsonSerializer.Deserialize<List<Collection>>(await File.ReadAllTextAsync(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static Item GetItem(int id)
        {
            return _collections.SelectMany(x => x.Items).ToList().FirstOrDefault(x => x.Id == id);
        }

        public static int? GetItemId(string name)
        {
            return _collections.SelectMany(x => x.Items).ToList().FirstOrDefault(x => x.Name.ToLower() == name)?.Id;
        }

        public static List<string> CollectionNames => _collections.Select(x => x.Name).ToList();

        public static List<Item> GetCollectionItems(string name)
        {
            var items = _collections.FirstOrDefault(x => x.Name.ToLower() == name)?.Items;
            if (items == null) return null;

            var bank = BankHelper.GetFullBank().Where(x => items.Select(z => z.Id).Contains(x.Id)).ToList();
            if (!bank.Any()) return items;

            foreach (var item in bank)
            {
                var index = items.FindIndex(x => x.Id == item.Id);
                items[index] = item;
            }

            return items;
        }
    }
}
