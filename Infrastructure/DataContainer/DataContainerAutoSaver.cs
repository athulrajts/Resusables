using System.Collections.Generic;
using System.Timers;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Class to Automatically save the <see cref="IDataContainer"/> instance whenever it's properties changes.
    /// </summary>
    public class DataContainerAutoSaver
    {
        internal const int TIMER_INTERVAL = 5000;

        private readonly IDataContainer _container;
        private readonly Timer _timer = new Timer(TIMER_INTERVAL);
        private readonly List<string> _filters = new List<string>();

        /// <summary>
        /// Event fired when we're about to start saving
        /// </summary>
        public delegate void SavingStartedEvent();
        public event SavingStartedEvent SavingStarted;

        /// <summary>
        /// Event fired when we're finished saving
        /// </summary>
        public delegate void SavingFinishedEvent();
        public event SavingFinishedEvent SavingFinished;

        /// <summary>
        /// If true, the <see cref="IDataContainer"/> instance has changed but
        /// we're wating from <see cref="TIMER_INTERVAL"/> ms if there a bulk update to prevent consecutive saving.
        /// </summary>
        public bool HasPendingUpdates { get; set; } = false;

        /// <summary>
        /// Enable AutoSave
        /// </summary>
        private bool enabled;
        public bool Enabled 
        {
            get { return enabled; }
            set
            {
                if(enabled == value)
                {
                    return;
                }

                enabled = value;

                if(enabled)
                {
                    _container.PropertyChanged += PropertyChanged;
                }
                else
                {
                    _container.PropertyChanged -= PropertyChanged;
                }
            }
        }

        /// <summary>
        /// The amount of time waited in ms, after property changed event to save.
        /// the time is reset if another property changed event is fired before this is completed.
        /// </summary>
        public double SaveDelay
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        /// Specify true if we only want to save the instance when
        /// specific properties are changed.
        /// </summary>
        public bool UseFilters { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dc"></param>
        public DataContainerAutoSaver(IDataContainer dc)
        {
            _container = dc;
            _timer.AutoReset = false;
            _timer.Elapsed += Elapsed;

            Enabled = true;
        }

        public void AddFilter(string filter)
        {
            if(_filters.Contains(filter))
            {
                return;
            }

            _filters.Add(filter);
        }

        /// <summary>
        /// Triggered when <see cref="TIMER_INTERVAL"/> ms has elapsed the we'll save the instance;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            // notify listeners
            SavingStarted?.Invoke();

            // save;
            _container.Store();

            // notify listeners
            SavingFinished?.Invoke();
        }

        /// <summary>
        /// Invoked when <see cref="IDataContainer"/> invokes <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            /// Don't need to save it's a property we don't want.
            if(UseFilters)
            {
                if(_filters.Contains(e.PropertyName) == false)
                {
                    return;
                }
            }

            if(HasPendingUpdates)
            {
                // Reset timer if another property changed event comes before saving
                _timer.Stop();
                _timer.Start();
            }
            else
            {
                // start the timer, default is 5 sec. after that it'll save
                _timer.Start();
                HasPendingUpdates = true;
            }
        }

        /// <summary>
        /// Unsubscribe all events in finalizer
        /// </summary>
        ~DataContainerAutoSaver()
        {
            _timer.Elapsed -= Elapsed;
            _container.PropertyChanged -= PropertyChanged;
            _timer.Dispose();
        }
    }
}
