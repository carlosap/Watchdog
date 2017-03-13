using System;
using System.Collections.Generic;
using System.Threading;
namespace Watchdog
{
    public abstract class Work : IWork
    {
        public TimeSpan Interval {get; set;}
        public ExecutionMethod ObserverExecutionMethod {get; set;}
        public List<Action<object>> Observers {get; set;}
        public int RepeatTimes {get; set;}
        public TimeSpan StartDelay { get; set; }
        public bool StartImmediately {get; set; }
        public ApartmentState ThreadApartmentState {get; set; }
        public bool UseThreadPool {get; set; }
        protected Work()
        {
            this.Observers = new List<Action<object>>();
        }
        public abstract object GetResult();
        public virtual void Stop(bool immediate){}
    }
}
