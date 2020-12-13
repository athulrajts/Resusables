using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure.Logging
{
    public class SimpleLogManager : BaseLogManager
    {
        ConcurrentDictionary<string, ILogger> _loggers = new();

        private readonly SimpleLogger seedLogger;

        public SimpleLogManager(SimpleLogger seed)
        {
            DefaultLogger = seedLogger = seed;
        }

        public override ILogger GetLogger([CallerFilePath] string name = "")
        {
            string fileName = Path.GetFileNameWithoutExtension(name);

            return _loggers.GetOrAdd(name, s => seedLogger.Clone(s));
        }

        public override ILogger GetLogger(Type type)
            => _loggers.GetOrAdd(type.Name, s => seedLogger.Clone(s));
    }
}
