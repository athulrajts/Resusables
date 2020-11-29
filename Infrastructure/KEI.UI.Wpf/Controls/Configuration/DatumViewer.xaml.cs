using System;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using KEI.Infrastructure;
using KEI.UI.Wpf.Controls.ObjectEditors;
using ValidationResult = KEI.Infrastructure.Validation.ValidationResult;

namespace KEI.UI.Wpf.Configuration
{
    /// <summary>
    /// Interaction logic for DatumViewer.xaml
    /// </summary>
    public partial class DatumViewer : UserControl, INotifyPropertyChanged, IDataErrorInfo
    {

        public PropertyObject Data
        {
            get { return (PropertyObject)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(PropertyObject), typeof(DatumViewer), new PropertyMetadata(null, DataChanged));

        private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatumViewer template && e.NewValue is PropertyObject data)
            {
                template.StringValue = data.StringValue;

                template.RaisePropertyChanged(nameof(Editor));

                if (data.Type == "enum")
                {
                    template.EnumSource = new List<string>(Enum.GetNames(data.GetValue().GetType()));
                    template.RaisePropertyChanged(nameof(EnumSource));
                    template.StringValue = Enum.GetName(data.GetValue().GetType(), data.GetValue());
                }
                else if(data.Type == "opt")
                {
                    Selector s = data.GetType().GetProperty("Value").GetValue(data) as Selector;
                    template.EnumSource = s.Option;
                    template.StringValue = s.SelectedItem;
                }

                if (e.OldValue is PropertyObject oldValue)
                {
                    template.SubscribePropertyChanged(oldValue, false);
                }
                else if (e.NewValue is PropertyObject newValue)
                {
                    template.SubscribePropertyChanged(newValue, true);
                }

            }
        }

        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PropertyObject.StringValue))
            {
                stringValue = Data.StringValue;
                RaisePropertyChanged(nameof(StringValue));
            }
        }

        private ValidationResult validationResult = new ValidationResult(true);
        public ValidationResult ValidationResult
        {
            get => validationResult;
            set
            {
                validationResult = value;
                RaisePropertyChanged(nameof(ValidationResult));
            }
        }

        private string stringValue;
        public string StringValue
        {
            get => stringValue;
            set
            {
                if (stringValue == value)
                {
                    return;
                }

                stringValue = value;

                ValidationResult = Data.CanConvertFromString(stringValue)
                    ? new ValidationResult(true)
                    : new ValidationResult(false, "Invalid Type");

                if (ValidationResult.IsValid)
                {
                    if (Data.Validation is not null)
                    {
                        ValidationResult = Data.Validation.Validate(stringValue);

                        if (ValidationResult.IsValid)
                        {
                            SetValue(value);
                        }
                    }
                    else
                    {
                        SetValue(value);
                    }
                }

                RaisePropertyChanged(nameof(StringValue));
            }
        }

        private void SetValue(string value)
        {
            Data.StringValue = value;
        }
        public EditorType Editor => Data.Editor;

        private void SubscribePropertyChanged(PropertyObject source, bool subscribe)
        {
            if (subscribe)
            {
                source.PropertyChanged += Data_PropertyChanged;
            }
            else
            {
                source.PropertyChanged -= Data_PropertyChanged;
            }
        }


        public DatumViewer()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var editor = new ObjectEditorWindow(Data.GetValue());
            editor.ShowDialog();
        }

        public List<string> EnumSource { get; set; }

        public string Error => null;

        public string this[string name]
        {
            get
            {
                return ValidationResult?.ErrorMessage;
            }
        }
    }



    public class DataTypeToEditorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditorType type && parameter != null)
            {
                if (type.ToString() == parameter.ToString())
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
