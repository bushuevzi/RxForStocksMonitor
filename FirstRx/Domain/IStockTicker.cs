using System;

namespace FirstRx
{
    /// <summary>
    /// Изменеине котировки
    /// </summary>
    public interface IStockTicker
    {
        event EventHandler<StockTick> StockTick;
    }
}