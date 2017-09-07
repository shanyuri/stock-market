using AutoMapper;
using Microsoft.AspNet.Identity;
using StockMarket.Core.Models.ViewModels;
using StockMarket.Filters;
using StockMarket.Infrastructure.Context;
using StockMarket.Infrastructure.Services;
using System.Linq;
using System.Web.Mvc;

namespace StockMarket.Controllers
{
    public class StockController : Controller
    {
        #region Fields
        private readonly IStockMarketDbContext _db;
        private readonly IStockService _stockService;
        #endregion

        #region Constructor
        public StockController(IStockMarketDbContext dbContext, IStockService stockService)
        {
            _db = dbContext;
            _stockService = stockService;
        }
        #endregion

        #region BuyStock
        [HttpGet]
        [ActionName("Buy")]
        [Authorize(Roles = "User")]
        [MarketOpenFilter]
        public ActionResult BuyStock(string stockCode)
        {
            var latestStockDetails = _stockService.GetLatestStockDetails(stockCode);
            if (latestStockDetails == null)
            {
                TempData["Message"] = "You cannot buy selected stock.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Dashboard", controllerName: "Home");
            }

            BuyStockViewModel buyStockModel = Mapper.Map<BuyStockViewModel>(latestStockDetails);
            ViewBag.BuyStockStatus = TempData["BuyStockStatus"];

            return View(buyStockModel);
        }

        [HttpPost]
        [ActionName("Buy")]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [MarketOpenFilter]
        public ActionResult BuyStock(BuyStockViewModel buyStockModel)
        {
            return (ModelState.IsValid) ? 
                View("BuyConfirm", buyStockModel) : View("Buy", buyStockModel);
        }

        [HttpPost]
        [ActionName("BuyConfirm")]
        [Authorize(Roles = "User")]
        [MarketOpenFilter]
        public ActionResult BuyStockConfirmed(BuyStockViewModel buyStockModel)
        {
            if (ModelState.IsValid == false)
            {
                return View("Buy", buyStockModel);
            }

            var userId = User.Identity.GetUserId();
            var buyStockStatus = _stockService.BuyStock(userId, buyStockModel);

            if (buyStockStatus == Infrastructure.Enum.StockBuyStatus.Success)
            {
                TempData["Message"] = "You have successfully bought stocks!";
                TempData["MessageType"] = "success";
                return RedirectToAction("dashboard", controllerName: "home");
            }
            else
            {
                TempData["BuyStockStatus"] = buyStockStatus;
                return RedirectToAction("Buy", new { stockCode = buyStockModel.Code.ToLower() });
            }
        }
        #endregion

        #region SellStock
        [HttpGet]
        [ActionName("Sell")]
        [Authorize(Roles = "User")]
        [MarketOpenFilter]
        public ActionResult SellStock(string stockCode)
        {
            var latestStockDetails = _stockService.GetLatestStockDetails(stockCode);
            if (latestStockDetails == null)
            {
                TempData["Message"] = "You cannot sell selected stock.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Dashboard", controllerName: "Home");
            }

            SellStockViewModel sellStockModel = Mapper.Map<SellStockViewModel>(latestStockDetails);
            sellStockModel.AmountAvailable = _stockService.GetUserStockAmountForCode(User.Identity.GetUserId(), stockCode);
            if (sellStockModel.AmountAvailable == 0)
            {
                return View("SellIsNotPossible");
            }

            ViewBag.SellStockStatus = TempData["SellStockStatus"];
            var previousAmountToSell = TempData["SellStockModel.AmountToSell"];

            if (previousAmountToSell != null && previousAmountToSell.ToString() != "")
            {
                int.TryParse(previousAmountToSell.ToString(), out int amountToSell);
                sellStockModel.AmountToSell = amountToSell;
                var validateResult = TryValidateModel(sellStockModel);
            }

            return View(sellStockModel);
        }

        [HttpPost]
        [ActionName("Sell")]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [MarketOpenFilter]
        public ActionResult SellStock(SellStockViewModel sellStockModel)
        {
            return (ModelState.IsValid) ?
                View("SellConfirm", sellStockModel) : View("Sell", sellStockModel);
        }

        [HttpPost]
        [ActionName("SellConfirm")]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [MarketOpenFilter]
        public ActionResult SellStockConfirmed(SellStockViewModel sellStockModel)
        {
            if (ModelState.IsValid == false)
            {
                TempData["SellStockModel.AmountToSell"] = sellStockModel.AmountToSell;
                return RedirectToAction("sell", new { stockCode = sellStockModel.Code.ToLower() });
            }

            var userId = User.Identity.GetUserId();
            var sellStockStatus = _stockService.SellStock(userId, sellStockModel);

            if (sellStockStatus == Infrastructure.Enum.StockSellStatus.Success)
            {
                TempData["Message"] = "You have successfully sold stocks!";
                TempData["MessageType"] = "success";
                return RedirectToAction("dashboard", controllerName: "home");
            }
            else
            {
                TempData["SellStockStatus"] = sellStockStatus;
                return RedirectToAction("Sell", new { stockCode = sellStockModel.Code.ToLower() });
            }
        }
        #endregion

        #region LatestStockPricesInJson
        [Route("{controlller}/latestStockPrices/{numberOfLatestStockPrices=1:int}")]
        [OutputCache(Duration = 10, VaryByParam = "numberOfLatestStockPrices")]
        public JsonResult GetLatestNStockPrices(int? numberOfLatestStockPrices)
        {
            if (numberOfLatestStockPrices == null || numberOfLatestStockPrices <= 0)
            {
                return null;
            }

            var latestNStockPricesQuery = _db.StockValue
                .AsNoTracking()
                .GroupBy(x => x.StockID)
                .Select(x => x.OrderByDescending(g => g.PublicationDate)
                    .Take((int)numberOfLatestStockPrices)
                    .Select(r => new
                    {
                        ID = r.StockID,
                        Name = r.Stock.Name,
                        Code = r.Stock.Code,
                        Unit = r.Unit,
                        Price = r.Price,
                        PublicationDate = r.PublicationDate
                    })
                );

            var latestNStockPrices = latestNStockPricesQuery.ToList();

            return Json(latestNStockPrices, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}