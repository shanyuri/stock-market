using Microsoft.Practices.Unity;
using StockMarket.App_Start;
using StockMarket.Infrastructure.Automapper;
using StockMarket.Infrastructure.Services;
using StockMarket.Jobs;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace StockMarket
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutomapperConfig.RegisterMappings();

            try
            {
                ReadAppSettings(out string urlToExternalServer, out int updateStockDataInterval);

                var container = UnityConfig.GetConfiguredContainer();
                container.RegisterType<IStockWebReader, StockWebReader>(new InjectionConstructor(urlToExternalServer));

                var stockUpdateTimer = container.Resolve<IStockUpdateTimer>();
                stockUpdateTimer.StartInterval(milliseconds: updateStockDataInterval);
                //stockUpdateTimer.StartOnce();
            }
            catch { throw; }
        }

        private static void ReadAppSettings(out string url, out int interval)
        {
            const string urlToServerAppSettingKey = "UrlToServerReturningStocksJson";
            const string intervalAppSettingKey = "UpdateStockIntervalMilliseconds";

            string urlToServerValue = ConfigurationManager.AppSettings[urlToServerAppSettingKey];
            string intervalValue = ConfigurationManager.AppSettings[intervalAppSettingKey];

            url = urlToServerValue;
            bool parseIntervalValueResult = Int32.TryParse(intervalValue, out interval);

            if (string.IsNullOrEmpty(intervalValue))
            {
                throw new Exception($"Key {intervalAppSettingKey} is not provided in Web.config appSettings section.");
            }
            if (string.IsNullOrEmpty(urlToServerValue))
            {
                throw new Exception($"Key {urlToServerAppSettingKey} is not provided in Web.config appSettings section.");
            }
            if (parseIntervalValueResult == false || interval <= 0)
            {
                throw new Exception($"Key {intervalAppSettingKey} is not valid in Web.config appSettings section.");
            }
        }
    }
}
