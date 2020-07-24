using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Logging
{
    public abstract class BaseAppender : ILogAppender
    {
        private readonly BlockingCollection<LogEvent> _blockingCollection = new BlockingCollection<LogEvent>();


        public BaseAppender()
        {
            Task.Factory.StartNew(ProcessLog, TaskCreationOptions.LongRunning);
        }

        private void ProcessLog()
        {
            foreach (var log in _blockingCollection.GetConsumingEnumerable())
            {
                ProcessLogInternal(log);
            }
        }

        protected abstract void ProcessLogInternal(LogEvent logEvent);


        public void Append(LogEvent msg) => _blockingCollection.Add(msg);
    }
}
