using System;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Class to help update a <see cref="IDataContainer"/> whenever the File it was loaded from changes.
    /// It doesn't add/remove new properties
    /// Only updates the values of existing ones.
    /// May be Option to add/remove properties could be added ??? but turned off by default.
    /// </summary>
    public class DataContainerAutoUpdater
    {
        private readonly IDataContainer _container;
        private readonly IFileWatcher _watcher;

        /// <summary>
        /// Event raised when we are about to update the properties
        /// </summary>
        public delegate void UpdateStartedEvent();
        public event UpdateStartedEvent UpdateStarted;

        /// <summary>
        /// Event raised when all properties are updated.
        /// </summary>
        public delegate void UpdateFinishedEvent();
        public event UpdateFinishedEvent UpdateFinished;

        /// <summary>
        /// Enable AutoUpdate
        /// </summary>
        public bool Enabled
        {
            get { return _watcher.Watch; }
            set { _watcher.Watch = value; }
        }

        /// <summary>
        /// Polling Interval
        /// Putting this here now, if there are other IFileWatcher implementations
        /// this should be moved.
        /// </summary>
        public double PollingInterval
        {
            get
            {
                return _watcher is TimerBasedFileWatcher tfw
                    ? tfw.Interval
                    : 500;
            }
            set
            {
                if(_watcher is TimerBasedFileWatcher tfw)
                {
                    tfw.Interval = value;
                }
            }
        }

        public bool CanAddItems { get; set; }

        public bool CanRemoveItems { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dc"></param>
        public DataContainerAutoUpdater(IDataContainer dc)
        {
            _container = dc;

            if(string.IsNullOrEmpty(_container.FilePath))
            {
                throw new ArgumentException("FilePath cannot be empty");
            }

            _watcher = new TimerBasedFileWatcher
            {
                Path = dc.FilePath,
                Watch = true
            };

            _watcher.OnFileChanged += OnFileChanged;
        }

        /// <summary>
        /// Raised when file is modifiled.
        /// </summary>
        private void OnFileChanged()
        {
            // notify listeners
            UpdateStarted?.Invoke();

            // load file
            IDataContainer changed = (IDataContainer)XmlHelper.DeserializeFromFile(_container.FilePath, _container.GetType());

            // update properties
            if (changed is not null)
            {
                if(CanAddItems)
                {
                    _container.Merge(changed);
                }

                if(CanRemoveItems)
                {
                    _container.InvertedRemove(changed);
                }

                _container.Refresh(changed);
            }

            // notify listeners
            UpdateFinished?.Invoke();
        }
    }
}
