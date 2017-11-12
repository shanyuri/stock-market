using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Core.Models
{
    public class StockDetails
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Unit { get; set; }
        public decimal Price { get; set; }
        public int AvailableAmount { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}