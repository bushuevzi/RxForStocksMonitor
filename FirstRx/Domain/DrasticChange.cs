namespace FirstRx
{
    /// <summary>
    /// Критические изменения которовок
    /// </summary>
    public class DrasticChange
    {
        /// <summary>
        /// Новая цена
        /// </summary>
        public decimal NewPrice { get; set; }

        /// <summary>
        /// Название компании
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Изменение котировки
        /// </summary>
        public decimal ChangeRatio { get; set; }

        /// <summary>
        /// Старая цена
        /// </summary>
        public decimal OldPrice { get; set; }
    }
}