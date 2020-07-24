using System.Windows.Input;
using System.Collections.ObjectModel;
using KEI.Infrastructure.Screen;

namespace Application.Production.Screen
{
    public interface IScreenViewModel : IViewModel
    {
        ObservableCollection<IScreenViewModel> SubViews { get; set; }
        ObservableCollection<ScreenCommand> Commands { get; set; }
        IScreenViewModel CurrentSubViewModel { get; set; }
        IScreenViewModel ParentScreenViewModel { get; set; }
        bool IsChildScreen { get; set; }
        string DisplayName { get; set; }
        Icon Icon { get; set; }
        bool IsSelected { get; set; }
        ICommand NavigateSubViewCommand { get; }
    }

    public interface IViewModel
    {
        string ScreenName { get; }
    }
}
