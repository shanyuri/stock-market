namespace StockMarket.Jobs
{
    public interface IStockUpdateTimer
    {
        void StartInterval(int milliseconds);
        void StartOnce();
    }
}
