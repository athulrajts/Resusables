using KEI.Infrastructure;
using KEI.UI.Wpf.ViewService;
using Localizer.Views;
using Prism.Events;
using Prism.Services.Dialogs;

namespace Localizer
{

    public interface ILocalizerViewSerivce : IViewService 
    {
        void ShowTranslateResXDialog();
    }

    public class LocalizerViewService : BaseViewService, ILocalizerViewSerivce
    {
        public LocalizerViewService(IDialogService dialogService, ILogManager logManager, IEventAggregator eventAggregator) 
            : base(dialogService,logManager,eventAggregator)
        {

        }

        public void ShowTranslateResXDialog()
        {
            var window = new LocalizeFromBaseResxWindow { WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen };

            window.Owner = System.Windows.Application.Current.MainWindow;

            window.ShowDialog();
        }
    }
}
