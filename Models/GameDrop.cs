using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionLogBot.Models
{
    public class GameDrop
    {
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public int ItemQuantity { get; set; }
        public string EventName { get; set; }
        public string Username { get; set; }
    }
}
