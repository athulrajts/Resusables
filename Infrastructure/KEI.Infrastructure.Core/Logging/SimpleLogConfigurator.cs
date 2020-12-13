using System;

namespace KEI.Infrastructure.Logging
{
    /// <summary>
    /// Class to create <see cref="ILogManager"/>
    /// </summary>
    public class SimpleLogConfigurator
    {
        private readonly SimpleLogger logger = new SimpleLogger("System");

        public static ILogManager ConfigureConsoleLogger() => Configure().WriteToConsole().Finish();

        public static ILogManager ConfigureFileLogger(string file, string pattern)
        {
            return Configure().WriteToFile(file, a => a.Pattern(pattern)).Finish();
        }
        
        public static SimpleLogConfigurator Configure()
        {
            return new SimpleLogConfigurator();
        }

        /// <summary>
        /// Create logger to write to File
        /// Creates <see cref="PatternAppender"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="appenderBuilder"></param>
        /// <returns></returns>
        public SimpleLogConfigurator WriteToFile(string fileName, Action<PatternFileAppenderBuilder> appenderBuilder = null)
        {
            var appender = new PatternAppender { FilePath = fileName };
            logger.Appenders.Add(appender);
            appenderBuilder?.Invoke(new PatternFileAppenderBuilder(appender));
            return this;
        }

        /// <summary>
        /// Get instance of <see cref="ILogManager"/> with the appenders built
        /// </summary>
        /// <returns></returns>
        public ILogManager Finish()
        {
            return new SimpleLogManager(logger);
        }

        /// <summary>
        /// Creates logger to write to console
        /// </summary>
        /// <param name="appenderBuilder"></param>
        /// <returns></returns>
        public SimpleLogConfigurator WriteToConsole(Action<BaseAppenderBuilder> appenderBuilder = null)
        {
            var appender = new ColorConsoleAppender();
            logger.Appenders.Add(appender);
            appenderBuilder?.Invoke(new BaseAppenderBuilder(appender));
            return this;
        }

        /// <summary>
        /// Creates a logger to write to <see cref="System.Diagnostics.Trace"/>
        /// </summary>
        /// <param name="appenderBuilder"></param>
        /// <returns></returns>
        public SimpleLogConfigurator WriteToTrace(Action<BaseAppenderBuilder> appenderBuilder = null)
        {
            var appender = new TraceAppender();
            logger.Appenders.Add(appender);
            appenderBuilder?.Invoke(new BaseAppenderBuilder(appender));
            return this;
        }
    }

}
