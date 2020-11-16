using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KEI.Infrastructure.Service
{
    public static class ServiceManager
    {
        private static readonly List<Type> services = new List<Type>();

        public static TService Acquire<TService>()
        {
            if (services.Contains(typeof(TService)))
            {
                return ContainerLocator.Container.Resolve<TService>();
            }

            return default;
        }

        public static void InitializeServices()
        {
            foreach (var service in services)
            {
                if(ContainerLocator.Container.Resolve(service) is IInitializable initializableService)
                {
                    initializableService.Initialize();
                }
            }
        }

        public static void RegisterService(Type serviceType)
        {
            if(services.Contains(serviceType) == false)
            {
                services.Add(serviceType);
            }
        }
    }
}
