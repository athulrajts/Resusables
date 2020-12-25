using KEI.Infrastructure.Validation;
using Prism.Mvvm;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Validation.Attributes;
using KEI.UI.Wpf.Controls.ObjectEditors;
using ValidationResult = KEI.Infrastructure.Validation.ValidationResult;

namespace KEI.UI.Wpf.Controls.Configuration
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class PropertyGrid : UserControl
    {
        public object EditingObject
        {
            get { return GetValue(EditingObjectProperty); }
            set { SetValue(EditingObjectProperty, value); }
        }

        public static readonly DependencyProperty EditingObjectProperty =
            DependencyProperty.Register("EditingObject", typeof(object), typeof(PropertyGrid), new PropertyMetadata(null, OnEditingObjectChanged));

        private static void OnEditingObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pg = d as PropertyGrid;

            if (e.NewValue is IList)
                return;

            if (e.NewValue != null)
            {
                pg.props.Clear();

                var type = e.NewValue.GetType();

                if (e.NewValue is IPropertyContainer)
                {
                    var config = e.NewValue as IPropertyContainer;
                    pg.objectNameTxt.Text = config.Name;

                    foreach (var data in config)
                    {
                        pg.props.Add(new DataItemPropertyInfo((PropertyObject)data));
                    }
                }
                else
                {
                    pg.objectNameTxt.Text = type.GenericTypeArguments.Length > 0 ? $"{type.Name.Split('`')[0]}<{type.GenericTypeArguments[0].Name}>" : type.Name;

                    var props = e.NewValue.GetType().GetProperties();

                    foreach (var prop in props)
                    {
                        if (prop.GetCustomAttribute<BrowsableAttribute>() is BrowsableAttribute ba)
                        {
                            if (!ba.Browsable)
                            {
                                continue;
                            }
                        }

                        if (prop.SetMethod == null)
                            continue;

                        pg.props.Add(new PropertyInfo(e.NewValue, prop));
                    }

                    if (props.Length > 0)
                    {
                        pg.itemsControl.SelectedItem = pg.props[0];
                    }
                }


            }
        }

        public PropertyGrid()
        {
            InitializeComponent();

            itemsControl.ItemsSource = props;
        }

        private ObservableCollection<PropertyInfoBase> props = new ObservableCollection<PropertyInfoBase>();

    }

    internal abstract class PropertyInfoBase : BindableBase, IDataErrorInfo
    {
        protected TypeConverter converter = null;
        private DelegateCommand openEditorCommand;
        public ICommand OpenEditorCommand
        {
            get
            {
                if (openEditorCommand == null)
                {
                    openEditorCommand = new DelegateCommand(() => 
                    {
                        new ObjectEditorWindow(GetPropertyValue()).ShowDialog();
                    });
                }
                return openEditorCommand;
            }
        }
        public List<string> EnumSource { get; set; }

        public string Error => null;

        public Type PropertyType { get; set; }

        private string propertyValue;
        public string PropertyValue
        {
            get => propertyValue;
            set => SetProperty(ref propertyValue, value);
        }
        public string PropertyDescription { get; set; }
        public string PropertyName { get; set; }
        
        public string PropertyEditor { get; set; }
        
        public BrowseOptions BrowseOption { get; set; }

        public string this[string name]
        {
            get
            {
                var validationResult = new ValidationResult(true);
                if (name == nameof(PropertyValue))
                {
                    if (PropertyType?.IsPrimitiveType() == true)
                    {
                        validationResult = Validators.Type(PropertyType).Validate(PropertyValue);
                    }

                    if (validationResult.IsValid)
                    {
                        SetPropertyValue(PropertyValue);

                        validationResult = Validate(PropertyValue);
                    }
                }

                return validationResult.ErrorMessage;
            }
        }

        protected string GetEditor(Type type)
        {
            if (typeof(IConvertible).IsAssignableFrom(type))
            {
                if (type.IsEnum)
                    return "enum";
                else if (type == typeof(bool))
                    return "bool";
                return "string";
            }
            return "dc";
        }

        public abstract void SetPropertyValue(string value);

        public abstract ValidationResult Validate(string value);

        public abstract object GetPropertyValue();
    }

    internal class PropertyInfo : PropertyInfoBase
    {
        private System.Reflection.PropertyInfo propInfo;
        private object source;

        public PropertyInfo(object parent, System.Reflection.PropertyInfo info)
        {
            propInfo = info;
            source = parent;

            PropertyType = info.PropertyType;
            PropertyDescription = info.GetDescription();
            PropertyName = info.Name;
            PropertyValue = GetPropertyValue()?.ToString();
            converter = TypeDescriptor.GetConverter(PropertyType);

            if (PropertyType.IsEnum)
            {
                EnumSource = new List<string>(Enum.GetNames(PropertyType));
            }

            PropertyEditor = GetEditor(PropertyType);
            BrowseOption = info.GetBrowseOption();
        }
        public override object GetPropertyValue() => propInfo.GetValue(source);
        public override void SetPropertyValue(string value) => propInfo.SetValue(source, converter?.ConvertFrom(value));

        public override ValidationResult Validate(string value)
        {
            var attrs = propInfo.GetCustomAttributes<ValidationAttribute>();

            foreach (var validator in attrs)
            {
                var result = validator.Validate(value);

                if (result.IsValid == false)
                    return result;
            }

            return new ValidationResult(true);
        }
    }

    internal class DataItemPropertyInfo : PropertyInfoBase, IWeakEventListener
    {
        private PropertyObject source;

        public DataItemPropertyInfo(PropertyObject item)
        {
            source = item;
            PropertyValue = item.GetValue().ToString();
            PropertyName = item.Name;
            PropertyType = item.GetDataType();
            PropertyDescription = item.Description;

            if (PropertyType != null)
            {
                converter = TypeDescriptor.GetConverter(PropertyType); 
            }

            if (source.Type == "opt")
            {
                Selector s = source.GetType().GetProperty("Value").GetValue(source) as Selector;
                EnumSource = s.Option;
                PropertyValue = s.SelectedItem;
            }
            else if(source.Type == "enum")
            {
                Enum value = source.GetValue() as Enum;

                EnumSource = new List<string>(Enum.GetNames(value.GetType()));
                PropertyValue = Enum.GetName(value.GetType(), value);
            }

            PropertyEditor = source.Type;
            BrowseOption = item.BrowseOption;

            PropertyChangedEventManager.AddListener(source, this, "Value");
        }

        ~DataItemPropertyInfo()
        {
            PropertyChangedEventManager.RemoveListener(source, this, "Value");
        }

        public override object GetPropertyValue() => source.GetValue();

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                if (sender is PropertyObject p)
                {
                    if (PropertyValue != p.StringValue)
                    {
                        PropertyValue = p.StringValue;
                        RaisePropertyChanged(nameof(PropertyValue)); 
                    }
                }

                return true;
            }
            return false;
        }

        public override void SetPropertyValue(string value) => source.StringValue = value;

        public override ValidationResult Validate(string value) => source.Validation == null 
            ?  new ValidationResult(true) 
            : source.Validation.Validate(value);
    }
}
