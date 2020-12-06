using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Prism.Ioc;
using Prism.Events;
using Prism.Regions;
using Prism.Modularity;
using KEI.Infrastructure.Localizer;

namespace KEI.Infrastructure.Prism
{
    /// <summary>
    /// Deafult implementation for <see cref="IModule"/> that automagically registers
    /// types to the container based on
    /// <see cref="RegisterSingletonAttribute"/>
    /// <see cref="RegisterForNavigationAttribute"/>
    /// <see cref="RegisterWithRegionAttribute"/>
    /// Also does a named registration a <see cref="ResourceManagerStringLocalizer"/> as an implementation for <see cref="IStringLocalizer"/> which
    /// will be used by <see cref="LocalizationManager"/>
    /// </summary>
    public class Module : IModule
    {
        private readonly IEnumerable<Type> _typesForNavigation;
        private readonly IEnumerable<Type> _typesForViewRegistration;
        private readonly IEnumerable<Type> _typesForSingletonRegistration;
        private readonly string name;
        private readonly List<Type> _typesForResolve = new List<Type>();

        protected readonly Type[] _assemblyTypes;
        protected readonly IRegionManager _regionManager;
        protected readonly IEventAggregator _eventAggregator;

        public Module(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            name = GetType().Assembly.GetName().Name;

            SplashScreenLogger.Instance.Log($"{name}: Scaning for types..");

            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _assemblyTypes = GetType().Assembly.GetTypes();
            
            //Get Views/ViewModels to register for navigation
            _typesForNavigation = _assemblyTypes
                .Where(t => t.GetCustomAttributes<RegisterForNavigationAttribute>().Any());

            _typesForViewRegistration = _assemblyTypes
                .Where(t => t.GetCustomAttributes<RegisterWithRegionAttribute>().Any());

            _typesForSingletonRegistration = _assemblyTypes
                .Where(t => t.GetCustomAttributes<RegisterSingletonAttribute>().Any());

            int count = _typesForNavigation.Count() + _typesForViewRegistration.Count()  + _typesForSingletonRegistration.Count();

            SplashScreenLogger.Instance.Log($"{name}: {count} type(s) found !");

        }

        public virtual void OnInitialized(IContainerProvider containerProvider)
        {
            foreach (var type in _typesForResolve)
            {
                containerProvider.Resolve(type);
            }

            SplashScreenLogger.Instance.Log($"{name}: Initialized");
        }

        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
            SplashScreenLogger.Instance.Log($"{name}: Registering Types..");

            containerRegistry.RegisterInstance<IStringLocalizer>(new ResourceManagerStringLocalizer(GetType().Assembly), name);

            // Register Types for Singleton Registration
            foreach (var t in _typesForSingletonRegistration)
            {
                var needResolve = t.GetCustomAttribute<RegisterSingletonAttribute>().NeedResolve;

                containerRegistry.RegisterSingleton(t);

                if(needResolve)
                {
                    _typesForResolve.Add(t);
                }
            }

            // Register Types for navigation
            foreach (var t in _typesForNavigation)
            {
                if (!containerRegistry.IsRegistered(t))
                {
                    containerRegistry.RegisterForNavigation(t, t.Name);
                }
            }

            
            // Register views with Region
            foreach (var t in _typesForViewRegistration)
            {
                var regionName = t.GetCustomAttribute<RegisterWithRegionAttribute>().RegionName;

                if (!containerRegistry.IsRegistered(t))
                {
                    _regionManager.RegisterViewWithRegion(regionName, t);
                }
            }

            SplashScreenLogger.Instance.Log($"{name}: Registering Types Completed..");
        }

    }
}
