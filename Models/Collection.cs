using System.Collections.Generic;

namespace CollectionLogBot.Models
{
    public class Collection
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<Item> Items { get; set; }
    }
}
