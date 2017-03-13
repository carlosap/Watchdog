using System;
using System.Threading;

namespace Watchdog
{
    partial class Program
    {
        public static void ObserverA(object data)
        {
            Console.WriteLine("Observer A called at: {0}. Thread ID: {1}.", DateTime.Now,
                Thread.CurrentThread.ManagedThreadId);
        }
    }
}