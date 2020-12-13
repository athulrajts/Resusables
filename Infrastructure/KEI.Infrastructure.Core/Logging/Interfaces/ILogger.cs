using System;
using System.Runtime.CompilerServices;
using KEI.Infrastructure.Logging;

namespace KEI.Infrastructure
{
    public interface ILogger
    {
        void Log(LogLevel level, string message,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0);

        void Log(LogLevel level, string message,
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0);
    }

    public interface ILogger<T> : ILogger
    {

    }

    /// <summary>
    /// Extension methods for <see cref="ILogger"/>
    /// </summary>
    public static class LoggerExtensions
    {
        public static void LogDebug(this ILogger logger,
            string message,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Debug, message, fileName, callerName, lineNumber);
        }

        public static void LogDebug(this ILogger logger,
            string message,
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Debug, message, ex, fileName, callerName, lineNumber);
        }

        public static void LogInfromation(this ILogger logger,
            string message,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Information, message, fileName, callerName, lineNumber);
        }

        public static void LogInfromation(this ILogger logger,
            string message,
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Information, message, ex, fileName, callerName, lineNumber);
        }

        public static void LogWarning(this ILogger logger,
            string message,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Warning, message, fileName, callerName, lineNumber);
        }

        public static void LogWarning(this ILogger logger,
            string message,
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Warning, message, ex, fileName, callerName, lineNumber);
        }

        public static void LogError(this ILogger logger,
            string message,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Error, message, fileName, callerName, lineNumber);
        }

        public static void LogError(this ILogger logger,
            string message,
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Error, message, ex, fileName, callerName, lineNumber);
        }

        public static void LogCritical(this ILogger logger,
            string message,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Critical, message, fileName, callerName, lineNumber);
        }

        public static void LogCritical(this ILogger logger,
            string message,
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.Log(LogLevel.Critical, message, ex, fileName, callerName, lineNumber);
        }

    }
}