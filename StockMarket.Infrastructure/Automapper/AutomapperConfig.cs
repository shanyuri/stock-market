using AutoMapper;
using StockMarket.Core.Models;
using StockMarket.Core.Models.ViewModels;

namespace StockMarket.Infrastructure.Automapper
{
    public static class AutomapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<StockDetails, BuyStockViewModel>();
                config.CreateMap<StockDetails, SellStockViewModel>();
                config.CreateMap<StockValue, StockDetails>()
                    .ForMember(x => x.Code, opt => opt.MapFrom(x => x.Stock.Code))
                    .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Stock.Name))
                    .ForMember(x => x.AvailableAmount, opt => opt.MapFrom(x => x.Stock.AvailableAmount));
            });
        }
    }
}