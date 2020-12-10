using System.Reflection;
using Prism.Ioc;
using KEI.Infrastructure;
using KEI.Infrastructure.Localizer;
using KEI.UI.Wpf.Hotkey;

namespace KEI.UI.Wpf.ViewService
{
    public static class ContainerRegisteryExtensions
    {
        public static void RegisterUIServices(this IContainerRegistry registry)
        {
            registry.RegisterSingleton<IViewService, BaseViewService>();
            registry.RegisterSingleton<IHotkeyService, HotkeyService>();
            registry.RegisterDialog<ConfigsChangedDialog>();
            registry.RegisterInstance<IStringLocalizer>(new ResourceManagerStringLocalizer(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly().GetName().Name);

            Controls.PropertyGridEditorDefinitions.SetDefaultEditors();

            Infrastructure.ViewService.Service = ContainerLocator.Container.Resolve<IViewService>();
        }
    }
}
