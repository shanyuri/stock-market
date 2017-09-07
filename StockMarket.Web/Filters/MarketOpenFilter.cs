using StockMarket.Hubs;
using System.Web.Mvc;

namespace StockMarket.Filters
{
    public class MarketOpenFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (StockTicker.Instance.MarketState == Infrastructure.Enum.MarketState.Closed)
            {
                filterContext.Result = new RedirectResult("~/Home/Closed");
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}