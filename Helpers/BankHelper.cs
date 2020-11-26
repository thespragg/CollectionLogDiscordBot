using CollectionLogBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CollectionLogBot.Helpers
{
    public class BankHelper
    {
        private static readonly List<Item> _items = new List<Item>();
        private static readonly string baseDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Data"), "Bank");
        private static void CreateBank() => Directory.CreateDirectory(baseDir);
        public static List<Item> GetFullBank() => _items;
        public static Item GetItemFromBank(string name) => _items.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        public static IEnumerable<Item> GetItemsFromBank(IEnumerable<int> ids) => _items.Where(x => ids.Contains(x.Id));
        private static readonly HttpClient ItemPriceClient = new HttpClient();

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

        public static async Task RemoveItemFromBank(string name, int dropIndex)
        {
            var item = _items.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (item == null) return;
            var path = Path.Combine(baseDir, $"{item.Id}.json");
            item.Quantity -= 1;
            item.Drops.RemoveAt(dropIndex-1);

            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(item));
        }

        public static async Task AddItemToBank(int id, string username, DateTime dropped)
        {
            Item item;
            var path = Path.Combine(baseDir, $"{id}.json");

            if (_items.Any(x => x.Id == id)) item = _items.FirstOrDefault(x => x.Id == id);
            else
            {
                item = CollectionHandler.GetItem(id);
                _items.Add(item);
            }

            item!.Quantity += 1;
            item.Drops.Add(new Drop(username, dropped));
            item.Obtained = true;

            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(item));
        }

        public static string GetLatestDrops(int amount = 5)
        {
            if (amount > 30) return "Please choose a smaller number to show";
            var res = _items.SelectMany(x => x.Drops).OrderByDescending(x => x.Dropped).Take(amount).ToList();
            var builder = new StringBuilder();

            var count = 1;
            foreach (var x in res)
            {
                builder.AppendLine($"**{count}.**    *{x.Username}*    {x.Dropped}");
            }

            return builder.ToString();
        }


        public static async Task<long> GetItemPrice(Item item)
        {
            if (!item.Tradeable) return 0;

            if (ValueFromKmb(item.Price) != 0 && item.LastChecked >= DateTime.Now.AddHours(-12)) return 0;

            var itemPromise = await ItemPriceClient.GetAsync(
                "http://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item=" + item.Id);
            if (!itemPromise.IsSuccessStatusCode)
            {
                _items.FirstOrDefault(x => x.Id == item.Id).Tradeable = false;
                return 0;
            }

            var itemDetails = JsonSerializer.Deserialize<RsItem>(await itemPromise.Content.ReadAsStringAsync());
            var price = itemDetails.item.current.price;
            return long.TryParse(price, out var lPrice) ? lPrice : ValueFromKmb(price);
        }

        public static string ValueToKmb(long val)
        {
            if (val > 999999999 || val < -999999999)
            {
                return Math.Round(val / 1000000000f, 3) + "b";
            }
            else if (val > 999999 || val < -999999)
            {
                return Math.Round(val / 1000000f, 3) + "m";
            }
            else if (val > 999 || val < -999)
            {
                return Math.Round(val / 1000f) + "k";
            }
            else
            {
                return $"{val:n0}";
            }
        }

        public static long ValueFromKmb(string val)
        {
            var newVal = val.ToLower().Replace(",", "");
            newVal = newVal.Replace(" ", "");
            newVal = newVal.ToLower().Replace("k", "");
            newVal = newVal.ToLower().Replace("m", "");
            newVal = newVal.ToLower().Replace("b", "");

            var items = newVal.Split('.');
            if (items.Length == 1) return long.Parse(items[0]);

            var price = items[0];
            var secondaryVal = items[1];

            if (val.Contains('b'))
            {
                price += secondaryVal + new string('0', 9);
                price = price.Remove(price.Length - secondaryVal.Length);
            }
            else if (val.Contains('m'))
            {
                price += secondaryVal + new string('0', 6);
                price = price.Remove(price.Length - secondaryVal.Length);
            }
            else if (val.Contains('k'))
            {
                price += secondaryVal + new string('0', 3);
                price = price.Remove(price.Length - secondaryVal.Length);
            }

            try
            {
                return long.Parse(price);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
