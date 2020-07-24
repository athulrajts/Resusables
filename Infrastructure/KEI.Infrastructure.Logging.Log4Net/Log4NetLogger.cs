using System;
using System.Runtime.CompilerServices;
using log4net;

namespace KEI.Infrastructure
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog log4NetLogger;

        public Log4NetLogger(ILog p_Log)
        {
            log4NetLogger = p_Log;
        }

        #region ILogger Members

        public bool IsDebugEnabled
        {
            get { return log4NetLogger.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return log4NetLogger.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return log4NetLogger.IsFatalEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return log4NetLogger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return log4NetLogger.IsWarnEnabled; }
        }

        public void Debug(object message,
            [CallerFilePath] string callerFile = "",
            [CallerMemberName] string callerMember = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Debug(message);
        }

        public void Debug(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Debug(message, exception);
        }

        public void Error(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Error(message);
        }

        public void Error(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Error(message, exception);
        }

        public void Fatal(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Fatal(message);
        }

        public void Fatal(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Fatal(message, exception);
        }

        public void Info(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Info(message);
        }

        public void Info(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Info(message, exception);
        }

        public void Warn(object message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Warn(message);
        }

        public void Warn(object message, Exception exception, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "", [CallerLineNumber] int lineNumber = 0)
        {
            log4NetLogger.Warn(message, exception);
        }

        #endregion
    }
}
