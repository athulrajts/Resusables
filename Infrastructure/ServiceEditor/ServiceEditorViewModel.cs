using KEI.Infrastructure.Service;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Linq;
using Prism.Commands;
using KEI.Infrastructure;
using TypeInfo = KEI.Infrastructure.Types.TypeInfo;
using System.Collections.Generic;
using KEI.Infrastructure.Logging;
using System.Xml;
using System.Diagnostics;
using CommonServiceLocator;
using System.Runtime.Serialization;
using KEI.Infrastructure.Configuration;

namespace ServiceEditor
{
    public class ServiceEditorViewModel : BindableBase
    {
        private const string FilePath = "Configs/Services.cfg";
        public ObservableCollection<Service> Services { get; set; } = new ObservableCollection<Service>();

        private ObservableCollection<Service> selectedServices;
        public ObservableCollection<Service> SelectedServices
        {
            get { return selectedServices; }
            set { SetProperty(ref selectedServices, value); }
        }

        private readonly List<Assembly> assemblies = new List<Assembly>();

        public ServiceEditorViewModel()
        { 

            foreach (var assemblyPath in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(assemblyPath));
                }
                catch (Exception) { }
            }

            foreach (var assembly in assemblies.ToList())
            {
                if (assembly.IsDynamic == false)
                {
                    try
                    {
                        var types = assembly.GetExportedTypes();

                        foreach (var type in types)
                        {
                            if (type.GetCustomAttribute<ServiceAttribute>() is ServiceAttribute sa)
                            {
                                Services.Add(new Service(type, GetImplementations(type), sa));
                            }

                        }
                    }
                    catch (Exception) { }
                }
            }

            SelectedServices = XmlHelper.Deserialize<ObservableCollection<Service>>(FilePath);

            if(SelectedServices == null)
            {
                SelectedServices = Services;
                SaveServicesCommand.Execute();
            }


            SetImplementations();

        }

        private List<Type> GetImplementations(Type t)
        {
            var implementationsTypes = new List<Type>();
            foreach (var assembly in assemblies.ToList())
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
            return implementationsTypes;
        }

        private void SetImplementations()
        {
            foreach (var service in SelectedServices)
            {
                service.AvailableImplementations = Services.FirstOrDefault(x => x.Name == service.Name)?.AvailableImplementations;
                if (service.ImplementationType is TypeInfo t)
                    service.ImplementationType = service.AvailableImplementations.FirstOrDefault(x => x.FullName == t.FullName);
            }
        }

        private DelegateCommand saveServicesCommnd;
        public DelegateCommand SaveServicesCommand =>
            saveServicesCommnd ?? (saveServicesCommnd = new DelegateCommand(ExecuteSaveServicesCommand));

        void ExecuteSaveServicesCommand() => XmlHelper.Serialize(Services, FilePath);

        private DelegateCommand<Service> configureServiceCommand;
        public DelegateCommand<Service> ConfigureServiceCommand =>
            configureServiceCommand ??= configureServiceCommand = new DelegateCommand<Service>(ExecuteConfigureServiceCommand);

        void ExecuteConfigureServiceCommand(Service service)
        {
            var implementationType = service.ImplementationType.GetUnderlyingType();

            if(implementationType == null)
            {
                return;
            }

            if(implementationType.GetProperty("ConfigPath") is PropertyInfo pi)
            {
                var obj = FormatterServices.GetUninitializedObject(implementationType);

                var configPath = pi.GetValue(obj)?.ToString();
                
                if (File.Exists(configPath) == false)
                {
                    if (implementationType.GetMethod("DefineConfigShape", BindingFlags.NonPublic | BindingFlags.Instance) is MethodInfo mi)
                    {
                        var cfg = mi.Invoke(obj, null);

                        if(mi.ReturnType.GetMethod("Build") is MethodInfo bmi)
                        {
                            IDataContainer config = (IDataContainer)bmi.Invoke(cfg, null);
                            config.Store(configPath);
                        }
                    } 
                }

                if (File.Exists(configPath))
                {
                    Process.Start("ConfigEditor.exe", configPath); 
                } 
            }
        }
    }
}
