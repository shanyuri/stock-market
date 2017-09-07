using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StockMarket.Core.Models
{
    public class User : IdentityUser
    {
        [Required]
        [Column(TypeName = "Money")]
        [Range(0.0d, double.MaxValue, ErrorMessage = "The Money field cannot be negative.")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal Money { get; set; }

        public virtual ICollection<UserStocks> Stocks { get; set; }

        public User()
        {
            Stocks = new List<UserStocks>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}