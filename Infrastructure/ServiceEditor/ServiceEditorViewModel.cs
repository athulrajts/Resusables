using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Utils;
using KEI.Infrastructure.Types;
using KEI.Infrastructure.Service;
using TypeInfo = KEI.Infrastructure.Types.TypeInfo;

namespace ServiceEditor.ViewModels
{
    public class ServiceEditorViewModel : BindableBase
    {
        private readonly string FilePath = PathUtils.GetPath("Configs/Services.cfg");
        private readonly IViewService _viewService;

        public ServiceEditorViewModel(IViewService viewService)
        {
            ServiceManager.Instance.LoadAssemblies();

            _viewService = viewService;

            Services = new ObservableCollection<ServiceInfo>(ServiceManager.Instance.GetServices());

            SelectedServices = XmlHelper.DeserializeFromFile<ObservableCollection<ServiceInfo>>(FilePath) ?? new ObservableCollection<ServiceInfo>();

            // Add required services
            foreach (var requiredService in Services.Where(x => x.IsRequired))
            {
                if(SelectedServices.FirstOrDefault(x => x.Name == requiredService.Name) is null)
                {
                    SelectedServices.Add(requiredService);
                }
            }


            SetImplementations();
        }

        #region Properties

        private ObservableCollection<ServiceInfo> services;
        public ObservableCollection<ServiceInfo> Services
        {
            get { return services; }
            set { SetProperty(ref services, value); }
        }

        private ObservableCollection<ServiceInfo> selectedServices;
        public ObservableCollection<ServiceInfo> SelectedServices
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
                service.ImplementationType = service.AvailableImplementations?.FirstOrDefault();
            }
        }

        #endregion

        #region Store Service Config

        private DelegateCommand saveServiceConfigCommnd;
        public DelegateCommand SaveServiceConfigCommand =>
            saveServiceConfigCommnd ??= saveServiceConfigCommnd = new DelegateCommand(ExecuteSaveServiceConfigCommand);

        void ExecuteSaveServiceConfigCommand()
        {
            if(XmlHelper.SerializeToFile(SelectedServices, FilePath) == true)
            {
                _viewService.Inform("Config updated !");
            }
        }

        #endregion

        #region Configure Service

        private DelegateCommand<ServiceInfo> configureServiceCommand;
        public DelegateCommand<ServiceInfo> ConfigureServiceCommand =>
            configureServiceCommand ??= configureServiceCommand = new DelegateCommand<ServiceInfo>(ExecuteConfigureServiceCommand);

        void ExecuteConfigureServiceCommand(ServiceInfo service)
        {
            var implementationType = service.ImplementationType.GetUnderlyingType();

            if(implementationType == null)
            {
                return;
            }

            var configurableService = FormatterServices.GetUninitializedObject(implementationType) as IConfigurable;
            
            string configPath = configurableService.ConfigPath;
            if (File.Exists(configPath) == false)
            {
                configurableService.ResetConfig();
                configurableService.StoreConfig(configPath);
            }

            if (File.Exists(configPath))
            {
                Process.Start("ConfigEditor.exe", configPath); 
            } 
        }

        #endregion

        #region Load Service Config

        private DelegateCommand loadServiceConfigCommand;
        public DelegateCommand LoadServiceConfigCommand =>
            loadServiceConfigCommand ??= loadServiceConfigCommand = new DelegateCommand(ExecuteLoadServiceConfigCommand);

        void ExecuteLoadServiceConfigCommand()
        {
            var fileName = _viewService.BrowseFile(new FilterCollection { new Filter("Config File", "cfg") });

            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }

            if(XmlHelper.DeserializeFromFile<ObservableCollection<ServiceInfo>>(fileName) is ObservableCollection<ServiceInfo> cfg)
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
            if(parameter[0] is ServiceInfo s && parameter[1] is TypeInfo t)
            {

                foreach (var dependency in s.Dependencies)
                {
                    if (SelectedServices.FirstOrDefault(x => x.ServiceType.Equals(dependency)) is null)
                    {
                        _viewService.Error($"Unable to add service \"{s.Name}\", requires additional dependency \"{dependency}\"");
                        
                        return;
                    }
                }

                if(SelectedServices.FirstOrDefault(x => x.Name == s.Name) is ServiceInfo ss)
                {
                    ss.ImplementationType = t;
                }
                else
                {
                    s.ImplementationType = t;
                    SelectedServices.Add(s);
                }
            }
        }

        #endregion
    }
}
