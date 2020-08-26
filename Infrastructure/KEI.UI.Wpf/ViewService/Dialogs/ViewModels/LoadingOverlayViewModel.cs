using KEI.UI.Wpf.ViewService.ViewModels;
using Prism.Services.Dialogs;

namespace KEI.UI.Wpf.ViewService.Dialogs
{
    public class LoadingOverlayViewModel : BaseDialogViewModel
    {

        private string loadingText;
        public string LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
        }

        public override string Title { get; set; } = "Loading";

        public void SetBusyText(string msg)
        {
            LoadingText = msg;
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            LoadingText = parameters.GetValue<string>("text");
        }
    }
}
