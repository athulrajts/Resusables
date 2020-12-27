using System;
using KEI.Infrastructure.Validation;

namespace KEI.Infrastructure
{
    public interface IPropertyContainer : IDataContainer, ICloneable
    {
        /// <summary>
        /// Update <see cref="PropertyObject.BrowseOption"/> of instance with key <paramref name="property"/>
        /// with <paramref name="option"/>
        /// </summary>
        /// <param name="property"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool SetBrowseOptions(string property, BrowseOptions option);

        /// <summary>
        /// Update <see cref="PropertyObject.Validation"/> of instance with key <paramref name="property"/>
        /// with <paramref name="validation"/>
        /// </summary>
        /// <param name="property"></param>
        /// <param name="validation"></param>
        /// <returns></returns>
        public bool SetValidation(string property, ValidatorGroup validation);
    }
}