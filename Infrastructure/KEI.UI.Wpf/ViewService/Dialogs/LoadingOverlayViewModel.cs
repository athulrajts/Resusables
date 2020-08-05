using CommonServiceLocator;
using Prism.Events;
using Prism.Mvvm;

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
