using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KEI.Infrastructure.Validation;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Abstraction for holding different type of data
    /// </summary>
    public class PropertyObject : DataObject
    {

        #region Properties

        /// <summary>
        /// A Description of what the Data would be used for.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string Description { get; set; }

        /// <summary>
        /// Property to indicate whether this Object stores
        /// a Valid object of <see cref="Type"/>
        /// </summary>
        [XmlIgnore]
        public bool IsValid { get; set; }


        [Browsable(false)]
        [XmlAttribute]
        public EditorType Editor { get; set; }

        /// <summary>
        /// Validations for this property
        /// </summary>
        [XmlElement(IsNullable = false)]
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
        [XmlAttribute]
        public BrowseOptions BrowseOption { get; set; }

        /// <summary>
        /// Force Ivoke RaisPropertyChanged() so that Validations will get exectued
        /// Need to re rerun validation when validation is editted.
        /// </summary>
        public void ForceValueChanged() => RaisePropertyChanged(nameof(ValueString));

        #endregion

        #region Private/Internal Methods
        internal bool HasOnlyValue()
        {
            return Validation == null && string.IsNullOrEmpty(Description);
        }

        #endregion

    }

    /// <summary>
    /// Wrapper class to avoid an extra node when serializing <see cref="PropertyContainer"/> to XML
    /// </summary>
    public class PropertyObjectCollection : ObservableCollection<PropertyObject>
    {
        public PropertyObjectCollection() : base() { }
        public PropertyObjectCollection(IEnumerable<PropertyObject> source) : base(source) { }

    }
}
