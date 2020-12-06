namespace KEI.Infrastructure
{

    /// <summary>
    /// Helps a Property Editor to know which kind of Editor to use for the PropertyObject
    /// </summary>
    public enum EditorType
    {
        /// <summary>
        /// For types the can be easily converted to and fro from <see cref="string"/>
        /// Applicable for <see cref="int"/>, <see cref="double"/>, <see cref="float"/>,
        /// <see cref="string"/> etc.
        /// </summary>
        String,

        /// <summary>
        /// For types whos values are restricted to a set of values
        /// Applicable for <see cref="System.Enum"/>
        /// </summary>
        Enum,

        /// <summary>
        /// For Types which can only have 2/3 states 
        /// Applicable for <see cref="bool"/>
        /// </summary>
        Bool,

        /// <summary>
        /// For colors
        /// </summary>
        Color,
        /// <summary>
        /// Encapsulates a string type, Property Editors should show a File Browser as Editor
        /// </summary>
        File,

        /// <summary>
        /// Encapsulates a string type, Property Editors should show a Folder Browser as Editor
        /// </summary>
        Folder,

        /// <summary>
        /// For types which are composed of one or more of the above types
        /// </summary>
        Object,

        /// <summary>
        /// User defined editor
        /// </summary>
        Custom,
    }

    /// <summary>
    /// Reprents how a PropertyEditor can manipulate the property
    /// </summary>
    public enum BrowseOptions
    {
        /// <summary>
        /// Can View and Edit.
        /// </summary>
        Browsable,

        /// <summary>
        /// Hidden from user.
        /// </summary>
        NonBrowsable,

        /// <summary>
        /// Can view but cannot Edit.
        /// </summary>
        NonEditable
    }

    public enum BindingMode
    {
        /// <summary>
        /// Causes changes to either the source property or the target property to automatically
        /// update the other. This type of binding is appropriate for editable forms or other
        /// fully-interactive UI scenarios.
        /// </summary>
        TwoWay,

        /// <summary>
        /// Updates the binding target (target) property when the binding source (source) changes. 
        ///</summary>
        OneWay,

        /// <summary>
        /// Updates the source property when the target property changes.
        /// </summary> 
        OneWayToSource,

        /// <summary>
        /// Updates the binding target when the application starts or when the data context
        /// changes. This type of binding is appropriate if you are using data where either
        /// a snapshot of the current state is appropriate to use or the data is truly static.
        /// This type of binding is also useful if you want to initialize your target property
        /// with some value from a source property.
        /// </summary>
        OneTime,
    }
}
