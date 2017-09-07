using NLog;
using StockMarket.Core.Models;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace StockMarket.Infrastructure.Services
{
    public class StockWebReader : IStockWebReader
    {
        #region Fields/Properties
        private readonly string _url;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        private StockWebReader()
        {
        }

        public StockWebReader(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                var errorMessage = "Url cannot be null or empty.";
                _logger.Error(errorMessage);
                throw new ArgumentNullException(errorMessage);
            }

            _url = url;
        }
        #endregion

        #region Public Methods
        public async Task<StocksJSON> GetCurrentStocksAsync()
        {
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/json";

            HttpWebResponse response = await GetResponseAsync(request);
            StocksJSON stocks = null;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                stocks = await ProcessResponseAsync(response);
            }

            return stocks;
        }
        #endregion

        #region Private Methods
        private async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse) await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                _logger.Error("Error during getting response: " + e.Message);
                throw;
            }

            return response;
        }

        private static async Task<StocksJSON> ProcessResponseAsync(HttpWebResponse response)
        {
            string json = "";
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                json = await streamReader.ReadToEndAsync();
            }

            var jsSerializer = new JavaScriptSerializer();
            StocksJSON stocks = null;
            try
            {
                stocks = jsSerializer.Deserialize<StocksJSON>(json);
            }
            catch (Exception e)
            {
                _logger.Error("Error during serializing JSON data:" + e.Message);
                stocks = null;
                throw;
            }

            return stocks;
        }
        #endregion
    }
}