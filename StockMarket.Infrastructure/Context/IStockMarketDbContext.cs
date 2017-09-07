using StockMarket.Core.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace StockMarket.Infrastructure.Context
{
    public interface IStockMarketDbContext
    {
        DbSet<Stock> Stock { get; set; }
        DbSet<StockValue> StockValue { get; set; }
        DbSet<User> UserApp { get; set; }
        DbSet<UserStocks> UserStocks { get; set; }

        int SaveChanges();
        Database Database { get; }
        DbEntityEntry Entry(object entity);
    }
}
