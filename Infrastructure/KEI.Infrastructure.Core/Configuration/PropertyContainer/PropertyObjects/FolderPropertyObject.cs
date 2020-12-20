namespace KEI.Infrastructure
{
    /// <summary>
    /// PropertyObject implementation for storing folder paths
    /// There is no corresponding DataObject implementation, since the only
    /// Difference between <see cref="StringPropertyObject"/> <see cref="FilePropertyObject"/>
    /// is the Editor in PropertyGrid. <see cref="StringDataObject"/> should be used for storing folderpaths
    /// in <see cref="DataContainer"/>
    /// </summary>
    internal class FolderPropertyObject : StringPropertyObject
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Folder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public FolderPropertyObject(string name, string value) : base(name, value) { }
    }
}
