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
        public List<Drop> Drops { get; set; }
        public Item() =>(Drops,Tradeable,Price) = (new List<Drop>(),true,"0");
        public string Price { get; set; }
        public DateTime LastChecked { get; set; }
        public bool Tradeable { get; set; }
    }

    public class Drop
    {
        public string Username { get; set; }
        public DateTime Dropped { get; set; }
        public Drop(string username, DateTime dropped) => (Username, Dropped) = (username, dropped);
        public Drop() { }
    }
}
