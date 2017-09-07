using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Core.Models
{
    [NotMapped]
    public class StockJSON
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Unit { get; set; }
        public decimal Price { get; set; }
    }
}