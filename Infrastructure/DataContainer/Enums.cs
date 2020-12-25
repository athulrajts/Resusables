namespace KEI.Infrastructure
{
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
