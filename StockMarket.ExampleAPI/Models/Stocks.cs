using System;
using System.Collections.Generic;

namespace StockMarket.ExampleAPI.Models
{
    public class Stocks
    {
        public DateTime PublicationDate { get; set; }
        public IList<Stock> Items { get; set; }
    }
}
