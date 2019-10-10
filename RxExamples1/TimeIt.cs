using System;
using System.Diagnostics;

namespace RxExamples1
{
    public class TimeIt : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch _watch;

        public TimeIt(string name)
        {
            _name = name;
            _watch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _watch.Stop();
            Console.WriteLine($"{_name} took {_watch.Elapsed}");
        }
    }
}