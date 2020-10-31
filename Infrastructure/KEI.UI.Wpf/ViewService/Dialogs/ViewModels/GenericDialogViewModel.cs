using KEI.Infrastructure;
using Prism.Services.Dialogs;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KEI.UI.Wpf.ViewService.ViewModels
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

        private PromptResult defaultResult;
        public PromptResult DefaultResult
        {
            get { return defaultResult; }
            set { SetProperty(ref defaultResult, value); }
        }

        private bool isAutoClose;
        public bool IsAutoClose
        {
            get { return isAutoClose; }
            set { SetProperty(ref isAutoClose, value); }
        }

        private int timeRemaining;
        public int TimeRemaining
        {
            get { return timeRemaining; }
            set { SetProperty(ref timeRemaining, value); }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
            Title = parameters.GetValue<string>("title");
            Buttons = parameters.GetValue<string>("buttons");

            if(parameters.ContainsKey("timeout") && 
                parameters.ContainsKey("defaultResult"))
            {
                var timeout = parameters.GetValue<TimeSpan>("timeout");
                defaultResult = parameters.GetValue<PromptResult>("defaultResult");
                IsAutoClose = true;

                TimeRemaining = (int)timeout.TotalSeconds;
                
                AwaitTimeout(timeout);
            }
        }

        private async void AwaitTimeout(TimeSpan timeout)
        {
            for (int i = 0; i < (int)timeout.TotalSeconds; i++)
            {
                await Task.Delay(1000);
                TimeRemaining -= 1;
            }

            if (IsOpen)
            {
                CloseDialog(defaultResult.ToString()); 
            }
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

    public class TitleToBackgroundConverter : BaseValueConverter<TitleToBackgroundConverter>
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

    public class DialogButtonVisibilityConverter : BaseValueConverter<DialogButtonVisibilityConverter>
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
