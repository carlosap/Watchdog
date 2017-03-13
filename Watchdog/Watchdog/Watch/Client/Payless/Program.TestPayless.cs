using System;
using System.Collections.Generic;
using Watchdog.Watch.Client.Payless;

namespace Watchdog
{
    partial class Program
    {
        private static void TestPayless()
        {
            var payless = new Test()
            {
                UseThreadPool = true,
                StartImmediately = true,
                RepeatTimes = 5, //(int)Recurrence.Infinite,
                Interval = TimeSpan.FromSeconds(2),
                ObserverExecutionMethod = ExecutionMethod.Sequential,
                Observers = new List<Action<object>>
                {
                    ObserverA,
                    ObserverB
                }
            };

            Console.WriteLine("Application started at: {0}\n", DateTime.Now);
            StartTest(payless);
        }
        private static void StartTest(Test payless)
        {
            using (var scheduler = new Scheduler(payless))
            {
                scheduler.Start();
                Console.WriteLine("Application is currently performing background tasks with Concurrently executing observers...\n");
                //scheduler.Stop();
                Console.ReadKey();
            }
        }

    }
}