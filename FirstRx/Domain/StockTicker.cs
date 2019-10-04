using System;

namespace FirstRx
{
    /// <summary>
    /// Изменеине котировки
    /// </summary>
    public class StockTicker : IStockTicker
    {
        /// <summary>
        /// Событие изменение котировки
        /// </summary>
        public event EventHandler<StockTick> StockTick = delegate { };
    }
}