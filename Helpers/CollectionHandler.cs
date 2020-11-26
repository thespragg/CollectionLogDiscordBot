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

        public static Item GetItem(int id) => _collections.SelectMany(x => x.Items).ToList().FirstOrDefault(x => x.Id == id);
        public static int? GetItemId(string name) => _collections.SelectMany(x => x.Items).ToList().FirstOrDefault(x => x.Name.ToLower() == name)?.Id;
        public static List<string> CollectionNames() => _collections.Select(x => x.Name).ToList();
        public static List<string> CollectionNamesByType(string type) => _collections.Where(x => x.Type == type).Select(x => x.Name).ToList();

        public static List<KeyValuePair<string, bool>> CollectionNamesByTypeWithCompletedBool(string type)
        {
            var collections = _collections.Where(x => x.Type == type).Select(x=>x.Name).ToList();

            var res = new List<KeyValuePair<string, bool>>();

            foreach (var collection in collections)
            {
                var items = GetCollectionItems(collection).Items.Select(x=>x.Id);
                var isCompleted = BankHelper.GetFullBank().Distinct().Count(x => items.Contains(x.Id)) == items.Count();
                res.Add(new KeyValuePair<string, bool>(collection, isCompleted));
            }

            return res;
        }

        public static List<string> GetTypes() => _collections.Select(x => x.Type).Distinct().ToList();
        public static Collection GetCollectionItems(string name)
        {
            var collection = _collections.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (collection == null) return null;

            var bank = BankHelper.GetFullBank().Where(x => collection.Items.Select(z => z.Id).Contains(x.Id)).ToList();
            if (!bank.Any()) return collection;

            foreach (var item in bank)
            {
                var index = collection.Items.FindIndex(x => x.Id == item.Id);
                collection.Items[index] = item;
            }

            return collection;
        }
    }
}
