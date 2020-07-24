using System;
using System.Timers;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace KEI.Infrastructure.PeriodicTasks
{
    public abstract class PeriodicTask : BindableBase, IPeriodicTask, IDisposable
    {
        private const int MINUTE_TO_MS_FACTOR = 60000;
        private object executeLock = new object();
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
                if (!CanExecute())
                    return;

                InternalExecute();

                LastExecutedOn = DateTime.Now;

                TaskExecuted?.Invoke(this, LastExecutedOn);
            }
        }

        public async Task Start()
        {
            _timer = new Timer((int)Period * MINUTE_TO_MS_FACTOR);
            _timer.Elapsed += timer_Elapsed;

            InitializeParameters();

            var timeSinceLastExecution = DateTime.Now - LastExecutedOn;
            bool needExecute = false;

            switch (Period)
            {
                case Period.OneDay:
                    needExecute = timeSinceLastExecution.TotalDays > 0;
                    break;
                case Period.OneWeek:
                    needExecute = timeSinceLastExecution.TotalDays > 7;
                    break;
                case Period.OneMonth:
                    needExecute = timeSinceLastExecution.TotalDays > 30;
                    break;
                default:
                    needExecute = timeSinceLastExecution.TotalMinutes > (int)Period;
                    break;
            }

            if (needExecute)
                await Task.Run(() => Execute());

            _timer.Start();
        }


        public EventHandler<DateTime> TaskExecuted;

        public void Dispose()
        {
            _timer.Elapsed -= timer_Elapsed;
        }

        private async void timer_Elapsed(object sender, ElapsedEventArgs e) => await Task.Run(() => Execute());

    }
}
