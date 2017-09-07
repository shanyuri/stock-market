using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockMarket.Core.Models.ViewModels
{
    public class UserWalletViewModel
    {
        public IEnumerable<UserWalletStocks> userStocks;
        public decimal Money { get; set; }
    }

    public class UserWalletStocks
    {
        [Display(Name = "Stock Code")]
        public string Code { get; set; }
        [Display(Name = "Stock Name")]
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int Unit { get; set; }
        [Display(Name = "Unit Price")]
        public decimal Price { get; set; }
        [Display(Name = "Total Value")]
        public decimal TotalValue => Amount / Unit * Price;
    }
}