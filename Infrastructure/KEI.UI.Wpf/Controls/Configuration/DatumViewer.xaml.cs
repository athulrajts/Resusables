using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Types;
using KEI.Infrastructure.Validation;
using KEI.UI.Wpf.Controls.Configuration;
using KEI.UI.Wpf.Controls.ObjectEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ValidationResult = KEI.Infrastructure.Validation.ValidationResult;
using ValidationRule = KEI.Infrastructure.Validation.ValidationRule;

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

        private ValidationRule typeValidator;

        private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatumViewer template && e.NewValue is PropertyObject data)
            {

                template.typeValidator = Validators.Type(data.Type);

                template.stringValue = data.ValueString;

                template.RaisePropertyChanged(nameof(Editor));
                template.RaisePropertyChanged(nameof(StringValue));

                if (data.Value is Selector s)
                {
                    template.EnumSource = s.Option;
                    template.RaisePropertyChanged("EnumSource");
                    template.StringValue = s.SelectedItem;
                }

                if (e.OldValue is PropertyObject oldValue)
                    template.SubscribePropertyChanged(oldValue, false);
                else if (e.NewValue is PropertyObject newValue)
                    template.SubscribePropertyChanged(newValue, true);

            }
        }

        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PropertyObject.ValueString))
            {
                stringValue = Data.ValueString;
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
                    return;

                stringValue = value;

                if (Editor == EditorType.String ||
                    Editor == EditorType.Folder ||
                    Editor == EditorType.File)
                {
                    ValidationResult = typeValidator.Validate(value);
                }
                
                if (ValidationResult == null)
                    ValidationResult = new ValidationResult(true);

                if(ValidationResult.IsValid)
                {
                    if (Data.Validation is ValidatorGroup vg)
                    {
                        ValidationResult = vg.Validate(value);

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
            if (Data.Value is Selector s)
            {
                Data.Value = s.Clone(value);
            }
            else
            {
                Data.ValueString = value;
            }
        }
        public EditorType Editor => Data.Editor;

        private void SubscribePropertyChanged(PropertyObject source, bool subscribe)
        {
            if (subscribe)
                source.PropertyChanged += Data_PropertyChanged;
            else
                source.PropertyChanged -= Data_PropertyChanged;
        }


        public DatumViewer()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!typeof(IConvertible).IsAssignableFrom(Data.Type))
            {
                var editor = new ObjectEditorWindow(Data.Value);
                editor.ShowDialog();
            }
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
