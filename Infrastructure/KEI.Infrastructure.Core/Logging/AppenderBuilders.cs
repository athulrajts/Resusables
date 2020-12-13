using System;

namespace KEI.Infrastructure.Logging
{
    public class BaseAppenderBuilder
    {
        readonly BaseAppender _appender;

        public BaseAppenderBuilder(BaseAppender appender)
        {
            _appender = appender;
        }

        public virtual BaseAppenderBuilder MinimumLogLevel(LogLevel logLevel)
        {
            _appender.MinimumLogLevel = logLevel;
            return this;
        }

        public virtual BaseAppenderBuilder Filter(Predicate<LogEvent> predicate)
        {
            _appender.Filter = predicate;
            return this;
        }
    }

    public class FileAppenderBuilder : BaseAppenderBuilder
    {
        private readonly RollingFileAppender _appender;

        public FileAppenderBuilder(RollingFileAppender appender) : base(appender)
        {
            _appender = appender;
        }

        public override FileAppenderBuilder MinimumLogLevel(LogLevel logLevel)
        {
            return (FileAppenderBuilder)base.MinimumLogLevel(logLevel);
        }

        public override FileAppenderBuilder Filter(Predicate<LogEvent> predicate)
        {
            return (FileAppenderBuilder)base.Filter(predicate);
        }

        public virtual FileAppenderBuilder RollingMode(RollingMode mode)
        {
            _appender.Mode = mode;
            return this;
        }

        public virtual FileAppenderBuilder RollingInterval(int interval)
        {
            _appender.RollingInterval = interval;
            return this;
        }

        public virtual FileAppenderBuilder RollingSize(int size)
        {
            _appender.RollingSize = size;
            return this;
        }
    }

    public class PatternFileAppenderBuilder : FileAppenderBuilder
    {
        private readonly PatternAppender _appender;

        public PatternFileAppenderBuilder(PatternAppender appender) : base(appender)
        {
            _appender = appender;
        }

        public override PatternFileAppenderBuilder RollingMode(RollingMode mode)
        {
            return (PatternFileAppenderBuilder)base.RollingMode(mode);
        }

        public override PatternFileAppenderBuilder RollingInterval(int interval)
        {
            return (PatternFileAppenderBuilder)base.RollingInterval(interval);
        }

        public override PatternFileAppenderBuilder RollingSize(int size)
        {
            return (PatternFileAppenderBuilder)base.RollingSize(size);
        }

        public override PatternFileAppenderBuilder MinimumLogLevel(LogLevel logLevel)
        {
            return (PatternFileAppenderBuilder)base.MinimumLogLevel(logLevel);
        }

        public override PatternFileAppenderBuilder Filter(Predicate<LogEvent> predicate)
        {
            return (PatternFileAppenderBuilder)base.Filter(predicate);
        }

        public PatternFileAppenderBuilder Pattern(string pattern)
        {
            _appender.LayoutPattern = pattern;
            return this;
        }
    }
}
