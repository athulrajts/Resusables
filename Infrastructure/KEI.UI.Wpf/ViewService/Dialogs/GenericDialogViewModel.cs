using KEI.Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace KEI.UI.Wpf.ViewService
{
    public class GenericDialogViewModel : BaseDialogViewModel
    {

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _title = DialogType.Other.ToString();
        public override string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _buttons = PromptOptions.Ok.ToString();
        public string Buttons
        {
            get { return _buttons; }
            set { SetProperty(ref _buttons, value); }
        }

        private Brush titleBackground;
        public Brush TitleBackground
        {
            get => titleBackground;
            set => SetProperty(ref titleBackground, value);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
            Title = parameters.GetValue<string>("title");
            Buttons = parameters.GetValue<string>("buttons");
        }
    }

    internal enum DialogType
    {
        Info,
        Warning,
        Error,
        Confirm,
        Other
    }

    public class TitleToBackgroundConverter : ValueConverterExtension<TitleToBackgroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str == DialogType.Info.ToString())
                    return Brushes.LightBlue;
                if (str == DialogType.Warning.ToString())
                    return Brushes.LightGoldenrodYellow;
                if (str == DialogType.Error.ToString())
                    return Brushes.OrangeRed;
                if (str == DialogType.Confirm.ToString())
                    return Brushes.LightGreen;
                if (str == DialogType.Other.ToString())
                    return Brushes.LightGray;

            }
            return Brushes.Orange;
        }
    }

    public class DialogButtonVisibilityConverter : ValueConverterExtension<DialogButtonVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && parameter is string param)
            {
                var arr = param.Split(',');
                foreach (var buttonType in arr)
                {
                    if (str.ToLower() == buttonType.ToLower())
                        return Visibility.Visible;
                }

            }
            return Visibility.Collapsed;
        }
    }
}
