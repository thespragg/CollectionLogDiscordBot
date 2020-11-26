using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionLogBot.Models
{
    public class RsItem
    {
        public SubItem item { get; set; }
    }

    public class SubItem
    {
        public Price current { get; set; }
    }

    public class Price
    {
        public string price { get; set; }
    }
}
