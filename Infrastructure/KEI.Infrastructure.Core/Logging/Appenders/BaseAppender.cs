using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace KEI.Infrastructure.Logging
{
    /// <summary>
    /// Base implementation for <see cref="ILogAppender"/> which is a long running thread for
    /// processing log events
    /// </summary>
    public abstract class BaseAppender : ILogAppender
    {
        private readonly BlockingCollection<LogEvent> _blockingCollection = new BlockingCollection<LogEvent>();

        /// <summary>
        /// Minimum log level, anything lower should be ignored by the appender
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;

        /// <summary>
        /// Only log events satisfying the predicate should be logged by the appender.
        /// </summary>
        public Predicate<LogEvent> Filter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseAppender()
        {
            Task.Factory.StartNew(ProcessLog, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Process log events thread.
        /// </summary>
        private void ProcessLog()
        {
            foreach (var log in _blockingCollection.GetConsumingEnumerable())
            {
                // Inheritors should implement
                ProcessLogInternal(log);
            }
        }

        /// <summary>
        /// Implementation for <see cref="ILogAppender.Append(LogEvent)"/>
        /// </summary>
        /// <param name="msg"></param>
        public void Append(LogEvent msg)
        {
            if(msg.Level < MinimumLogLevel)
            {
                return;
            }

            if(Filter is not null && Filter(msg) == false)
            {
                return;
            }

            _blockingCollection.Add(msg);
        }

        /// <summary>
        /// Appenders should handle how to process a log event.
        /// </summary>
        /// <param name="logEvent"></param>
        protected abstract void ProcessLogInternal(LogEvent logEvent);

    }
}
