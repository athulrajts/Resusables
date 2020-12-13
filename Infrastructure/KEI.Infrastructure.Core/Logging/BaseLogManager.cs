using System;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure.Logging
{
    public abstract class BaseLogManager : ILogManager
    {
        private ILogger defaultLogger;
        public ILogger DefaultLogger
        {
            get => defaultLogger ??= GetLogger("Default");
            protected set
            {
                if (defaultLogger == value)
                    return;

                defaultLogger = value;

                Logger.Log = defaultLogger;
            }
        }

        public abstract ILogger GetLogger([CallerFilePath] string p_strName = "");

        public abstract ILogger GetLogger(Type p_Type);

        public ILogger<T> GetLoggerT<T>()
        {
            return new Logger<T>(GetLogger(typeof(T)));
        }
    }
}
