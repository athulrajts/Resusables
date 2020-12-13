using System;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure.Logging
{
    /// <summary>
    /// Contains a static instance of <see cref="ILogger"/>
    /// does nothing if that property is not set
    /// It's set as <see cref="ILogManager.DefaultLogger"/> when logmanager is initialized with
    /// the prism container extension methods.
    /// </summary>
    public static class Logger
    {
        public static ILogger Log { get; set; }

        public static void Debug(string message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogDebug(message, fileName, callerName, lineNumer);

        public static void Debug(string message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogDebug(message, exception, fileName, callerName, lineNumer);

        public static void Error(string message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogError(message, fileName, callerName, lineNumer);

        public static void Error(string message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogError(message, exception, fileName, callerName, lineNumer);
      
        public static void Critical(string message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogCritical(message, fileName, callerName, lineNumer);
        
        public static void Critical(string message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogCritical(message, exception, fileName, callerName, lineNumer);
        
        public static void Information(string message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogInfromation(message, fileName, callerName, lineNumer);
        
        public static void Information(string message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogInfromation(message, exception, fileName, callerName, lineNumer);
        
        public static void Warning(string message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.LogWarning(message, fileName, callerName, lineNumer);
        
        public static void Warning(string message, Exception exception, string fileName = "", string callerName = "", int lineNumer = 0)
            => Log?.LogWarning(message, exception, fileName, callerName, lineNumer);
    }

    /// <summary>
    /// Generic wrapper that allows injection of <see cref="ILogger{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Logger<T> : ILogger<T>
    {
        private readonly ILogger _logger;
        public Logger(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(LogLevel level, string message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            _logger.Log(level, message, fileName, callerName, lineNumber);
        }

        public void Log(LogLevel level, string message, Exception ex, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            _logger.Log(level, message, ex, fileName, callerName, lineNumber);
        }
    }

}
