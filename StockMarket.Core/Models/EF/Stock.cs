using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Core.Models
{
    public class Stock
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Stock Name")]
        [Index("StockIndex", order: 1, IsUnique = true)]
        [StringLength(maximumLength: 30, MinimumLength = 5, ErrorMessage = "Allowed length 5-30 characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Stock Code")]
        [Index("StockIndex", order: 2, IsUnique = true)]
        [StringLength(maximumLength: 10, MinimumLength = 2, ErrorMessage = "Allowed length 2-10 characters.")]
        public string Code { get; set; }
        
        [Display(Name = "Available Amount")]
        [Range(0d, int.MaxValue, ErrorMessage = "Available Amount cannot be negative.")]
        public int AvailableAmount { get; set; }

        public virtual ICollection<StockValue> Values { get; set; }

        public Stock()
        {
            Values = new List<StockValue>();
        }
    }
}