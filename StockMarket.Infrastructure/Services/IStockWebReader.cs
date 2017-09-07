using StockMarket.Core.Models;
using System.Threading.Tasks;

namespace StockMarket.Infrastructure.Services
{
    public interface IStockWebReader
    {
        Task<StocksJSON> GetCurrentStocksAsync();
    }
}
