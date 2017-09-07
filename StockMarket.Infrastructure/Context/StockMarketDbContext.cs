using StockMarket.Core.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace StockMarket.Infrastructure.Context
{
    public class StockMarketDbContext : IdentityDbContext, IStockMarketDbContext
    {
        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockValue> StockValue { get; set; }
        public DbSet<User> UserApp { get; set; }
        public DbSet<UserStocks> UserStocks { get; set; }

        public StockMarketDbContext()
            : base(nameOrConnectionString: "StockMarketDb")
        {
        }

        public static StockMarketDbContext Create()
        {
            return new StockMarketDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}