using System;
using System.IO;
using System.Collections.Generic;
using Prism.Ioc;

namespace KEI.Infrastructure.Logging
{
    public static class ContainerExtensions
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

        public static IContainerRegistry RegisterLogger(this IContainerRegistry registry, Action<SimpleLogConfigurator> builder)
        {
            var configurator = new SimpleLogConfigurator();
            builder.Invoke(configurator);
            return registry.RegisterInstance(configurator.Finish());
        }

        public static IContainerRegistry RegisterConsoleLogger(this IContainerRegistry registry)
        {
            return registry.RegisterInstance(SimpleLogConfigurator.Configure().WriteToConsole().Finish());
        }
    }
}
