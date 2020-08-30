﻿using System.Collections.Generic;
using Prism.Ioc;
using KEI.Infrastructure.Service;
using KEI.Infrastructure.Localizer;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.UserManagement;
using KEI.Infrastructure.Logging;
using System.Reflection;

namespace KEI.Infrastructure.Prism
{
    public static class ContainerRegistryExtentions
    {

        public static IContainerRegistry RegisterInfrastructureServices(this IContainerRegistry registry)
        {
            registry.RegisterSingleton<IConfigManager, ConfigManager>();
            registry.RegisterSingleton<IUserManager, UserManager>();
            registry.RegisterSingleton<ILocalizationProvider, LocalizationProvider>();
            registry.RegisterSingleton<IEssentialServices, EssentialServices>();
            LocalizationManager.Instance.Initialize();
            return registry;
        }

        public static IContainerRegistry RegisterLocalizationInAssembly(this IContainerRegistry registry)
        {
            registry.RegisterInstance<IStringLocalizer>(new ResourceManagerStringLocalizer(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly().GetName().Name);
            return registry;
        }

        public static IContainerRegistry RegisterServices(this IContainerRegistry registry, IEnumerable<Service.Service> services)
        {
            foreach (var service in services)
            {
                if (service.ServiceType == null || service.ImplementationType == null)
                    continue;

                registry.RegisterSingleton(service.ServiceType.GetUnderlyingType(), service.ImplementationType.GetUnderlyingType());
            }
            return registry;
        }
    }
}
