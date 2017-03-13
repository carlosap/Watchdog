using System;
using System.Collections.Generic;
using System.Threading;
namespace Watchdog
{
    public interface IWork
    {
        TimeSpan Interval {get;set;}
        TimeSpan StartDelay { get; set; }
        ExecutionMethod ObserverExecutionMethod {get;set;}
        List<Action<object>> Observers {get; set;}
        int RepeatTimes {get; set; }
        bool StartImmediately {get; set;}
        bool UseThreadPool { get; set; }
        ApartmentState ThreadApartmentState { get; set; }
        object GetResult();
        void Stop(bool immediate);

    }
}
