using System;
using System.IO;
using System.Timers;

namespace KEI.Infrastructure
{
    public delegate void FileChangedEvent();

    /// <summary>
    /// Interfaces implemented by class would monitors for changes in a file
    /// </summary>
    public interface IFileWatcher
    {
        public event FileChangedEvent OnFileChanged;

        public bool Watch { get; set; }

        public string Path { get; set; }
    }

    /// <summary>
    /// Polls the file regularly and check's whether it was modified, raises an event if it's modified.
    /// </summary>
    public class TimerBasedFileWatcher : IFileWatcher
    {
        internal const int TIMER_INTERVAL = 4000;
        private readonly Timer _timer = new Timer(TIMER_INTERVAL);
        
        private FileInfo _info;
        private DateTime _lastModifiedDate;

        /// <summary>
        /// <see cref="IFileWatcher.OnFileChanged"/>
        /// </summary>
        public event FileChangedEvent OnFileChanged;

        /// <summary>
        /// Enable/Disable raising change events.
        /// </summary>
        public bool Watch 
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        /// <summary>
        /// Polling interval
        /// </summary>
        public double Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        /// File to watch for changes.
        /// </summary>
        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                if(path == value)
                {
                    return;
                }

                path = value;

                _info = new FileInfo(System.IO.Path.GetFullPath(path));
                _lastModifiedDate = _info.LastWriteTimeUtc;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TimerBasedFileWatcher()
        {
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = false;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~TimerBasedFileWatcher()
        {
            _timer.Elapsed -= Timer_Elapsed;
        }

        /// <summary>
        /// When timer elapsed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _info.Refresh();

            if(_lastModifiedDate < _info.LastWriteTimeUtc)
            {
                _lastModifiedDate = _info.LastWriteTimeUtc;

                OnFileChanged?.Invoke();
            }
        }
    }
}
