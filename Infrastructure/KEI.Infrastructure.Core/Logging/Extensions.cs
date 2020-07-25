using KEI.Infrastructure.Configuration;
using Prism.Ioc;
using System.Collections.Generic;
using System.IO;

namespace KEI.Infrastructure.Logging
{
    public static class Extensions
    {
        public static IContainerRegistry RegisterLogger(this IContainerRegistry registry, ILogManager manager)
        {
            return registry.RegisterInstance(manager);
        }

        public static IContainerRegistry RegisterLogger(this IContainerRegistry registry, FileInfo config)
        {
            if (config.Exists == false)
                return registry;

            if(DataContainer.FromFile(config.FullName) is DataContainer dc)
            {
                if(dc.Morph() is List<ILogAppender> appenders)
                {
                    return registry.RegisterInstance(new SimpleLogManager(new SimpleLogger("Default") { Appenders = appenders }));
                }
            }

            return registry;
        }

        public static IContainerRegistry RegisterLogger(this IContainerRegistry registry,string file, string pattern)
        {
            return registry.RegisterInstance(SimpleLogConfigurator.Configure().WriteToFile(file, pattern).Create().Finish());
        }

        public static IContainerRegistry RegisterLogger(this IContainerRegistry registry, string file)
        {
            return registry.RegisterInstance(SimpleLogConfigurator.Configure().WriteToFile(file).Create().Finish());
        }

        public static IContainerRegistry RegisterConsoleLogger(this IContainerRegistry registry)
        {
            return registry.RegisterInstance(SimpleLogConfigurator.Configure().WriteToConsole().Finish());
        }
    }
}
