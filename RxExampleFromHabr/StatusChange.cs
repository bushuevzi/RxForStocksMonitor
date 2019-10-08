namespace RxExampleFromHabr
{
    /// <summary>
    /// Статус заказа
    /// </summary>
    public class StatusChange
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
    }
}