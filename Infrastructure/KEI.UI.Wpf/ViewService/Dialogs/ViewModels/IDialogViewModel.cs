using Prism.Services.Dialogs;
using System;
using System.ComponentModel;

namespace KEI.UI.Wpf.ViewService
{
    public interface IDialogViewModel : IDialogAware, INotifyPropertyChanged
    {
        Action CloseDialogAnimation { get; set; }
    }
}
