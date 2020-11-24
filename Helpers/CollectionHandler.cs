using CollectionLogBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CollectionLogBot.Helpers
{
    public static class CollectionHandler
    {
        private static List<Collection> _collections;

        public static async Task<List<Collection>> LoadFromFile()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/collection-log.json");
            return JsonSerializer.Deserialize<List<Collection>>(await File.ReadAllTextAsync(path),new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
        }
    }
}
