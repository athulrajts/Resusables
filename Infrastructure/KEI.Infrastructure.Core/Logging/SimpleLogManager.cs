using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure.Logging
{
    public class SimpleLogManager : BaseLogManager
    {
        private readonly SimpleLogger seedLogger;

        public SimpleLogManager(SimpleLogger seed)
        {
            DefaultLogger = seedLogger = seed;
            DefaultLogger.Debug("Logmanager Initialized");
        }

        public override ILogger GetLogger([CallerFilePath] string name = "")
            => seedLogger.Clone(Path.GetFileName(name));

        public override ILogger GetLogger(Type type)
            => seedLogger.Clone(type.Name);
    }
}
