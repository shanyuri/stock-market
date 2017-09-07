using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Core.Models
{
    [NotMapped]
    public class StocksJSON
    {
        public DateTime PublicationDate { get; set; }
        public IList<StockJSON> Items { get; set; }
    }
}