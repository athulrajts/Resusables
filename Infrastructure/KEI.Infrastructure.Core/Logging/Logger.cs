using System;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure.Logging
{
    public static class Logger
    {
        public static ILogger Log { get; set; }

        public static void Debug(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Debug(message, fileName, callerName, lineNumer);

        public static void Debug(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Debug(message, exception, fileName, callerName, lineNumer);

        public static void Error(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Error(message, fileName, callerName, lineNumer);

        public static void Error(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Error(message, exception, fileName, callerName, lineNumer);
      
        public static void Fatal(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Fatal(message, fileName, callerName, lineNumer);
        
        public static void Fatal(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Fatal(message, exception, fileName, callerName, lineNumer);
        
        public static void Info(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Info(message, fileName, callerName, lineNumer);
        
        public static void Info(object message, Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Info(message, exception, fileName, callerName, lineNumer);
        
        public static void Warn(object message, [CallerFilePath] string fileName = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumer = 0)
            => Log?.Warn(message, fileName, callerName, lineNumer);
        
        public static void Warn(object message, Exception exception, string fileName = "", string callerName = "", int lineNumer = 0)
            => Log?.Warn(message, exception, fileName, callerName, lineNumer);
    }

}
