using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KEI.Infrastructure.Service
{
    public class ServiceManager
    {
        private static readonly List<Type> services = new List<Type>();
        private readonly List<Assembly> assemblies = new List<Assembly>();

        private static ServiceManager instance;
        public static ServiceManager Instance => instance ??= new ServiceManager();

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

        public void LoadAssemblies(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Assembly.GetEntryAssembly().Location;
            }

            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(path), "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);

                    if (assemblies.Contains(assembly) == false)
                    {
                        assemblies.Add(assembly);
                    }
                }
                catch (Exception) { }
            }
        }

        public List<ServiceInfo> GetServices()
        {
            var services = new List<ServiceInfo>();

            foreach (var assembly in assemblies)
            {
                if (assembly.IsDynamic == false)
                {
                    try
                    {
                        var types = assembly.GetExportedTypes();

                        foreach (var type in types)
                        {
                            if (type.GetCustomAttribute<ServiceAttribute>(true) is ServiceAttribute sa)
                            {
                                services.Add(new ServiceInfo(type, sa));
                            }

                        }
                    }
                    catch (Exception) { }
                }
            }
            return services;
        }
    }
}
