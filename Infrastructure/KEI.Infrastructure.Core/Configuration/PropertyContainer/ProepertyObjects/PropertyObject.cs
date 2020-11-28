using System;
using System.ComponentModel;
using KEI.Infrastructure.Validation;
using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Abstraction for holding different type of data
    /// </summary>
    public abstract class PropertyObject : DataObject
    {
        public const string BROWSE_ATTRIBUTE = "browse"; 

        #region Properties

        /// <summary>
        /// A Description of what the Data would be used for.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Property to indicate whether this Object stores
        /// a Valid object of <see cref="Type"/>
        /// </summary>
        public bool IsValid { get; private set; }


        [Browsable(false)]
        public abstract EditorType Editor { get; }

        /// <summary>
        /// Validations for this property
        /// </summary>
        private ValidatorGroup validation;
        public ValidatorGroup Validation
        {
            get { return validation; }
            set { SetProperty(ref validation, value); }
        }

        /// <summary>
        /// Gets/Sets BrowseOptions for the Property
        /// Used by Property Editors Hide Property or Disable editing
        /// </summary>
        [Browsable(false)]
        public BrowseOptions BrowseOption { get; set; } = BrowseOptions.Browsable;

        /// <summary>
        /// Force Ivoke RaisPropertyChanged() so that Validations will get exectued
        /// Need to re rerun validation when validation is editted.
        /// </summary>
        public void ForceValueChanged() => RaisePropertyChanged(nameof(StringValue));

        #endregion

        protected override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);

            if(BrowseOption != BrowseOptions.Browsable)
            {
                writer.WriteAttributeString(BROWSE_ATTRIBUTE, BrowseOption.ToString());
            }

            if(string.IsNullOrEmpty(Description) == false)
            {
                writer.WriteElementString(nameof(Description), Description);
            }

            if(Validation is not null)
            {
                writer.WriteObjectXML(Validation);
            }

        }

        protected override void ReadAttrubutes(XmlReader reader)
        {
            base.ReadAttrubutes(reader);

            if (reader.GetAttribute(BROWSE_ATTRIBUTE) is string attr)
            {
                BrowseOption = (BrowseOptions)Enum.Parse(typeof(BrowseOptions), attr);
            }
            else
            {
                BrowseOption = BrowseOptions.Browsable;
            }
        }

        protected override bool ReadElement(string elementName, XmlReader reader)
        {
            if (elementName == nameof(Description))
            {
                Description = reader.ReadElementContentAsString();

                return true;
            }

            else if (elementName == "Validations")
            {
                Validation = reader.ReadObjectXML<ValidatorGroup>();

                return true;
            }

            return false;
        }

        public PropertyObject SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public PropertyObject SetBrowsePermission(BrowseOptions option)
        {
            BrowseOption = option;
            return this;
        }

        public static PropertyObject Create(string name, object value) => DataObjectFactory.GetPropertyObjectFor(name, value);

    }

    public abstract class PropertyObject<T> : PropertyObject
    {
        protected T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                stringValue = value?.ToString();

                SetProperty(ref _value, value);

                RaisePropertyChanged(nameof(StringValue));
            }
        }

        public PropertyObject(string name, T value)
        {
            Value = value;
            Name = name;
        }

        public PropertyObject()
        {

        }

        public override object GetValue() => Value;

        public override bool SetValue(object value)
        {
            if (value.GetType() != typeof(T))
            {
                return false;
            }

            Value = (T)value;

            return true;
        }

        protected override void OnStringValueChanged(string value)
        {
            _value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertTo(value, typeof(T));

            RaisePropertyChanged(nameof(Value));
        }

        public override bool ValidateForType(string value)
        {
            return TypeDescriptor.GetConverter(typeof(T)).IsValid(value);
        }

        public override Type GetDataType()
        {
            return typeof(T);
        }
    }
}
