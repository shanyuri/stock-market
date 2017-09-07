using StockMarket.Core.Models.ViewModels;
using StockMarket.Infrastructure.Context;
using StockMarket.Infrastructure.Services;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace StockMarket.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        private readonly IStockMarketDbContext _db;
        private readonly IUserService _userService;
        #endregion

        #region Constructors
        private HomeController()
        {
        }

        public HomeController(IStockMarketDbContext dbContext, IUserService userService)
        {
            _db = dbContext;
            _userService = userService;
        }
        #endregion

        #region Action Methods
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Dashboard()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.MessageType = TempData["MessageType"];

            UserWalletViewModel userWalletModel = null;
            if (User.Identity.IsAuthenticated)
            {
                userWalletModel = _userService.GetUserWalletViewModel(User.Identity.GetUserId());
            }

            return View(userWalletModel);
        }

        public ActionResult Closed()
        {
            return View();
        }
        #endregion
    }
}