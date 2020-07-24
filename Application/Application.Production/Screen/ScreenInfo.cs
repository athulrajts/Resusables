using Prism.Mvvm;
using KEI.Infrastructure.Screen;

namespace Application.Production.Screen
{
    public class ScreenInfo : BindableBase
    {
        public string DisplayName { get; set; }

        public string ScreenName { get; set; }

        public string ParentScreenName { get; set; }

        private Icon icon;
        public Icon Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        public bool IsMandatory { get; set; }
    }
}
