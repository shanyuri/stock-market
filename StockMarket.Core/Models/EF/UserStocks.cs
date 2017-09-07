using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Core.Models
{
    public class UserStocks
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Index("UserIDStockIDIndex", order: 1, IsUnique = true)]
        public string UserID { get; set; }

        [Required]
        [Index("UserIDStockIDIndex", order: 2, IsUnique = true)]
        public int StockID { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Positive value must be provided.")]
        public int Amount { get; set; }

        public virtual User Owner { get; set; }
        public virtual Stock Stock { get; set; }
    }
}