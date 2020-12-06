using System;
using System.Timers;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace KEI.Infrastructure.PeriodicTasks
{
    public abstract class PeriodicTask : BindableBase, IPeriodicTask, IDisposable
    {
        private const int MINUTE_TO_MS_FACTOR = 60000;
        private readonly object executeLock = new object();
        private Timer _timer;

        #region IPeriodicTask Members

        private Period period;
        public Period Period
        {
            get { return period; }
            set { SetProperty(ref period, value); }
        }

        private DateTime lastExecutedOn;
        public DateTime LastExecutedOn
        {
            get { return lastExecutedOn; }
            set { SetProperty(ref lastExecutedOn, value); }
        }

        private string name = "No Name";
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public virtual bool CanExecute() => true;
        
        protected abstract void InternalExecute();
        
        protected abstract void InitializeParameters();
        
        public void Pause()
        {
            _timer.Enabled = false;
        }

        public void Resume()
        {
            _timer.Enabled = true;
        }

        #endregion

        private void Execute()
        {
            lock (executeLock)
            {
                if (CanExecute() == false)
                {
                    return;
                }

                InternalExecute();

                LastExecutedOn = DateTime.Now;

                TaskExecuted?.Invoke(this, LastExecutedOn);
            }
        }

        public async Task Start()
        {
            _timer = new Timer((int)Period * MINUTE_TO_MS_FACTOR);
            _timer.Elapsed += Timer_Elapsed;

            InitializeParameters();

            var timeSinceLastExecution = DateTime.Now - LastExecutedOn;
            bool needExecute = false;

            needExecute = Period switch
            {
                Period.OneDay => timeSinceLastExecution.TotalDays > 0,
                Period.OneWeek => timeSinceLastExecution.TotalDays > 7,
                Period.OneMonth => timeSinceLastExecution.TotalDays > 30,
                _ => timeSinceLastExecution.TotalMinutes > (int)Period,
            };

            if (needExecute)
            {
                await Task.Run(() => Execute());
            }

            _timer.Start();
        }


        public EventHandler<DateTime> TaskExecuted;

        public void Dispose()
        {
            _timer.Elapsed -= Timer_Elapsed;
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e) => await Task.Run(() => Execute());

    }
}
