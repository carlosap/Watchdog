using System;
using System.Threading;

namespace Watchdog
{
    partial class Program
    {
        public static void ObserverB(object data)
        {
            Thread.Sleep(5000);
            Console.WriteLine("Observer B called at: {0} Thread ID: {1}.\n", DateTime.Now,
                Thread.CurrentThread.ManagedThreadId);
        }
    }
}