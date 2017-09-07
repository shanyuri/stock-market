using StockMarket.Core.Models.ViewModels;

namespace StockMarket.Infrastructure.Services
{
    public interface IUserService
    {
        UserWalletViewModel GetUserWalletViewModel(string userId);
    }
}
