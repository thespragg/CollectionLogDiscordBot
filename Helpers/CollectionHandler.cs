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
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/collection-log.json");
            _collections = JsonSerializer.Deserialize<List<Collection>>(await File.ReadAllTextAsync(path),new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
        }

        public static Item GetItem(int id)
        {
            return _collections.SelectMany(x => x.Items).ToList().FirstOrDefault(x=>x.Id == id);
        }

        public static int? GetItemId(string name)
        {
            return _collections.SelectMany(x => x.Items).ToList().FirstOrDefault(x => x.Name == name)?.Id;
        }
    }
}
