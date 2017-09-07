using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StockMarket.Core.Models
{
    public class StockValue
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [ForeignKey("Stock")]
        [Index("StockValueIndex", order: 2, IsUnique = true)]
        public int StockID { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Positive value must be provided.")]
        public int Unit { get; set; }

        [Required]
        [Column(TypeName = "Money")]
        [Range(0.0d, double.MaxValue, ErrorMessage = "Positive value must be provided.")]
        [Display(Name = "Unit Price")]
        [DisplayFormat(DataFormatString = "{0.N4}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Publication Date")]
        [Index("StockValueIndex", order: 1, IsUnique = true)]
        public DateTime PublicationDate { get; set; }

        public virtual Stock Stock { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"ID={ID}, ");
            builder.Append($"StockID={StockID}, ");
            builder.Append($"Unit={Unit}, ");
            builder.Append($"Price={Price}, ");
            builder.Append($"PublicationDate={PublicationDate}, ");

            return builder.ToString();
        }
    }
}