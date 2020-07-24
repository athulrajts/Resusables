using System;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }

        void Debug(object message , [CallerFilePath]string fileName="", [CallerMemberName]string callerName="", [CallerLineNumber]int lineNumer=0);
        void Debug(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);

        void Error(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);
        void Error(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);

        void Fatal(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);
        void Fatal(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);

        void Info(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);
        void Info(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);

        void Warn(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);
        void Warn(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0);
    }
}
