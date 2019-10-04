using System;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;

namespace FirstRx
{
    public class RxStockMonitor :IDisposable
    {
        private IDisposable _subscription;

        /// <summary>
        /// Отслеживание котировок
        /// </summary>
        /// <param name="ticker">Изменеине котировки</param>
        public RxStockMonitor(IStockTicker ticker)
        {
            const decimal maxChangeRatio = 0.1m;

            // ИЗДАТЕЛЬ
            // Создаем Наблюдаемую переменную котировок, из потока событий изменения котировок
            IObservable<StockTick> ticks =
                // Создаем наплюдаемый поток СОБЫТИЙ изменения котировок
                Observable.FromEventPattern<EventHandler<StockTick>, StockTick>(
                        evHandl => ticker.StockTick += evHandl,
                        evHandl => ticker.StockTick -= evHandl)
                    // Преобразовываем события изменения котировок в САМУ КОТИРОВКУ 
                    .Select(tickEvent => tickEvent.EventArgs)
                    .Synchronize();

            // Собираем только критические измениня котировок
            var drasticChanges =
                // Группировка котировок по компаниям
                from tick in ticks
                group tick by tick.QuoteSymbol
                into company
                // Выбираем последние 2 стоимости акции и загоняем их в буфер и работаем с ними
                from tickPair in company.Buffer(2, 1)
                let changeRatio = Math.Abs((tickPair[1].Price - tickPair[0].Price) / tickPair[0].Price)
                where changeRatio > maxChangeRatio
                select new DrasticChange()
                {
                    Symbol = company.Key,
                    ChangeRatio = changeRatio,
                    OldPrice = tickPair[0].Price,
                    NewPrice = tickPair[1].Price
                };

            // ПОДПИСЧИК
            // Подписываемся на поток событий
            _subscription = drasticChanges.Subscribe(
                change =>
                {
                    Console.WriteLine(
                        $"Stock:{change.Symbol} has changed with {change.ChangeRatio}, Old Price: {change.OldPrice}, New Price: {change.NewPrice}");
                },
                ex => { /* Код для обработки исключений */ },
                () => { /* Код который будет выполнен при окончании потока сообщений */ });
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}