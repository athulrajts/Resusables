using Prism.Mvvm;
using KEI.Infrastructure.Screen;
using System.ComponentModel;
using KEI.UI.Wpf.Controls.PropertyGridEditors;

namespace Application.Production.Screen
{
    public class ScreenInfo : BindableBase
    {
        private Icon icon;

        public string DisplayName { get; set; }

        public string ScreenName { get; set; }

        public string ParentScreenName { get; set; }

        [Editor(typeof(ComboBoxEditor), typeof(ComboBoxEditor))]
        public Icon Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        public bool IsMandatory { get; set; }
    }
}
