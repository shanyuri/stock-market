using StockMarket.Infrastructure.Context;
using StockMarket.Infrastructure.Enum;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using StockMarket.Core.Models.ViewModels;
using StockMarket.Core.Models;

namespace StockMarket.Hubs
{
    public class StockTicker
    {
        #region Members
        private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(
            () => new StockTicker(
                GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients,
                new StockMarketDbContext()
            )
        );

        private IStockMarketDbContext _dbContext { get; set; }
        private IHubConnectionContext<dynamic> Clients { get; set; }
        private ConcurrentBag<StockDetails> Stocks;

        private MarketState _marketState;
        public MarketState MarketState
        {
            get
            {
                return _marketState;
            }
            set
            {
                if (_marketState != value)
                {
                    _marketState = value;
                    BroadcastMarketStateChange(_marketState);
                }
            }
        }

        private readonly object _updateStocksLock = new object();
        private volatile bool _updatingStockPrices = false;

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        #endregion

        #region Constructor
        private StockTicker(IHubConnectionContext<dynamic> clients, IStockMarketDbContext dbContext)
        {
            Clients = clients;
            _dbContext = dbContext;
            LoadStocks();
        }
        #endregion

        #region Private Methods
        private void LoadStocks()
        {
            var latestStockValuesQuery = from x in _dbContext.StockValue.AsNoTracking()
                                         group x by x.StockID into g
                                         select g
                                            .OrderByDescending(r => r.PublicationDate)
                                            .Take(1)
                                            .FirstOrDefault();
            var list = latestStockValuesQuery.ToList();
            var stocksValues = AutoMapper.Mapper.Map<List<StockValue>, List<StockDetails>>(list);

            lock (_updateStocksLock)
            {
                Stocks = new ConcurrentBag<StockDetails>();

                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    foreach (var value in stocksValues)
                    {
                        Stocks.Add(value);
                    }

                    _updatingStockPrices = false;
                }
            }
        }
        #endregion

        #region Public Methods
        public IEnumerable<StockDetails> GetAllStocks()
        {
            LoadStocks();
            return Stocks.ToList();
        }

        public void BroadcastMarketStateChange(MarketState marketState)
        {
            switch (marketState)
            {
                case MarketState.Opened:
                    Clients.All.marketOpened();
                    break;
                case MarketState.Closed:
                    Clients.All.marketClosed();
                    break;
            }
        }

        public void BroadcastStocks()
        {
            Clients.All.updateStocks(GetAllStocks());
        }
        #endregion
    }
}