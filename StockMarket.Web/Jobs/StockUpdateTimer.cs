using StockMarket.Hubs;
using StockMarket.Infrastructure.Enum;
using StockMarket.Infrastructure.Services;
using NLog;
using System;
using System.Timers;

namespace StockMarket.Jobs
{
    public class StockUpdateTimer : IStockUpdateTimer
    {
        #region Fields
        private Timer _timer;
        private IStockService _stockService;
        private IStockWebReader _stockWebReader;
        private StockTicker _stockTicker;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        private StockUpdateTimer()
        {
        }

        public StockUpdateTimer(IStockService stockService, IStockWebReader stockWebReader)
        {
            _stockTicker = StockTicker.Instance;
            _stockService = stockService;
            _stockWebReader = stockWebReader;
        }
        #endregion

        #region Public
        public void StartInterval(int milliseconds)
        {
            if (milliseconds < 5000)
            {
                var errorMessage = "Argument \"milliseconds\" cannot be lower than 5000.";
                _logger.Error(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            _timer = new Timer { Interval = milliseconds };
            _timer.Elapsed += ActionAsync;
            _timer.Start();
        }

        public void StartOnce()
        {
            ActionAsync(null, null);
        }
        #endregion

        #region Private
        private async void ActionAsync(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Timer_Elapsed: " + DateTime.Now);

            var stocks = await _stockWebReader.GetCurrentStocksAsync();
            if (stocks == null)
            {
                _logger.Error("MarketState=Closed");
                _stockTicker.MarketState = MarketState.Closed;
            }
            else
            {
                _logger.Info("MarketState=Opened");
                _stockTicker.MarketState = MarketState.Opened;

                bool updated = _stockService.AddStockValuesForExistingStocksIfNewer(stocks);
                if (updated)
                {
                    _stockTicker.BroadcastStocks();
                }
            }
        }
        #endregion
    }
}