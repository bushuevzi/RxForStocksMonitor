namespace FirstRx
{
    /// <summary>
    /// Объект описывающий текущую кодировку
    /// </summary>
    public class StockTick
    {
        /// <summary>
        /// Название акции
        /// </summary>
        public string QuoteSymbol { get; set; }

        /// <summary>
        /// Стоимость котировки
        /// </summary>
        public decimal Price { get; set; }
    }
}