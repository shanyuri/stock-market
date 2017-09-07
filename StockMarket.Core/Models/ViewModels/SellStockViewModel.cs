using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockMarket.Core.Models.ViewModels
{
    public class SellStockViewModel : IValidatableObject
    {
        [Display(Name = "Stock code")]
        public string Code { get; set; }

        [Display(Name = "Stock name")]
        public string Name { get; set; }

        public int Unit { get; set; }
        
        [Display(Name = "Unit price")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal Price { get; set; }

        [Display(Name = "Publication date")]
        [DataType(DataType.DateTime)]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Your amount")]
        public int AmountAvailable { get; set; }

        [Display(Name = "Amount to sell")]
        public int AmountToSell { get; set; }

        [Display(Name = "Total cost")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal TotalCost => AmountToSell / Unit * Price;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (AmountToSell % Unit != 0)
            {
                results.Add(new ValidationResult("Amount must be a multiplication of unit value."));
            }

            if (AmountToSell <= 0)
            {
                results.Add(new ValidationResult("Amount value must be greater than 0."));
            }

            return results;
        }
    }
}