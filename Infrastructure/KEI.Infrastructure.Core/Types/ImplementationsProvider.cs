using System;
using System.Reflection;
using System.Collections.Generic;
using KEI.Infrastructure.Validation;
using System.IO;

namespace KEI.Infrastructure.Types
{
    public class ImplementationsProvider
    {
        private readonly Dictionary<Type, List<Type>> _implementations = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<Type, List<Type>> _services = new Dictionary<Type, List<Type>>();
        private readonly List<Assembly> assemblies = new List<Assembly>();

        private static ImplementationsProvider instance;
        public static ImplementationsProvider Instance => instance ??= new ImplementationsProvider();

        public ImplementationsProvider()
        {
            var path = Assembly.GetEntryAssembly().Location;

            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(path), "*.dll"))
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(file));
                }
                catch (Exception) { }
            }

            GetImplementations(typeof(ValidationRule));
            GetImplementations(typeof(PeriodicTasks.PeriodicTask));
        }

        public IEnumerable<Assembly> GetAssemblies() => assemblies;

        public List<Type> GetImplementations<T>() => GetImplementations(typeof(T));

        public List<Type> GetImplementations(Type t)
        {
            if (_implementations.ContainsKey(t) == false)
            {
                var implementationsTypes = new List<Type>();
                foreach (var assembly in assemblies)
                {
                    if (assembly.IsDynamic == false)
                    {
                        try
                        {
                            var types = assembly.GetExportedTypes();

                            foreach (var type in types)
                            {
                                if (t.IsAssignableFrom(type) && t != type)
                                {
                                    implementationsTypes.Add(type);
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                }
                _implementations.Add(t, implementationsTypes);
            }
            return _implementations[t];
        }

        public List<Service.Service> GetServices()
        {
            var services = new List<Service.Service>();

            foreach (var assembly in assemblies)
            {
                if (assembly.IsDynamic == false)
                {
                    try
                    {
                        var types = assembly.GetExportedTypes();

                        foreach (var type in types)
                        {
                            if (type.GetCustomAttribute<Service.ServiceAttribute>() is Service.ServiceAttribute sa)
                            {
                                services.Add(new Service.Service(type, GetImplementations(type), sa));
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
