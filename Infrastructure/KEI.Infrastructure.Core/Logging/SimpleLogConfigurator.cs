using System;

namespace KEI.Infrastructure.Logging
{
    public class SimpleLogConfigurator
    {
        private readonly SimpleLogger logger = new SimpleLogger("System");

        public static ILogManager ConfigureConsoleLogger() => Configure().WriteToConsole().Finish();

        public static ILogManager ConfigureFileLogger(string file, string pattern)
        {
            return Configure().WriteToFile(file, a => a.Pattern(pattern)).Finish();
        }

        public static ILogManager ConfigureXmlLogger(string file)
        {
            return Configure().WriteToXml(file).Finish();
        }

        public static SimpleLogConfigurator Configure()
        { 
            return new SimpleLogConfigurator();
        }

        public SimpleLogConfigurator WriteToFile(string fileName, Action<PatternFileAppenderBuilder> appenderBuilder = null)
        {
            var appender = new PatternAppender { FilePath = fileName };
            logger.Appenders.Add(appender);
            appenderBuilder?.Invoke(new PatternFileAppenderBuilder(appender));
            return this;
        }

        public ILogManager Finish()
        {
            return new SimpleLogManager(logger);
        }

        public SimpleLogConfigurator WriteToConsole()
        {
            logger.Appenders.Add(new ConsoleAppender());
            return this;
        }

        public SimpleLogConfigurator WriteToXml(string fileName, Action<FileAppenderBuilder> appenderBuilder = null)
        {
            var appender = new XmlFileAppender { FilePath = fileName };
            logger.Appenders.Add(appender);
            appenderBuilder?.Invoke(new FileAppenderBuilder(appender));
            return this;
        }

    }

    public class FileAppenderBuilder
    {
        private RollingFileAppender _appender;

        public FileAppenderBuilder(RollingFileAppender appender)
        {
            _appender = appender;
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
        private PatternAppender _appender;

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

        public PatternFileAppenderBuilder Pattern(string pattern)
        {
            _appender.LayoutPattern = pattern;
            return this;
        }
    }

}
