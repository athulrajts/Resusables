using CommonServiceLocator;
using KEI.Infrastructure.Events;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace KEI.UI.Wpf.ViewService.Dialogs
{
    public class LoadingOverlayViewModel : BindableBase
    {

        private string loadingText;
        public string LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
        }

        public LoadingOverlayViewModel()
        { 
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<UpdateBusyText>().Subscribe(msg => LoadingText = msg, ThreadOption.UIThread);
        }

    }
}
