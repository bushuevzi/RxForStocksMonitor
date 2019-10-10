using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.VisualBasic;

namespace RxExamples1
{
    class Program
    {
        static void Main(string[] args)
        {
            ObservableRange();
        }

        public static void TryReplaySubject()
        {
            var subject = new ReplaySubject<int>();
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
            subject.OnNext(4);
            subject.Subscribe(Console.WriteLine);
        }

        public static void TryReplaySubjectWithTimeBuffer()
        {
            var window = TimeSpan.FromMilliseconds(150);
            var subject = new ReplaySubject<int>(window);
            subject.OnNext(1);
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext(2);
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext(3);
            subject.Subscribe(Console.WriteLine);
            subject.OnNext(4);
        }

        public static void TryAsyncSubject()
        {
            var subject = new AsyncSubject<int>();
            subject.OnNext(1);
            var subscriber = subject.Subscribe(Console.WriteLine);
            subject.OnNext(2);
            subject.OnNext(3);
            
            subject.OnCompleted();

            subscriber.Dispose();
        }

        public static void TrySubscribeAndUnsubscribe()
        {
            var values = new Subject<int>();
            var firstSubscription = values.Subscribe(value => Console.WriteLine($"1st subscription received {value}"));
            var secondSubscription = values.Subscribe(value => Console.WriteLine($"2st subscription received {value}"));

            values.OnNext(1);
            values.OnNext(2);
            values.OnNext(3);
            values.OnNext(300);
            firstSubscription.Dispose();
            Console.WriteLine("Disposed of 1st subscription");
            values.OnNext(5);
            values.OnNext(6);
        }

        public static void TryHandleDispose()
        {
            using (new TimeIt("Outer scope"))
            {
                using (new TimeIt("Inner scope A"))
                {
                    Task.Delay(100);
                }
                using (new TimeIt("Inner scope B"))
                {
                    Task.Delay(22);
                }
            }
        }

        public static void TryObsReturn()
        {
            var singleValue = Observable.Return<string>("Value");
            var subscriber = singleValue.Subscribe(Console.WriteLine);
        }

        public static IObservable<string> BlockingMethod()
        {
            var subject = new ReplaySubject<string>();
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            Thread.Sleep(1000);
            return subject;
        }

        public static IObservable<string> NonBlocking()
        {
            return Observable.Create<string>(
                (IObserver<string> observer) =>
                {
                    observer.OnNext("a");
                    observer.OnNext("b");
                    observer.OnCompleted();
                    Thread.Sleep(1000);
                    return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
                    //or can return an Action like 
                    //return () => Console.WriteLine("Observer has unsubscribed"); 
                });
        }

        /// <summary>
        /// Бесконечный таймер
        /// </summary>
        public static void NonBlocking_event_driven()
        {
            var observable = Observable.Create<string>(
                observer =>
                {
                    var timer = new System.Timers.Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += (sender, elapsedEventArgs) => observer.OnNext("tick");
                    timer.Elapsed += OnTimerElapsed;
                    timer.Start();
                    return () =>
                    {
                        timer.Elapsed -= OnTimerElapsed;
                        timer.Dispose();
                    };
                });
            var suscription = observable.Subscribe(Console.WriteLine);
            Console.ReadLine();
            suscription.Dispose();
        }

        public static void OnTimerElapsed(object sender, ElapsedEventArgs e) => Console.WriteLine(e.SignalTime);

        private static IEnumerable<T> Unfold<T>(T seed, Func<T, T> accumulator)
        {
            var nextValue = seed;
            while (true)
            {
                yield return nextValue;
                nextValue = accumulator(nextValue);
            }
        }

        public static void ObservableRange()
        {
            var range = Observable.Range(10, 15);
            range.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }
    }
}
