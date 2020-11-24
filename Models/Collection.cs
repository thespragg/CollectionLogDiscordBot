using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionLogBot.Models
{
    public class Collection
    {
        public string Name { get; set; }
        public string KillCount { get; set; }
        public List<Item> Items { get; set; }
    }
}
