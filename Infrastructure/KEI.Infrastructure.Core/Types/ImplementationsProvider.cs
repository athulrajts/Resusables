using System;
using System.Reflection;
using System.Collections.Generic;
using KEI.Infrastructure.Validation;

namespace KEI.Infrastructure.Types
{
    public class ImplementationsProvider
    {
        private readonly Dictionary<Type, List<Type>> _implementations = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<Type, List<Type>> _services = new Dictionary<Type, List<Type>>();

        public ImplementationsProvider()
        {
            GetImplementations(typeof(ValidationRule));
            //GetImplementations(typeof(PeriodicTasks.PeriodicTask));
        }

        public List<Type> GetImplementations(Type t)
        {
            if (_implementations.ContainsKey(t) == false)
            {
                var results = AppDomain.CurrentDomain.GetAssemblies();


                var implementationsTypes = new List<Type>();
                foreach (var assembly in results)
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
            var results = AppDomain.CurrentDomain.GetAssemblies();

            var services = new List<Service.Service>();

            foreach (var assembly in results)
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
