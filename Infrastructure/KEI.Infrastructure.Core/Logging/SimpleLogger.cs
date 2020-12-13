using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Logging
{
    /// <summary>
    /// <see cref="ILogger"/> Implementation
    /// </summary>
    public class SimpleLogger : ILogger
    {
        public string Name { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public SimpleLogger(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Called by <see cref="ILogManager.GetLogger(string)"/> <see cref="ILogManager.GetLogger(Type)"/>
        /// and <see cref="ILogManager.GetLoggerT{T}"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SimpleLogger Clone(string name)
        {
            var clone = (SimpleLogger)MemberwiseClone();
            clone.Name = name;
            return clone;
        }

        /// <summary>
        /// List of appenders
        /// </summary>
        public List<ILogAppender> Appenders { get; set; } = new List<ILogAppender>();

        /// <summary>
        /// Implementation for <see cref="ILogger.Log(LogLevel, string, string, string, int)"/>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="fileName"></param>
        /// <param name="callerName"></param>
        /// <param name="lineNumber"></param>
        public void Log(LogLevel level, string message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (Appenders is null || Appenders.Any() == false)
            {
                return;
            }

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = level,
                FileName = Path.GetFileName(fileName),
                MethodName = callerName,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
            };

            Log(logEvent);
        }

        /// <summary>
        /// Implementation for <see cref="ILogger.Log(LogLevel, string, Exception, string, string, int)"/>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="fileName"></param>
        /// <param name="callerName"></param>
        /// <param name="lineNumber"></param>
        public void Log(LogLevel level, string message, Exception ex, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (Appenders is null || Appenders.Any() == false)
            {
                return;
            }

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = level,
                FileName = Path.GetFileName(fileName),
                MethodName = callerName,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
                Exception = ex.Message,
                StackTrace = ex.StackTrace
            };

            Log(logEvent);
        }

        private void Log(LogEvent logEvent)
        {
            Parallel.ForEach(Appenders, a => a.Append(logEvent));
        }
    }
}
