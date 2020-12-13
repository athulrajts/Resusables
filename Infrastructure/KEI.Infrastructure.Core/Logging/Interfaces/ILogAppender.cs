using System;

namespace KEI.Infrastructure.Logging
{
    public interface ILogAppender
    {
        LogLevel MinimumLogLevel { get; set; }

        Predicate<LogEvent> Filter { get; set; }

        void Append(LogEvent msg);
    }
}
