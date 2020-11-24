using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionLogBot.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Obtained { get; set; }
        public int Quantity { get; set; }
    }
}
