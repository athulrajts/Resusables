using System;
using System.ComponentModel;
using KEI.Infrastructure.Validation;
using System.Xml;
using System.Collections.Generic;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Abstraction for holding different type of data in <see cref="IPropertyContainer"/>
    /// Handles Xml Serialization and Deserialization
    /// Handles validation when updating values.
    /// </summary>
    public abstract class PropertyObject : DataObject
    {
        // xml strings
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


        /// <summary>
        /// Helps a property editors to choose what control to be used to
        /// edits this objects value.
        /// </summary>
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

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlInternal(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlInternal(XmlWriter writer)
        {
            // write base implementation
            base.WriteXmlInternal(writer);

            /// Only write browse option if it's value is not <see cref="BrowseOptions.Browsable"/>
            /// Most of the properties is expected to be browsable so only write if it's not that to decrease file length
            if(BrowseOption != BrowseOptions.Browsable)
            {
                writer.WriteAttributeString(BROWSE_ATTRIBUTE, BrowseOption.ToString());
            }

            // Write description if we have one
            if(string.IsNullOrEmpty(Description) == false)
            {
                writer.WriteElementString(nameof(Description), Description);
            }

            // Write validation if we have one
            if(Validation is not null)
            {
                writer.WriteObjectXML(Validation);
            }

        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlAttrubutes(XmlReader)"/>
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttrubutes(XmlReader reader)
        {
            // do base implementation
            base.ReadXmlAttrubutes(reader);

            // read browse option
            if (reader.GetAttribute(BROWSE_ATTRIBUTE) is string attr)
            {
                BrowseOption = (BrowseOptions)Enum.Parse(typeof(BrowseOptions), attr);
            }
            else
            {
                BrowseOption = BrowseOptions.Browsable;
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            // Read description
            if (elementName == nameof(Description))
            {
                Description = reader.ReadElementContentAsString();

                return true;
            }

            // Read Validations
            else if (elementName == "Validations")
            {
                Validation = reader.ReadObjectXML<ValidatorGroup>();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper fuction to set Description in <see cref="PropertyContainerBuilder"/>
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public PropertyObject SetDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Helper function to set Validations in <see cref="PropertyContainerBuilder"/>
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public PropertyObject SetBrowsePermission(BrowseOptions option)
        {
            BrowseOption = option;
            return this;
        }

    }

    /// <summary>
    /// Generic implemenation of <see cref="PropertyObject"/> for primitive types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PropertyObject<T> : PropertyObject
    {
        /// <summary>
        /// Value held by this object
        /// </summary>
        protected T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value) == true)
                {
                    return;
                }

                _value = value;

                // update string value whenever our value changes
                stringValue = _value?.ToString();

                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(StringValue));
            }
        }

        /// <summary>
        /// Implemenatiton for <see cref="DataObject.GetValue"/>
        /// </summary>
        /// <returns></returns>
        public override object GetValue() => Value;

        /// <summary>
        /// Implementation for <see cref="DataObject.SetValue(object)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if (value.GetType() != typeof(T))
            {
                return false;
            }

            Value = (T)value;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return typeof(T);
        }
    }
}
