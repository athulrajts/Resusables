using System;
using System.Linq;
using System.Collections.ObjectModel;
using CommonServiceLocator;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using KEI.Infrastructure.Types;
using Application.Core.Interfaces;
using KEI.UI.Wpf.ViewService.ViewModels;
using KEI.Infrastructure.Prism;

namespace Application.UI.AdvancedSetup
{
    [RegisterSingleton(NeedResolve = false)]
    public class AdvancedSetupDialogViewModel : BaseDialogViewModel
    {
        public ObservableCollection<IAdvancedSetup> AdvancedSetups { get; set; } = new ObservableCollection<IAdvancedSetup>();
        public IAdvancedSetup First => AdvancedSetups.FirstOrDefault(x => x.IsAvailable == true);
        
        public AdvancedSetupDialogViewModel(ImplementationsProvider provider)
        {
            var types = provider.GetImplementations(typeof(IAdvancedSetup));

            foreach (var type in types.OrderBy(type => type.Name))
            {
                AdvancedSetups.Add((IAdvancedSetup)ServiceLocator.Current.GetInstance(type));
            }

            RaisePropertyChanged(nameof(First));
        }
        
        public override string Title { get; set; } = "Advanced Setup";
    }
}
