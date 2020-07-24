using System.Windows;
using Prism.Ioc;
using Prism.Regions;
using KEI.Infrastructure;
using KEI.UI.Wpf.ViewService;
using KEI.Infrastructure.Logging;
using ConfigEditor.Views;
using ConfigEditor.ViewModels;

#pragma warning disable 3277

namespace ConfigEditor
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell() => Container.Resolve<ConfigEditorWindow>();

        public int CommandLineArgsCount { get; set; }
        public string LeftPath { get; set; }
        public string RightPath { get; set; }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterConsoleLogger();
            containerRegistry.RegisterUIServices();

            containerRegistry.RegisterSingleton<ConfigEditorViewModel>();
            containerRegistry.RegisterSingleton<ConfigViewerTabsViewModel>();
            containerRegistry.RegisterSingleton<MergeViewModel>();

            containerRegistry.RegisterForNavigation<ConfigViewerTabs>();
            containerRegistry.RegisterForNavigation<MergeView>();
        }

        protected override void OnInitialized()
        {
            var regionManager = Container.Resolve<IRegionManager>();

            regionManager.RequestNavigate("MainContent", nameof(ConfigViewerTabs));

            if(CommandLineArgsCount == 1)
            {
                Container.Resolve<ConfigEditorViewModel>().OpenFile(LeftPath);
            }
            else if(CommandLineArgsCount == 2)
            {
                var vm = Container.Resolve<MergeViewModel>();
                vm.LeftPath = LeftPath;
                vm.RightPath = RightPath;
                vm.RefreshMergeCommand.Execute();
                regionManager.RequestNavigate("MainContent", nameof(MergeView));
            }

            base.OnInitialized();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CommandLineArgsCount = e.Args.Length;

            if(CommandLineArgsCount > 0)
            {
                LeftPath = e.Args[0];
                if (CommandLineArgsCount == 2)
                    RightPath = e.Args[1];
            }

            base.OnStartup(e);
        }
    }
}
