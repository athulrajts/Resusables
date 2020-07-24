using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using KEI.Infrastructure;
using KEI.Infrastructure.Logging;
using KEI.Infrastructure.Prism;
using KEI.UI.Wpf.ViewService;
using Localizer.Core;
using Localizer.ViewModels;
using Localizer.Views;
using Prism.Ioc;
using Prism.Regions;

namespace Localizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<LocalizerWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterConsoleLogger();
            containerRegistry.RegisterInfrastructureServices();
            containerRegistry.RegisterUIServices();

            containerRegistry.RegisterSingleton<TranslationFileEditorTabsViewModel>();

            containerRegistry.RegisterForNavigation<TranslationFileEditorTabs>();

            var x = Translator.Languages;
        }

        protected override void OnInitialized()
        {
            Container.Resolve<IRegionManager>().RequestNavigate("ContentRegion", nameof(TranslationFileEditorTabs));

            base.OnInitialized();
        }
    }
}
