using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using KEI.Infrastructure.Service;

namespace KEI.Infrastructure.Types
{
    public class ImplementationsProvider
    {
        private readonly Dictionary<Type, List<Type>> _implementations = new Dictionary<Type, List<Type>>();
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
                                if(type.IsAbstract)
                                {
                                    continue;
                                }    

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
