using KEI.Infrastructure.Service;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Linq;
using Prism.Commands;
using KEI.Infrastructure;
using TypeInfo = KEI.Infrastructure.Types.TypeInfo;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Types;

namespace ServiceEditor.ViewModels
{
    public class ServiceEditorViewModel : BindableBase
    {
        private const string FilePath = "Configs/Services.cfg";
        private readonly IViewService _viewService;

        public ServiceEditorViewModel(IViewService viewService, ImplementationsProvider provider)
        {
            _viewService = viewService;

            Services = new ObservableCollection<Service>(provider.GetServices());

            SelectedServices = XmlHelper.Deserialize<ObservableCollection<Service>>(FilePath) ?? new ObservableCollection<Service>();

            SetImplementations();
        }

        #region Properties

        private ObservableCollection<Service> services;
        public ObservableCollection<Service> Services
        {
            get { return services; }
            set { SetProperty(ref services, value); }
        }

        private ObservableCollection<Service> selectedServices;
        public ObservableCollection<Service> SelectedServices
        {
            get { return selectedServices; }
            set { SetProperty(ref selectedServices, value); }
        }

        #endregion

        #region Private Functions

        private void SetImplementations()
        {
            foreach (var service in SelectedServices)
            {
                service.AvailableImplementations = Services.FirstOrDefault(x => x.Name == service.Name)?.AvailableImplementations;
                if (service.ImplementationType is TypeInfo t)
                {
                    service.ImplementationType = service.AvailableImplementations.FirstOrDefault(x => x.FullName == t.FullName);
                }
            }
        }

        #endregion

        #region Store Service Config

        private DelegateCommand saveServiceConfigCommnd;
        public DelegateCommand SaveServiceConfigCommand =>
            saveServiceConfigCommnd ??= saveServiceConfigCommnd = new DelegateCommand(ExecuteSaveServiceConfigCommand);

        void ExecuteSaveServiceConfigCommand()
        {
            if(XmlHelper.Serialize(SelectedServices, FilePath) == true)
            {
                _viewService.Inform("Config updated !");
            }
        }

        #endregion

        #region Configure Service

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

        #endregion

        #region Load Service Config

        private DelegateCommand loadServiceConfigCommand;
        public DelegateCommand LoadServiceConfigCommand =>
            loadServiceConfigCommand ??= loadServiceConfigCommand = new DelegateCommand(ExecuteLoadServiceConfigCommand);

        void ExecuteLoadServiceConfigCommand()
        {
            var fileName = _viewService.BrowseFile("Config File", "cfg");

            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }

            if(XmlHelper.Deserialize<ObservableCollection<Service>>(fileName) is ObservableCollection<Service> cfg)
            {
                SelectedServices = cfg;
                SetImplementations();
            }
            else
            {
                _viewService.Error("Unable to load config !. corrupted file !");
            }
        }

        #endregion

        #region Add Service

        private DelegateCommand<object[]> addServiceCommand;
        public DelegateCommand<object[]> AddServiceCommand =>
            addServiceCommand ??= addServiceCommand = new DelegateCommand<object[]>(ExecuteAddServiceCommand);

        void ExecuteAddServiceCommand(object[] parameter)
        { 
            if(parameter[0] is Service s && parameter[1] is TypeInfo t)
            {
                if(SelectedServices.FirstOrDefault(x => x.Name == s.Name) is Service ss)
                {
                    ss.ImplementationType = t;
                }
                else
                {
                    SelectedServices.Add(s);
                }
            }
        }

        #endregion
    }
}
