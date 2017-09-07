using StockMarket.Core.Models;
using StockMarket.Core.Models.ViewModels;
using StockMarket.Infrastructure.Enum;
using System;
using System.Threading.Tasks;

namespace StockMarket.Infrastructure.Services
{
    public interface IStockService
    {
        bool AddStockValuesForExistingStocksIfNewer(StocksJSON stocksJson);
        int AddStockIfNotExists(string stockCode, string stockName, int availableAmount = 0);
        StockValue GetLatestValueForStock(int stockId);
        bool AddStockValueIfNewer(int stockId, int unit, decimal price, DateTime publicationDate);
        StockDetails GetLatestStockDetails(string stockCode);
        int GetUserStockAmountForCode(string userId, string stockCode);
        StockBuyStatus BuyStock(string userId, BuyStockViewModel buyStockModel);
        StockSellStatus SellStock(string userId, SellStockViewModel sellStockModel);
    }
}
