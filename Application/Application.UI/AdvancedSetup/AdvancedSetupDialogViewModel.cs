using System;
using System.Linq;
using System.Collections.ObjectModel;
using CommonServiceLocator;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using KEI.Infrastructure.Types;
using Application.Core.Interfaces;

namespace Application.UI.AdvancedSetup
{
    public class AdvancedSetupDialogViewModel : BindableBase, IDialogAware
    {
        public ObservableCollection<IAdvancedSetup> AdvancedSetups { get; set; } = new ObservableCollection<IAdvancedSetup>();
        public IAdvancedSetup First => AdvancedSetups.FirstOrDefault(x => x.IsAvailable == true);
        public AdvancedSetupDialogViewModel()
        {
            var types = ServiceLocator.Current.GetInstance<ImplementationsProvider>().GetImplementations(typeof(IAdvancedSetup));

            foreach (var type in types.OrderBy(type => type.Name))
            {
                AdvancedSetups.Add((IAdvancedSetup)ServiceLocator.Current.GetInstance(type));
            }

            RaisePropertyChanged(nameof(First));
        }
        public string Title => "Advanced Setup";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            return;
        }
    }
}
