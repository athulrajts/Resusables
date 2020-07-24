using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Logging
{
    public class SimpleLogger : ILogger
    {
        internal string Name { get; set; }

        public SimpleLogger(string name)
        {
            Name = name;
        }

        public SimpleLogger Clone(string name)
        {
            var clone = (SimpleLogger)MemberwiseClone();
            clone.Name = name;
            return clone;
        }


        public List<ILogAppender> Appenders { get; set; } = new List<ILogAppender>();

        public bool IsDebugEnabled { get; set; } = true;

        public bool IsErrorEnabled { get; set; } = true;

        public bool IsFatalEnabled { get; set; } = true;

        public bool IsInfoEnabled { get; set; } = true;

        public bool IsWarnEnabled { get; set; } = true;

        public void Debug(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsDebugEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Debug,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName
            };

            Log(logEvent);

        }

        private void Log(LogEvent logEvent)
        {
            Parallel.ForEach(Appenders, a => a.Append(logEvent));
        }


        public void Debug(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsDebugEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Debug,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
                Exception = exception.Message,
                StackTrace = exception.StackTrace
            };

            Log(logEvent);
        }

        public void Error(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsErrorEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Error,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName
            };

            Log(logEvent);
        }

        public void Error(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsErrorEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Error,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
                Exception = exception.Message,
                StackTrace = exception.StackTrace
            };

            Log(logEvent);
        }

        public void Fatal(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsFatalEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Fatal,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
            };

            Log(logEvent);
        }

        public void Fatal(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsFatalEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Fatal,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
                Exception = exception.Message,
                StackTrace = exception.StackTrace
            };

            Log(logEvent);
        }

        public void Info(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsInfoEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Info,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName
            };

            Log(logEvent);
        }

        public void Info(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsInfoEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Info,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
                Exception = exception.Message,
                StackTrace = exception.StackTrace
            };

            Log(logEvent);
        }

        public void Warn(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsWarnEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Warn,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName
            };

            Log(logEvent);
        }

        public void Warn(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (IsWarnEnabled == false)
                return;

            if (Appenders == null)
                return;

            var logEvent = new LogEvent
            {
                Name = Name,
                Level = LogLevel.Warn,
                FileName = Path.GetFileName(callerFile),
                MethodName = callerMember,
                LineNumber = lineNumber,
                Time = DateTime.Now,
                Message = message?.ToString(),
                MachineName = Environment.MachineName,
                Exception = exception.Message,
                StackTrace = exception.StackTrace
            };

            Log(logEvent);
        }
    }
}
