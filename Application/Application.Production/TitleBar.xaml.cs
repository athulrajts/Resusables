using Application.Production.ViewModels;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Application.Production.Views
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = ContainerLocator.Container.Resolve<TitleBarViewModel>();
            }
        }
    }
}
