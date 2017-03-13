using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Hosting;

namespace Watchdog
{

    public class Scheduler : IDisposable, IRegisteredObject
    {
        private struct WorkThreadConfiguration
        {
            public ManualResetEvent ShutdownEvent;
            public IWork Work;
        }
        private readonly Scheduler.WorkThreadConfiguration[] _configurations;
        private bool _isSchedulerActive;
        private bool _hostingEnvironmentConfigured;
        private readonly object _syncObj = new object();
        public Action<Exception> ExceptionHandler {get; set;}
        public ExceptionHandlingAction ExceptionHandling { get; set; }
        public Scheduler(IWork work) : this(new IWork[] { work }){}
        public Scheduler(IWork[] work)
        {
            if (work == null)
            {
                throw new ArgumentNullException();
            }
            if ((int)work.Length == 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.SetupHostingEnvironment();
            this._configurations = new WorkThreadConfiguration[(int)work.Length];
            for (int i = 0; i < (int)this._configurations.Length; i++)
            {
                WorkThreadConfiguration workThreadConfiguration = new WorkThreadConfiguration()
                {
                    ShutdownEvent = new ManualResetEvent(false),
                    Work = work[i]
                };
                this._configurations[i] = workThreadConfiguration;
            }
        }

        public void Start()
        {
            lock (this._syncObj)
            {
                if (!this._isSchedulerActive)
                {
                    for (int i = 0; i < (int)this._configurations.Length; i++)
                    {
                        if (!this._configurations[i].Work.UseThreadPool)
                        {
                            Thread worker = new Thread(new ParameterizedThreadStart(this.DoContinuousWork));
                            worker.SetApartmentState(this._configurations[i].Work.ThreadApartmentState);
                            worker.Start(this._configurations[i]);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(this.DoContinuousWork), this._configurations[i]);
                        }
                    }
                    this._isSchedulerActive = true;
                }
            }
        }
        public void Stop(bool immediate = true)
        {
            lock (this._syncObj)
            {
                if (this._isSchedulerActive)
                {
                    for (int i = 0; i < (int)this._configurations.Length; i++)
                    {
                        this._configurations[i].Work.Stop(true);
                        this._configurations[i].ShutdownEvent.Set();
                    }
                    if (this._hostingEnvironmentConfigured)
                    {
                        this.CleanupHostingEnvironment();
                    }
                    this._isSchedulerActive = false;
                }
            }
        }
        private void DoWork(IWork work)
        {
            try
            {
                this.NotifyObservers(work, work.GetResult());
            }
            catch (Exception exception)
            {
                Exception e = exception;
                switch (this.ExceptionHandling)
                {
                    case ExceptionHandlingAction.DefaultBehaviour:
                        {
                            throw;
                        }
                    case ExceptionHandlingAction.CallEventHandler:
                        {
                            this.OnException(e);
                            break;
                        }
                    case ExceptionHandlingAction.CallEventHandlerAndLeakException:
                        {
                            this.OnException(e);
                            throw;
                        }
                }
            }
        }

        private void OnException(Exception e)
        {
            ExceptionHandler?.Invoke(e);
        }
        private void NotifyObservers(IWork work, object data)
        {
            if (work.Observers != null && work.Observers.Count > 0)
            {
                switch (work.ObserverExecutionMethod)
                {
                    case ExecutionMethod.Sequential:
                        {
                            List<Action<object>>.Enumerator enumerator = work.Observers.GetEnumerator();
                            try
                            {
                                while (enumerator.MoveNext())
                                {
                                    Action<object> observer = enumerator.Current;
                                    if (observer == null)
                                    {
                                        continue;
                                    }
                                    observer(data);
                                }
                                break;
                            }
                            finally
                            {
                                ((IDisposable)enumerator).Dispose();
                            }
                            break;
                        }
                    case ExecutionMethod.Parallel:
                        {
                            List<Action<object>>.Enumerator enumerator1 = work.Observers.GetEnumerator();
                            try
                            {
                                while (enumerator1.MoveNext())
                                {
                                    Action<object> current = enumerator1.Current;
                                    if (current == null)
                                    {
                                        continue;
                                    }
                                    if (!work.UseThreadPool)
                                    {
                                        (new Thread(() => current(data))).Start();
                                    }
                                    else
                                    {
                                        ThreadPool.QueueUserWorkItem((object state) => current(data));
                                    }
                                }
                                break;
                            }
                            finally
                            {
                                ((IDisposable)enumerator1).Dispose();
                            }
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }
            }
        }

        private void CleanupHostingEnvironment()
        {
            if (_hostingEnvironmentConfigured)
            {
                HostingEnvironment.UnregisterObject(this);
            }
        }

        private void DoContinuousWork(object data)
        {
            this.DoContinuousWork((WorkThreadConfiguration)data);
        }

        private void DoContinuousWork(WorkThreadConfiguration configuration)
        {
            bool firstPass = true;
            TimeSpan interval = configuration.Work.Interval;
            if (configuration.Work.StartImmediately)
            {
                configuration.Work.Interval = TimeSpan.FromSeconds(0);
            }
            else if (configuration.Work.StartDelay > TimeSpan.Zero)
            {
                configuration.Work.Interval = configuration.Work.StartDelay;
            }
            uint workRepeatCounter = 0;
            while (!configuration.ShutdownEvent.WaitOne(configuration.Work.Interval, true))
            {
                this.DoWork(configuration.Work);
                if (configuration.Work.StartImmediately && firstPass || configuration.Work.StartDelay > TimeSpan.Zero && firstPass)
                {
                    configuration.Work.Interval = interval;
                    firstPass = false;
                }
                if (configuration.Work.RepeatTimes <= 0)
                {
                    continue;
                }
                workRepeatCounter++;
                if (workRepeatCounter != configuration.Work.RepeatTimes)
                {
                    continue;
                }
                return;
            }
        }

        private void SetupHostingEnvironment()
        {
            try
            {
                HostingEnvironment.RegisterObject(this);
                _hostingEnvironmentConfigured = true;
            }
            catch (Exception exception)
            {
                _hostingEnvironmentConfigured = false;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop(true);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
