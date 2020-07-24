using Application.Core;
using Application.Production.Screen;
using Application.UI;
using CommonServiceLocator;
using KEI.Infrastructure.Localizer;
using KEI.Infrastructure.Prism;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Application.Production
{
    /// <summary>
    /// Interaction logic for ProductionView.xaml
    /// </summary>
    [RegisterWithRegion(RegionNames.ApplicationShell)]
    public partial class ProductionView : UserControl, IWeakEventListener
    {
        private readonly ISystemStatusManager _systemStatusManager;
        private readonly IStringLocalizer _localizer;
        public ProductionView(ISystemStatusManager systemStatusManager, ILocalizationProvider localizer)
        {

            InitializeComponent();

            _systemStatusManager = systemStatusManager;
            _localizer = localizer.Provide();

            DataContext = ServiceLocator.Current.GetInstance<ProductionViewModel>();

            PropertyChangedEventManager.AddListener((INotifyPropertyChanged)_systemStatusManager, this, string.Empty);
            PropertyChangedEventManager.AddListener((INotifyPropertyChanged)_localizer, this, string.Empty);
        }


        // HACK
        // TODO: FIX
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                var eArgs = e as PropertyChangedEventArgs;
                string propName = eArgs.PropertyName;

                if (sender is ISystemStatusManager ss)
                {
                    if (propName == "CurrentScreen")
                    {
                        if (rootSubview.DataContext is IScreenViewModel rootVm)
                        {
                            if (rootVm.ScreenName == ss.CurrentScreen)
                                rootSubview.IsChecked = true;
                        }
                    }
                }
                else if (sender is IStringLocalizer)
                {
                    if (rootSubview.DataContext is IScreenViewModel vm)
                    {
                        rootSubview.Content = _localizer[vm.DisplayName];
                    }
                }

                return true;
            }
            return false;
        }

        private void RootSubview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is IScreenViewModel vm)
            {
                rootSubview.IsChecked = vm.ScreenName == _systemStatusManager.CurrentScreen;
                rootSubview.Content = _localizer[vm.DisplayName];
            }
        }
    }
}
