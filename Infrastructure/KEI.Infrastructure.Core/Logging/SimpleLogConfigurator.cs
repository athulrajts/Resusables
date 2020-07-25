using Prism;

namespace KEI.Infrastructure.Logging
{
    public class SimpleLogConfigurator : ISimpleLogConfigurator, IFileAppenderConfigurator
    {
        private SimpleLogger logger = new SimpleLogger("System");
        private ILogAppender logAppender;

        public static ILogManager ConfigureConsoleLogger() => Configure().WriteToConsole().Finish();
        
        public static ILogManager ConfigureFileLogger(string file, string pattern)
            => Configure().WriteToFile(file, pattern).Create().Finish();

        public static ILogManager ConfigureFileLogger(string file)
            => Configure().WriteToFile(file).Create().Finish();

        public static ILogManager ConfigureXMLLogger(string file)
            => Configure().WriteToXml(file).Create().Finish();

        public static ISimpleLogConfigurator Configure()
        { 
            return new SimpleLogConfigurator();
        }

        public ISimpleLogConfigurator Create()
        {
            logger.Appenders.Add(logAppender);
            return this;
        }

        public IFileAppenderConfigurator Pattern(string pattern)
        {
            if (logAppender is PatternAppender appender)
            {
                appender.LayoutPattern = pattern;
            }

            return this;
        }

        public IFileAppenderConfigurator RollingInterval(int interval)
        {
            if (logAppender is RollingFileAppender appender)
            {
                appender.RollingInterval = interval; 
            }
            return this;
        }

        public IFileAppenderConfigurator RollingMode(RollingMode mode)
        {
            if (logAppender is RollingFileAppender appender)
            {
                appender.Mode = mode; 
            }
            return this;
        }

        public IFileAppenderConfigurator RollingSize(int size)
        {
            if (logAppender is RollingFileAppender appender)
            {
                appender.RollingSize = size; 
            }
            return this;
        }

        public IFileAppenderConfigurator WriteToFile(string fileName, string pattern)
        {
            logAppender = new PatternAppender { FilePath = fileName, LayoutPattern = pattern };
            return this;
        }

        public IFileAppenderConfigurator WriteToFile(string fileName)
        {
            logAppender = new PatternAppender { FilePath = fileName };
            return this;
        }

        public ILogManager Finish()
        {
            return new SimpleLogManager(logger);
        }

        public ISimpleLogConfigurator WriteToConsole()
        {
            logAppender = new ConsoleAppender();
            logger.Appenders.Add(logAppender);
            return this;
        }

        public IFileAppenderConfigurator WriteToXml(string fileName)
        {
            logAppender = new XmlFileAppender { FilePath = fileName };
            return this;
        }

    }

    public interface ISimpleLogConfigurator
    {
        public IFileAppenderConfigurator WriteToFile(string fileName, string pattern);
        public IFileAppenderConfigurator WriteToFile(string fileName);
        public ISimpleLogConfigurator WriteToConsole();
        public IFileAppenderConfigurator WriteToXml(string fileName);
        public ILogManager Finish();
    }

    public interface IFileAppenderConfigurator
    {
        public IFileAppenderConfigurator RollingMode(RollingMode mode);
        public IFileAppenderConfigurator RollingInterval(int interval);
        public IFileAppenderConfigurator RollingSize(int size);
        public ISimpleLogConfigurator Create();
    }

}
