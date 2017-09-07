using NLog;
using StockMarket.Core.Models.ViewModels;
using StockMarket.Infrastructure.Context;
using System;
using System.Linq;

namespace StockMarket.Infrastructure.Services
{
    public class UserService : IUserService
    {
        #region Fields/Properties
        private readonly IStockMarketDbContext _db;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public UserService(IStockMarketDbContext db)
        {
            _db = db;
        }
        #endregion

        #region Public
        public UserWalletViewModel GetUserWalletViewModel(string userId)
        {
            UserWalletViewModel userWallet = new UserWalletViewModel();

            userWallet.Money = _db.UserApp
                .AsNoTracking()
                .Where(x => x.Id.Equals(userId))
                .Select(x => x.Money)
                .FirstOrDefault();

            var userStocksWallet = _db.UserStocks
                .AsNoTracking()
                .Where(x => x.UserID.Equals(userId))
                .Select(x => new UserWalletStocks
                {
                    Name = x.Stock.Name,
                    Code = x.Stock.Code,
                    Amount = x.Amount,
                    Unit = x.Stock.Values.OrderByDescending(y => y.PublicationDate).Take(1).Select(s => s.Unit).FirstOrDefault(),
                    Price = x.Stock.Values.OrderByDescending(y => y.PublicationDate).Take(1).Select(s => s.Price).FirstOrDefault()
                });

            userWallet.userStocks = userStocksWallet.ToList();

            return userWallet;
        }
        #endregion
    }
}