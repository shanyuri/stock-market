using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockMarket.ExampleAPI.Models;
using System;
using System.Collections.Generic;

namespace StockMarket.ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    public class StocksController : Controller
    {
        private static Stocks _stocks { get; set; }
        private static int _generateNewStockDataAfterSeconds { get; set; }
        private static DateTime _lastUpdateDateTime = DateTime.Now;
        private static readonly object _updateLock = new object();

        public StocksController(IOptions<AppSettings> appSettings)
        {
            _generateNewStockDataAfterSeconds = appSettings.Value.GenerateNewStockDataAfterSeconds <= 0 ?
                10 : appSettings.Value.GenerateNewStockDataAfterSeconds;

            if (_stocks == null)
            {
                Init(DateTime.Now);
            }
        }

        [HttpGet]
        public JsonResult Get()
        {
            var currentDateTime = DateTime.Now;
            if (currentDateTime.Subtract(_lastUpdateDateTime).TotalSeconds > _generateNewStockDataAfterSeconds)
            {
                Init(currentDateTime);
            }

            return Json(_stocks);
        }

        private void Init(DateTime updateDateTime)
        {
            var random = new Random();

            lock (_updateLock)
            {
                _stocks = new Stocks
                {
                    PublicationDate = updateDateTime,
                    Items = new List<Stock>
                    {
                        new Stock { Code = "STARK", Name = "House Stark", Unit = 1, Price = (decimal)(6 + random.NextDouble()) },
                        new Stock { Code = "TARGARYEN", Name = "House Targaryen", Unit = 1, Price = (decimal)(9 + random.NextDouble()) },
                        new Stock { Code = "LANNISTER", Name = "House Lannister", Unit = 1, Price = (decimal)(10 + random.NextDouble()) },
                        new Stock { Code = "GREYJOY", Name = "House Greyjoy", Unit = 5, Price = (decimal)(6 + random.NextDouble()) },
                        new Stock { Code = "NKING", Name = "Night King", Unit = 10, Price = (decimal)(7 + random.NextDouble()) },
                        new Stock { Code = "NWATCH", Name = "Nights Watch", Unit = 100, Price = (decimal)(5 + random.NextDouble()) }
                        //new Stock { Code = "TestCode", Name = "TestName", Unit = 100, Price = (decimal)(5 + random.NextDouble()) }
                    }
                };

                _lastUpdateDateTime = updateDateTime;
            }
        }
    }
}
