namespace StockMarket.Infrastructure.Enum
{
    public enum StockSellStatus
    {
        MarketIsClosed,
        AmountIsNotAMultiplicationOfStockUnit,
        UserHasNotEnoughStocks,
        StockValueDataHaveChanged,
        Success,
        Unknown
    }
}