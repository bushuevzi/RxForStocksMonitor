using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxExampleFromHabr
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Основные отличия Rx от Event:
             * 1) у Rx есть Linq -- можно пускать на обработчик тольк определенные события
             * 2) у Rx есть метод OnCompleted с помощью него ИЗДАТЕЛЬ отключает от себя подписчиков
             * 3) в Rx Издатель может отключить от себя всех подписчиков методом Dispose
             */

            #region "Приложение", которое меняет статусы заказа
            // Создаем субъект рекактивного расширения (Reactive Extensions Subject)
            // Он по сути является прокси между Издателем и Подписчиком соответственно может как подписывать обработчики
            // так и принимать сообщения для отправки обработчикам
            ISubject<StatusChange> statChange = new Subject<StatusChange>();

            #region Простая подписка на события
            // Подписываем обработчик, который просто выводит статус
            statChange.Subscribe(StatusChanged, OnError, OnCompleted);
            // Аналогично но в апперкейсе
            statChange.Subscribe(StatusUpperCase);
            #endregion

            #region Подписка на события с учетом фильтрации событий
            // Подписываем обработчк только на на события в которых статус == "Processing"
            // Способ 1:
            statChange.Where(cs => cs.OrderStatus.ToLower() == "processing").Subscribe(AlarmHandler);
            // Способ 2:
            var scs = from sc in statChange
                      where sc.OrderStatus.ToLower() == "processing"
                      select sc;
            scs.Subscribe(AlarmHandler);
            // Способ 3:
            var sub = (from sc in statChange
                       where sc.OrderStatus.ToLower() == "processing"
                       select sc).Subscribe(AlarmHandler); 
            #endregion

            // Пуляем в Observable сообщения (события)
            // Получаем из консоли новые статусы и отправляем их обработчикам
            for (int i = 0; i < 3; i++)
            {
                statChange.OnNext(new StatusChange { OrderId = 1, OrderStatus = Console.ReadLine() });
            } 
            // Когда поток заказнчивается дергается метод OnCompleted (Может быть вызван соответствующий )
            #endregion
        }

        #region "Приложение", которое обрабатывает измение статусов
        // Обработчик событий, который просто выводит статус
        public static void StatusChanged(StatusChange status) => Console.WriteLine(status.OrderStatus);
        // Аналогично но в апперкейсе
        public static void StatusUpperCase(StatusChange status) => Console.WriteLine(status.OrderStatus.ToUpper()); 

        // Прям аларм
        public static void AlarmHandler(StatusChange status) =>
            Console.WriteLine($"------!!!!! Заказ №{status.OrderId} в стадии {status.OrderStatus} !!!!!------");


        #region Стандартные обработчики окончания потока и ошибки в потоке
        public static void OnError(Exception ex) => Console.Error.WriteLine(ex.Message);
        public static void OnCompleted() => Console.WriteLine("Order processing completed"); 
        #endregion

        #endregion
    }
}
