using NLog;
using StockMarket.Core.Models;
using StockMarket.Core.Models.ViewModels;
using StockMarket.Infrastructure.Context;
using StockMarket.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockMarket.Infrastructure.Services
{
    public class StockService : IStockService
    {
        #region Fields
        private readonly IStockMarketDbContext _db;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        private StockService()
        {
        }

        public StockService(IStockMarketDbContext dbContext)
        {
            _db = dbContext;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Save stock values for existing stocks if provided stock's publication date is newer.
        /// </summary>
        /// <param name="stocks"></param>
        /// <returns></returns>
        public bool AddStockValuesForExistingStocksIfNewer(StocksJSON stocks)
        {
            if (stocks == null)
            {
                throw new ArgumentNullException();
            }
            if (stocks.Items?.Count == 0)
            {
                return false;
            }

            // select max publication date for stock values
            var maxPublicationDate = _db.StockValue
                .Select(x => x.PublicationDate).Max();
            
            // prepare stock codes
            var codesList = stocks.Items.Select(x => x.Code).ToList();
            
            // select existing stocks
            var existingStocks = _db.Stock
                .AsNoTracking()
                .Where(x => codesList.Contains(x.Code.ToLower()))
                .ToList();
           
            // check whether max publication date is older than publication date of fetched data
            if (DateTime.Compare(maxPublicationDate, stocks.PublicationDate) > -1) // first date is newer than or equal to second date
            {
                _logger.Info("Data skipped for publication date: " + stocks.PublicationDate.ToString());
                return false;
            }
            // ...there was an update
            var stockValues = new List<StockValue>();
            existingStocks.ForEach(stock =>
            {
                var price = stocks.Items
                    .Where(x => x.Code.ToLower().Equals(stock.Code.ToLower()))
                    .Select(x => x.Price)
                    .FirstOrDefault();
                var unit = stocks.Items
                    .Where(x => x.Code.ToLower().Equals(stock.Code.ToLower()))
                    .Select(x => x.Unit)
                    .FirstOrDefault();
                //var stockInfo = $"{stock.ID}, price={price}, unit={unit}";
                //_logger.Info(stockInfo);

                stockValues.Add(new StockValue
                {
                    StockID = stock.ID,
                    Price = price,
                    Unit = unit,
                    PublicationDate = stocks.PublicationDate
                });
            });

            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    stockValues.ForEach(s => _db.StockValue.Add(s));
                    int affectedRows = _db.SaveChanges();
                    dbContextTransaction.Commit();
                    _logger.Info("Successfully added new stocks values.");
                    return affectedRows > 0;
                }
                catch (Exception e)
                {
                    _logger.Error("Error during adding stocks values: " + e.Message);
                    _logger.Info("Rollback");
                    dbContextTransaction.Rollback();
                    return false;
                }
            }
        }
        
        /// <summary>
        /// Save stocks if not exists and creates stock values (if newer) for them.
        /// </summary>
        /// <param name="stocks"></param>
        /// <returns></returns>
        public bool AddStockWithValues(StocksJSON stocks)
        {
            int addedOrUpdatedStocks = 0;
            foreach (var stock in stocks.Items)
            {
                try
                {
                    int stockId = AddStockIfNotExists(stock.Code, stock.Name);
                    if (AddStockValueIfNewer(stockId, stock.Unit, stock.Price, stocks.PublicationDate))
                    {
                        addedOrUpdatedStocks++;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("Exception: " + e.Message);
                }
            }

            return addedOrUpdatedStocks > 0;
        }

        /// <summary>
        /// Creates Stock in database and returns ID of created row.
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="stockName"></param>
        /// <returns>Returns ID of created row.</returns>
        public int AddStockIfNotExists(string stockCode, string stockName, int availableAmount = 0)
        {
            int stockId = GetStockId(stockCode, stockName);
            if (stockId > 0)
            {
                return stockId;
            }

            Stock newStock = null;
            try
            {
                newStock = new Stock { Code = stockCode, Name = stockName, AvailableAmount = availableAmount };
                _db.Stock.Add(newStock);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error("Exception during adding new stock: " + e.Message);
            }

            return newStock.ID;
        }

        public StockValue GetLatestValueForStock(int stockId)
        {
            StockValue latestStockValue = null;

            try
            {
                latestStockValue = _db.StockValue
                    .AsNoTracking()
                    .Where(x => x.StockID == stockId)
                    .OrderByDescending(x => x.PublicationDate)
                    .Take(1)
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.Error("Exception during fetching latest value for stock: " + e.Message);
            }

            return latestStockValue;
        }

        public bool AddStockValueIfNewer(int stockId, int unit, decimal price, DateTime publicationDate)
        {
            bool saveResult = false;
            StockValue latestValueForStock = GetLatestValueForStock(stockId);
            if (latestValueForStock == null)
            {
                return saveResult;
            }

            // compare d1 to d2
            // d1 - publication date of the latest stock value for specified stock in Database
            // d2 - publication date of specified stock fetched from external server
            int dateComparisonResult = DateTime.Compare(latestValueForStock.PublicationDate, publicationDate);
            // Less than zero => d1 is earlier than d2.
            // Zero => d1 is the same as d2.
            // Greater than zero => d1 is later than d2.
            if (dateComparisonResult >= 0)
            {
                return saveResult;
            }

            //_logger.Info("Date comparison result: " + dateComparisonResult + ", latest in db: "
            //    + latestValueForStock.PublicationDate + ", current: " + publicationDate);

            try
            {
                var newStockValue = new StockValue
                {
                    StockID = stockId,
                    Unit = unit,
                    Price = price,
                    PublicationDate = publicationDate
                };
                _db.StockValue.Add(newStockValue);
                //_db.Entry(newStockValue).State = System.Data.Entity.EntityState.Added;
                saveResult = _db.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                _logger.Error("Exception during adding new stock VALUE: " + e.Message);
                if (e.InnerException != null)
                {
                    _logger.Error(e.InnerException.Message);
                }
            }

            return saveResult;
        }

        public StockDetails GetLatestStockDetails(string stockCode)
        {
            if (String.IsNullOrEmpty(stockCode))
            {
                throw new ArgumentException("Argument cannot be null or empty.");
            }

            StockDetails latestStockDetailsForStockCode = _db.StockValue
                .AsNoTracking()
                .Where(x => x.Stock.Code.ToLower().Equals(stockCode.ToLower()))
                .OrderByDescending(x => x.PublicationDate)
                .Take(1)
                .Select(x => new StockDetails
                {
                    Code = x.Stock.Code,
                    Name = x.Stock.Name,
                    Price = x.Price,
                    Unit = x.Unit,
                    AvailableAmount = x.Stock.AvailableAmount,
                    PublicationDate = x.PublicationDate
                })
                .FirstOrDefault();

            return latestStockDetailsForStockCode;
        }

        public int GetUserStockAmountForCode(string userId, string stockCode)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(stockCode))
            {
                throw new ArgumentException("Argument(s) cannot be null or empty.");
            }

            int userStockAmountForCode = _db.UserStocks
                .AsNoTracking()
                .Where(
                    x => x.UserID.Equals(userId)
                    && x.Stock.Code.ToLower().Equals(stockCode.ToLower()))
                .Select(x => x.Amount)
                .FirstOrDefault();

            return userStockAmountForCode;
        }

        public StockBuyStatus BuyStock(string userId, BuyStockViewModel buyStockModel)
        {
            // check if user change the form data, provided stock value has to exists in database
            StockValue purchasingStockValue = GetStockForCodeAndValue(buyStockModel.Code, buyStockModel.Unit, buyStockModel.Price);
            if (purchasingStockValue == null)
            {
                return StockBuyStatus.Unknown;
            }
            decimal totalCost = buyStockModel.AmountToBuy / purchasingStockValue.Unit * purchasingStockValue.Price;

            // case 1: User does not have enough money
            User buyer = _db.UserApp.Where(x => x.Id.Equals(userId) && x.Money >= totalCost)
                .FirstOrDefault();
            if (buyer == null)
            {
                return StockBuyStatus.UserHasNotEnoughMoney;
            }
            // case 2: Stock Exchange does not have enough stock
            if (purchasingStockValue.Stock.AvailableAmount < buyStockModel.AmountToBuy)
            {
                return StockBuyStatus.AmountNotAvailable;
            }

            // case 3: Stock value data have changed - there was an update
            StockDetails latestStockDetails = GetLatestStockDetails(buyStockModel.Code);
            bool isBuyerBuyingStockByLatestPrice = AreDatesTheSameComparisonLikeHuman(
                latestStockDetails.PublicationDate, purchasingStockValue.PublicationDate);
            if (isBuyerBuyingStockByLatestPrice == false)
            {
                return StockBuyStatus.StockValueDataHaveChanged;
            }

            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // then 1: Subtract money from User wallet
                    buyer.Money -= totalCost;

                    // then 2: Add specified amount of stock to UserStocks
                    UserStocks stockOwnedByBuyer = buyer.Stocks.FirstOrDefault(
                        x => x.StockID == purchasingStockValue.StockID);
                    if (stockOwnedByBuyer != null)
                    {
                        stockOwnedByBuyer.Amount += buyStockModel.AmountToBuy;
                        _db.Entry(stockOwnedByBuyer).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        stockOwnedByBuyer = new UserStocks
                        {
                            UserID = userId,
                            StockID = purchasingStockValue.StockID,
                            Amount = buyStockModel.AmountToBuy
                        };
                        _db.UserStocks.Add(stockOwnedByBuyer);
                    }

                    // then 3: Decrease available stock amount
                    _db.Stock.Attach(purchasingStockValue.Stock);
                    _db.Entry(purchasingStockValue.Stock).Reload();
                    if (purchasingStockValue.Stock.AvailableAmount < buyStockModel.AmountToBuy)
                    {
                        _logger.Info("Rollback");
                        dbContextTransaction.Rollback();
                        return StockBuyStatus.AmountNotAvailable;
                    }
                    purchasingStockValue.Stock.AvailableAmount -= buyStockModel.AmountToBuy;
                    _db.Entry(purchasingStockValue.Stock).State = System.Data.Entity.EntityState.Modified;

                    // save and commit
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                    return StockBuyStatus.Success; // everything went well, so finally success
                }
                catch (Exception e)
                {
                    _logger.Error("Transaction error during buying stock. Error:" + e.Message);
                    _logger.Error("Rollback...");
                    dbContextTransaction.Rollback();
                }
            }

            return StockBuyStatus.Unknown;
        }

        public StockSellStatus SellStock(string userId, SellStockViewModel sellStockModel)
        {
            // check if user change the form data, provided stock value has to exists in database
            StockValue sellingStockValue = GetStockForCodeAndValue(
                sellStockModel.Code, sellStockModel.Unit, sellStockModel.Price);
            if (sellingStockValue == null)
            {
                return StockSellStatus.Unknown;
            }
            decimal totalCost = sellStockModel.AmountToSell / sellingStockValue.Unit * sellingStockValue.Price;

            // case 1: User does not have enough stock
            User seller = _db.UserApp.Where(x => x.Id.Equals(userId)).FirstOrDefault();
            UserStocks stockOwnedBySeller = seller.Stocks
                .Where(x => x.StockID == sellingStockValue.StockID)
                .FirstOrDefault();
            if (stockOwnedBySeller == null || stockOwnedBySeller.Amount < sellStockModel.AmountToSell)
            {
                return StockSellStatus.UserHasNotEnoughStocks;
            }

            // case 2: Stock value data have changed - there was an update
            StockDetails latestStockDetails = GetLatestStockDetails(sellStockModel.Code);
            bool isSellerSellingStockByLatestPrice = AreDatesTheSameComparisonLikeHuman(
                latestStockDetails.PublicationDate, sellingStockValue.PublicationDate);
            if (isSellerSellingStockByLatestPrice == false)
            {
                return StockSellStatus.StockValueDataHaveChanged;
            }

            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // then 1: Add money to User wallet
                    seller.Money += totalCost;

                    // then 2: Subtract specified amount of stock from UserStocks
                    stockOwnedBySeller.Amount -= sellStockModel.AmountToSell;
                    if (stockOwnedBySeller.Amount == 0)
                    {
                        _db.UserStocks.Remove(stockOwnedBySeller);
                    }

                    //then 3: Increase available stock amount
                    _db.Stock.Attach(sellingStockValue.Stock);
                    _db.Entry(sellingStockValue.Stock).Reload();
                    sellingStockValue.Stock.AvailableAmount += sellStockModel.AmountToSell;
                    _db.Entry(sellingStockValue.Stock).State = System.Data.Entity.EntityState.Modified;

                    // save and commit
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                    return StockSellStatus.Success; // everything went well, so finally success
                }
                catch (Exception e)
                {
                    _logger.Error("Transaction error during selling stock. Error " + e.Message);
                    _logger.Error("Rollback...");
                    dbContextTransaction.Rollback();
                }
            }

            return StockSellStatus.Unknown;
        }
        #endregion

        #region Private Methods
        private StockValue GetStockForCodeAndValue(string stockCode, int unit, decimal price)
        {
            StockValue stockValue = _db.StockValue
                .AsNoTracking()
                .Where(
                    x => x.Unit == unit
                    && x.Price == price
                    && x.Stock.Code.ToLower().Equals(stockCode.ToLower()))
                .OrderByDescending(x => x.PublicationDate)
                .FirstOrDefault();

            return stockValue;
        }

        private int GetStockId(string stockCode, string stockName)
        {
            try
            {
                int stockId = _db.Stock
                    .AsNoTracking()
                    .Where(x =>
                        x.Name.ToLower().Equals(stockName.ToLower())
                        && x.Code.ToLower().Equals(stockCode.ToLower()))
                    .Select(x => x.ID)
                    .FirstOrDefault();

                return stockId;
            }
            catch (Exception e)
            {
                _logger.Error("Exception during fetching stock id: " + e.Message);
                _logger.Error(e.InnerException.Message);
                return 0;
            }
        }

        /// <summary>
        /// Compares whether dates are equal. Compares by Year,Month,Day,Hour,Minute,Second.
        /// </summary>
        /// <param name="first">First date</param>
        /// <param name="second">Second date</param>
        /// <returns>Returns true if dates are equal, otherwise returns false.</returns>
        private bool AreDatesTheSameComparisonLikeHuman(DateTime first, DateTime second)
        {
            return first.Year == second.Year && first.Month == second.Month && first.Day == second.Day
                && first.Hour == second.Hour && first.Minute == second.Minute && first.Second == second.Second;
        }
        #endregion
    }
}