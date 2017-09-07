namespace StockMarket.Infrastructure.Enum
{
    public enum StockBuyStatus
    {
        MarketIsClosed,
        AmountIsNotAMultiplicationOfStockUnit,
        UserHasNotEnoughMoney,
        AmountNotAvailable,
        StockValueDataHaveChanged,
        Success,
        Unknown
    }
}