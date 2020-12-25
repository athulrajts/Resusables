using KEI.Infrastructure.Validation;
using System;

namespace KEI.Infrastructure
{
    public interface IPropertyContainer : IDataContainer, ICloneable
    {
        public bool SetBrowseOptions(string property, BrowseOptions option);
        public bool SetValidation(string property, ValidatorGroup validation);
    }
}