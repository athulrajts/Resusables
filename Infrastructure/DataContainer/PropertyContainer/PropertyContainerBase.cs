using KEI.Infrastructure.Validation;

namespace KEI.Infrastructure
{
    public abstract class PropertyContainerBase : DataContainerBase, IPropertyContainer
    {
        /// <summary>
        /// Set <see cref="PropertyObject.BrowseOption"/> of the <see cref="PropertyObject"/>
        /// identified by name.
        /// </summary>
        /// <param name="property">name of <see cref="PropertyObject"/> to update BrowseOption</param>
        /// <param name="option"></param>
        public bool SetBrowseOptions(string property, BrowseOptions option)
        {
            if (FindRecursive(property) is PropertyObject di)
            {
                di.BrowseOption = option;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set <see cref="PropertyObject.Validation"/> of the <see cref="PropertyObject"/>
        /// identified by name.
        /// </summary>
        /// <param name="property">name of <see cref="PropertyObject"/> to update Validation</param>
        /// <param name="option"></param>
        public bool SetValidation(string property, ValidatorGroup validation)
        {
            if (FindRecursive(property) is PropertyObject di)
            {
                di.Validation = validation;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataContainerBase.GetUnitializedDataObject(string)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override DataObject GetUnitializedDataObject(string type)
        {
            return DataObjectFactory.GetPropertyObject(type);
        }

        public abstract object Clone();

    }
}
